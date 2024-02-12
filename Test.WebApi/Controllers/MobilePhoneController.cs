using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Test.Common;
using Test.Model;
using Test.Model.Common;
using Test.Service;
using Test.Service.Common;
using Test.WebApi.Models;

namespace Test.WebApi.Controllers
{
    public class MobilePhoneController : ApiController
    {
        private readonly IMobilePhoneService mobilePhoneService = new MobilePhoneSerivce();

        // GET: api/MobilePhone
        //filter by brand, model, operating system, storage capacity, ram, color
        public async Task<HttpResponseMessage> GetAsync([FromUri] MobilePhoneFilter filter)
        {
            List<IMobilePhone> mobilePhones;
            try { 
                mobilePhones = await mobilePhoneService.GetAllAsync(filter);
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
            IMobilePhone mobilePhone = await mobilePhoneService.GetByIdAsync(id, includeShops);
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
                await mobilePhoneService.AddAsync(mobilePhone);
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
            IMobilePhone mobilePhone = await mobilePhoneService.GetByIdAsync(mobilePhoneId);
            if (mobilePhone == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Mobile phone with this ID doesn't exists");
            }

            try
            {
                await mobilePhoneService.AddShopsAsync(mobilePhoneId, shopsId);
                mobilePhone = await mobilePhoneService.GetByIdAsync(mobilePhoneId, true);
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
            IMobilePhone mobilePhone = await mobilePhoneService.GetByIdAsync(id);
            if (mobilePhone == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Mobile phone with this ID doesn't exists");
            }

            try
            {
                await mobilePhoneService.UpdateAsync(id, new MobilePhone()
                {
                    OperatingSystem = mobilePhoneUpdate.OperatingSystem,
                    Color = mobilePhoneUpdate.Color
                });
                mobilePhone = await mobilePhoneService.GetByIdAsync(id);
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
            IMobilePhone mobilePhone = await mobilePhoneService.GetByIdAsync(id);

            if (mobilePhone == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Mobile phone with this ID doesn't exists");
            }

            try
            {
                await mobilePhoneService.DeleteAsync(id);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadGateway, e.Message);
            }
            
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

    }
}
