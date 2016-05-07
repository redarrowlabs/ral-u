using System;

namespace Autofac101.Services.ProvideManners
{
	public class SpanishMannersProvider : IProvideWelcome, IProvideFarewell
	{
		public void Greet()
		{
			Console.WriteLine("¡Hola!");
		}

		public void Farewell()
		{
			Console.WriteLine("¡Adiós!");
		}
	}
}
