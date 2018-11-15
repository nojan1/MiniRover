using Core.Vision;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/camera")]
    public class CameraController : Controller
    {
        private CameraStreamRepack _cameraStreamRepack;

        public CameraController(CameraStreamRepack cameraStreamRepack)
        {
            _cameraStreamRepack = cameraStreamRepack;
        }

        [HttpPost("source/{source}")]
        public IActionResult SetSource(CameraStreamSource source)
        {
            _cameraStreamRepack.Source = source;
            return Ok();
        }
    }
}