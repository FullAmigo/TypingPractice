#region References

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using TypingPracticeApp.Domain;

#endregion

namespace TypingPracticeApp.Services
{
    public class AppContextService : DisposableBindableBase
    {
        private const string Filepath = @"Exercises.json";
        private readonly ObservableCollection<PracticeItem> practiceItems;

        public event EventHandler<KeyEventArgs> KeyDetected;
        public event EventHandler PracticeStarted;
        public event Action<PracticeResultItem> PracticeItemResulted;
        public event EventHandler PracticeRestarting;

        public AppContextService()
        {
            this.practiceItems = new ObservableCollection<PracticeItem>();
        }

        public ObservableCollection<PracticeItem> PracticeItems => this.practiceItems;

        public static Task BeepAsync() => Task.Run(() => Console.Beep());

        private static async Task SavePracticeItemsAsync(IEnumerable<PracticeItem> practiceItemsToSave)
        {
            try
            {
                using (var file = File.OpenWrite(AppContextService.Filepath))
                using (var writer = new StreamWriter(file))
                {
                    var json = JsonConvert.SerializeObject(practiceItemsToSave, Formatting.Indented);
                    await writer.WriteAsync(json);
                }
            }
            catch (Exception ex)
            {
                DebugLog.Print($"{ex}");
            }
        }

        public void PublishKeyDetected(KeyEventArgs e)
        {
            var eh = Interlocked.CompareExchange(ref this.KeyDetected, null, null);
            eh?.Invoke(this, e);
        }

        public void PublishPracticeStarted()
        {
            var eh = Interlocked.CompareExchange(ref this.PracticeStarted, null, null);
            eh?.Invoke(this, EventArgs.Empty);
        }

        public void PublishPracticeItemResulted(PracticeResultItem result)
        {
            var eh = Interlocked.CompareExchange(ref this.PracticeItemResulted, null, null);
            eh?.Invoke(result);
        }

        public void PublishPracticeRestarting()
        {
            var eh = Interlocked.CompareExchange(ref this.PracticeRestarting, null, null);
            eh?.Invoke(this, EventArgs.Empty);
        }

        public async Task LoadPracticeItemsAsync()
        {
            this.PracticeItems.Clear();
            try
            {
                using (var file = File.OpenText(AppContextService.Filepath))
                {
                    var json = await file.ReadToEndAsync();
                    var loadedItems = JsonConvert.DeserializeObject<PracticeItem[]>(json)?.ToList();
                    //this.PracticeItems.AddRange(loadedItems);
                    loadedItems?.ForEach(this.PracticeItems.Add);
                }
            }
            catch (Exception ex)
            {
                DebugLog.Print($"{ex}");
                var defaultPracticeItems = PracticeItem.CreateDefaultPracticeItems().ToList();
                await AppContextService.SavePracticeItemsAsync(defaultPracticeItems);
                //this.PracticeItems.AddRange(defaultPracticeItems);
                defaultPracticeItems.ForEach(this.PracticeItems.Add);
            }
        }
    }
}
