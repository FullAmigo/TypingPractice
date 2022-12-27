#region References

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public static string AssemblyVersion => Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        public ObservableCollection<PracticeItem> PracticeItems => this.practiceItems;

        public static Task BeepAsync() => Task.Run(() => Console.Beep());

        private static string ResolveOdaiFilePath() => AppContextService.Filepath;

        private static async Task SavePracticeItemsAsync(string odaiFilePath, IEnumerable<PracticeItem> practiceItemsToSave)
        {
            using (var file = File.OpenWrite(odaiFilePath))
            using (var writer = new StreamWriter(file))
            {
                var json = JsonConvert.SerializeObject(practiceItemsToSave, Formatting.Indented);
                await writer.WriteAsync(json).ConfigureAwait(false);
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

            var loaded = false;
            var odaiFilePath = AppContextService.ResolveOdaiFilePath();
            if (File.Exists(odaiFilePath))
            {
                try
                {
                    await this.LoadPracticeItemsAsync(odaiFilePath).ConfigureAwait(false);
                    loaded = true;
                }
                catch (Exception ex)
                {
                    DebugLog.Print($"{ex}");
                }
            }

            if (!loaded)
            {
                try
                {
                    await this.CreateDefaultPracticeItemsAsync(odaiFilePath).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    DebugLog.Print($"{ex}");
                }
            }
        }

        private async Task LoadPracticeItemsAsync(string odaiFilePath)
        {
            using (var file = File.OpenText(odaiFilePath))
            {
                var json = await file.ReadToEndAsync().ConfigureAwait(false);
                var loadedItems = JsonConvert.DeserializeObject<PracticeItem[]>(json)?.ToList();
                //this.PracticeItems.AddRange(loadedItems);
                loadedItems?.ForEach(this.PracticeItems.Add);
            }
        }

        private async Task CreateDefaultPracticeItemsAsync(string odaiFilePath)
        {
            var defaultPracticeItems = PracticeItem.CreateDefaultPracticeItems().ToList();
            //this.PracticeItems.AddRange(defaultPracticeItems);
            defaultPracticeItems.ForEach(this.PracticeItems.Add);

            await AppContextService.SavePracticeItemsAsync(odaiFilePath, defaultPracticeItems).ConfigureAwait(false);
        }
    }
}
