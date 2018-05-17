#region References

using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using TypingPracticeApp.Extensions;
using TypingPracticeApp.Services;

#endregion

namespace TypingPracticeApp.ViewModels
{
    [DebuggerDisplay("ExpectedKey={ExpectedKey.Value} IsInputted={IsInputted.Value} IsMatch={IsMatch.Value}")]
    public class PracticeKeyInfoViewModel : ViewModelDependencyBase
    {
        public PracticeKeyInfoViewModel(AppContextService appService, char expectedKey, bool isCurrent)
            : base(appService)
        {
            this.ExpectedKey = new ReactivePropertySlim<char>(expectedKey).ToReadOnlyReactivePropertySlim().AddTo(this.Disposables);
            this.InputtedKey = new ReactivePropertySlim<char?>().AddTo(this.Disposables);
            this.IsCurrent = new ReactivePropertySlim<bool>(isCurrent).AddTo(this.Disposables);
            this.IsInputted = this.InputtedKey.Select(actual => actual.HasValue).ToReadOnlyReactivePropertySlim().AddTo(this.Disposables);
            this.IsMatch = this.ExpectedKey
                .CombineLatest(this.InputtedKey.Where(actual => actual.HasValue), (expect, actual) => (bool?)Nullable.Equals(expect, actual))
                .ToReadOnlyReactivePropertySlim()
                .AddTo(this.Disposables);
            this.KeyInputtedCount = new ReactivePropertySlim<int>().AddTo(this.Disposables);
            this.KeyMistakedCount = new ReactivePropertySlim<int>().AddTo(this.Disposables);
        }

        public ReadOnlyReactivePropertySlim<char> ExpectedKey { get; }
        public ReactivePropertySlim<char?> InputtedKey { get; }
        public ReactivePropertySlim<bool> IsCurrent { get; }
        public ReadOnlyReactivePropertySlim<bool> IsInputted { get; }
        public ReadOnlyReactivePropertySlim<bool?> IsMatch { get; }
        public ReactivePropertySlim<int> KeyInputtedCount { get; }
        public ReactivePropertySlim<int> KeyMistakedCount { get; }

        internal async Task<bool> SetInputtedKeyAsync(Key actualKey, BooleanNotifier notMatchedKeyNotifier, BooleanNotifier keyMissingNotifier)
        {
            if (actualKey.TryToChar(out var actualChar))
            {
                this.KeyInputtedCount.Value++;
                this.InputtedKey.Value = actualChar;
                if (actualKey == this.ExpectedKey.Value.ToKey())
                {
                    notMatchedKeyNotifier.TurnOff();
                    return true;
                }

                this.KeyMistakedCount.Value++;
                notMatchedKeyNotifier.TurnOn();
                keyMissingNotifier.TurnOn();
                var task1 = AppContextService.BeepAsync();
                var task2 = Task.Delay(TimeSpan.FromSeconds(0.2));
                await Task.WhenAny(task1, task2);
                keyMissingNotifier.TurnOff();
            }

            return false;
        }
    }
}
