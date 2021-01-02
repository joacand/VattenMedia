using System;
using VattenMedia.Core.Interfaces;

namespace VattenMedia.Infrastructure.Services
{
    internal class StatusTextService : IStatusTextService
    {
        private Action<string, TimeSpan?, bool> changeStatus;

        public StatusTextService()
        {
        }

        public void SetCallback(Action<string, TimeSpan?, bool> changeStatus)
        {
            this.changeStatus = changeStatus;
        }

        public void ChangeStatusText(string status, TimeSpan? time = null, bool isError = false)
        {
            changeStatus(status, time, isError);
        }
    }
}
