using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace VattenMedia.Models
{
    public class ChatMessage : INotifyPropertyChanged
    {
        public DateTime DateTime { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
        public string UsernameColor { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
