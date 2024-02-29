using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
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
        private readonly IMobilePhoneService _mobilePhoneService;

        public MobilePhoneController(IMobilePhoneService mobilePhoneService)
        {
            _mobilePhoneService = mobilePhoneService;
        }

        // GET: api/MobilePhone
        //filter by brand, model, operating system, storage capacity, ram, color
        public async Task<HttpResponseMessage> GetAsync(
            string searchQuery = "",
            Guid? shopId = null,
            int? minStorageCapacityGB = null,
            int? maxStorageCapacityGB = null,
            int? minRamGB = null,
            int? maxRamGB = null,
            string sortBy = "Brand",
            bool isAscending = true,
            int pageNumber = 1,
            int pageSize = 10
            )
        {
            MobilePhoneFilter filter = new MobilePhoneFilter
            {
                SearchQuery = searchQuery,
                ShopId = shopId,
                MinStorageCapacityGB = minStorageCapacityGB,
                MaxStorageCapacityGB = maxStorageCapacityGB,
                MinRamGB = minRamGB,
                MaxRamGB = maxRamGB
            };
            Sorting sorting = new Sorting
            {
                SortBy = sortBy,
                IsAscending = isAscending
            };
            Paging paging = new Paging
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            PagedList<IMobilePhone> mobilePhones;
            try { 
                mobilePhones = await _mobilePhoneService.GetAllAsync(filter, sorting, paging);
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
            IMobilePhone mobilePhone = await _mobilePhoneService.GetByIdAsync(id, includeShops);
            if (mobilePhone == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Mobile phone with this ID doesn't exists");
            }
            return Request.CreateResponse(HttpStatusCode.OK, mobilePhone);
        }

        // POST: api/MobilePhone
        public async Task<HttpResponseMessage> PostAsync([FromBody] MobilePhonePost mobilePhonePost)
        {
            if (mobilePhonePost == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            IMobilePhone mobilePhone = new MobilePhone
            {
                Id = Guid.NewGuid(),
                Brand = mobilePhonePost.Brand,
                Model = mobilePhonePost.Model,
                OperatingSystem = mobilePhonePost.OperatingSystem,
                StorageCapacityGB = mobilePhonePost.StorageCapacityGB,
                RamGB = mobilePhonePost.RamGB,
                Color = mobilePhonePost.Color
            };
            int rowsAffected = await _mobilePhoneService.AddAsync(mobilePhone);
            if (rowsAffected <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadGateway, "Error while adding mobile phone");
            }
            return Request.CreateResponse(HttpStatusCode.Created, mobilePhone);
            
        }

        // POST: api/MobilePhone/5
        public async Task<HttpResponseMessage> PostAsync(Guid mobilePhoneId, [FromBody] List<Guid> shopsId)
        {
            if (shopsId == null || mobilePhoneId == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            IMobilePhone mobilePhone = await _mobilePhoneService.GetByIdAsync(mobilePhoneId);
            if (mobilePhone == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Mobile phone with this ID doesn't exists");
            }
            int rowsAffected;
            try
            {
                rowsAffected = await _mobilePhoneService.AddShopsAsync(mobilePhoneId, shopsId);
                mobilePhone = await _mobilePhoneService.GetByIdAsync(mobilePhoneId, true);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadGateway, e.Message);
            }
            if (rowsAffected <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadGateway, "Error while adding shops to mobile phone");
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
            int rowsAffected = await _mobilePhoneService.UpdateAsync(id, new MobilePhone()
            {
                OperatingSystem = mobilePhoneUpdate.OperatingSystem,
                StorageCapacityGB = mobilePhoneUpdate.StorageCapacityGB,
                RamGB = mobilePhoneUpdate.RamGB,
                Color = mobilePhoneUpdate.Color
            });
            if (rowsAffected == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Mobile phone with this ID doesn't exists");
            }
            else if (rowsAffected < 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadGateway, "Error while updating mobile phone");
            }
            IMobilePhone mobilePhone = await _mobilePhoneService.GetByIdAsync(id);
            return Request.CreateResponse(HttpStatusCode.OK, mobilePhone); 
        }

        // DELETE: api/MobilePhone/5
        public async Task<HttpResponseMessage> DeleteAsync(Guid? id)
        {
            if(id == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "ID is invalid");
            }
            int rowsAffected = await _mobilePhoneService.DeleteAsync(id.Value);
            if (rowsAffected == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Mobile phone with this ID doesn't exists");
            } 
            else if (rowsAffected < 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadGateway, "Error while deleting mobile phone");
            }
            
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

    }
}
