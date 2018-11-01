using System;
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
                builder.Register<ISodarDriver>(x => new Moq.Mock<ISodarDriver>().Object).AsImplementedInterfaces();
            }

            RegisterServices(builder);

            builder.RegisterType<ServiceRunner>().SingleInstance();
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(BaseService).Assembly)
                .Where(t => t.IsSubclassOf(typeof(BaseService)))
                .As<BaseService>();
        }
    }
}