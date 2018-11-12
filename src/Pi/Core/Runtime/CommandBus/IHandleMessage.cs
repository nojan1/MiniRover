using System.Threading.Tasks;

namespace Core.Runtime.CommandBus
{
    public interface IHandleMessage<T>
    {
         Task Handle(T message);
    }
}