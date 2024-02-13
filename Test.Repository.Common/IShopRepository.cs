using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Model;
using Test.Model.Common;

namespace Test.Repository.Common
{
    public interface IShopRepository
    {
        Task<List<IShop>> GetAllAsync();
        Task<IShop> GetByIdAsync(Guid id);
        Task<int> AddAsync(IShop shop);
        Task<int> UpdateAsync(Guid id, IShop shop);
        Task<int> DeleteAsync(Guid id);
    }
}
