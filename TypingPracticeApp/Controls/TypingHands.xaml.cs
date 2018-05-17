#region References

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LinqToXaml;
using TypingPracticeApp.Domain;

#endregion

namespace TypingPracticeApp.Controls
{
    /// <summary>
    /// Interaction logic for TypingHands
    /// </summary>
    public partial class TypingHands : UserControl
    {
        public static readonly DependencyProperty AccentKeyProperty = DependencyProperty.Register(nameof(TypingHands.AccentKey), typeof(Key), typeof(TypingHands), new PropertyMetadata(Key.None, TypingHands.AccentKeyPropertyChanged));
        public static readonly DependencyProperty IsNotMatchedAccentKeyProperty = DependencyProperty.Register(nameof(TypingHands.IsNotMatchedAccentKey), typeof(bool), typeof(TypingHands), new PropertyMetadata(false, TypingHands.IsNotMatchedAccentKeyPropertyChanged));
        private readonly List<TypingFinger> mapping;

        public TypingHands()
        {
            this.InitializeComponent();
            this.mapping = new List<TypingFinger>();

            void LoadedHandler(object sender, RoutedEventArgs e)
            {
                this.Loaded -= LoadedHandler;
                var typingFingerControls = (this.Content as DependencyObject)?.DescendantsAndSelf().OfType<TypingFinger>() ?? Enumerable.Empty<TypingFinger>();
                this.mapping.AddRange(typingFingerControls.Where(typingFinger => typingFinger.Finger != FingerKind.None));
                this.SetAccent(this.AccentKey, true);
                this.SetMissMatched(false);
            }

            this.Loaded += LoadedHandler;
        }

        public Key AccentKey
        {
            get => (Key)this.GetValue(TypingHands.AccentKeyProperty);
            set => this.SetValue(TypingHands.AccentKeyProperty, value);
        }

        public bool IsNotMatchedAccentKey
        {
            get => (bool)this.GetValue(TypingHands.IsNotMatchedAccentKeyProperty);
            set => this.SetValue(TypingHands.IsNotMatchedAccentKeyProperty, value);
        }

        private static void AccentKeyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (TypingHands)d;
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
            var instance = (TypingHands)d;
            if (e.NewValue is bool isNotMatched)
            {
                instance.SetMissMatched(isNotMatched);
            }
        }

        private void SetAccent(Key keyToSet, bool isAccent)
        {
            var finger = KeyToFingerConverter.Convert(keyToSet);
            var typingFingerControls = this.mapping.Where(typingFinger => typingFinger.Finger == finger).ToList();
            foreach (var typingFinger in typingFingerControls)
            {
                typingFinger.IsAccent = isAccent;
            }
        }

        private void SetMissMatched(bool isNotMatched)
        {
            var finger = KeyToFingerConverter.Convert(this.AccentKey);
            var typingFingerControls = this.mapping.Where(typingFinger => typingFinger.Finger == finger).ToList();
            foreach (var typingFinger in typingFingerControls)
            {
                typingFinger.IsMissMatch = isNotMatched;
            }
        }
    }
}
