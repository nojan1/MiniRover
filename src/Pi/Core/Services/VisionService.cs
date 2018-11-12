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
    public class VisionService : BaseService
    {
        private const int FPS = 15;

        private DateTime _nextFrame;
        private TimeSpan _frameDelay;
        private ILogger _logger;
        private ICommandBus _bus;

        public VisionService(ILogger logger, ICommandBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        public override Task Run(CancellationToken token)
        {
            _frameDelay = TimeSpan.FromMilliseconds(1000.0 / FPS);

            return Task.Run(() =>
            {
                var videoCapture = VideoCapture.FromCamera(0);

                while (!token.IsCancellationRequested)
                {
                    if (DateTime.Now >= _nextFrame)
                    {

                        var mat = videoCapture.RetrieveMat();

                        //TODO: Fix performance
                        // _bus.SendLocal(new VisionUpdate
                        // {
                        //     Image = mat.ToMemoryStream().ToArray()
                        // });

                        _nextFrame += _frameDelay;
                    }
                    else
                    {
                        Task.Delay(5).Wait();
                    }
                }
            });
        }
    }
}