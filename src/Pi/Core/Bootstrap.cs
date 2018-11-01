using System;
using System.Linq;
using System.Runtime.InteropServices;
using Autofac;
using Core.Runtime;
using Core.Services;
using Core.Servo;
using Core.Sodar;

namespace Core
{
    public static class Bootstrap
    {
        public static void Configure(ContainerBuilder builder)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                builder.Register<ServoDriver>(x => new ServoDriver(0x40)).AsImplementedInterfaces().SingleInstance();
                builder.Register<SodarDriver>(x => new SodarDriver(0x41)).AsImplementedInterfaces().SingleInstance();
            }
            else
            {
                builder.Register<IServoDriver>(x => new Moq.Mock<IServoDriver>().Object).AsImplementedInterfaces();
                builder.Register<ISodarDriver>(x => CreateSodarDriverMock().Object).AsImplementedInterfaces();
            }

            RegisterServices(builder);

            builder.RegisterType<ServiceRunner>().SingleInstance();
        }

        private static Moq.Mock<ISodarDriver> CreateSodarDriverMock()
        {
            var mock = new Moq.Mock<ISodarDriver>();

            var rnd = new Random();
            mock.Setup(x => x.GetRanges())
                .Returns(() => Enumerable.Range(0, 10).Select(x => rnd.Next(-1,10)).ToArray());

            return mock;
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(BaseService).Assembly)
                .Where(t => t.IsSubclassOf(typeof(BaseService)))
                .As<BaseService>();
        }
    }
}