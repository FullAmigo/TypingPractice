#region References

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TypingPracticeApp.Domain;

#endregion

namespace TypingPracticeApp.Controls
{
    public class TypingKey : ContentControl
    {
        public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(nameof(TypingKey.DisplayText), typeof(string), typeof(TypingKey), new PropertyMetadata(null));
        public static readonly DependencyProperty MappedKeyProperty = DependencyProperty.Register(nameof(TypingKey.MappedKey), typeof(Key), typeof(TypingKey), new PropertyMetadata(Key.None, TypingKey.MappedKeyPropertyChanged));
        public static readonly DependencyProperty FingerProperty = DependencyProperty.Register(nameof(TypingKey.Finger), typeof(FingerKind), typeof(TypingKey), new PropertyMetadata(FingerKind.None));
        public static readonly DependencyProperty IsAccentProperty = DependencyProperty.Register(nameof(TypingKey.IsAccent), typeof(bool), typeof(TypingKey), new PropertyMetadata(false));
        public static readonly DependencyProperty IsMissMatchProperty = DependencyProperty.Register(nameof(TypingKey.IsMissMatch), typeof(bool), typeof(TypingKey), new PropertyMetadata(false));

        static TypingKey()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(TypingKey), new FrameworkPropertyMetadata(typeof(TypingKey)));
        }

        public TypingKey()
        {
            void LoadedHandler(object sender, RoutedEventArgs e)
            {
                this.Loaded -= LoadedHandler;
                this.Finger = KeyToFingerConverter.Convert(this.MappedKey);
            }

            this.Loaded += LoadedHandler;
        }

        public string DisplayText
        {
            get => (string)this.GetValue(TypingKey.DisplayTextProperty);
            set => this.SetValue(TypingKey.DisplayTextProperty, value);
        }

        public Key MappedKey
        {
            get => (Key)this.GetValue(TypingKey.MappedKeyProperty);
            set => this.SetValue(TypingKey.MappedKeyProperty, value);
        }

        public FingerKind Finger
        {
            get => (FingerKind)this.GetValue(TypingKey.FingerProperty);
            set => this.SetValue(TypingKey.FingerProperty, value);
        }

        public bool IsAccent
        {
            get => (bool)this.GetValue(TypingKey.IsAccentProperty);
            set => this.SetValue(TypingKey.IsAccentProperty, value);
        }

        public bool IsMissMatch
        {
            get => (bool)this.GetValue(TypingKey.IsMissMatchProperty);
            set => this.SetValue(TypingKey.IsMissMatchProperty, value);
        }

        private static void MappedKeyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (TypingKey)d;
            if (e.NewValue is Key key)
            {
                instance.Finger = KeyToFingerConverter.Convert(key);
            }
        }
    }
}
