using System;
using System.Linq;
using System.Runtime.InteropServices;
using Autofac;
using Core.Runtime;
using Core.Services;
using Core.Drivers;
using System.Reflection;
using Core.Services.Models;

namespace Core
{
    public static partial class Bootstrap
    {
        public static void Configure(ContainerBuilder builder, IProgramAssemblyProvider programAssemblyProvider)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                builder.Register<ServoDriver>(x => new ServoDriver(0x40)).AsImplementedInterfaces().SingleInstance();
                builder.Register<SodarDriver>(x => new SodarDriver(0x41)).AsImplementedInterfaces().SingleInstance();
                builder.Register<IMUDriver>(x => new IMUDriver(0x43, MPU6050Scale.MPU6050_SCALE_2000DPS, MPU6050Range.MPU6050_RANGE_2G)).AsImplementedInterfaces().SingleInstance();
            }
            else
            {
                RegisterMockImplementations(builder);
            }

            RegisterServices(builder);

            if (programAssemblyProvider != null)
                RegisterPrograms(builder, programAssemblyProvider);

            builder.RegisterType<ProgramResolver>().AsImplementedInterfaces();
            builder.RegisterType<SensorPlatform>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ServiceRunner>().SingleInstance();
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