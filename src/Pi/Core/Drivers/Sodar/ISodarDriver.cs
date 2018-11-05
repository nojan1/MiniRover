namespace Core.Drivers
{
    public interface ISodarDriver
    {
        void Start();
        void Stop();
        int[] GetRanges();
        bool GetIsActive();
    }
}