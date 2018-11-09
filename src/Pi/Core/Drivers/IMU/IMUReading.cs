namespace Core.Drivers
{
    public class IMUReading
    {
        public double Yaw { get; set; }
        public double Pitch { get; set; }
        public double Roll { get; set; }

        public override string ToString() => $"Pitch: {Pitch}, Roll: {Roll}, Yaw: {Yaw}";
    }
}