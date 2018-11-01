using System.Linq;
using Core.Services;
using Core.Services.Models;
using Core.Sodar;
using Moq;
using Rebus.TestHelpers;
using Rebus.TestHelpers.Events;
using Xunit;

namespace Core.Tests
{
    public class SodarServiceTests
    {
        [Fact]
        public void Loop_GetValues_CallsSodarDriver()
        {
            var bus = new FakeBus();
            var sodarDriver = new Mock<ISodarDriver>();
            var sodarService = new SodarService(sodarDriver.Object, bus);

            sodarService.Loop();

            sodarDriver.Verify(x => x.GetRanges());
        }

        [Fact]
        public void Loop_Bus_CorrectValuesAreSend()
        {
            var ranges = new int[] { 0, 0, 0, 34, 54, 123, 34, 0, 0 };
            var bus = new FakeBus();
            var sodarDriver = new Mock<ISodarDriver>();
            sodarDriver.Setup(x => x.GetRanges())
                .Returns(ranges);

            var sodarService = new SodarService(sodarDriver.Object, bus);

            sodarService.Loop();

            var update = bus.Events.OfType<MessageSent<SodarUpdate>>().Single();
            Assert.True(ranges.SequenceEqual(update.CommandMessage.Ranges));
        }
    }
}