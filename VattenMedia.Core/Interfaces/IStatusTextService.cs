using System;

namespace VattenMedia.Core.Interfaces
{
    public interface IStatusTextService
    {
        void SetCallback(Action<string, TimeSpan?> changeStatus);
        void ChangeStatusText(string status, TimeSpan? time = null);
    }
}
