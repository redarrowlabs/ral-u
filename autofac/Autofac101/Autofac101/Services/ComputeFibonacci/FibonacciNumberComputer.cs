using System;
using System.Linq;

namespace Autofac101.Services.ComputeFibonacci
{
	public class FibonacciNumberComputer : IComputeFibonacciNumber
	{
		public void Compute()
		{
			var count = 10;

			var sequence = Enumerable.Range(0, count)
				.Select(Compute)
				.ToArray();

			Console.WriteLine($"The first {count} numbers in the Fibonacci sequence are:");
			Console.WriteLine(string.Join(" ", sequence));
		}

		public int Compute(int n)
		{
			var a = 0;
			var b = 1;
			// In N steps compute Fibonacci sequence iteratively.
			for (var i = 0; i < n; i++)
			{
				var temp = a;
				a = b;
				b = temp + b;
			}
			return a;
		}
	}
}
