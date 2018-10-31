using System;
using Core.Sodar;

namespace Core.Services
{
    public class SodarService : BaseService
    {
        private ISodarDriver _sodarDriver;

        public SodarService(ISodarDriver sodarDriver) 
            : base(TimeSpan.FromSeconds(1))
        {
            _sodarDriver = sodarDriver;
        }

        public override void Loop()
        {
            _sodarDriver.GetRanges();

            //Send along to the sensing subsystem provider thingy?
        }
    }
}