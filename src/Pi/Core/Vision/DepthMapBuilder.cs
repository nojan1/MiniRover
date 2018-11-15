using System;
using System.Threading.Tasks;
using Core.Runtime.CommandBus;
using Core.Services.Models;
using OpenCvSharp;

namespace Core.Vision
{
    public class DepthMapUpdate
    {
        public Mat DepthMap { get; set; }
    }

    public class DepthMapBuilder : IHandleMessage<CameraUpdate>
    {
        private const int DEPTH_MAP_PER_SECONDS = 5;

        private DateTime _nextEmit;
        private TimeSpan _emitDelay;
        private ICommandBus _commandBus;
        private StereoMatcher _stereoMatcher;

        public DepthMapBuilder(ICommandBus commandBus)
        {
            _commandBus = commandBus;
            _emitDelay = TimeSpan.FromMilliseconds(1000.0 / DEPTH_MAP_PER_SECONDS);
            _stereoMatcher = StereoBM.Create();

            _stereoMatcher.MinDisparity = 4;
            _stereoMatcher.NumDisparities = 128;
            _stereoMatcher.BlockSize = 21;
            _stereoMatcher.SpeckleRange = 16;
            _stereoMatcher.SpeckleWindowSize = 45;
        }

        public Task Handle(CameraUpdate message)
        {
            if (DateTime.Now < _nextEmit)
                return Task.FromResult(true);

            _nextEmit = DateTime.Now + _emitDelay;

            return Task.Run(() =>
            {
                if (message.LeftImage.Empty() || message.RightImage.Empty())
                    return;

                var output = new Mat();
                var grayLeft = message.LeftImage.CvtColor(ColorConversionCodes.BGR2GRAY);
                var grayRight = message.RightImage.CvtColor(ColorConversionCodes.BGR2GRAY);

                _stereoMatcher.Compute(grayLeft, grayRight, output);

                _commandBus.SendMessage(new DepthMapUpdate
                {
                    DepthMap = output
                });
            });
        }
    }
}