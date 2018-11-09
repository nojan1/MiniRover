using Core;

namespace Web.Models
{
    public class CoreConfiguration : II2CAddressProvider
    {
        public int? ServoDriverAddress { get; set; }

        public int? IMUAddress { get; set; }

        public int? SodarAddress { get; set; }
    }
}