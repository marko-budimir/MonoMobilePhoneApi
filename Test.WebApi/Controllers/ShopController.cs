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
        private string connectionString = "Server=127.0.0.1;Port=5432;Database=MonoTest;User Id=postgres;Password=postgres;";
        private readonly IShopService shopService;

        public ShopController()
        {
            shopService = new ShopService();
        }

        // GET: api/Shop
        public async Task<HttpResponseMessage> GetAsync()
        {
            List<IShop> shops;
            try {
                shops = await shopService.GetAllAsync();
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
                shop = await shopService.GetByIdAsync(id);
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
                await shopService.AddAsync(shop);
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
            IShop shop = await shopService.GetByIdAsync(id);
            if (shop == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            try
            {
                await shopService.UpdateAsync(id, new Shop()
                {
                    Name = newShop.Name,
                    Address = newShop.Address,
                    Mail = newShop.Mail,
                    PhoneNumber = newShop.PhoneNumber
                });
                shop = await shopService.GetByIdAsync(id);
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
            IShop shop = await shopService.GetByIdAsync(id);
            if (shop == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Shop with this ID doesn't exists");
            }
            await shopService.DeleteAsync(id);
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
