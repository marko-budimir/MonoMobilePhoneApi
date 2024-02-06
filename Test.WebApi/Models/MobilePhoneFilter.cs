using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test.WebApi.Models
{
    public class MobilePhoneFilter
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public string OperatingSystem { get; set; }
        public int StorageCapacityGB { get; set; }
        public int RamGB { get; set; }
        public string Color { get; set; }
    }
}