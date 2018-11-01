using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Services
{
    public abstract class BaseService
    {
        private Task _workingTask;
        private TimeSpan _loopInterval;

        public BaseService(TimeSpan loopInterval)
        {
            _loopInterval = loopInterval;
        }

        public Task Run(CancellationToken token)
        {
            _workingTask = Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    Loop();
                    await Task.Delay(_loopInterval, token);
                }
            }, token);

            return _workingTask;
        }

        public abstract void Loop();
    }
}