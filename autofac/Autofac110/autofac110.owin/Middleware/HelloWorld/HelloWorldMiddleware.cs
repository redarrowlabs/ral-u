using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

namespace autofac110.owin.Middleware.HelloWorld
{
	public class HelloWorldMiddleware : OwinMiddleware
	{
		public HelloWorldMiddleware(OwinMiddleware next) : base(next)
		{
		}

		public override Task Invoke(IOwinContext context)
		{
			return Next.Invoke(context);
		}
	}
}