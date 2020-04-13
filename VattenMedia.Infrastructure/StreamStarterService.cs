using System;
using System.Collections.Generic;
using System.Diagnostics;
using VattenMedia.Core.Entities;
using VattenMedia.Core.Interfaces;

namespace VattenMedia.Infrastructure
{
    internal class StreamStarterService : IStreamStarterService
    {
        private readonly AppConfiguration appConfiguration;
        private readonly IStatusManager statusManager;
        private int runningProcesses;

        public event EventHandler<int> RunningProcessesChanged;

        public StreamStarterService(IStatusManager statusManager, AppConfiguration appConfiguration)
        {
            this.statusManager = statusManager;
            this.appConfiguration = appConfiguration;
        }

        public void StartStream(Uri url, List<string> qualityOptions)
        {
            var process = appConfiguration.StreamUtilityPath;
            var args = $@" {url} {string.Join(",", qualityOptions)} --no-version-check --config {appConfiguration.StreamUtilityRcPath}";
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
                args += " --player-passthrough=hls --hls-segment-threads 4";
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
            processTemp.ErrorDataReceived += ProcessTemp_DataReceived;

            try
            {
                IncrementProcesses();
                processTemp.Start();
                processTemp.BeginOutputReadLine();
                processTemp.BeginErrorReadLine();
            }
            catch (Exception e)
            {
                statusManager.ChangeStatusText($"Error: Could not start process. Exception: {e}");
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
    }
}
