using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;

namespace Core.Runtime.CommandBus
{
    public class CommandBus : ICommandBus
    {
        private IComponentContext _componentContext;

        public CommandBus(IComponentContext componentContext)
        {
            _componentContext = componentContext;
        }

        public Task SendMessage<T>(T message)
        {
            var handlers = FindHandler<IEnumerable<IHandleMessage<T>>>();
            if (handlers == null)
                return Task.FromResult(false);

            var tasks = handlers
                .Select(h => h.Handle(message));

            return Task.WhenAll(tasks);
        }

        public Task<TReturn> SendRequest<TReturn, TParameter>(TParameter parameter)
        {
            var handler = FindHandler<IHandleRequest<TReturn, TParameter>>();
            if (handler == null)
                return Task.FromResult(default(TReturn));

            var taskCompletionSource = new TaskCompletionSource<TReturn>();
            handler.HandleRequest(parameter, taskCompletionSource);

            return taskCompletionSource.Task;
        }

        private T FindHandler<T>()
        {
            var handler = _componentContext.Resolve<T>();
            if (handler == null)
            {
                //TODO: Attach logging and log these cases
                //throw new Exception($"Failed to resolve message handler for type {typeof(T).Name}");
            }

            return handler;
        }
    }
}