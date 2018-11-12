using System.Collections.Generic;
using Core.Runtime;
using Core.Services.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Rebus.Bus;

namespace Web.Controllers
{
    [Route("api/program")]
    public class ProgramController : Controller
    {
        private IProgramResolver _programResolver;
        private IBus _bus;
        private ILogger _logger;

        public ProgramController(IProgramResolver programResolver, IBus bus, ILogger logger)
        {
            _programResolver = programResolver;
            _bus = bus;
            _logger = logger;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            return Ok(_programResolver.GetProgramNames());
        }

        [HttpPost("{programName}/run")]
        public IActionResult Run(string programName)
        {
            if (!_programResolver.IsKnownProgram(programName))
            {
                _logger.Information("Unknown program specified: {programName}", programName);
                return NotFound();
            }

            _bus.SendLocal(new ProgramRunRequest { ProgramName = programName });
            return Ok();
        }

        [HttpPost("stop")]
        public IActionResult Stop(){
            _bus.SendLocal(new ProgramStopRequest());
            return Ok();
        }
    }
}