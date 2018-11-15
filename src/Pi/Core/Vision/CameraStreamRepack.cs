using System.Threading.Tasks;
using Core.Runtime.CommandBus;
using Core.Services.Models;

namespace Core.Vision
{
    public class StreamFeedUpdate
    {
        public byte[] Image { get; set; }
    }

    public enum CameraStreamSource
    {
        Left = 1,
        Right = 2,
        DepthMap = 3
    }

    public class CameraStreamRepack : IHandleMessage<CameraUpdate>, IHandleMessage<DepthMapUpdate>
    {
        private ICommandBus _commandBus;

        public CameraStreamSource Source { get; set; } = CameraStreamSource.Left;

        public CameraStreamRepack(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }

        public Task Handle(CameraUpdate message)
        {
            if (Source != CameraStreamSource.Left && Source != CameraStreamSource.Right)
                return Task.FromResult(true);

            return _commandBus.SendMessage(new StreamFeedUpdate
            {
                Image = (Source == CameraStreamSource.Left ? message.LeftImage : message.RightImage).ToMemoryStream().ToArray()
            });
        }

        public Task Handle(DepthMapUpdate message)
        {
            if (Source != CameraStreamSource.DepthMap)
                return Task.FromResult(true);


            return _commandBus.SendMessage(new StreamFeedUpdate
            {
                //Image = (message.DepthMap / 2048).ToMat().ToMemoryStream().ToArray()
                Image = message.DepthMap.ToMemoryStream().ToArray()
            });
        }
    }
}