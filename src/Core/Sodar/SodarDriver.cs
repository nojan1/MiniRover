using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace Core.Sodar
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

        }

        public void Stop()
        {

        }

        public bool GetIsActive()
        {
            return _device.ReadAddressByte(STATUS_REGISTER) == 1;
        }

        public int[] GetRanges()
        {
            return null;
        }
    }
}