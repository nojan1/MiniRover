using System.Collections.Generic;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace Core.Drivers
{
    public class SodarDriver : ISodarDriver
    {
        const int STATUS_REGISTER = 0x1;
        const int RANGE_REGISTER = 0x2;

        private I2CDevice _device;

        public SodarDriver(int address)
        {
            _device = Pi.I2C.AddDevice(address);
        }

        public void Start()
        {
            _device.WriteAddressByte(STATUS_REGISTER, 1);
        }

        public void Stop()
        {
            _device.WriteAddressByte(STATUS_REGISTER, 0);
        }

        public bool GetIsActive()
        {
            return _device.ReadAddressByte(STATUS_REGISTER) == 1;
        }

        public IDictionary<int,int> GetRanges()
        {
            return null;
        }
    }
}