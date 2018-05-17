#region References

using System;
using System.Collections.Generic;
using System.Windows.Input;

#endregion

namespace TypingPracticeApp.Domain
{
    public class CharacterFinger
    {
        public CharacterFinger(char character, FingerKind finger)
        {
            this.Character = character;
            this.Finger = finger;
        }
        public char Character { get; }
        public FingerKind Finger { get; }
    }
    public static class KeyMapping
    {
        private static readonly Lazy<Dictionary<Key, CharacterFinger>> LazyKeyCharacterFingerMapping = new Lazy<Dictionary<Key, CharacterFinger>>(KeyMapping.CreateKeyCharacterFingerMapping, false);

        public static Dictionary<Key, CharacterFinger> KeyCharacterFingerMapping => KeyMapping.LazyKeyCharacterFingerMapping.Value;

        private static Dictionary<Key, CharacterFinger> CreateKeyCharacterFingerMapping()
        {
            return new Dictionary<Key, CharacterFinger>
            {
                { Key.D1, new CharacterFinger('1', FingerKind.L04) },
                { Key.D2, new CharacterFinger('2', FingerKind.L03) },
                { Key.D3, new CharacterFinger('3', FingerKind.L02) },
                { Key.D4, new CharacterFinger('4', FingerKind.L01) },
                { Key.D5, new CharacterFinger('5', FingerKind.L01) },
                { Key.D6, new CharacterFinger('6', FingerKind.R01) },
                { Key.D7, new CharacterFinger('7', FingerKind.R01) },
                { Key.D8, new CharacterFinger('8', FingerKind.R02) },
                { Key.D9, new CharacterFinger('9', FingerKind.R03) },
                { Key.D0, new CharacterFinger('0', FingerKind.R04) },
                { Key.OemMinus, new CharacterFinger('-', FingerKind.R04) },
                { Key.Q, new CharacterFinger('Q', FingerKind.L04) },
                { Key.W, new CharacterFinger('W', FingerKind.L03) },
                { Key.E, new CharacterFinger('E', FingerKind.L02) },
                { Key.R, new CharacterFinger('R', FingerKind.L01) },
                { Key.T, new CharacterFinger('T', FingerKind.L01) },
                { Key.Y, new CharacterFinger('Y', FingerKind.R01) },
                { Key.U, new CharacterFinger('U', FingerKind.R01) },
                { Key.I, new CharacterFinger('I', FingerKind.R02) },
                { Key.O, new CharacterFinger('O', FingerKind.R03) },
                { Key.P, new CharacterFinger('P', FingerKind.R04) },
                { Key.Oem3, new CharacterFinger('@', FingerKind.R04) },
                { Key.A, new CharacterFinger('A', FingerKind.L04) },
                { Key.S, new CharacterFinger('S', FingerKind.L03) },
                { Key.D, new CharacterFinger('D', FingerKind.L02) },
                { Key.F, new CharacterFinger('F', FingerKind.L01) },
                { Key.G, new CharacterFinger('G', FingerKind.L01) },
                { Key.H, new CharacterFinger('H', FingerKind.R01) },
                { Key.J, new CharacterFinger('J', FingerKind.R01) },
                { Key.K, new CharacterFinger('K', FingerKind.R02) },
                { Key.L, new CharacterFinger('L', FingerKind.R03) },
                { Key.OemPlus, new CharacterFinger(';', FingerKind.R04) },
                { Key.Oem1, new CharacterFinger(':', FingerKind.R04) },
                { Key.Z, new CharacterFinger('Z', FingerKind.L04) },
                { Key.X, new CharacterFinger('X', FingerKind.L03) },
                { Key.C, new CharacterFinger('C', FingerKind.L02) },
                { Key.V, new CharacterFinger('V', FingerKind.L01) },
                { Key.B, new CharacterFinger('B', FingerKind.L01) },
                { Key.N, new CharacterFinger('N', FingerKind.R01) },
                { Key.M, new CharacterFinger('M', FingerKind.R01) },
                { Key.OemComma, new CharacterFinger(',', FingerKind.R02) },
                { Key.OemPeriod, new CharacterFinger('.', FingerKind.R03) },
                { Key.OemQuestion, new CharacterFinger('/', FingerKind.R04) },
                { Key.Space, new CharacterFinger(' ', FingerKind.L00) },
            };
        }
    }
}
