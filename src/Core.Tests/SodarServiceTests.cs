using Core.Services;
using Core.Sodar;
using Moq;
using Xunit;

namespace Core.Tests
{
    public class SodarServiceTests
    {
        [Fact]
        public void Loop_GetValues_CallsSodarDriver()
        {
            var sodarDriver = new Mock<ISodarDriver>();
            var sodarService = new SodarService(sodarDriver.Object);

            sodarService.Loop();
            
            sodarDriver.Verify(x => x.GetRanges());
        }
    }
}