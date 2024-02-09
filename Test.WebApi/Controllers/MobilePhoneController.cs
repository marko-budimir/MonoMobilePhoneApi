using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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
        public HttpResponseMessage Get([FromUri] MobilePhoneFilter filter)
        {
            List<IMobilePhone> mobilePhones;
            try { 
                mobilePhones = mobilePhoneService.GetAll(filter);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadGateway, e.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, mobilePhones);
        }

        // GET: api/MobilePhone/5
        public HttpResponseMessage Get(Guid id, bool includeShops = false)
        {
            IMobilePhone mobilePhone = mobilePhoneService.GetById(id, includeShops);
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

            try 
            { 
                mobilePhoneService.Add(mobilePhone);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadGateway, e.Message);
            }
            return Request.CreateResponse(HttpStatusCode.Created);
        }

        // POST: api/MobilePhone/5
        public HttpResponseMessage Post(Guid mobilePhoneId, [FromBody] List<Guid> shopsId)
        {
            if (shopsId == null || mobilePhoneId == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            IMobilePhone mobilePhone = mobilePhoneService.GetById(mobilePhoneId);
            if (mobilePhone == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Mobile phone with this ID doesn't exists");
            }

            try
            {
                mobilePhoneService.AddShops(mobilePhoneId, shopsId);
                mobilePhone = mobilePhoneService.GetById(mobilePhoneId, true);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadGateway, e.Message);
            }
            return Request.CreateResponse(HttpStatusCode.Created, mobilePhone);
        }

        // PUT: api/MobilePhone/5
        public HttpResponseMessage Put(Guid id, [FromBody] MobilePhoneUpdate mobilePhoneUpdate)
        {
            if (mobilePhoneUpdate == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            IMobilePhone mobilePhone = mobilePhoneService.GetById(id);
            if (mobilePhone == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Mobile phone with this ID doesn't exists");
            }

            try
            {
                mobilePhoneService.Update(id, new MobilePhone()
                {
                    OperatingSystem = mobilePhoneUpdate.OperatingSystem,
                    Color = mobilePhoneUpdate.Color
                });
                mobilePhone = mobilePhoneService.GetById(id);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadGateway, e.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, mobilePhone);
        }

        // DELETE: api/MobilePhone/5
        public HttpResponseMessage Delete(Guid id)
        {
            if(id == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            IMobilePhone mobilePhone = mobilePhoneService.GetById(id);

            if (mobilePhone == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Mobile phone with this ID doesn't exists");
            }

            try
            {
                mobilePhoneService.Delete(id);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadGateway, e.Message);
            }
            
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

    }
}
