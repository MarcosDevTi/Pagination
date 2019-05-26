
using Arch.Infra.IoC;
using AutoMapper;
using AutoMapper.Configuration;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Lifestyles;
using System.Web;
using System.Web.Http;

namespace Arch.Api
{
    public class SimpleInjectorInitializer
    {
        private static Container _container;

        public static void Initialize(Container container, HttpConfiguration config)
        {
            _container = container;



            var hybrid = Lifestyle.CreateHybrid(() => HttpContext.Current != null,
                new WebRequestLifestyle(),
                new ThreadScopedLifestyle());

            _container.Options.DefaultScopedLifestyle = hybrid;

            InitializeContainer();
            //_container.Register<NotificationFilter>();
            container.Verify();
            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
        }

        private static void InitializeContainer()
        {
            ArchBootstrapper.Register(_container);
        }
    }
}