using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;
using Microsoft.Extensions.Configuration;

namespace Web
{
    public class ConfigurationProgramAssemblyProvider : IProgramAssemblyProvider
    {
        private IConfiguration _configuration;

        public ConfigurationProgramAssemblyProvider(IConfiguration configuration){
            _configuration = configuration;
        }

        public ICollection<string> GetAbsolutePaths()
        {
            var programAssemblies = _configuration.GetSection("CoreConfiguration:ProgramAssemblies")?.Get<string[]>() ?? new string[] {};
            return programAssemblies
                .Select(x => Path.GetFullPath(x))
                .ToList();
        }
    }
}