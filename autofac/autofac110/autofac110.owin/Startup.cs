using System.Web.Http;
using autofac110.api;
using autofac110.owin;
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

			app.UseWebApi(config);
		}
	}
}