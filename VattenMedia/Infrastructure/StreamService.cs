using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace VattenMedia.Infrastructure
{
    class StreamService : IStreamStarterService
    {
        private IStatusManager statusManager;
        private int runningProcesses;

        public event EventHandler<int> RunningProcessesChanged;

        public StreamService(IStatusManager statusManager)
        {
            this.statusManager = statusManager;
        }

        public void StartStream(Uri url, List<string> qualityOptions)
        {
            string process = Properties.Settings.Default.StreamUtilityPath;
            string args = $@" {url} {String.Join(",", qualityOptions)} --config {Properties.Settings.Default.StreamUtilityRcPath}";
            StartProcess(process, args);
        }

        private void StartProcess(string process, string args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = process,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process processTemp = new Process
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
