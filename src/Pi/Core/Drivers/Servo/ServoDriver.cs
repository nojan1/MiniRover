using System;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace Core.Drivers
{
    public class ServoDriver : IServoDriver
    {
        const int SERVO_MIN = 150;
        const int SERVO_MAX = 600;

        const int PCA9685_ADDRESS = 0x40;
        const int MODE1 = 0x00;
        const int MODE2 = 0x01;
        const int SUBADR1 = 0x02;
        const int SUBADR2 = 0x03;
        const int SUBADR3 = 0x04;
        const int PRESCALE = 0xFE;
        const int LED0_ON_L = 0x06;
        const int LED0_ON_H = 0x07;
        const int LED0_OFF_L = 0x08;
        const int LED0_OFF_H = 0x09;
        const int ALL_LED_ON_L = 0xFA;
        const int ALL_LED_ON_H = 0xFB;
        const int ALL_LED_OFF_L = 0xFC;
        const int ALL_LED_OFF_H = 0xFD;


        const byte RESTART = 0x80;
        const byte SLEEP = 0x10;
        const byte ALLCALL = 0x01;
        const byte INVRT = 0x10;
        const byte OUTDRV = 0x04;

        private I2CDevice _device;

        public ServoDriver(int address)
        {
            _device = Pi.I2C.AddDevice(address);
        }

        public void Init(int frequency = 60)
        {
            SetAllPwm(0, 0);

            _device.WriteAddressByte(MODE2, OUTDRV);
            _device.WriteAddressByte(MODE1, ALLCALL);

            Task.Delay(5).Wait(); // wait for oscillator

            var mode1 = _device.ReadAddressByte(MODE1);
            mode1 = (byte)(mode1 & ~SLEEP);  // wake up (reset sleep)
            _device.WriteAddressByte(MODE1, mode1);

            Task.Delay(5).Wait(); // wait for oscillator

            SetPwmFreq(frequency);
        }

        public void SetServoPosition(int channel, int pulse)
        {
            if (channel < 0 || channel > 15)
                throw new ArgumentException("Channel must be between 0 and 15", nameof(channel));

            if (pulse < SERVO_MIN || pulse > SERVO_MAX)
                throw new ArgumentException($"Pulse must be between {SERVO_MIN} and {SERVO_MAX}", nameof(pulse));

            var pulse_length = 1000000;    // 1,000,000 us per second
            pulse_length /= 60;       // 60 Hz
            pulse_length /= 4096;     // 12 bits of resolution

            pulse *= 1000;
            pulse /= pulse_length;

            SetPwm(channel, 0, pulse);
        }

        private void SetPwmFreq(int freq_hz)
        {
            var prescaleval = 25000000.0;    // 25MHz
            prescaleval /= 4096.0;       // 12-bit
            prescaleval /= ((double)freq_hz * 0.9); //Compensation: https://github.com/adafruit/Adafruit-PWM-Servo-Driver-Library/issues/11
            prescaleval -= 1.0;

            var prescale = (byte)Math.Floor(prescaleval + 0.5);

            var oldmode = _device.ReadAddressByte(MODE1);
            var newmode = (oldmode & 0x7F) | 0x10;   // sleep

            _device.WriteAddressWord(MODE1, (byte)newmode);  // go to sleep
            _device.WriteAddressWord(PRESCALE, prescale);
            _device.WriteAddressWord(MODE1, oldmode);

            Task.Delay(5).Wait(); // wait for oscillator

            _device.WriteAddressByte(MODE1, (byte)(oldmode | 0x80));
        }

        private void SetPwm(int channel, int on, int off)
        {
            _device.WriteAddressByte(LED0_ON_L + 4 * channel, (byte)(on & 0xFF));
            _device.WriteAddressByte(LED0_ON_H + 4 * channel, (byte)(on >> 8));
            _device.WriteAddressByte(LED0_OFF_L + 4 * channel, (byte)(off & 0xFF));
            _device.WriteAddressByte(LED0_OFF_H + 4 * channel, (byte)(off >> 8));
        }

        private void SetAllPwm(int on, int off)
        {
            _device.WriteAddressByte(ALL_LED_ON_L, (byte)(on & 0xFF));
            _device.WriteAddressByte(ALL_LED_ON_H, (byte)(on >> 8));
            _device.WriteAddressByte(ALL_LED_OFF_L, (byte)(off & 0xFF));
            _device.WriteAddressByte(ALL_LED_OFF_H, (byte)(off >> 8));
        }
    }
}