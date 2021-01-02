using System;
using System.IO;
using VattenMedia.Core.Interfaces;

namespace VattenMedia.Infrastructure.Services
{
    public class Logger : ILogger
    {
        private string LogFileTodayDate => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToShortDateString() + ".log");

        public void LogError(string message)
        {
            var logFile = GetLogFile();

            using StreamWriter sw = File.AppendText(logFile);
            sw.WriteLine($"{DateTime.Now} - Error: {message}");
        }

        private string GetLogFile()
        {
            var logFile = LogFileTodayDate;

            if (!File.Exists(logFile))
            {
                using (File.Create(logFile)) { };
            }

            return logFile;
        }
    }
}
