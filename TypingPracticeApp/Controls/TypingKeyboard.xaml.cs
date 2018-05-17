#region References

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LinqToXaml;

#endregion

namespace TypingPracticeApp.Controls
{
    /// <summary>
    /// Interaction logic for TypingKeyboard
    /// </summary>
    public partial class TypingKeyboard : UserControl
    {
        public static readonly DependencyProperty AccentKeyProperty = DependencyProperty.Register(nameof(TypingKeyboard.AccentKey), typeof(Key), typeof(TypingKeyboard), new PropertyMetadata(Key.None, TypingKeyboard.AccentKeyPropertyChanged));
        public static readonly DependencyProperty IsNotMatchedAccentKeyProperty = DependencyProperty.Register(nameof(TypingKeyboard.IsNotMatchedAccentKey), typeof(bool), typeof(TypingKeyboard), new PropertyMetadata(false, TypingKeyboard.IsNotMatchedAccentKeyPropertyChanged));

        private readonly List<TypingKey> mapping;

        public TypingKeyboard()
        {
            this.InitializeComponent();
            this.mapping = new List<TypingKey>();

            void LoadedHandler(object sender, RoutedEventArgs e)
            {
                this.Loaded -= LoadedHandler;

                var typingKeyControls = (this.Content as DependencyObject)?.DescendantsAndSelf().OfType<TypingKey>() ?? Enumerable.Empty<TypingKey>();
                this.mapping.AddRange(typingKeyControls.Where(typingKey => typingKey.MappedKey != Key.None));
                this.SetAccent(this.AccentKey, true);
                this.SetMissMatched(false);
            }

            this.Loaded += LoadedHandler;
        }

        public Key AccentKey
        {
            get => (Key)this.GetValue(TypingKeyboard.AccentKeyProperty);
            set => this.SetValue(TypingKeyboard.AccentKeyProperty, value);
        }

        public bool IsNotMatchedAccentKey
        {
            get => (bool)this.GetValue(TypingKeyboard.IsNotMatchedAccentKeyProperty);
            set => this.SetValue(TypingKeyboard.IsNotMatchedAccentKeyProperty, value);
        }

        private static void AccentKeyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (TypingKeyboard)d;
            if (e.OldValue is Key oldKey)
            {
                instance.SetAccent(oldKey, false);
            }

            if (e.NewValue is Key newKey)
            {
                instance.SetAccent(newKey, true);
            }
        }

        private static void IsNotMatchedAccentKeyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (TypingKeyboard)d;
            if (e.NewValue is bool isNotMatched)
            {
                instance.SetMissMatched(isNotMatched);
            }
        }

        private void SetAccent(Key keyToSet, bool isAccent)
        {
            var typingKeyControls = this.mapping.Where(typingKey => typingKey.MappedKey == keyToSet).ToList();
            foreach (var typingKey in typingKeyControls)
            {
                typingKey.IsAccent = isAccent;
            }
        }

        private void SetMissMatched(bool isNotMatched)
        {
            var typingKeyControls = this.mapping.Where(typingKey => typingKey.MappedKey == this.AccentKey).ToList();
            foreach (var typingKey in typingKeyControls)
            {
                typingKey.IsMissMatch = isNotMatched;
            }
        }
    }
}
