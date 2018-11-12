using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Services.Models;

namespace Core.Services
{
    public class EmergencyStopService : BaseService, IPanic
    {
        public void OnPanic()
        {
            Console.WriteLine("Hey entering panic mode here!");
        }

        public override Task Run(CancellationToken token)
        {
            //Nope
            return Task.FromResult(true);
        }
    }
}