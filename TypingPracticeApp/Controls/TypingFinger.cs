#region References

using System.Windows;
using System.Windows.Controls;
using TypingPracticeApp.Domain;

#endregion

namespace TypingPracticeApp.Controls
{
    public class TypingFinger : ContentControl
    {
        public static readonly DependencyProperty FingerProperty = DependencyProperty.Register(nameof(TypingFinger.Finger), typeof(FingerKind), typeof(TypingFinger), new PropertyMetadata(FingerKind.None));
        public static readonly DependencyProperty IsAccentProperty = DependencyProperty.Register(nameof(TypingFinger.IsAccent), typeof(bool), typeof(TypingFinger), new PropertyMetadata(false));
        public static readonly DependencyProperty IsMissMatchProperty = DependencyProperty.Register(nameof(TypingFinger.IsMissMatch), typeof(bool), typeof(TypingFinger), new PropertyMetadata(false));
        public static readonly DependencyProperty OffsetThicknessProperty = DependencyProperty.Register(nameof(TypingFinger.OffsetThickness), typeof(Thickness), typeof(TypingFinger), new PropertyMetadata());

        static TypingFinger()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(TypingFinger), new FrameworkPropertyMetadata(typeof(TypingFinger)));
        }

        public FingerKind Finger
        {
            get => (FingerKind)this.GetValue(TypingFinger.FingerProperty);
            set => this.SetValue(TypingFinger.FingerProperty, value);
        }

        public bool IsAccent
        {
            get => (bool)this.GetValue(TypingFinger.IsAccentProperty);
            set => this.SetValue(TypingFinger.IsAccentProperty, value);
        }

        public bool IsMissMatch
        {
            get => (bool)this.GetValue(TypingFinger.IsMissMatchProperty);
            set => this.SetValue(TypingFinger.IsMissMatchProperty, value);
        }

        public Thickness OffsetThickness
        {
            get => (Thickness)this.GetValue(TypingFinger.OffsetThicknessProperty);
            set => this.SetValue(TypingFinger.OffsetThicknessProperty, value);
        }
    }
}
