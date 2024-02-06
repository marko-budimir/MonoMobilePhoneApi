using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test.WebApi.Models
{
    public class MobilePhoneUpdate
    {
        public string OperatingSystem { get; set; }
        public int StorageCapacityGB { get; set; }
        public int RamGB { get; set; }
        public string Color { get; set; }
    }
}