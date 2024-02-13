using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test.WebApi.Models
{
    public class MobilePhonePost
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public string OperatingSystem { get; set; }
        public int StorageCapacity { get; set; }
        public int Ram { get; set; }
        public string Color { get; set; }
    }
}