using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Model;
using Test.Model.Common;

namespace Test.Service.Common
{
    public interface IShopService
    {
        List<IShop> GetAll();
        IShop GetById(Guid id);
        void Add(IShop shop);
        void Update(Guid id, IShop shop);
        void Delete(Guid id);
    }
}
