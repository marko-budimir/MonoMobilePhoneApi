using System;
using Test.Model.Common;

namespace Test.Model
{
    public class Shop :IShop

    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Mail { get; set; }
    }
}
