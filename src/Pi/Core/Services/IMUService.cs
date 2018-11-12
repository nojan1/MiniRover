using System;
using Core.Drivers;
using Core.Runtime.CommandBus;

namespace Core.Services
{
    public class IMUService : IntervalServiceBase
    {
        private IIMUDriver _imuDriver;
        private ICommandBus _bus;

        public IMUService(IIMUDriver imuDriver, ICommandBus bus)
            : base(TimeSpan.FromMilliseconds(500))
        {
            _imuDriver = imuDriver;
            _bus = bus;
        }

        public override void Loop()
        {
            if(!_imuDriver.IsCalibrated)
                _imuDriver.Calibrate(10);

            _bus.SendMessage(_imuDriver.GetReading());
        }
    }
}