#region References

using System;
using System.Threading.Tasks;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using TypingPracticeApp.Domain;
using TypingPracticeApp.Services;

#endregion

namespace TypingPracticeApp.ViewModels
{
    internal interface IViewModelDependencyBase : IDisposable
    {
        AppContextService AppService { get; }
    }

    public abstract class ViewModelBase : DisposableBindableBase
    {
        protected ViewModelBase()
        {
            this.IsVisibledNotifier = new BooleanNotifier();
            this.ViewLoadedCommand = new AsyncReactiveCommand().AddTo(this.Disposables);
            this.ViewLoadedCommand.Subscribe(this.OnViewLoadedAsync).AddTo(this.Disposables);

            this.ViewUnloadedCommand = new AsyncReactiveCommand().AddTo(this.Disposables);
            this.ViewUnloadedCommand.Subscribe(this.OnViewUnloadedAsync).AddTo(this.Disposables);
        }

        public AsyncReactiveCommand ViewLoadedCommand { get; }
        public AsyncReactiveCommand ViewUnloadedCommand { get; }
        protected BooleanNotifier IsVisibledNotifier { get; }

        protected virtual Task OnViewLoadedAsync()
        {
            DebugLog.Print($"{this.GetType().Name}.{nameof(this.OnViewLoadedAsync)}.");
            this.IsVisibledNotifier.TurnOn();
            return Task.CompletedTask;
        }

        protected virtual Task OnViewUnloadedAsync()
        {
            DebugLog.Print($"{this.GetType().Name}.{nameof(this.OnViewUnloadedAsync)}.");
            this.IsVisibledNotifier.TurnOff();
            return Task.CompletedTask;
        }
    }

    public abstract class ViewModelDependencyBase : ViewModelBase, IViewModelDependencyBase
    {
        protected ViewModelDependencyBase(AppContextService appService)
        {
            this.AppService = appService;
        }

        public AppContextService AppService { get; }
    }
}
