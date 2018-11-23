using System;
using System.Collections.Generic;
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
            var baseData = new Dictionary<int, int>() {
                {15, 44},
                {22, 44},
                {30, 40},
                {37, 41},
                {45, 58},
                {52, 2194},
                {60, 2195},
                {67, 205},
                {75, 214},
                {82, 214},
                {90, 217},
                {97, 2195},
                {105, 41},
                {112, 39},
                {120, 40},
                {127, 29},
                {135, 28},
                {142, 29},
                {150, 29},
                {157, 31}
            };

            var rnd = new Random();
            mock.Setup(x => x.GetRanges())
                .Returns(() => baseData.ToDictionary(x => x.Key, x => x.Value + rnd.Next(-5,5)));

            return mock;
        }

        private static Moq.Mock<IIMUDriver> CreateIMUDriverMock()
        {
            var mock = new Moq.Mock<IIMUDriver>();

            var rnd = new Random();
            mock.Setup(x => x.GetReading())
                .Returns(() => new IMUReading
                {
                    Pitch = rnd.Next(-5, 5),
                    Roll = rnd.Next(-5, 5),
                    Yaw = rnd.Next(-2, 3)
                });

            return mock;
        }
    }
}