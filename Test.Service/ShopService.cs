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

        public List<IShop> GetAll()
        {
            try
            {
                return shopRepository.GetAll();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IShop GetById(Guid id)
        {
            try
            {
                return shopRepository.GetById(id);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Add(IShop shop)
        {
            try
            {
                shopRepository.Add(shop);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Update(Guid id, IShop shop)
        {
            try
            {
                shopRepository.Update(id, shop);
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
                shopRepository.Delete(id);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
