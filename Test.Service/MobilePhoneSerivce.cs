using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Common;
using Test.Model;
using Test.Model.Common;
using Test.Repository;
using Test.Repository.Common;
using Test.Service.Common;

namespace Test.Service
{
    public class MobilePhoneSerivce : IMobilePhoneService
    {
        private readonly IMobilePhoneRepository mobilePhoneRepository;
        private readonly ShopRepository shopRepository;

        public MobilePhoneSerivce()
        {
            mobilePhoneRepository = new MobilePhoneRepository();
            shopRepository = new ShopRepository();
        }

        public List<IMobilePhone> GetAll(MobilePhoneFilter filter)
        {
            try
            {
                return mobilePhoneRepository.GetAll(filter);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IMobilePhone GetById(Guid id, bool includeShops = false)
        {
            try
            {
                return mobilePhoneRepository.GetById(id, includeShops);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Add(IMobilePhone mobilePhone)
        {
            try
            {
                mobilePhoneRepository.Add(mobilePhone);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void AddShops(Guid mobilePhoneId, List<Guid> shopIds)
        {
            try
            {
                foreach (var shopId in shopIds)
                {
                    if (shopRepository.GetById(shopId) == null)
                    {
                        throw new Exception($"Shop with id {shopId} not found");
                    }
                }
                mobilePhoneRepository.AddShops(mobilePhoneId, shopIds);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Update(Guid id, IMobilePhone mobilePhone)
        {
            try
            {
                mobilePhoneRepository.Update(id, mobilePhone);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Delete(Guid id)
        {
            try
            {
                mobilePhoneRepository.Delete(id);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
