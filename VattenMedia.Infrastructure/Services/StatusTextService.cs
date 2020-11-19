using System;
using VattenMedia.Core.Interfaces;

namespace VattenMedia.Infrastructure.Services
{
    internal class StatusTextService : IStatusTextService
    {
        private Action<string, TimeSpan?> changeStatus;

        public StatusTextService()
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
