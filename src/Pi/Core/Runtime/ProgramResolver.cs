using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Autofac;
using Core.Services.Models;

namespace Core.Runtime
{
    [Serializable]
    public class ProgramNotFoundException : Exception
    {
        public ProgramNotFoundException()
        {
        }

        public ProgramNotFoundException(string message) : base(message)
        {
        }
    }

    public class ProgramResolver : IProgramResolver
    {
        private IComponentContext _componentContext;

        public ProgramResolver(IComponentContext componentContext)
        {
            _componentContext = componentContext;
        }

        public ICollection<string> GetProgramNames()
        {
            return _componentContext.Resolve<IEnumerable<IProgram>>()
                .Select(x => x.GetType().Name)
                .ToList();
        }

        public IProgram ResolveByName(string name)
        {
            var program = GetByName(name);

            if (program == null)
                throw new ProgramNotFoundException(name);

            return program;
        }

        public bool IsKnownProgram(string name) => GetByName(name) != null;

        private IProgram GetByName(string name){
            return _componentContext.Resolve<IEnumerable<IProgram>>()
                .FirstOrDefault(p => p.GetType().Name == name);
        }
    }

}