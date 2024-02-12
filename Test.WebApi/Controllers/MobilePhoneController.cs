using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Test.Common;
using Test.Model;
using Test.Model.Common;
using Test.Service.Common;
using Test.WebApi.Models;

namespace Test.WebApi.Controllers
{
    public class MobilePhoneController : ApiController
    {
        private readonly IMobilePhoneService MobilePhoneService;

        public MobilePhoneController(IMobilePhoneService mobilePhoneService)
        {
            MobilePhoneService = mobilePhoneService;
        }

        // GET: api/MobilePhone
        //filter by brand, model, operating system, storage capacity, ram, color
        public async Task<HttpResponseMessage> GetAsync([FromUri] MobilePhoneFilter filter)
        {
            List<IMobilePhone> mobilePhones;
            try { 
                mobilePhones = await MobilePhoneService.GetAllAsync(filter);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadGateway, e.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, mobilePhones);
        }

        // GET: api/MobilePhone/5
        public async Task<HttpResponseMessage> GetAsync(Guid id, bool includeShops = false)
        {
            IMobilePhone mobilePhone = await MobilePhoneService.GetByIdAsync(id, includeShops);
            if (mobilePhone == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Mobile phone with this ID doesn't exists");
            }
            return Request.CreateResponse(HttpStatusCode.OK, mobilePhone);
        }

        // POST: api/MobilePhone
        public async Task<HttpResponseMessage> PostAsync([FromBody] MobilePhone mobilePhone)
        {
            if (mobilePhone == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            try 
            { 
                await MobilePhoneService.AddAsync(mobilePhone);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadGateway, e.Message);
            }
            return Request.CreateResponse(HttpStatusCode.Created);
        }

        // POST: api/MobilePhone/5
        public async Task<HttpResponseMessage> PostAsync(Guid mobilePhoneId, [FromBody] List<Guid> shopsId)
        {
            if (shopsId == null || mobilePhoneId == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            IMobilePhone mobilePhone = await MobilePhoneService.GetByIdAsync(mobilePhoneId);
            if (mobilePhone == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Mobile phone with this ID doesn't exists");
            }

            try
            {
                await MobilePhoneService.AddShopsAsync(mobilePhoneId, shopsId);
                mobilePhone = await MobilePhoneService.GetByIdAsync(mobilePhoneId, true);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadGateway, e.Message);
            }
            return Request.CreateResponse(HttpStatusCode.Created, mobilePhone);
        }

        // PUT: api/MobilePhone/5
        public async Task<HttpResponseMessage> PutAsync(Guid id, [FromBody] MobilePhoneUpdate mobilePhoneUpdate)
        {
            if (mobilePhoneUpdate == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            IMobilePhone mobilePhone = await MobilePhoneService.GetByIdAsync(id);
            if (mobilePhone == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Mobile phone with this ID doesn't exists");
            }

            try
            {
                await MobilePhoneService.UpdateAsync(id, new MobilePhone()
                {
                    OperatingSystem = mobilePhoneUpdate.OperatingSystem,
                    StorageCapacityGB = mobilePhoneUpdate.StorageCapacityGB,
                    RamGB = mobilePhoneUpdate.RamGB,
                    Color = mobilePhoneUpdate.Color
                });
                mobilePhone = await MobilePhoneService.GetByIdAsync(id);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadGateway, e.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, mobilePhone);
        }

        // DELETE: api/MobilePhone/5
        public async Task<HttpResponseMessage> DeleteAsync(Guid id)
        {
            if(id == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            IMobilePhone mobilePhone = await MobilePhoneService.GetByIdAsync(id);

            if (mobilePhone == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Mobile phone with this ID doesn't exists");
            }

            try
            {
                await MobilePhoneService.DeleteAsync(id);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadGateway, e.Message);
            }
            
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

    }
}
