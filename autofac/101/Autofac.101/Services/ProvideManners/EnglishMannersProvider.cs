using System;

namespace Autofac101.Services.ProvideManners
{
	public class EnglishMannersProvider : IProvideWelcome, IProvideFarewell
	{
		public void Greet()
		{
			Console.WriteLine("Hello!");
		}

		public void Farewell()
		{
			Console.WriteLine("Goodbye!");
		}
	}
}
