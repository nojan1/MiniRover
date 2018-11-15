using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Core.Runtime.CommandBus;
using Core.Services.Models;
using OpenCvSharp;
using Serilog;

namespace Core.Services
{
    public class CameraService : BaseService
    {
        private const int FPS = 15;

        private DateTime _nextFrame;
        private TimeSpan _frameDelay;
        private ILogger _logger;
        private ICommandBus _bus;
        private ICameraConfiguration _cameraConfiguration;

        public CameraService(ILogger logger, ICommandBus bus, ICameraConfiguration cameraConfiguration)
        {
            _logger = logger;
            _bus = bus;
            _cameraConfiguration = cameraConfiguration;
        }

        public override Task Run(CancellationToken token)
        {
            _frameDelay = TimeSpan.FromMilliseconds(1000.0 / FPS);

            return Task.Run(() =>
            {
                var leftEyeCapture = CreateCapture(_cameraConfiguration.LeftCameraId);
                var rightEyeCapture = CreateCapture(_cameraConfiguration.RightCameraId);

                if (leftEyeCapture != null || rightEyeCapture != null)
                {
                    while (!token.IsCancellationRequested)
                    {
                        if (DateTime.Now >= _nextFrame)
                        {
                            leftEyeCapture?.Grab();
                            rightEyeCapture?.Grab();

                            var leftMat = leftEyeCapture?.RetrieveMat() ?? new Mat();
                            var rightMat = rightEyeCapture?.RetrieveMat() ?? new Mat();

                            _bus.SendMessage(new CameraUpdate
                            {
                                LeftImage = leftMat,
                                RightImage = rightMat
                            });

                            _nextFrame = DateTime.Now + _frameDelay;
                        }
                        else
                        {
                            Task.Delay(5).Wait();
                        }
                    }

                    leftEyeCapture?.Release();
                    rightEyeCapture?.Release();
                }
            });
        }

        private VideoCapture CreateCapture(int? captureId)
        {
            if (!captureId.HasValue)
                return null;

            var capture = VideoCapture.FromCamera(captureId.Value);
            capture.Set(CaptureProperty.FrameWidth, _cameraConfiguration.Width);
            capture.Set(CaptureProperty.FrameHeight, _cameraConfiguration.Height);

            var fourCC = Enum.Parse(typeof(FourCC), _cameraConfiguration.Format);
            capture.Set(CaptureProperty.FourCC, (int)fourCC);

            return capture;
        }
    }
}