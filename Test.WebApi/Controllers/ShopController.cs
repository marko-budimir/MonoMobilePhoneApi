using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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
        public HttpResponseMessage Get()
        {
            List<IShop> shops;
            try {
                shops = shopService.GetAll();
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, shops);
        }

        // GET: api/Shop/5
        public HttpResponseMessage Get(Guid id)
        {
            IShop shop;
            try 
            {
                shop = shopService.GetById(id);
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
        public HttpResponseMessage Post([FromBody]Shop shop)
        {
            if (shop == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            try
            {
                shopService.Add(shop);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            return Request.CreateResponse(HttpStatusCode.Created);
        }

        // PUT: api/Shop/5
        public HttpResponseMessage Put(Guid id, [FromBody]ShopUpdate newShop)
        {
            if (newShop == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            IShop shop = shopService.GetById(id);
            if (shop == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            try
            {
                shopService.Update(id, new Shop()
                {
                    Name = newShop.Name,
                    Address = newShop.Address,
                    Mail = newShop.Mail,
                    PhoneNumber = newShop.PhoneNumber
                });
                shop = shopService.GetById(id);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, shop);
        }

        // DELETE: api/Shop/5
        public HttpResponseMessage Delete(Guid id)
        {
            IShop shop = shopService.GetById(id);
            if (shop == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Shop with this ID doesn't exists");
            }
            shopService.Delete(id);
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
