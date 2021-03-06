using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Services
{
    public abstract class IntervalServiceBase : BaseService
    {
        private Task _workingTask;
        private TimeSpan _loopInterval;

        public IntervalServiceBase(TimeSpan loopInterval) : base()
        {
            _loopInterval = loopInterval;
        }

        public override Task Run(CancellationToken token)
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

    public abstract class BaseService
    {
        public BaseService() { }

        public abstract Task Run(CancellationToken token);
    }
}