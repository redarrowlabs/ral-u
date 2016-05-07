using System;

namespace Autofac101.Services.ProvideManners
{
	public class FrenchMannersProvider : IProvideWelcome, IProvideFarewell
	{
		public void Greet()
		{
			Console.WriteLine("Bonjour!");
		}

		public void Farewell()
		{
			Console.WriteLine("Au Revoir!");
		}
	}
}
