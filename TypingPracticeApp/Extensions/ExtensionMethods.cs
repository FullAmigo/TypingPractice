#region References

using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using Reactive.Bindings.Notifiers;
using TypingPracticeApp.Domain;
using TypingPracticeApp.Services;

#endregion

namespace TypingPracticeApp.Extensions
{
    public static class ExtensionMethods
    {
        public static IObservable<KeyEventArgs> KeyDetectedAsObservable(this AppContextService self)
        {
            return self == null
                ? Observable.Empty<KeyEventArgs>()
                : Observable.FromEvent<EventHandler<KeyEventArgs>, KeyEventArgs>(
                    onNext => (sender, args) => onNext(args),
                    handler => self.KeyDetected += handler,
                    handler => self.KeyDetected -= handler);
        }

        public static IObservable<EventArgs> PracticeStartedAsObservable(this AppContextService self)
        {
            return self == null
                ? Observable.Empty<EventArgs>()
                : Observable.FromEvent<EventHandler, EventArgs>(
                    onNext => (sender, args) => onNext(args),
                    handler => self.PracticeStarted += handler,
                    handler => self.PracticeStarted -= handler);
        }

        public static IObservable<PracticeResultItem> PracticeItemResultedAsObservable(this AppContextService self)
        {
            return self == null
                ? Observable.Empty<PracticeResultItem>()
                : Observable.FromEvent<Action<PracticeResultItem>, PracticeResultItem>(
                    onNext => onNext,
                    handler => self.PracticeItemResulted += handler,
                    handler => self.PracticeItemResulted -= handler);
        }

        public static IObservable<EventArgs> PracticeRestartingAsObservable(this AppContextService self)
        {
            return self == null
                ? Observable.Empty<EventArgs>()
                : Observable.FromEvent<EventHandler, EventArgs>(
                    onNext => (sender, args) => onNext(args),
                    handler => self.PracticeRestarting += handler,
                    handler => self.PracticeRestarting -= handler);
        }

        public static void SwitchValueWith(this BooleanNotifier self, Action<bool> action)
        {
            action?.Invoke(self.Value);
            self.SwitchValue();
        }

        public static Key ToKey(this char self) => KeyMapping.KeyCharacterFingerMapping.ToDictionary(kvp => kvp.Value.Character, kvp => kvp.Key).TryGetValue(self, out var key) ? key : Key.None;

        public static bool TryToChar(this Key self, out char result)
        {
            result = '\0';
            var mapping = KeyMapping.KeyCharacterFingerMapping.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Character);
            return mapping.TryGetValue(self, out result);
        }
    }
}
