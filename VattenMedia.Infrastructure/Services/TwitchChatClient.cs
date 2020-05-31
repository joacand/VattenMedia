using System;
using System.IO;
using System.Net.Sockets;
using VattenMedia.Core.Exceptions;
using VattenMedia.Core.Interfaces;

namespace VattenMedia.Infrastructure.Services
{
    internal class TwitchChatClient : ITwitchChatClient
    {
        private string _userName;
        private string _channel;
        private TcpClient _tcpClient;
        private StreamReader _inputStream;
        private StreamWriter _outputStream;
        private PingSender _pingSender;
        private bool _started;

        private static string Address => "irc.twitch.tv";
        private static int Port => 6667;
        private static int PingInterval => 5 * 60 * 1000; // 5 minutes

        public TwitchChatClient()
        {
            _pingSender = new PingSender(this, Address, PingInterval);
        }

        public void Start(string userName, string accessToken, string channel)
        {
            if (_started)
            {
                throw new ChatViewerException($"{nameof(TwitchChatClient)} is already running");
            }

            _userName = userName;
            _channel = channel;

            _tcpClient = new TcpClient(Address, Port);
            _inputStream = new StreamReader(_tcpClient.GetStream());
            _outputStream = new StreamWriter(_tcpClient.GetStream());

            // Try to join the room
            _outputStream.WriteLine($"PASS oauth:{accessToken}");
            _outputStream.WriteLine($"NICK {userName}");
            _outputStream.WriteLine($"USER {userName} 8 * :{userName}");
            _outputStream.WriteLine($"JOIN #{channel}");
            _outputStream.Flush();

            _pingSender.Start();
            _started = true;
        }

        public void Stop()
        {
            _pingSender.Stop();
            _tcpClient?.Dispose();
            _inputStream?.Dispose();
            _outputStream?.Dispose();
            _started = false;
        }

        public string ReadMessage()
        {
            try
            {
                return _inputStream.ReadLine();
            }
            catch (Exception ex)
            {
                return $"Exception when receiving message: {ex}";
            }
        }

        public void SendIrcMessage(string message)
        {
            try
            {
                _outputStream.WriteLine(message);
                _outputStream.Flush();
            }
            catch (Exception ex)
            {
                throw new ChatViewerException($"Exception when sending IRC message: {ex}");
            }
        }
    }
}
