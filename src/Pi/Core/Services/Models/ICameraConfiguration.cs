namespace Core.Services.Models
{
    public interface ICameraConfiguration
    {
        int? LeftCameraId { get; }
        int? RightCameraId { get; }
        int Width { get; }
        int Height { get; }
        string Format { get; }
    }
}