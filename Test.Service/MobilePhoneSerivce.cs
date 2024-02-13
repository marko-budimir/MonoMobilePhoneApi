using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Test.Common;
using Test.Model.Common;
using Test.Repository.Common;
using Test.Service.Common;

namespace Test.Service
{
    public class MobilePhoneSerivce : IMobilePhoneService
    {
        private readonly IMobilePhoneRepository _mobilePhoneRepository;
        private readonly IShopRepository _shopRepository;

        public MobilePhoneSerivce(IMobilePhoneRepository mobilePhoneRepository, IShopRepository shopRepository)
        {
            _mobilePhoneRepository = mobilePhoneRepository;
            _shopRepository = shopRepository;
        }

        public async Task<List<IMobilePhone>> GetAllAsync(MobilePhoneFilter filter)
        {
            try
            {
                return await _mobilePhoneRepository.GetAllAsync(filter);
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
                return await _mobilePhoneRepository.GetByIdAsync(id, includeShops);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<int> AddAsync(IMobilePhone mobilePhone)
        {
            return await _mobilePhoneRepository.AddAsync(mobilePhone);
        }

        public async Task<int> AddShopsAsync(Guid mobilePhoneId, List<Guid> shopIds)
        {
            try
            {
                foreach (var shopId in shopIds)
                {
                    if (_shopRepository.GetByIdAsync(shopId) == null)
                    {
                        throw new Exception($"Shop with id {shopId} not found");
                    }
                }
            }
            catch (Exception e)
            {
                   throw e;
            }
            return await _mobilePhoneRepository.AddShopsAsync(mobilePhoneId, shopIds);

        }

        public async Task<int> UpdateAsync(Guid id, IMobilePhone mobilePhone)
        {
            return await _mobilePhoneRepository.UpdateAsync(id, mobilePhone);
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            return await _mobilePhoneRepository.DeleteAsync(id);
        }
    }
}
