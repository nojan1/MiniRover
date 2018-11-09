namespace Core
{
    public interface II2CAddressProvider
    {
        int? ServoDriverAddress { get; }
        int? IMUAddress { get; }
        int? SodarAddress { get; }
    }
}