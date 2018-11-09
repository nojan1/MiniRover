using System;
using System.Linq;
using Autofac;
using Core.Drivers;

namespace Core
{
    public static partial class Bootstrap
    {
        private static Moq.Mock<ISodarDriver> CreateSodarDriverMock()
        {
            var mock = new Moq.Mock<ISodarDriver>();

            var rnd = new Random();
            mock.Setup(x => x.GetRanges())
                .Returns(() => Enumerable.Range(0, 10).Select(x => rnd.Next(-1, 50)).ToArray());

            return mock;
        }

        private static Moq.Mock<IIMUDriver> CreateIMUDriverMock()
        {
            var mock = new Moq.Mock<IIMUDriver>();

            var rnd = new Random();
            mock.Setup(x => x.GetReading())
                .Returns(() => new IMUReading {
                    Pitch = rnd.Next(-5, 5),
                    Roll = rnd.Next(-5, 5),
                    Yaw = rnd.Next(-2,3)
                });

            return mock;
        }
    }
}