using System;
using Core.Services.Models;
using Core.Sodar;
using Rebus.Bus;

namespace Core.Services
{
    public class SodarService : BaseService
    {
        private ISodarDriver _sodarDriver;
        private IBus _bus;

        public SodarService(ISodarDriver sodarDriver, IBus bus)
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

            _bus.SendLocal(updatePackage);
        }
    }
}