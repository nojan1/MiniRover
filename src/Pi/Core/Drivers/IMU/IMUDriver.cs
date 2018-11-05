using System;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace Core.Drivers
{
    //Designed for MPU-6050

    class Vector
    {
        public double XAxis { get; set; }
        public double YAxis { get; set; }
        public double ZAxis { get; set; }
    }

    public enum MPU6050ClockSource
    {
        MPU6050_CLOCK_KEEP_RESET = 0b111,
        MPU6050_CLOCK_EXTERNAL_19MHZ = 0b101,
        MPU6050_CLOCK_EXTERNAL_32KHZ = 0b100,
        MPU6050_CLOCK_PLL_ZGYRO = 0b011,
        MPU6050_CLOCK_PLL_YGYRO = 0b010,
        MPU6050_CLOCK_PLL_XGYRO = 0b001,
        MPU6050_CLOCK_INTERNAL_8MHZ = 0b000
    }

    public enum MPU6050Scale
    {
        MPU6050_SCALE_2000DPS = 0b11,
        MPU6050_SCALE_1000DPS = 0b10,
        MPU6050_SCALE_500DPS = 0b01,
        MPU6050_SCALE_250DPS = 0b00
    }

    public enum MPU6050Range
    {
        MPU6050_RANGE_16G = 0b11,
        MPU6050_RANGE_8G = 0b10,
        MPU6050_RANGE_4G = 0b01,
        MPU6050_RANGE_2G = 0b00,
    }

    public class IMUDriver : IIMUDriver
    {
        public bool IsCalibrated
        {
            get; private set;
        }
        private const int MPU6050_REG_WHO_AM_I = 0x75; // Who Am I
        private const int MPU6050_REG_PWR_MGMT_1 = 0x6B; // Power Management 1
        private const int MPU6050_REG_GYRO_CONFIG = 0x1B;// Gyroscope Configuration
        private const int MPU6050_REG_ACCEL_CONFIG = 0x1C;// Accelerometer Configuration
        private const int MPU6050_REG_GYRO_XOUT_H = 0x43;

        private I2CDevice _device;
        private double dpsPerDigit;
        private double rangePerDigit;
        private double actualThreshold;
        private Vector gyroDelta = new Vector();
        private Vector thresholdVector = new Vector();
        private Vector thresholdGyro = new Vector();

        private IMUReading lastReading;
        private DateTime lastReadingTime;

        public IMUDriver(int address, MPU6050Scale scale, MPU6050Range range)
        {
            _device = Pi.I2C.AddDevice(address);

            if (!IsCorrectModel())
                throw new Exception("Wrong gyro model detected");

            SetClockSource(MPU6050ClockSource.MPU6050_CLOCK_PLL_XGYRO);
            SetScale(scale);
            SetRange(range);
            SetSleepEnable(false);
        }

        public IMUReading GetReading()
        {
            var normalizedGyro = ReadNormalizedGyro();

            if (lastReading == null)
            {
                lastReadingTime = DateTime.Now;

                return lastReading = new IMUReading
                {
                    Pitch = normalizedGyro.YAxis,
                    Roll = normalizedGyro.XAxis,
                    Yaw = normalizedGyro.ZAxis
                };
            }
            else
            {
                var timeDiff = DateTime.Now - lastReadingTime;
                lastReadingTime = DateTime.Now;

                return lastReading = new IMUReading
                {
                    Pitch = lastReading.Pitch + (normalizedGyro.YAxis * timeDiff.TotalSeconds),
                    Roll = lastReading.Roll + (normalizedGyro.XAxis * timeDiff.TotalSeconds),
                    Yaw = lastReading.Yaw + (normalizedGyro.ZAxis * timeDiff.TotalSeconds)
                };
            }
        }

        public void Calibrate(int samples)
        {
            IsCalibrated = true;

            // Reset values
            double sumX = 0;
            double sumY = 0;
            double sumZ = 0;
            double sigmaX = 0;
            double sigmaY = 0;
            double sigmaZ = 0;

            // Read n-samples
            for (var i = 0; i < samples; ++i)
            {
                var rawGyro = ReadRawGyro();
                sumX += rawGyro.XAxis;
                sumY += rawGyro.YAxis;
                sumZ += rawGyro.ZAxis;

                sigmaX += rawGyro.XAxis * rawGyro.XAxis;
                sigmaY += rawGyro.YAxis * rawGyro.YAxis;
                sigmaZ += rawGyro.ZAxis * rawGyro.ZAxis;

                Task.Delay(5).Wait();
            }

            // Calculate delta vectors
            gyroDelta.XAxis = sumX / samples;
            gyroDelta.YAxis = sumY / samples;
            gyroDelta.ZAxis = sumZ / samples;

            // Calculate thresholdVectorreshold vectors
            thresholdVector.XAxis = Math.Sqrt((sigmaX / 50) - (gyroDelta.XAxis * gyroDelta.XAxis));
            thresholdVector.YAxis = Math.Sqrt((sigmaY / 50) - (gyroDelta.YAxis * gyroDelta.YAxis));
            thresholdVector.ZAxis = Math.Sqrt((sigmaZ / 50) - (gyroDelta.ZAxis * gyroDelta.ZAxis));

            // If already set threshold, recalculate threshold vectors
            if (actualThreshold > 0)
            {
                SetThreshold(actualThreshold);
            }
        }

        private void SetThreshold(double multiple)
        {
            if (multiple > 0)
            {
                // If not calibrated, need calibrate
                if (!IsCalibrated)
                {
                    Calibrate(10);
                }

                // Calculate threshold vectors
                thresholdGyro.XAxis = thresholdVector.XAxis * multiple;
                thresholdGyro.YAxis = thresholdVector.YAxis * multiple;
                thresholdGyro.ZAxis = thresholdVector.ZAxis * multiple;
            }
            else
            {
                // No threshold
                thresholdGyro.XAxis = 0;
                thresholdGyro.YAxis = 0;
                thresholdGyro.ZAxis = 0;
            }

            // Remember old threshold value
            actualThreshold = multiple;
        }

        private Vector ReadRawGyro()
        {
            _device.Write(MPU6050_REG_GYRO_XOUT_H);
            var data = _device.Read(6);

            int xha = data[0];
            int xla = data[1];
            int yha = data[2];
            int yla = data[3];
            int zha = data[4];
            int zla = data[5];

            return new Vector
            {
                XAxis = xha << 8 | xla,
                YAxis = yha << 8 | yla,
                ZAxis = zha << 8 | zla
            };
        }

        private Vector ReadNormalizedGyro()
        {
            var rawGyro = ReadRawGyro();

            var normalizedGyro = new Vector();

            if (IsCalibrated)
            {
                normalizedGyro.XAxis = (rawGyro.XAxis - gyroDelta.XAxis) * dpsPerDigit;
                normalizedGyro.YAxis = (rawGyro.YAxis - gyroDelta.YAxis) * dpsPerDigit;
                normalizedGyro.ZAxis = (rawGyro.ZAxis - gyroDelta.ZAxis) * dpsPerDigit;
            }
            else
            {
                normalizedGyro.XAxis = rawGyro.XAxis * dpsPerDigit;
                normalizedGyro.YAxis = rawGyro.YAxis * dpsPerDigit;
                normalizedGyro.ZAxis = rawGyro.ZAxis * dpsPerDigit;
            }

            if (actualThreshold > 0)
            {
                if (Math.Abs(normalizedGyro.XAxis) < thresholdGyro.XAxis)
                    normalizedGyro.XAxis = 0;

                if (Math.Abs(normalizedGyro.YAxis) < thresholdGyro.YAxis)
                    normalizedGyro.YAxis = 0;

                if (Math.Abs(normalizedGyro.ZAxis) < thresholdGyro.ZAxis)
                    normalizedGyro.ZAxis = 0;
            }

            return normalizedGyro;
        }

        private bool IsCorrectModel() => _device.ReadAddressByte(MPU6050_REG_WHO_AM_I) == 0x68;

        private void SetClockSource(MPU6050ClockSource source)
        {
            int value = _device.ReadAddressByte(MPU6050_REG_PWR_MGMT_1);
            value &= 0b11111000;
            value |= (int)source;
            _device.WriteAddressByte(MPU6050_REG_PWR_MGMT_1, (byte)value);
        }

        private void SetScale(MPU6050Scale scale)
        {
            switch (scale)
            {
                case MPU6050Scale.MPU6050_SCALE_250DPS:
                    dpsPerDigit = .007633;
                    break;
                case MPU6050Scale.MPU6050_SCALE_500DPS:
                    dpsPerDigit = .015267;
                    break;
                case MPU6050Scale.MPU6050_SCALE_1000DPS:
                    dpsPerDigit = .030487;
                    break;
                case MPU6050Scale.MPU6050_SCALE_2000DPS:
                    dpsPerDigit = .060975;
                    break;
            }

            int value = _device.ReadAddressByte(MPU6050_REG_GYRO_CONFIG);
            value &= 0b11100111;
            value |= ((int)scale << 3);
            _device.WriteAddressByte(MPU6050_REG_GYRO_CONFIG, (byte)value);
        }

        private void SetRange(MPU6050Range range)
        {
            switch (range)
            {
                case MPU6050Range.MPU6050_RANGE_2G:
                    rangePerDigit = .000061;
                    break;
                case MPU6050Range.MPU6050_RANGE_4G:
                    rangePerDigit = .000122;
                    break;
                case MPU6050Range.MPU6050_RANGE_8G:
                    rangePerDigit = .000244;
                    break;
                case MPU6050Range.MPU6050_RANGE_16G:
                    rangePerDigit = .0004882;
                    break;
                default:
                    break;
            }

            int value = _device.ReadAddressByte(MPU6050_REG_ACCEL_CONFIG);
            value &= 0b11100111;
            value |= ((int)range << 3);
            _device.WriteAddressByte(MPU6050_REG_ACCEL_CONFIG, (byte)value);
        }

        private void SetSleepEnable(bool isEnabled)
        {
            int value = _device.ReadAddressByte(MPU6050_REG_PWR_MGMT_1);

            if (isEnabled)
                value |= (1 << 6);
            else
                value &= ~(1 << 6);

            _device.WriteAddressByte(MPU6050_REG_PWR_MGMT_1, (byte)value);
        }
    }
}