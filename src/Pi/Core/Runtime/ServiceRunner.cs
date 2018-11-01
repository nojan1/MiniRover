using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Core.Services;

namespace Core.Runtime
{
    public class ServiceRunner
    {
        private CancellationTokenSource _cancellationTokenSource;
        private List<Task> _tasks;
        private IComponentContext _context;

        public ServiceRunner(IComponentContext context)
        {
            _context = context;
        }

        public void Start()
        {
            if (_cancellationTokenSource != null)
                return;

            _cancellationTokenSource = new CancellationTokenSource();
            _tasks = new List<Task>();

            foreach (var service in _context.Resolve<IEnumerable<BaseService>>())
            {
                _tasks.Add(service.Run(_cancellationTokenSource.Token));
            }
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            Task.WaitAll(_tasks.ToArray());
        }
    }
}