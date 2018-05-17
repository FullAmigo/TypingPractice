#region References

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using TypingPracticeApp.Domain;

#endregion

namespace TypingPracticeApp.Controls
{
    public class KeyToFingerConverter : IValueConverter
    {
        public static FingerKind Convert(Key key) => KeyMapping.KeyCharacterFingerMapping.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Finger).TryGetValue(key, out var finger) ? finger : FingerKind.None;

        /// <summary>
        /// 値を変換します。
        /// </summary>
        /// <param name="value">バインディング ソースによって生成された値。</param>
        /// <param name="targetType">バインディング ターゲット プロパティの型。</param>
        /// <param name="parameter">使用するコンバーター パラメーター。</param>
        /// <param name="culture">コンバーターで使用するカルチャ。</param>
        /// <returns>変換された値。メソッドが <see langword="null" /> を返す場合は、正しい null 値が使用されます。</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Key key)
            {
                return KeyMapping.KeyCharacterFingerMapping.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Finger).TryGetValue(key, out var finger) ? finger : FingerKind.None;
            }

            return null;
        }

        /// <summary>
        /// 値を変換します。
        /// </summary>
        /// <param name="value">バインディング ターゲットによって生成された値。</param>
        /// <param name="targetType">変換後の型。</param>
        /// <param name="parameter">使用するコンバーター パラメーター。</param>
        /// <param name="culture">コンバーターで使用するカルチャ。</param>
        /// <returns>変換された値。メソッドが <see langword="null" /> を返す場合は、正しい null 値が使用されます。</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
