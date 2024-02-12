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
        private readonly IShopRepository shopRepository;

        public ShopService()
        {
            this.shopRepository = new ShopRepository();
        }

        public async Task<List<IShop>> GetAllAsync()
        {
            try
            {
                return await shopRepository.GetAllAsync();
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
                return await shopRepository.GetByIdAsync(id);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task AddAsync(IShop shop)
        {
            try
            {
                await shopRepository.AddAsync(shop);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task UpdateAsync(Guid id, IShop shop)
        {
            try
            {
                await shopRepository.UpdateAsync(id, shop);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                await shopRepository.DeleteAsync(id);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
