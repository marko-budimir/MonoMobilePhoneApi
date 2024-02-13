using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Common;
using Test.Model;
using Test.Model.Common;

namespace Test.Repository.Common
{
    public interface IMobilePhoneRepository
    {
        Task<List<IMobilePhone>> GetAllAsync(MobilePhoneFilter filter);
        Task<IMobilePhone> GetByIdAsync(Guid id, bool includeShops = false);
        Task<int> AddAsync(IMobilePhone mobilePhone);
        Task<int> AddShopsAsync(Guid mobilePhoneId, List<Guid> shopIds);
        Task<int> UpdateAsync(Guid id, IMobilePhone mobilePhone);
        Task<int> DeleteAsync(Guid id);
    }
}
