using System.Linq;
using Core.Services;
using Core.Services.Models;
using Core.Drivers;
using Moq;
using Xunit;
using Core.Runtime.CommandBus;

namespace Core.Tests
{
    public class SodarServiceTests
    {
        [Fact]
        public void Loop_GetValues_CallsSodarDriver()
        {
            var bus = new Mock<ICommandBus>();
            var sodarDriver = new Mock<ISodarDriver>();
            var sodarService = new SodarService(sodarDriver.Object, bus.Object);

            sodarService.Loop();

            sodarDriver.Verify(x => x.GetRanges());
        }

        // [Fact]
        // public void Loop_Bus_CorrectValuesAreSend()
        // {
        //     var ranges = new int[] { 0, 0, 0, 34, 54, 123, 34, 0, 0 };
        //     new Mock<ICommandBus>();
        //     var sodarDriver = new Mock<ISodarDriver>();
        //     sodarDriver.Setup(x => x.GetRanges())
        //         .Returns(ranges);

        //     var sodarService = new SodarService(sodarDriver.Object, bus.Object);

        //     sodarService.Loop();

        //     var update = bus.Events.OfType<MessageSent<SodarUpdate>>().Single();
        //     Assert.True(ranges.SequenceEqual(update.CommandMessage.Ranges));
        // }
    }
}