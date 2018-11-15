using System;
using System.Linq;
using System.Runtime.InteropServices;
using Autofac;
using Core.Runtime;
using Core.Services;
using Core.Drivers;
using System.Reflection;
using Core.Services.Models;
using Core.Runtime.CommandBus;
using Core.Vision;

namespace Core
{
    public static partial class Bootstrap
    {
        public static void Configure(ContainerBuilder builder, IProgramAssemblyProvider programAssemblyProvider, II2CAddressProvider i2CAddressProvider)
        {
            RegisterDrivers(builder, i2CAddressProvider);
            RegisterServices(builder);

            if (programAssemblyProvider != null)
                RegisterPrograms(builder, programAssemblyProvider);

            RegisterVisionHandlers(builder);
            RegisterRuntime(builder);
        }

        private static void RegisterVisionHandlers(ContainerBuilder builder)
        {
            builder.RegisterType<CameraStreamRepack>().AsImplementedInterfaces();
            builder.RegisterType<DepthMapBuilder>().AsImplementedInterfaces();
        }

        private static void RegisterRuntime(ContainerBuilder builder)
        {
            builder.RegisterType<ProgramResolver>().AsImplementedInterfaces();
            builder.RegisterType<SensorPlatform>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<ServiceRunner>().SingleInstance();
            builder.RegisterType<CommandBus>().AsImplementedInterfaces().SingleInstance();
        }

        private static void RegisterDrivers(ContainerBuilder builder, II2CAddressProvider i2CAddressProvider)
        {
            if (i2CAddressProvider.ServoDriverAddress.HasValue)
                builder.Register<ServoDriver>(x => new ServoDriver(i2CAddressProvider.ServoDriverAddress.Value)).AsImplementedInterfaces().SingleInstance();
            else
                builder.Register<IServoDriver>(x => new Moq.Mock<IServoDriver>().Object);

            if (i2CAddressProvider.IMUAddress.HasValue)
                builder.Register<IMUDriver>(x => new IMUDriver(i2CAddressProvider.IMUAddress.Value, MPU6050Scale.MPU6050_SCALE_2000DPS, MPU6050Range.MPU6050_RANGE_2G)).AsImplementedInterfaces().SingleInstance();
            else
                builder.Register<IIMUDriver>(x => CreateIMUDriverMock().Object);

            if (i2CAddressProvider.SodarAddress.HasValue)
                builder.Register<SodarDriver>(x => new SodarDriver(i2CAddressProvider.SodarAddress.Value)).AsImplementedInterfaces().SingleInstance();
            else
                builder.Register<ISodarDriver>(x => CreateSodarDriverMock().Object);
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(BaseService).Assembly)
                .Where(t => t.IsSubclassOf(typeof(BaseService)))
                .AsImplementedInterfaces()
                .As<BaseService>()
                .SingleInstance();
        }

        private static void RegisterPrograms(ContainerBuilder builder, IProgramAssemblyProvider programAssemblyProvider)
        {
            var assemblies = programAssemblyProvider.GetAbsolutePaths()
                .Select(p => Assembly.LoadFile(p))
                .ToArray();

            if (assemblies.Any())
            {
                builder.RegisterAssemblyTypes(assemblies)
                    .Where(t => t.GetInterfaces().Contains(typeof(IProgram)))
                    .As<IProgram>();
            }
        }
    }
}