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
        Task AddAsync(IMobilePhone mobilePhone);
        Task AddShopsAsync(Guid mobilePhoneId, List<Guid> shopIds);
        Task UpdateAsync(Guid id, IMobilePhone mobilePhone);
        Task DeleteAsync(Guid id);
    }
}
