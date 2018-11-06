using System.Collections.Generic;
using Core.Services.Models;

namespace Core.Runtime
{
    public interface IProgramResolver
    {
        ICollection<string> GetProgramNames();
         IProgram ResolveByName(string name);
         bool IsKnownProgram(string name);
    }
}