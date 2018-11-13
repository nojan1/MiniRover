using System.Collections.Generic;

namespace Core.Drivers
{
    public interface ISodarDriver
    {
        void Start();
        void Stop();
        IDictionary<int,int> GetRanges();
        bool GetIsActive();
    }
}