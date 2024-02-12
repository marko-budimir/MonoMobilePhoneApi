using Autofac.Integration.WebApi;
using Autofac;
using System.Reflection;
using Test.Repository;
using Test.Service;
using System.Web.Http;

namespace Test.WebApi.App_Start
{
    public static class DIConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterModule(new ServiceModules());
            builder.RegisterModule(new RepositoryModules());
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}