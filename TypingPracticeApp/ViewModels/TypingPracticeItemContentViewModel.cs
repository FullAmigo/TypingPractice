#region References

using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using TypingPracticeApp.Domain;
using TypingPracticeApp.Extensions;
using TypingPracticeApp.Services;

#endregion

namespace TypingPracticeApp.ViewModels
{
    [DebuggerDisplay("{nameof(TypingPracticeItemContentViewModel)} {OdaiText.Value}")]
    public class TypingPracticeItemContentViewModel : ViewModelDependencyBase
    {
        private readonly CompositeDisposable practiceKeyInfoViewModelsDisposable;
        private readonly BooleanNotifier notMatchedKeyNotifier;
        private readonly BooleanNotifier keyMissingNotifier;
        private int currentIndex;
        private DateTime? startDateTime;

        public TypingPracticeItemContentViewModel()
            : this(null)
        {
            DesignerSupport.ThrowIfNotInDesignMode();
            this.SetPracticeItem(TypingPracticeItemContentViewModel.CreatePracticeItemInDesignMode());
        }

        public TypingPracticeItemContentViewModel(AppContextService appService)
            : base(appService)
        {
            this.practiceKeyInfoViewModelsDisposable = new CompositeDisposable().AddTo(this.Disposables);
            this.notMatchedKeyNotifier = new BooleanNotifier();
            this.keyMissingNotifier = new BooleanNotifier();

            this.OdaiText = new ReactivePropertySlim<string>().AddTo(this.Disposables);
            this.YomiText = new ReactivePropertySlim<string>().AddTo(this.Disposables);
            this.PracticeKeyInfoViewModels = new ReactiveCollection<PracticeKeyInfoViewModel>().AddTo(this.Disposables);
            this.CurrentExpectedKey = new ReactivePropertySlim<Key>().AddTo(this.Disposables);
            this.IsNotMatched = this.notMatchedKeyNotifier.ToReadOnlyReactivePropertySlim().AddTo(this.Disposables);
            this.IsKeyMissing = this.keyMissingNotifier.ToReadOnlyReactivePropertySlim().AddTo(this.Disposables);

            this.Initialize(appService);
        }

        public ReactivePropertySlim<string> OdaiText { get; }
        public ReactivePropertySlim<string> YomiText { get; }
        public ReactiveCollection<PracticeKeyInfoViewModel> PracticeKeyInfoViewModels { get; }
        public ReactivePropertySlim<Key> CurrentExpectedKey { get; }
        public ReadOnlyReactivePropertySlim<bool> IsNotMatched { get; }
        public ReadOnlyReactivePropertySlim<bool> IsKeyMissing { get; }

        internal void SetPracticeItem(PracticeItem practiceItem)
        {
            this.Reset();

            this.OdaiText.Value = practiceItem?.OdaiText;
            this.YomiText.Value = practiceItem?.YomiText;

            var practiceKeyInfoViewModels = (practiceItem?.ExpectedKeysText ?? string.Empty)
                .ToUpperInvariant()
                .Select((expectedKey, index) => new PracticeKeyInfoViewModel(this.AppService, expectedKey, index == 0).AddTo(this.practiceKeyInfoViewModelsDisposable))
                .ToList();
            var firstPracticeKeyInfoViewModel = practiceKeyInfoViewModels.FirstOrDefault();
            this.PracticeKeyInfoViewModels.AddRangeOnScheduler(practiceKeyInfoViewModels);
            this.SetCurrentBy(firstPracticeKeyInfoViewModel);
            this.startDateTime = DateTime.Now;
        }

        private void SetCurrentBy(PracticeKeyInfoViewModel nextPracticeKeyInfoViewModel, PracticeKeyInfoViewModel prevPracticeKeyInfoViewModel = null)
        {
            if (prevPracticeKeyInfoViewModel != null)
            {
                prevPracticeKeyInfoViewModel.IsCurrent.Value = false;
            }

            this.CurrentExpectedKey.Value = nextPracticeKeyInfoViewModel?.ExpectedKey.Value.ToKey() ?? Key.None;
            if (nextPracticeKeyInfoViewModel != null)
            {
                nextPracticeKeyInfoViewModel.IsCurrent.Value = true;
            }
        }

        private void Initialize(AppContextService appService)
        {
            appService?.KeyDetectedAsObservable()
                .Where(_ => this.IsVisibledNotifier.Value)
                .Where(e => KeyMapping.KeyCharacterFingerMapping.Keys.Any(key => key == e.Key) || e.Key == Key.Escape)
                .Subscribe(async e => await this.SubscribeKeyDetectedAsync(e))
                .AddTo(this.Disposables);
        }

        private void Reset()
        {
            this.currentIndex = 0;
            this.notMatchedKeyNotifier.TurnOff();
            this.keyMissingNotifier.TurnOff();

            this.PracticeKeyInfoViewModels.Clear();
            this.practiceKeyInfoViewModelsDisposable.Clear();
        }

        private static PracticeItem CreatePracticeItemInDesignMode()
        {
            if (DesignerSupport.IsInDesignMode)
            {
                return new PracticeItem
                {
                    OdaiText = "本日は晴天なり",
                    YomiText = "ほんじつはせいてんなり",
                    ExpectedKeysText = "HONJITUHASEITENNNARI",
                };
            }

            return null;
        }

        private async Task SubscribeKeyDetectedAsync(KeyEventArgs e)
        {
            DebugLog.Print($"{this.GetType().Name}.{nameof(this.SubscribeKeyDetectedAsync)}: {e.Key}");
            var isPracticeItemCompleted = e.Key == Key.Escape;
            if (!isPracticeItemCompleted)
            {
                try
                {
                    var current = this.PracticeKeyInfoViewModels.ElementAtOrDefault(this.currentIndex);
                    if (current != null && await current.SetInputtedKeyAsync(e.Key, this.notMatchedKeyNotifier, this.keyMissingNotifier))
                    {
                        this.currentIndex++;
                        if (this.currentIndex < this.PracticeKeyInfoViewModels.Count)
                        {
                            this.SetCurrentBy(this.PracticeKeyInfoViewModels.ElementAtOrDefault(this.currentIndex), current);
                        }
                        else
                        {
                            isPracticeItemCompleted = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    DebugLog.Print($"{this.GetType().Name}.{nameof(this.SubscribeKeyDetectedAsync)} is detected Exception on Subscribe: {ex}");
                }
            }

            if (isPracticeItemCompleted)
            {
                DebugLog.Print($"{this.GetType().Name}.{nameof(this.SubscribeKeyDetectedAsync)}: お題完了！");
                var result = this.CreateResult();
                this.AppService?.PublishPracticeItemResulted(result);
            }
        }

        private PracticeResultItem CreateResult()
        {
            var practiceResultKeyInfos = this.PracticeKeyInfoViewModels
                .Select(
                    viewModel =>
                        new PracticeResultKeyInfo
                        {
                            ExpectedKey = viewModel.ExpectedKey.Value,
                            KeyInputtedCount = viewModel.KeyInputtedCount.Value,
                            KeyMistakedCount = viewModel.KeyMistakedCount.Value,
                        })
                .ToList();
            var result = new PracticeResultItem(practiceResultKeyInfos)
            {
                OdaiText = this.OdaiText.Value,
                YomiText = this.YomiText.Value,
                StartDateTime = this.startDateTime,
                EndDateTime = DateTime.Now,
            };
            return result;
        }
    }
}
