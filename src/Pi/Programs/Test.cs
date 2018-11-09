using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Drivers;
using Core.Runtime;
using Core.Services.Models;

namespace Programs
{
    public class Test : IProgram
    {
        public bool IsFinished { get; set; }

        private SensorPlatform _sensorPlattform;
        private IServoDriver _servoDriver;

        public Test(SensorPlatform sensorPlatform, IServoDriver servoDriver)
        {
            _sensorPlattform = sensorPlatform;
            _servoDriver = servoDriver;
        }

        public void Loop(CancellationToken cancellationToken)
        {
            var rnd = new Random();
            while (!cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine(_sensorPlattform.IMU);
                _servoDriver.SetServoPosition(0, rnd.Next(200, 500));

                Task.Delay(500, cancellationToken).Wait();
            }
        }

        public void Setup()
        {
            Console.WriteLine("Entering setup");
        }

        public void Teardown()
        {
            Console.WriteLine("Entering teardown");
        }
    }
}
