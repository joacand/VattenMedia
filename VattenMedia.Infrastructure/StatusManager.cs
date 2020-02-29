using System;
using VattenMedia.Core.Interfaces;

namespace VattenMedia.Infrastructure
{
    internal class StatusManager : IStatusManager
    {
        private Action<string, TimeSpan?> changeStatus;

        public StatusManager()
        {
        }

        public void SetCallback(Action<string, TimeSpan?> changeStatus)
        {
             this.changeStatus = changeStatus;
        }

        public void ChangeStatusText(string status, TimeSpan? time = null)
        {
            changeStatus(status, time);
        }
    }
}
