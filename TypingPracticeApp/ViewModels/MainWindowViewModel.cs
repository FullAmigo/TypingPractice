#region References

using System;
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
    public class MainWindowViewModel : ViewModelDependencyBase
    {
        private readonly BooleanNotifier booleanNotifier;
        private readonly SerialDisposable serialDisposable;

        public MainWindowViewModel()
            : this(new AppContextService())
        {
            //DesignerSupport.ThrowIfNotInDesignMode();
        }

        public MainWindowViewModel(AppContextService appService)
            : base(appService)
        {
            this.booleanNotifier = new BooleanNotifier();
            this.serialDisposable = new SerialDisposable().AddTo(this.Disposables);

            this.Title = new ReactivePropertySlim<string>($"キーボードタイピング練習 v{AppContextService.AssemblyVersion}").ToReadOnlyReactivePropertySlim().AddTo(this.Disposables);
            this.IsDark = new ReactivePropertySlim<bool>().AddTo(this.Disposables);
            this.ToggleThemeCommand = new ReactiveCommand().AddTo(this.Disposables);

            this.ContentsIndex = new ReactivePropertySlim<int>().AddTo(this.Disposables);
            this.KeyUpCommand = new ReactiveCommand<KeyEventArgs>().AddTo(this.Disposables);

            this.DialogViewModel = new ReactivePropertySlim<object>().AddTo(this.Disposables);
            this.IsDialogOpen = new ReactivePropertySlim<bool>().AddTo(this.Disposables);

            this.CurrentConfirmContentViewModel = new TypingConfirmContentViewModel(appService).AddTo(this.Disposables);
            this.CurrentPracticeItemContentViewModel = new TypingPracticeItemContentViewModel(appService).AddTo(this.Disposables);
            this.CurrentResultContentViewModel = new TypingResultContentViewModel(appService).AddTo(this.Disposables);

            this.Initialize(appService);
        }

        public ReadOnlyReactivePropertySlim<string> Title { get; }

        public ReactivePropertySlim<bool> IsDark { get; }
        public ReactiveCommand ToggleThemeCommand { get; }

        public ReactivePropertySlim<int> ContentsIndex { get; }
        public ReactiveCommand<KeyEventArgs> KeyUpCommand { get; }

        public ReactivePropertySlim<object> DialogViewModel { get; }
        public ReactivePropertySlim<bool> IsDialogOpen { get; }

        public TypingConfirmContentViewModel CurrentConfirmContentViewModel { get; }
        public TypingPracticeItemContentViewModel CurrentPracticeItemContentViewModel { get; }
        public TypingResultContentViewModel CurrentResultContentViewModel { get; }

        protected override async Task OnViewLoadedAsync()
        {
            await base.OnViewLoadedAsync();
            await this.LoadPracticeItemsAsync();
        }

        private void Initialize(AppContextService appService)
        {
            this.ToggleThemeCommand.Subscribe(() => ThemeHelper.ApplyBase(this.IsDark.Value)).AddTo(this.Disposables);
            this.ContentsIndex.Subscribe(index => DebugLog.Print($"{this.GetType().Name}.{nameof(this.ContentsIndex)}: {index}")).AddTo(this.Disposables);
            this.KeyUpCommand
                .Where(_ => !this.IsDialogOpen.Value)
                .Subscribe(e => appService?.PublishKeyDetected(e)).AddTo(this.Disposables);
            appService?.PracticeStartedAsObservable().Subscribe(_ => this.SubscribePracticeStarted()).AddTo(this.Disposables);
            appService?.PracticeItemResultedAsObservable().Subscribe(this.SubscribePracticeItemResulted).AddTo(this.Disposables);
            appService?.PracticeRestartingAsObservable().Subscribe(_ => this.SubscribePracticeRestarting()).AddTo(this.Disposables);
        }

        private async Task LoadPracticeItemsAsync()
        {
            var appService = this.AppService;
            using (var viewModel = new LoadingPracticeDialogViewModel())
            {
                this.DialogViewModel.Value = viewModel;
                this.IsDialogOpen.Value = true;

                await Task.WhenAll(appService.LoadPracticeItemsAsync(), Task.Delay(TimeSpan.FromSeconds(2)));

                this.IsDialogOpen.Value = false;
                this.DialogViewModel.Value = null;
            }
        }

        private void SubscribePracticeStarted()
        {
            var practiceItems = this.AppService?.PracticeItems ?? Enumerable.Empty<PracticeItem>();
            this.serialDisposable.Disposable = practiceItems
                .ToObservable()
                .Do(practiceItem => DebugLog.Print($"{this.GetType().Name}.{nameof(this.SubscribePracticeStarted)}: お題={practiceItem.OdaiText}"))
                .Zip(this.booleanNotifier, (practiceItem, _) => practiceItem)
                .Do(practiceItem => DebugLog.Print($"{this.GetType().Name}.{nameof(this.SubscribePracticeStarted)}: 次のお題={practiceItem.OdaiText}"))
                .Subscribe(
                    practiceItem => this.CurrentPracticeItemContentViewModel?.SetPracticeItem(practiceItem),
                    () =>
                    {
                        this.CurrentResultContentViewModel?.Summarize();
                        this.ContentsIndex.Value = 2;
                    });

            this.CurrentResultContentViewModel?.Reset();

            this.booleanNotifier.SwitchValueWith(_ => DebugLog.Print($"最初のスイッチ"));
            this.ContentsIndex.Value = 1;
        }

        private void SubscribePracticeItemResulted(PracticeResultItem resultItem)
        {
            this.CurrentResultContentViewModel?.AddPracticeResultItem(resultItem);
            this.booleanNotifier.SwitchValueWith(_ => DebugLog.Print($"次のスイッチ"));
        }

        private void SubscribePracticeRestarting()
        {
            this.ContentsIndex.Value = 0;
        }
    }
}
