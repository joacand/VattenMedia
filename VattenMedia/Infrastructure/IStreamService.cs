using System;
using System.Collections.Generic;

namespace VattenMedia.Infrastructure
{
    interface IStreamService
    {
        event EventHandler<int> RunningProcessesChanged;
        void StartStream(Uri url, List<string> qualityOptions);
    }
}
