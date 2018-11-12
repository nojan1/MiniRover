using System;
using Core.Services.Models;
using Core.Drivers;
using Core.Runtime.CommandBus;

namespace Core.Services
{
    public class SodarService : IntervalServiceBase
    {
        private ISodarDriver _sodarDriver;
        private ICommandBus _bus;

        public SodarService(ISodarDriver sodarDriver, ICommandBus bus)
            : base(TimeSpan.FromSeconds(1))
        {
            _sodarDriver = sodarDriver;
            _bus = bus;
        }

        public override void Loop()
        {
            var updatePackage = new SodarUpdate
            {
                Ranges = _sodarDriver.GetRanges()
            };

            _bus.SendMessage(updatePackage);
        }
    }
}