using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Model.Common
{
    public interface IShop
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string PhoneNumber { get; set; }
        string Address { get; set; }
        string Mail { get; set; }
    }
}
