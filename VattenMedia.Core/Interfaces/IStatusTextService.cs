using System;

namespace VattenMedia.Core.Interfaces
{
    public interface IStatusTextService
    {
        void SetCallback(Action<string, TimeSpan?, bool> changeStatus);
        void ChangeStatusText(string status, TimeSpan? time = null, bool isError = false);
    }
}
