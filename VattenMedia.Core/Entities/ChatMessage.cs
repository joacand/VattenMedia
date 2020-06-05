using System;

namespace VattenMedia.Core.Entities
{
    public class ChatMessage
    {
        public DateTime DateTime { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
        public string UsernameColor { get; set; }
        public bool IsEmpty { get; set; }

        public static ChatMessage Empty()
        {
            return new ChatMessage
            {
                DateTime = DateTime.Now,
                Username = string.Empty,
                Message = string.Empty,
                UsernameColor = string.Empty,
                IsEmpty = true
            };
        }

        public static ChatMessage ExceptionMessage(Exception ex)
        {
            var message = Empty();
            message.IsEmpty = false;
            message.Message = ex.ToString();
            message.UsernameColor = "#FF0000";
            return message;
        }
    }
}
