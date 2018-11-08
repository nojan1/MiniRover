using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Core.Services;
using Core.Services.Models;

namespace Core.Runtime
{
    public class ServiceRunner
    {
        private CancellationTokenSource _cancellationTokenSource;
        private Task[] _tasks;
        private Task _monitoringTask;
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
                    //TODO: Log
                    Console.WriteLine($"ERROR! Service task {faultedTask.Id} has faulted. Terminating all services");

                    _cancellationTokenSource.Cancel();

                    foreach (var panicService in _context.Resolve<IEnumerable<IPanic>>())
                    {
                        try
                        {
                            panicService.OnPanic();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Exception in panic handler ({nameof(panicService)}): {e.Message}");
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