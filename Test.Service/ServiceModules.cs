using Autofac;
using Test.Service.Common;

namespace Test.Service
{
    public class ServiceModules : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MobilePhoneSerivce>().As<IMobilePhoneService>();
            builder.RegisterType<ShopService>().As<IShopService>();
        }
    }
}
