using System;
using System.Collections.Generic;

namespace Test.Model.Common
{
    public interface IMobilePhone
    {
        Guid Id { get; set; }
        string Brand { get; set; }
        string Model { get; set; }
        string OperatingSystem { get; set; }
        int? StorageCapacityGB { get; set; }
        int? RamGB { get; set; }
        string Color { get; set; }
        List<IShop> Shops { get; set; }
    }
}
