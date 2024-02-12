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
        Task<List<IShop>> GetAllAsync();
        Task<IShop> GetByIdAsync(Guid id);
        Task AddAsync(IShop shop);
        Task UpdateAsync(Guid id, IShop shop);
        Task DeleteAsync(Guid id);
    }
}
