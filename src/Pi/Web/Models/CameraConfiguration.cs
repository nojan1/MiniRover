using Core.Services.Models;

namespace Web.Models
{
    public class CameraConfiguration : ICameraConfiguration
    {
        public int? LeftCameraId { get; set; }

        public int? RightCameraId { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string Format { get; set; }
    }
}