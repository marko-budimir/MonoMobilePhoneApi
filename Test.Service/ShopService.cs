using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Model;
using Test.Model.Common;
using Test.Repository;
using Test.Repository.Common;
using Test.Service.Common;

namespace Test.Service
{
    public class ShopService : IShopService
    {
        private readonly IShopRepository ShopRepository;

        public ShopService(IShopRepository shopRepository)
        {
            ShopRepository = shopRepository;
        }

        public async Task<List<IShop>> GetAllAsync()
        {
            try
            {
                return await ShopRepository.GetAllAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<IShop> GetByIdAsync(Guid id)
        {
            try
            {
                return await ShopRepository.GetByIdAsync(id);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<int> AddAsync(IShop shop)
        {
            return await ShopRepository.AddAsync(shop);
        }

        public async Task<int> UpdateAsync(Guid id, IShop shop)
        {
            return await ShopRepository.UpdateAsync(id, shop);
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            return await ShopRepository.DeleteAsync(id);
        }
    }
}
