using System.Collections.Generic;

namespace Core
{
    public interface IProgramAssemblyProvider
    {
         ICollection<string> GetAbsolutePaths();
    }
}