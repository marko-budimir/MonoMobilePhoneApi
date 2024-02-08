using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Test.WebApi.Models
{
    public class MobilePhone
    {
        public Guid Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string OperatingSystem { get; set; }
        public int StorageCapacityGB { get; set; }
        public int RamGB { get; set; }
        public string Color { get; set; }
        public List<ShopView> Shops { get; set; }
    }
}