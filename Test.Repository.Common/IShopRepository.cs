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
        Task AddAsync(IShop shop);
        Task UpdateAsync(Guid id, IShop shop);
        Task DeleteAsync(Guid id);
    }
}
