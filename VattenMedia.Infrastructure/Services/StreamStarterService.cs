using System;
using System.Collections.Generic;
using System.Diagnostics;
using VattenMedia.Core.Entities;
using VattenMedia.Core.Interfaces;

namespace VattenMedia.Infrastructure.Services
{
    internal class StreamStarterService : IStreamStarterService
    {
        private readonly AppConfiguration appConfiguration;
        private readonly IStatusTextService statusManager;
        private readonly ILogger logger;
        private int runningProcesses;

        public event EventHandler<int> RunningProcessesChanged;

        public StreamStarterService(
            IStatusTextService statusManager,
            ILogger logger,
            AppConfiguration appConfiguration)
        {
            this.statusManager = statusManager;
            this.logger = logger;
            this.appConfiguration = appConfiguration;
        }

        public void StartStream(Uri url, List<string> qualityOptions)
        {
            var process = appConfiguration.StreamUtilityPath;
            var userArgs = appConfiguration.StreamStarterOptionalCommandLineArguments.Split(',');
            var args = $@" {url} {string.Join(",", qualityOptions)} {string.Join(' ', userArgs)} --config {appConfiguration.StreamUtilityRcPath}";
            args = SetArgsIfVod(args, url);
            StartProcess(process, args);
        }

        /// <summary>
        /// Arguments specifically if the started video is a VOD
        /// </summary>
        private string SetArgsIfVod(string args, Uri url)
        {
            if (url.ToString().ToUpperInvariant().Contains("/VIDEOS/"))
            {
                args += " --player-passthrough=hls --stream-segment-threads 4";
            }
            return args;
        }

        private void StartProcess(string process, string args)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = process,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var processTemp = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };

            processTemp.Exited += ProcessTemp_Exited;
            processTemp.OutputDataReceived += ProcessTemp_DataReceived;
            processTemp.ErrorDataReceived += ProcessTemp_DataReceivedError;

            try
            {
                IncrementProcesses();
                processTemp.Start();
                processTemp.BeginOutputReadLine();
                processTemp.BeginErrorReadLine();
            }
            catch (Exception e)
            {
                var errorMessage = $"Error: Could not start process. Exception: {e}";
                statusManager.ChangeStatusText(errorMessage);
                logger.LogError(errorMessage);
            }
        }

        private void IncrementProcesses()
        {
            runningProcesses++;
            RunningProcessesChanged(this, runningProcesses);
        }

        private void ProcessTemp_Exited(object sender, EventArgs e)
        {
            runningProcesses--;
            RunningProcessesChanged(this, runningProcesses);
        }

        private void ProcessTemp_DataReceived(object sender, DataReceivedEventArgs e)
        {
            statusManager.ChangeStatusText(e.Data, TimeSpan.FromSeconds(2));
        }

        private void ProcessTemp_DataReceivedError(object sender, DataReceivedEventArgs e)
        {
            statusManager.ChangeStatusText(e.Data, TimeSpan.FromSeconds(10));
            logger.LogError(e.Data);
        }
    }
}
