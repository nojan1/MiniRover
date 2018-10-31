namespace Core.Sodar
{
    public interface ISodarDriver
    {
        void Start();
        void Stop();
        int[] GetRanges();
        bool GetIsActive();
    }
}