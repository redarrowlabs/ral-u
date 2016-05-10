using autofac110.shared.HelloWorld;
using Autofac;

namespace autofac110.shared.Autofac
{
	public class WebModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<SayHelloWorld>()
				.AsImplementedInterfaces();
		}
	}
}
