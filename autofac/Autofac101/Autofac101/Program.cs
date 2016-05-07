﻿using System;
using Autofac101.App;

namespace Autofac101
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var app = Bootstrapper.Bootstrap();
			app.Run();
			Console.WriteLine("Program completed successfully.  Press any key to exit");
			Console.ReadLine();
		}
	}
}
