#region References

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

#endregion

namespace TypingPracticeApp.Domain
{
    [DebuggerDisplay("{OdaiText} ({ExpectedKeysText})")]
    public class PracticeResultItem : BindableBase
    {
        private DateTime? startDateTime;
        private DateTime? endDateTime;
        private TimeSpan? elapsed;

        public PracticeResultItem(IEnumerable<PracticeResultKeyInfo> practiceResultKeyInfos)
        {
            var items = (practiceResultKeyInfos ?? Enumerable.Empty<PracticeResultKeyInfo>()).ToList();

            this.PracticeResultKeyInfos = new ObservableCollection<PracticeResultKeyInfo>(items);
            this.ExpectedKeysText = new string(items.Select(item => item.ExpectedKey).ToArray(), 0, items.Count);
            this.KeyInputtedCount = items.Sum(item => item.KeyInputtedCount);
            this.KeyMistakedCount = items.Sum(item => item.KeyMistakedCount);
        }

        public string OdaiText { get; set; }
        public string YomiText { get; set; }

        public DateTime? StartDateTime
        {
            get => this.startDateTime;
            set
            {
                this.startDateTime = value;
                this.CalculateElapsed();
            }
        }

        public DateTime? EndDateTime
        {
            get => this.endDateTime;
            set
            {
                this.endDateTime = value;
                this.CalculateElapsed();
            }
        }

        public TimeSpan? Elapsed
        {
            get => this.elapsed;
            private set => this.SetProperty(ref this.elapsed, value);
        }

        public ObservableCollection<PracticeResultKeyInfo> PracticeResultKeyInfos { get; }
        public string ExpectedKeysText { get; }
        public int KeyInputtedCount { get; }
        public int KeyMistakedCount { get; }

        private void CalculateElapsed()
        {
            var ticks1 = this.startDateTime?.Ticks;
            var ticks2 = this.endDateTime?.Ticks;
            if (ticks1.HasValue && ticks2.HasValue)
            {
                var lowerDatiTime = new DateTime(Math.Min(ticks1.Value, ticks2.Value));
                var upperDatiTime = new DateTime(Math.Max(ticks1.Value, ticks2.Value));
                this.Elapsed = upperDatiTime - lowerDatiTime;
            }
            else
            {
                this.Elapsed = null;
            }
        }
    }

    public class PracticeResultKeyInfo : BindableBase
    {
        public char ExpectedKey { get; set; }
        public int KeyInputtedCount { get; set; }
        public int KeyMistakedCount { get; set; }
        public bool HasKeyMistaked => this.KeyMistakedCount > 0;
    }

    public class PracticeResultSummary : BindableBase
    {
        private PracticeResultSummary()
        {
        }

        public string Title { get; private set; }
        public string Text { get; private set; }

        public static IEnumerable<PracticeResultSummary> CreateSummariesBy(IEnumerable<PracticeResultItem> practiceResultItems)
        {
            var resultItems = (practiceResultItems ?? Enumerable.Empty<PracticeResultItem>()).ToList();

            var totalElapsed = TimeSpan.FromTicks(resultItems.Select(item => item.Elapsed?.Ticks ?? 0).Sum());
            var totalMinutesText = $"{(int)totalElapsed.TotalMinutes:n0} 分 {totalElapsed.Seconds,2:d2} 秒 {Math.Round(totalElapsed.Milliseconds / 10d, MidpointRounding.AwayFromZero)}";
            yield return new PracticeResultSummary { Title="入力時間", Text = $"{totalMinutesText}"};

            var totalKeyInputtedCount = resultItems.Select(item => item.KeyInputtedCount).Sum();
            yield return new PracticeResultSummary { Title = "キー入力回数", Text = $"{totalKeyInputtedCount}" };

            var totalKeyMistakedCount = resultItems.Select(item => item.KeyMistakedCount).Sum();
            yield return new PracticeResultSummary { Title = "入力ミス回数", Text = $"{totalKeyMistakedCount}" };

            var present = totalKeyInputtedCount==0 ? 0 : 100 * (totalKeyInputtedCount - totalKeyMistakedCount) / totalKeyInputtedCount;
            yield return new PracticeResultSummary { Title = "正確率", Text = $"{present} ％" };

            var keyMistakedKeyInfos = resultItems.SelectMany(item => item.PracticeResultKeyInfos).Where(info => info.HasKeyMistaked).OrderByDescending(info => info.KeyMistakedCount).Take(10).ToList();
            var weakKeysText = keyMistakedKeyInfos.Any() ? string.Join(" ", keyMistakedKeyInfos.Select(info => $"{info.ExpectedKey}").ToArray()) : "－";
            yield return new PracticeResultSummary { Title = "苦手キー", Text = $"{weakKeysText}" };
        }
    }
}
