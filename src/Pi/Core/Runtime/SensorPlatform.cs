using System.Threading.Tasks;
using Core.Drivers;
using Core.Runtime.CommandBus;
using Core.Services.Models;

namespace Core.Runtime
{
    public class SensorPlatform : IHandleMessage<IMUReading>, IHandleMessage<SodarUpdate>
    {
        public IMUReading IMU { get; private set; }
        public SodarUpdate Sodar { get; private set; }

        public Task Handle(SodarUpdate message) => Task.FromResult(Sodar = message);

        public Task Handle(IMUReading message) => Task.FromResult(IMU = message);
    }
}