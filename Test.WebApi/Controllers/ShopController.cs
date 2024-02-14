using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Test.Model;
using Test.Model.Common;
using Test.Service.Common;
using Test.WebApi.Models;


namespace Test.WebApi.Controllers
{
    public class ShopController : ApiController
    {
        private readonly IShopService _shopService;

        public ShopController(IShopService shopService)
        {
            _shopService = shopService;
        }

        // GET: api/Shop
        public async Task<HttpResponseMessage> GetAsync()
        {
            List<IShop> shops;
            try {
                shops = await _shopService.GetAllAsync();
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
                shop = await _shopService.GetByIdAsync(id);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            if (shop == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Shop with this ID doesn't exists");
            }
            return Request.CreateResponse(HttpStatusCode.OK, shop);
        }

        // POST: api/Shop
        public async Task<HttpResponseMessage> PostAsync([FromBody]ShopPost shopPost)
        {
            if (shopPost == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            IShop shop = new Shop()
            {
                Id = Guid.NewGuid(),
                Name = shopPost.Name,
                Address = shopPost.Address,
                Mail = shopPost.Mail,
                PhoneNumber = shopPost.PhoneNumber
            };
            int affectedRows = await _shopService.AddAsync(shop);
            
            if (affectedRows <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadGateway, "Error while adding shop");
            }
            return Request.CreateResponse(HttpStatusCode.Created, shop);
        }

        // PUT: api/Shop/5
        public async Task<HttpResponseMessage> PutAsync(Guid id, [FromBody]ShopUpdate newShop)
        {
            if (newShop == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            int affectedRows = await _shopService.UpdateAsync(id, new Shop()
            {
                Name = newShop.Name,
                Address = newShop.Address,
                Mail = newShop.Mail,
                PhoneNumber = newShop.PhoneNumber
            });
            if (affectedRows == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Shop with this ID doesn't exists");
            }
            else if (affectedRows < 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadGateway, "Error while updating shop");
            }
            IShop shop = await _shopService.GetByIdAsync(id);
            return Request.CreateResponse(HttpStatusCode.OK, shop);
        }

        // DELETE: api/Shop/5
        public async Task<HttpResponseMessage> DeleteAsync(Guid id)
        {
            int rowsAffected = await _shopService.DeleteAsync(id);
            if (rowsAffected == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Shop with this ID doesn't exists");
            }
            else if (rowsAffected == -1)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
