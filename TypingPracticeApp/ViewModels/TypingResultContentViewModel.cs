#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using TypingPracticeApp.Domain;
using TypingPracticeApp.Extensions;
using TypingPracticeApp.Services;

#endregion

namespace TypingPracticeApp.ViewModels
{
    public class TypingResultContentViewModel : ViewModelDependencyBase
    {
        public TypingResultContentViewModel()
            : this(null)
        {
            var items = TypingResultContentViewModel.CreatePracticeResultItemsInDesignMode().ToList();
            this.PracticeResultItems.AddRangeOnScheduler(items);
            this.PracticeResultSummaries.AddRangeOnScheduler(PracticeResultSummary.CreateSummariesBy(items));
        }

        public TypingResultContentViewModel(AppContextService appService)
            : base(appService)
        {
            this.PracticeResultItems = new ReactiveCollection<PracticeResultItem>();
            this.PracticeResultSummaries = new ReactiveCollection<PracticeResultSummary>();

            this.PracticeRestartingCommand = new ReactiveCommand().AddTo(this.Disposables);
            this.PracticeRestartingCommand.Subscribe(() => this.AppService?.PublishPracticeRestarting()).AddTo(this.Disposables);

            this.Initialize(appService);
        }

        public ReactiveCollection<PracticeResultItem> PracticeResultItems { get; }
        public ReactiveCollection<PracticeResultSummary> PracticeResultSummaries { get; }
        public ReactiveCommand PracticeRestartingCommand { get; }

        private static IEnumerable<PracticeResultItem> CreatePracticeResultItemsInDesignMode()
        {
            if (DesignerSupport.IsInDesignMode)
            {
                yield return new PracticeResultItem("AIUEO".Select(expectedKey => new PracticeResultKeyInfo { ExpectedKey = expectedKey, KeyInputtedCount = 1 }))
                {
                    OdaiText = "あいうえお",
                    YomiText = "あいうえお",
                    StartDateTime = DateTime.Now,
                    EndDateTime = DateTime.Now.AddSeconds(1).AddMilliseconds(123),
                };
                yield return new PracticeResultItem("HONJITUHASEITENNNARI".Select(expectedKey => new PracticeResultKeyInfo { ExpectedKey = expectedKey, KeyInputtedCount = 1 }))
                {
                    OdaiText = "本日は晴天なり",
                    YomiText = "ほんじつはせいてんなり",
                    StartDateTime = DateTime.Now,
                    EndDateTime = DateTime.Now.AddSeconds(12).AddMilliseconds(123),
                };
            }
        }

        internal void Reset()
        {
            this.PracticeResultItems.Clear();
            this.PracticeResultSummaries.Clear();
        }

        internal void AddPracticeResultItem(PracticeResultItem practiceResultItem)
        {
            this.PracticeResultItems.Add(practiceResultItem);
        }

        internal void Summarize()
        {
            this.PracticeResultSummaries.AddRangeOnScheduler(PracticeResultSummary.CreateSummariesBy(this.PracticeResultItems));
        }

        private void Initialize(AppContextService appService)
        {
            appService?.KeyDetectedAsObservable()
                .Where(_ => this.IsVisibledNotifier.Value)
                .Where(e => e.Key == Key.Space)
                .Subscribe(e => this.AppService?.PublishPracticeRestarting())
                .AddTo(this.Disposables);
        }
    }
}
