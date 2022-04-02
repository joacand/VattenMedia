using System;
using System.Threading;
using VattenMedia.Core.Interfaces;

namespace VattenMedia.Infrastructure.Services
{
    internal class PingSender
    {
        private readonly IChatClient _chatClient;
        private readonly string _pingUrl;
        private readonly int _interval;
        private Thread pingSender;

        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;

        public PingSender(IChatClient chatCLient, string pingUrl, int interval)
        {
            _chatClient = chatCLient;
            _pingUrl = pingUrl;
            _interval = interval;
        }

        public void Start()
        {
            Stop();
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;

            pingSender = new Thread(new ThreadStart(Run))
            {
                IsBackground = true
            };
            pingSender.Start();
        }

        public void Stop()
        {
            if (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }
        }

        public void Run()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _chatClient.SendIrcMessage($"PING {_pingUrl}");
                Thread.Sleep(_interval);
            }
        }
    }
}
