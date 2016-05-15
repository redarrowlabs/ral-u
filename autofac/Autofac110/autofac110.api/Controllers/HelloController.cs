using System.Web.Http;

namespace autofac110.api.Controllers
{
    public class HelloController : ApiController
    {
	    public string Get()
	    {
		    return "Hello World!";
	    }
    }
}
