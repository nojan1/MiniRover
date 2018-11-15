using System.Threading.Tasks;
using Core.Runtime.CommandBus;
using Core.Services.Models;

namespace Core.Vision
{
    public class StreamFeedUpdate
    {
        public byte[] Image { get; set; }
    }

    public class CameraStreamRepack : IHandleMessage<CameraUpdate>
    {
        private ICommandBus _commandBus;

        public CameraStreamRepack(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }

        public Task Handle(CameraUpdate message) => 
            _commandBus.SendMessage(new StreamFeedUpdate{
                Image = message.LeftImage.ToMemoryStream().ToArray()
            });
    }
}