using System;
using Core.Drivers;
using Rebus.Bus;

namespace Core.Services
{
    public class IMUService : BaseService
    {
        private IIMUDriver _imuDriver;
        private IBus _bus;

        public IMUService(IIMUDriver imuDriver, IBus bus)
            : base(TimeSpan.FromMilliseconds(500))
        {
            _imuDriver = imuDriver;
            _bus = bus;
        }

        public override void Loop()
        {
            if(!_imuDriver.IsCalibrated)
                _imuDriver.Calibrate(10);

            _bus.SendLocal(_imuDriver.GetReading());
        }
    }
}