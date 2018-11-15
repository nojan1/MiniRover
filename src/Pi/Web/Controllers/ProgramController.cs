using System.Collections.Generic;
using Core.Runtime;
using Core.Services.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Core.Runtime.CommandBus;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [Route("api/program")]
    public class ProgramController : Controller
    {
        private IProgramResolver _programResolver;
        private ICommandBus _bus;
        private ILogger _logger;

        public ProgramController(IProgramResolver programResolver, ICommandBus bus, ILogger logger)
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

            _bus.SendMessage(new ProgramRunMessage { ProgramName = programName });
            return Ok();
        }

        [HttpPost("stop")]
        public IActionResult Stop()
        {
            _bus.SendMessage(new ProgramStopMessage());
            return Ok();
        }

        [HttpGet("state")]
        public async Task<IActionResult> GetStateAsync()
        {
            var state = await _bus.SendRequest<ProgramStateRequestResponse, object>(null);        
            return Ok(state);
        }
    }
}