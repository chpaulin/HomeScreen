using System;
using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;
using System.IO;
using System.Text;
using Windows.Storage;
using System.Threading.Tasks;

namespace HomeScreen
{
    internal class LogSink : ILogEventSink
    {
        private readonly StorageFile _logFile;

        private LogSink(StorageFile logFile)
        {
            _logFile = logFile;
        }

        public async void Emit(LogEvent logEvent)
        {
            using (var stream = await _logFile.OpenStreamForWriteAsync())
            using (var writer = new StreamWriter(stream))
            {
                await writer.WriteLineAsync(logEvent.RenderMessage());
            }
        }

        public static async Task<LogSink> CreateSink()
        {
            var storageFolder = ApplicationData.Current.LocalFolder;
            var logFile = await storageFolder.CreateFileAsync("log.txt", CreationCollisionOption.ReplaceExisting);

            return new LogSink(logFile);
        }
    }
}