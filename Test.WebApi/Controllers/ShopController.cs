using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Test.Model;
using Test.Model.Common;
using Test.Service;
using Test.Service.Common;
using Test.WebApi.Models;

namespace Test.WebApi.Controllers
{
    public class ShopController : ApiController
    {
        private readonly IShopService ShopService;

        public ShopController(IShopService shopService)
        {
            ShopService = shopService;
        }

        // GET: api/Shop
        public async Task<HttpResponseMessage> GetAsync()
        {
            List<IShop> shops;
            try {
                shops = await ShopService.GetAllAsync();
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, shops);
        }

        // GET: api/Shop/5
        public async Task<HttpResponseMessage> GetAsync(Guid id)
        {
            IShop shop;
            try 
            {
                shop = await ShopService.GetByIdAsync(id);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            if (shop == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Request.CreateResponse(HttpStatusCode.OK, shop);
        }

        // POST: api/Shop
        public async Task<HttpResponseMessage> PostAsync([FromBody]Shop shop)
        {
            if (shop == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            try
            {
                await ShopService.AddAsync(shop);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            return Request.CreateResponse(HttpStatusCode.Created);
        }

        // PUT: api/Shop/5
        public async Task<HttpResponseMessage> PutAsync(Guid id, [FromBody]ShopUpdate newShop)
        {
            if (newShop == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            IShop shop = await ShopService.GetByIdAsync(id);
            if (shop == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            try
            {
                await ShopService.UpdateAsync(id, new Shop()
                {
                    Name = newShop.Name,
                    Address = newShop.Address,
                    Mail = newShop.Mail,
                    PhoneNumber = newShop.PhoneNumber
                });
                shop = await ShopService.GetByIdAsync(id);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, shop);
        }

        // DELETE: api/Shop/5
        public async Task<HttpResponseMessage> DeleteAsync(Guid id)
        {
            IShop shop = await ShopService.GetByIdAsync(id);
            if (shop == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Shop with this ID doesn't exists");
            }
            await ShopService.DeleteAsync(id);
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
