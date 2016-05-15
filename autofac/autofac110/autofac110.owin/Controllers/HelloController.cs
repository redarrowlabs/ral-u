using System.Web.Http;

namespace autofac110.owin.Controllers
{
	public class HelloController : ApiController
	{
		public string Get()
		{
			return "Hello World!  I'm a hardcoded message!";
		}
	}
}