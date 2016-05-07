using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac101.Services.ProvideManners;

namespace Autofac101.App
{
	public class Application
	{
		private IProvideWelcome WelcomeProvider { get; }
		private IProvideFarewell FarewellProvider { get; }

		public Application(
			IProvideWelcome welcomeProvider,
			IProvideFarewell farewellProvider)
		{
			WelcomeProvider = welcomeProvider;
			FarewellProvider = farewellProvider;
		}

		public void Run()
		{
			WelcomeProvider.Greet();
			FarewellProvider.Farewell();
		}
	}
}
