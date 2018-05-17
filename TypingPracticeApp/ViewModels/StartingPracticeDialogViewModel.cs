#region References

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

#endregion

namespace TypingPracticeApp.ViewModels
{
    public class StartingPracticeDialogViewModel : ViewModelBase
    {
        public StartingPracticeDialogViewModel()
        {
            this.RemainingCount = new ReactivePropertySlim<int>(3).AddTo(this.Disposables);
        }

        public ReactivePropertySlim<int> RemainingCount { get; }
    }
}
