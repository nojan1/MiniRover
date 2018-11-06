using System.Collections.Generic;
using Core.Runtime;
using Core.Services.Models;
using Microsoft.AspNetCore.Mvc;
using Rebus.Bus;

namespace Web.Controllers
{
    [Route("api/program")]
    public class ProgramController : Controller
    {
        private IProgramResolver _programResolver;
        private IBus _bus;

        public ProgramController(IProgramResolver programResolver, IBus bus)
        {
            _programResolver = programResolver;
            _bus = bus;
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
                return NotFound();

            _bus.SendLocal(new ProgramRunRequest { ProgramName = programName });
            return Ok();
        }
    }
}