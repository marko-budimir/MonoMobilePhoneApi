using Autofac;
using Test.Repository.Common;

namespace Test.Repository
{
    public class RepositoryModules : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MobilePhoneRepository>().As<IMobilePhoneRepository>();
            builder.RegisterType<ShopRepository>().As<IShopRepository>();
        }
    }
}
