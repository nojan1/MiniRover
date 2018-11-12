using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Drivers;
using Core.Runtime;
using Core.Services.Models;
using Serilog;

namespace Programs
{
    public class Test : IProgram
    {
        public bool IsFinished { get; set; }

        private SensorPlatform _sensorPlattform;
        private IServoDriver _servoDriver;
        private ILogger _logger;

        public Test(SensorPlatform sensorPlatform, IServoDriver servoDriver, ILogger logger)
        {
            _sensorPlattform = sensorPlatform;
            _servoDriver = servoDriver;
            _logger = logger;
        }

        public void Loop(CancellationToken cancellationToken)
        {
            var rnd = new Random();
            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.Information("IMU {IMU}", _sensorPlattform.IMU);
                _servoDriver.SetServoPosition(0, rnd.Next(200, 500));

                Task.Delay(500, cancellationToken).ContinueWith(task => { }).Wait();
            }
        }

        public void Setup()
        {
            _logger.Debug("Program Test entering setup");
        }

        public void Teardown()
        {
            _logger.Debug("Program Test entering teardown");
        }
    }
}
