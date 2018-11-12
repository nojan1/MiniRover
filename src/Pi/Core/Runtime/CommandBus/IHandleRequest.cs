using System.Threading.Tasks;

namespace Core.Runtime.CommandBus
{
    public interface IHandleRequest<TReturn, TParameter>
    {
        Task HandleRequest(TParameter parameter, TaskCompletionSource<TReturn> completionSource);      
    }
}