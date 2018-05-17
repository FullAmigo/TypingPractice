#region References

using System;
using System.Windows;
using System.Windows.Input;

#endregion

namespace TypingPracticeApp.Interactivity
{
    /// <summary>
    /// Base behavior to handle connecting a <see cref="System.Windows.Controls.Control" /> to a Command.
    /// </summary>
    /// <typeparam name="T">The target object must derive from Control</typeparam>
    /// <remarks>
    /// CommandBehaviorBase can be used to provide new behaviors for commands.
    /// </remarks>
    public class CommandBehaviorBase<T>
        where T : UIElement
    {
        private ICommand _command;
        private object _commandParameter;
        private readonly WeakReference _targetObject;
        private readonly EventHandler _commandCanExecuteChangedHandler;

        /// <summary>
        /// Constructor specifying the target object.
        /// </summary>
        /// <param name="targetObject">The target object the behavior is attached to.</param>
        public CommandBehaviorBase(T targetObject)
        {
            this._targetObject = new WeakReference(targetObject);
            this._commandCanExecuteChangedHandler = this.CommandCanExecuteChanged;
        }

        bool _autoEnabled = true;

        public bool AutoEnable
        {
            get => this._autoEnabled;
            set
            {
                this._autoEnabled = value;
                this.UpdateEnabledState();
            }
        }

        /// <summary>
        /// Corresponding command to be execute and monitored for <see cref="ICommand.CanExecuteChanged" />
        /// </summary>
        public ICommand Command
        {
            get => this._command;
            set
            {
                if (this._command != null)
                {
                    this._command.CanExecuteChanged -= this._commandCanExecuteChangedHandler;
                }

                this._command = value;
                if (this._command != null)
                {
                    this._command.CanExecuteChanged += this._commandCanExecuteChangedHandler;
                    this.UpdateEnabledState();
                }
            }
        }

        /// <summary>
        /// The parameter to supply the command during execution
        /// </summary>
        public object CommandParameter
        {
            get => this._commandParameter;
            set
            {
                if (this._commandParameter != value)
                {
                    this._commandParameter = value;
                    this.UpdateEnabledState();
                }
            }
        }

        /// <summary>
        /// Object to which this behavior is attached.
        /// </summary>
        protected T TargetObject => this._targetObject.Target as T;

        /// <summary>
        /// Updates the target object's IsEnabled property based on the commands ability to execute.
        /// </summary>
        protected virtual void UpdateEnabledState()
        {
            if (this.TargetObject == null)
            {
                this.Command = null;
                this.CommandParameter = null;
            }
            else if (this.Command != null)
            {
                if (this.AutoEnable)
                {
                    this.TargetObject.IsEnabled = this.Command.CanExecute(this.CommandParameter);
                }
            }
        }

        private void CommandCanExecuteChanged(object sender, EventArgs e)
        {
            this.UpdateEnabledState();
        }

        /// <summary>
        /// Executes the command, if it's set, providing the <see cref="CommandParameter" />
        /// </summary>
        protected virtual void ExecuteCommand(object parameter)
        {
            this.Command?.Execute(this.CommandParameter ?? parameter);
        }
    }
}
