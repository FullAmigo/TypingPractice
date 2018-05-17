#region References

using System;
using System.Reactive.Linq;
using System.Threading;
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
    public class TypingConfirmContentViewModel : ViewModelDependencyBase
    {
        private readonly CancellationTokenSource cts;
        private readonly BusyNotifier busyNotifier;

        public TypingConfirmContentViewModel()
            : this(null)
        {
            DesignerSupport.ThrowIfNotInDesignMode();
        }

        public TypingConfirmContentViewModel(AppContextService appService)
            : base(appService)
        {
            this.cts = new CancellationTokenSource();
            this.cts.AddTo(this.Disposables);
            this.busyNotifier = new BusyNotifier();

            this.DialogViewModel = new ReactivePropertySlim<StartingPracticeDialogViewModel>().AddTo(this.Disposables);
            this.IsDialogOpen = new ReactivePropertySlim<bool>().AddTo(this.Disposables);

            this.Initialize(appService);
        }

        public ReactivePropertySlim<StartingPracticeDialogViewModel> DialogViewModel { get; }
        public ReactivePropertySlim<bool> IsDialogOpen { get; }

        protected override void DisposeManagedInstances()
        {
            DebugLog.Print($"キャンセル！");
            this.cts.Cancel();
            base.DisposeManagedInstances();
        }

        private void Initialize(AppContextService appService)
        {
            appService?.KeyDetectedAsObservable()
                .Where(_ => this.IsVisibledNotifier.Value)
                .Where(e => e.Key == Key.Space)
                .Subscribe(async e => await this.SubscribeKeyDetectedAsync(e))
                .AddTo(this.Disposables);
        }

        private async Task SubscribeKeyDetectedAsync(KeyEventArgs e)
        {
            if (this.busyNotifier.IsBusy)
            {
                return;
            }

            try
            {
                await this.ShowStartingPracticeDialogAsync(e.Key, this.cts.Token);
            }
            catch (Exception ex)
            {
                DebugLog.Print($"{this.GetType().Name}.{nameof(this.SubscribeKeyDetectedAsync)} is detected Exception on Subscribe: {ex}");
            }
        }

        private async Task ShowStartingPracticeDialogAsync(Key key, CancellationToken token)
        {
            DebugLog.Print($"{this.GetType().Name}.{nameof(this.ShowStartingPracticeDialogAsync)}: {key}");
            using (this.busyNotifier.ProcessStart())
            using (var dialogViewModel = new StartingPracticeDialogViewModel())
            {
                token.ThrowIfCancellationRequested();

                this.DialogViewModel.Value = dialogViewModel;
                this.IsDialogOpen.Value = true;

                // カウントダウン (3 [sec]) を表示します。
                for (var remainingCount = 3; remainingCount > 0; remainingCount--)
                {
                    dialogViewModel.RemainingCount.Value = remainingCount;
                    await Task.Delay(TimeSpan.FromSeconds(1), token);
                }

                this.IsDialogOpen.Value = false;
                this.DialogViewModel.Value = null;

                this.AppService?.PublishPracticeStarted();
            }
        }
    }
}
