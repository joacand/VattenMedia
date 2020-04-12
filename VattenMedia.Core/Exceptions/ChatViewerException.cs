using System;

namespace VattenMedia.Core.Exceptions
{
    public class ChatViewerException : Exception
    {
        public ChatViewerException(string message)
            : base(message)
        { }
    }
}
