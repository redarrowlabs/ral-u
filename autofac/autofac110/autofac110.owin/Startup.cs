using System.Web.Http;
using autofac110.api;
using autofac110.owin;
using autofac110.owin.Middleware.HelloWorld;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace autofac110.owin
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			var config = new HttpConfiguration();

			WebApiConfig.Register(config);
			
			//override response
			app.Use<HelloWorldMiddleware>();

			app.UseWebApi(config);
		}
	}
}