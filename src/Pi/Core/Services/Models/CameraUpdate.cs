using System.Drawing;
using OpenCvSharp;

namespace Core.Services.Models
{
    public class CameraUpdate
    {
        public Mat LeftImage { get; set; }
        public Mat RightImage { get; set; }
    }
}