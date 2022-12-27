#region References

using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

#endregion

namespace TypingPracticeApp.Domain
{
    [DebuggerDisplay("{OdaiText} ({ExpectedKeysText})")]
    public class PracticeItem : BindableBase
    {
        private string odaiText;
        private string yomiText;
        private string expectedKeysText;

        public static IEnumerable<PracticeItem> CreateDefaultPracticeItems()
        {
            //yield return new PracticeItem { OdaiText = "全キーテスト", YomiText = "てすと", ExpectedKeysText = "1234567890-qwertyuiop@asdfghjkl;:zxcvbnm,./" };
            yield return new PracticeItem { OdaiText = "あいうえお", YomiText = "あいうえお", ExpectedKeysText = "aiueo" };
            //yield return new PracticeItem { OdaiText = "本日は晴天なり", YomiText = "ほんじつはせいてんなり", ExpectedKeysText = "HONJITUHASEITENNNARI", };
            //yield return new PracticeItem { OdaiText = "かきくけこ", YomiText = "かきくけこ", ExpectedKeysText = "KAKIKUKEKO" };
            yield return new PracticeItem { OdaiText = "香りが引き立つ", YomiText = "かおりがひきたつ", ExpectedKeysText = "kaorigahikitatu" };
            //yield return new PracticeItem { OdaiText = "隠し味が効いている", YomiText = "かくしあじがきいている", ExpectedKeysText = "kakusiajigakiiteiru" };
            //yield return new PracticeItem { OdaiText = "シャキシャキした歯ごたえ", YomiText = "しゃきしゃきしたはごたえ", ExpectedKeysText = "SYAKISYAKISITAHAGOTAE" };
        }

        [JsonProperty(PropertyName = "odai")]
        public string OdaiText
        {
            get => this.odaiText;
            set => this.SetProperty(ref this.odaiText, value);
        }

        [JsonProperty(PropertyName = "yomi")]
        public string YomiText
        {
            get => this.yomiText;
            set => this.SetProperty(ref this.yomiText, value);
        }

        [JsonProperty(PropertyName = "keys")]
        public string ExpectedKeysText
        {
            get => this.expectedKeysText;
            set => this.SetProperty(ref this.expectedKeysText, value);
        }
    }
}
