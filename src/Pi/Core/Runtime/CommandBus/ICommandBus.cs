using System.Threading.Tasks;

namespace Core.Runtime.CommandBus
{
    public interface ICommandBus
    {
        Task SendMessage<T>(T message);
        Task<TReturn> SendRequest<TReturn, TParameter>(TParameter parameter);
    }
}