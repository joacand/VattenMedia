using System;

namespace VattenMedia.Infrastructure
{
    class StatusManager : IStatusManager
    {
        private readonly Action<string, TimeSpan?> changeStatus;

        public StatusManager(Action<string, TimeSpan?> changeStatus)
        {
            this.changeStatus = changeStatus;
        }

        public void ChangeStatusText(string status, TimeSpan? time = null)
        {
            changeStatus(status, time);
        }
    }
}
