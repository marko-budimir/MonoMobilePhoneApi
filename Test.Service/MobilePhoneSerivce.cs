using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Test.Common;
using Test.Model.Common;
using Test.Repository;
using Test.Repository.Common;
using Test.Service.Common;

namespace Test.Service
{
    public class MobilePhoneSerivce : IMobilePhoneService
    {
        private readonly IMobilePhoneRepository MobilePhoneRepository;
        private readonly IShopRepository ShopRepository;

        public MobilePhoneSerivce(IMobilePhoneRepository mobilePhoneRepository, IShopRepository shopRepository)
        {
            MobilePhoneRepository = mobilePhoneRepository;
            ShopRepository = shopRepository;
        }

        public async Task<List<IMobilePhone>> GetAllAsync(MobilePhoneFilter filter)
        {
            try
            {
                return await MobilePhoneRepository.GetAllAsync(filter);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<IMobilePhone> GetByIdAsync(Guid id, bool includeShops = false)
        {
            try
            {
                return await MobilePhoneRepository.GetByIdAsync(id, includeShops);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task AddAsync(IMobilePhone mobilePhone)
        {
            try
            {
                await MobilePhoneRepository.AddAsync(mobilePhone);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task AddShopsAsync(Guid mobilePhoneId, List<Guid> shopIds)
        {
            try
            {
                foreach (var shopId in shopIds)
                {
                    if (ShopRepository.GetByIdAsync(shopId) == null)
                    {
                        throw new Exception($"Shop with id {shopId} not found");
                    }
                }
                await MobilePhoneRepository.AddShopsAsync(mobilePhoneId, shopIds);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task UpdateAsync(Guid id, IMobilePhone mobilePhone)
        {
            try
            {
                await MobilePhoneRepository.UpdateAsync(id, mobilePhone);
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
                await MobilePhoneRepository.DeleteAsync(id);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
