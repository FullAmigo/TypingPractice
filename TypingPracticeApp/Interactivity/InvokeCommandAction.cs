#region References

using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

#endregion

namespace TypingPracticeApp.Interactivity
{
    /// <summary>
    /// Trigger action that executes a command when invoked.
    /// It also maintains the Enabled state of the target control based on the CanExecute method of the command.
    /// </summary>
    public class InvokeCommandAction : TriggerAction<UIElement>
    {
        private ExecutableCommandBehavior _commandBehavior;

        /// <summary>
        /// Dependency property identifying if the associated element should automaticlaly be enabled or disabled based on the result of the Command's CanExecute
        /// </summary>
        public static readonly DependencyProperty AutoEnableProperty =
            DependencyProperty.Register(
                "AutoEnable",
                typeof(bool),
                typeof(InvokeCommandAction),
                new PropertyMetadata(true, (d, e) => ((InvokeCommandAction)d).OnAllowDisableChanged((bool)e.NewValue)));

        /// <summary>
        /// Gets or sets whther or not the associated element will automatically be enabled or disabled based on the result of the commands CanExecute
        /// </summary>
        public bool AutoEnable
        {
            get { return (bool)this.GetValue(InvokeCommandAction.AutoEnableProperty); }
            set { this.SetValue(InvokeCommandAction.AutoEnableProperty, value); }
        }

        private void OnAllowDisableChanged(bool newValue)
        {
            var behavior = this.GetOrCreateBehavior();
            if (behavior != null)
                behavior.AutoEnable = newValue;
        }

        /// <summary>
        /// Dependency property identifying the command to execute when invoked.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                "Command",
                typeof(ICommand),
                typeof(InvokeCommandAction),
                new PropertyMetadata(null, (d, e) => ((InvokeCommandAction)d).OnCommandChanged((ICommand)e.NewValue)));

        /// <summary>
        /// Gets or sets the command to execute when invoked.
        /// </summary>
        public ICommand Command
        {
            get { return this.GetValue(InvokeCommandAction.CommandProperty) as ICommand; }
            set { this.SetValue(InvokeCommandAction.CommandProperty, value); }
        }

        private void OnCommandChanged(ICommand newValue)
        {
            var behavior = this.GetOrCreateBehavior();
            if (behavior != null)
                behavior.Command = newValue;
        }

        /// <summary>
        /// Dependency property identifying the command parameter to supply on command execution.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                "CommandParameter",
                typeof(object),
                typeof(InvokeCommandAction),
                new PropertyMetadata(null, (d, e) => ((InvokeCommandAction)d).OnCommandParameterChanged(e.NewValue)));

        /// <summary>
        /// Gets or sets the command parameter to supply on command execution.
        /// </summary>
        public object CommandParameter
        {
            get { return this.GetValue(InvokeCommandAction.CommandParameterProperty); }
            set { this.SetValue(InvokeCommandAction.CommandParameterProperty, value); }
        }

        private void OnCommandParameterChanged(object newValue)
        {
            var behavior = this.GetOrCreateBehavior();
            if (behavior != null)
                behavior.CommandParameter = newValue;
        }

        /// <summary>
        /// Dependency property identifying the TriggerParameterPath to be parsed to identify the child property of the trigger parameter to be used as the command parameter.
        /// </summary>
        public static readonly DependencyProperty TriggerParameterPathProperty =
            DependencyProperty.Register(
                "TriggerParameterPath",
                typeof(string),
                typeof(InvokeCommandAction),
                new PropertyMetadata(null, (d, e) => { }));

        /// <summary>
        /// Gets or sets the TriggerParameterPath value.
        /// </summary>
        public string TriggerParameterPath
        {
            get { return this.GetValue(InvokeCommandAction.TriggerParameterPathProperty) as string; }
            set { this.SetValue(InvokeCommandAction.TriggerParameterPathProperty, value); }
        }

        /// <summary>
        /// Public wrapper of the Invoke method.
        /// </summary>
        public void InvokeAction(object parameter)
        {
            this.Invoke(parameter);
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="parameter">This parameter is passed to the command; the CommandParameter specified in the CommandParameterProperty is used for command invocation if not null.</param>
        protected override void Invoke(object parameter)
        {
            if (!string.IsNullOrEmpty(this.TriggerParameterPath))
            {
                //Walk the ParameterPath for nested properties.
                var propertyPathParts = this.TriggerParameterPath.Split('.');
                object propertyValue = parameter;
                foreach (var propertyPathPart in propertyPathParts)
                {
                    var propInfo = propertyValue.GetType().GetTypeInfo().GetProperty(propertyPathPart);
                    propertyValue = propInfo.GetValue(propertyValue);
                }

                parameter = propertyValue;
            }

            var behavior = this.GetOrCreateBehavior();

            if (behavior != null)
            {
                behavior.ExecuteCommand(parameter);
            }
        }

        /// <summary>
        /// Sets the Command and CommandParameter properties to null.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.Command = null;
            this.CommandParameter = null;

            this._commandBehavior = null;
        }

        /// <summary>
        /// This method is called after the behavior is attached.
        /// It updates the command behavior's Command and CommandParameter properties if necessary.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            // In case this action is attached to a target object after the Command and/or CommandParameter properties are set,
            // the command behavior would be created without a value for these properties.
            // To cover this scenario, the Command and CommandParameter properties of the behavior are updated here.
            var behavior = this.GetOrCreateBehavior();

            behavior.AutoEnable = this.AutoEnable;

            if (behavior.Command != this.Command)
                behavior.Command = this.Command;

            if (behavior.CommandParameter != this.CommandParameter)
                behavior.CommandParameter = this.CommandParameter;
        }

        private ExecutableCommandBehavior GetOrCreateBehavior()
        {
            // In case this method is called prior to this action being attached, 
            // the CommandBehavior would always keep a null target object (which isn't changeable afterwards).
            // Therefore, in that case the behavior shouldn't be created and this method should return null.
            if (this._commandBehavior == null && this.AssociatedObject != null)
            {
                this._commandBehavior = new ExecutableCommandBehavior(this.AssociatedObject);
            }

            return this._commandBehavior;
        }

        /// <summary>
        /// A CommandBehavior that exposes a public ExecuteCommand method. It provides the functionality to invoke commands and update Enabled state of the target control.
        /// It is not possible to make the <see cref="InvokeCommandAction" /> inherit from <see cref="CommandBehaviorBase{T}" />, since the <see cref="InvokeCommandAction" />
        /// must already inherit from <see cref="TriggerAction{T}" />, so we chose to follow the aggregation approach.
        /// </summary>
        private class ExecutableCommandBehavior : CommandBehaviorBase<UIElement>
        {
            /// <summary>
            /// Constructor specifying the target object.
            /// </summary>
            /// <param name="target">The target object the behavior is attached to.</param>
            public ExecutableCommandBehavior(UIElement target)
                : base(target)
            {
            }

            /// <summary>
            /// Executes the command, if it's set.
            /// </summary>
            public new void ExecuteCommand(object parameter)
            {
                base.ExecuteCommand(parameter);
            }
        }
    }
}
