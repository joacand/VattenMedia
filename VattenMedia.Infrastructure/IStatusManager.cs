using System;

namespace VattenMedia.Infrastructure
{
    public interface IStatusManager
    {
        void SetCallback(Action<string, TimeSpan?> changeStatus);
        void ChangeStatusText(string status, TimeSpan? time = null);
    }
}
