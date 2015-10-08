using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.SignalR;
using RabbitMQ.Client;
using SignalR.MongoRabbit;

namespace SignalR.RabbitMQ.Example
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
           
            var factory = new ConnectionFactory
            {
                UserName = "guest",
                Password = "guest",
                HostName = "localhost"
            };

            var mongoConnectionString = "mongodb://localhost/signalr-test";
            var exchangeName = "SignalR.RabbitMQ-Example";
            var configuration = new MongoRabbitScaleoutConfiguration(mongoConnectionString, factory, exchangeName);
            var connection = new MongoRabbitConnection(configuration);
            GlobalHost.DependencyResolver.UseRabbitMqAdvanced(connection, configuration);
        }
    }
}