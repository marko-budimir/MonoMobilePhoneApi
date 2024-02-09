using System;
using System.Collections.Generic;
using Test.Model.Common;

namespace Test.Model
{
    public class MobilePhone : IMobilePhone
    {
        public Guid Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string OperatingSystem { get; set; }
        public int? StorageCapacityGB { get; set; }
        public int? RamGB { get; set; }
        public string Color { get; set; }
        public List<IShop> Shops { get; set; }
    }
}
