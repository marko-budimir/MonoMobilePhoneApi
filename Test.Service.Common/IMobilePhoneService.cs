using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Common;
using Test.Model;
using Test.Model.Common;

namespace Test.Service.Common
{
    public interface IMobilePhoneService
    {
        List<IMobilePhone> GetAll(MobilePhoneFilter filter);
        IMobilePhone GetById(Guid id, bool includeShops = false);
        void Add(IMobilePhone mobilePhone);
        void AddShops(Guid mobilePhoneId, List<Guid> shopIds);
        void Update(Guid id, IMobilePhone mobilePhone);
        void Delete(Guid id);
    }
}
