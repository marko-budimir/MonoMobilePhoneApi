using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Common
{
    public class MobilePhoneFilter
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public string OperatingSystem { get; set; }
        public int? StorageCapacityGB { get; set; }
        public int? RamGB { get; set; }
        public string Color { get; set; }
    }
}
