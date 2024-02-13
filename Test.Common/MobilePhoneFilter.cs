using System;

namespace Test.Common
{
    public class MobilePhoneFilter
    {
        public string SearchQuery { get; set;}
        public Guid? ShopId { get; set; }
        public int? MinStorageCapacityGB { get; set; }
        public int? MaxStorageCapacityGB { get; set; }
        public int? MinRamGB { get; set; }
        public int? MaxRamGB { get; set; }
    }
}
