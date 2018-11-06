using System.Threading.Tasks;
using Core.Drivers;
using Core.Services.Models;
using Rebus.Handlers;

namespace Core.Runtime
{
    public class SensorPlatform : IHandleMessages<IMUReading>, IHandleMessages<SodarUpdate>
    {
        public IMUReading IMU { get; private set; }
        public SodarUpdate Sodar { get; private set; }

        public Task Handle(SodarUpdate message) => Task.FromResult(Sodar = message);

        public Task Handle(IMUReading message) => Task.FromResult(IMU = message);
    }
}