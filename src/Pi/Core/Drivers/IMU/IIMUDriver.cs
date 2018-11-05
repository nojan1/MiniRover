namespace Core.Drivers
{
    public interface IIMUDriver
    {
        bool IsCalibrated { get; }
        IMUReading GetReading();
        void Calibrate(int numSamples);
    }
}