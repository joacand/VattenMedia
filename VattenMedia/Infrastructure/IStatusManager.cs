using System;

namespace VattenMedia.Infrastructure
{
    interface IStatusManager
    {
        void ChangeStatusText(string status, TimeSpan? time = null);
    }
}
