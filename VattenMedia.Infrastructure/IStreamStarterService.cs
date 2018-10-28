using System;
using System.Collections.Generic;

namespace VattenMedia.Infrastructure
{
    public interface IStreamStarterService
    {
        event EventHandler<int> RunningProcessesChanged;
        void StartStream(Uri url, List<string> qualityOptions);
    }
}
