using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Core.Services;
using Core.Services.Models;
using Serilog;

namespace Core.Runtime
{
    public class ServiceRunner
    {
        private CancellationTokenSource _cancellationTokenSource;
        private Task[] _tasks;
        private Task _monitoringTask;
        private IComponentContext _context;
        private ILogger _logger;

        public ServiceRunner(IComponentContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Start()
        {
            if (_cancellationTokenSource != null)
                return;

            _cancellationTokenSource = new CancellationTokenSource();
            var taskList = new List<Task>();

            foreach (var service in _context.Resolve<IEnumerable<BaseService>>())
            {
                taskList.Add(service.Run(_cancellationTokenSource.Token));
            }

            _tasks = taskList.ToArray();
            _monitoringTask = Task.Run(() => MonitoringTask());
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            Task.WaitAll(_tasks);
            _monitoringTask.Wait();
        }

        private void MonitoringTask()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                var faultedTask = _tasks.FirstOrDefault(t => t.IsFaulted);
                if (faultedTask != null)
                {
                    //Panic!
                    _logger.Fatal("Service task {faultedTask} has faulted. Terminating all services", faultedTask);

                    _cancellationTokenSource.Cancel();

                    foreach (var panicService in _context.Resolve<IEnumerable<IPanic>>())
                    {
                        try
                        {
                            panicService.OnPanic();
                        }
                        catch (Exception e)
                        {
                            _logger.Error("Exception in panic handler ({panicService}): {e}", panicService, e);
                        }
                    }

                    return;
                }

                Task.Delay(50).Wait();
            }
        }

        private void StopServiceTasks()
        {

        }
    }
}