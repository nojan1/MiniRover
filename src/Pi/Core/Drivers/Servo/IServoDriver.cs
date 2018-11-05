namespace Core.Drivers
{
    public interface IServoDriver
    {
         void Init(int frequency = 60);
         void SetServoPosition(int channel, int pulse);
    }
}