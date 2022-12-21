#region References

using System;
using System.Windows;
using Microsoft.Xaml.Behaviors;
using TypingPracticeApp.Domain;

#endregion

namespace TypingPracticeApp.Interactivity
{
    public class ViewModelBehavior : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            if (this.AssociatedObject != null)
            {
                this.AssociatedObject.Unloaded += this.AssociatedObjectOnUnloaded;
            }
        }

        private void AssociatedObjectOnUnloaded(object sender, RoutedEventArgs e)
        {
            if (this.AssociatedObject != null)
            {
                this.AssociatedObject.Unloaded -= this.AssociatedObjectOnUnloaded;
            }

            if (this.AssociatedObject?.DataContext is IDisposable viewModel)
            {
                DebugLog.Print($"{this.GetType().Name}.{nameof(this.AssociatedObjectOnUnloaded)}: Calling {viewModel.GetType().Name}.{nameof(viewModel.Dispose)}()");
                viewModel.Dispose();
            }
        }
    }
}
