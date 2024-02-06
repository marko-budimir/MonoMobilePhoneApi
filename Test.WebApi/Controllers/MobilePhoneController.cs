using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Test.WebApi.Models;

namespace Test.WebApi.Controllers
{
    public class MobilePhoneController : ApiController
    {
        private static List<MobilePhone> mobilePhones = new List<MobilePhone>
        {
            new MobilePhone { Id = 0, Brand = "Apple", Model = "iPhone 6", OperatingSystem = "iOS", StorageCapacityGB = 64, RamGB = 1, Color = "Space Gray" },
            new MobilePhone { Id = 1, Brand = "Samsung", Model = "Galaxy S6", OperatingSystem = "Android", StorageCapacityGB = 32, RamGB = 3, Color = "Black" },
            new MobilePhone { Id = 3, Brand = "Samsung", Model = "Galaxy S6", OperatingSystem = "Android", StorageCapacityGB = 32, RamGB = 3, Color = "Gold" },
            new MobilePhone { Id = 4, Brand = "Samsung", Model = "Galaxy S7", OperatingSystem = "Android", StorageCapacityGB = 32, RamGB = 4, Color = "Black" },
            new MobilePhone { Id = 5, Brand = "Samsung", Model = "Galaxy S7 Edge", OperatingSystem = "Android", StorageCapacityGB = 64, RamGB = 4, Color = "Black" },
            new MobilePhone { Id = 2,  Brand = "Huawei", Model = "P10", OperatingSystem = "Android", StorageCapacityGB = 32, RamGB = 3, Color = "Blue" }
        };

        // GET: api/MobilePhone
        public HttpResponseMessage Get([FromUri] MobilePhoneFilter filter)
        {
            if (filter == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, mobilePhones.OrderBy(m => m.Id));
            }
            var mobilePhonesFiltered = mobilePhones.Where(m =>
                (string.IsNullOrEmpty(filter.Brand) || m.Brand.Equals(filter.Brand, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(filter.Model) || m.Model.IndexOf(filter.Model, StringComparison.OrdinalIgnoreCase) >= 0) &&
                (string.IsNullOrEmpty(filter.OperatingSystem) || m.OperatingSystem.Equals(filter.OperatingSystem, StringComparison.OrdinalIgnoreCase)) &&
                (filter.StorageCapacityGB == 0 || m.StorageCapacityGB == filter.StorageCapacityGB) &&
                (filter.RamGB == 0 || m.RamGB == filter.RamGB) &&
                (string.IsNullOrEmpty(filter.Color) || m.Color.Equals(filter.Color, StringComparison.OrdinalIgnoreCase))
            ).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, mobilePhonesFiltered.OrderBy(m => m.Id));
        }

        // GET: api/MobilePhone/5
        public HttpResponseMessage Get(int id)
        {
            var mobilePhone = mobilePhones.FirstOrDefault(m => m.Id == id);
            if (mobilePhone == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Mobile phone with this ID doesn't exists");
            }
            return Request.CreateResponse(HttpStatusCode.OK, mobilePhone);
        }

        

        // POST: api/MobilePhone
        public HttpResponseMessage Post([FromBody] MobilePhone mobilePhone)
        {
            if (mobilePhone == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            else if (mobilePhones.Any(m => m.Id == mobilePhone.Id))
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, "Mobile phone with this ID already exists");
            }
            
            mobilePhones.Add(mobilePhone);
            return Request.CreateResponse(HttpStatusCode.Created, mobilePhone);
        }

        // PUT: api/MobilePhone/5
        public HttpResponseMessage Put(int id, [FromBody] MobilePhoneUpdate mobilePhoneUpdate)
        {
            if (mobilePhoneUpdate == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            else if (!mobilePhones.Any(m => m.Id == id))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Mobile phone with this ID doesn't exists");
            }
            var mobilePhone = mobilePhones.FirstOrDefault(m => m.Id == id);
            mobilePhone.OperatingSystem = mobilePhoneUpdate.OperatingSystem;
            mobilePhone.StorageCapacityGB = mobilePhoneUpdate.StorageCapacityGB;
            mobilePhone.RamGB = mobilePhoneUpdate.RamGB;
            mobilePhone.Color = mobilePhoneUpdate.Color;
            mobilePhones.Remove(mobilePhones.FirstOrDefault(m => m.Id == id));
            mobilePhones.Add(mobilePhone);
            return Request.CreateResponse(HttpStatusCode.OK, mobilePhone);
        }

        // DELETE: api/MobilePhone/5
        public HttpResponseMessage Delete(int id)
        {
            if (!mobilePhones.Any(m => m.Id == id))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Mobile phone with this ID doesn't exists");
            }
            mobilePhones.Remove(mobilePhones.FirstOrDefault(m => m.Id == id));
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
