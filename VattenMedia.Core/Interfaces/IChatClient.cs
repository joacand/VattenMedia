namespace VattenMedia.Core.Interfaces
{
    public interface IChatClient
    {
        void Start(string userName, string accessToken, string channel);

        void Stop();

        /// <summary>
        /// Returns the next message. Will block thread until a message is recieved.
        /// </summary>
        string ReadMessage();

        void SendIrcMessage(string message);
    }
}
