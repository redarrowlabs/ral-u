using System;
using System.Linq;

namespace Autofac101.Services.ComputeFibonacci
{
	public class FibonacciSequenceComputer : IComputeFibonacciSequence
	{
		private IComputeFibonacciNumber FibonacciNumber { get; }

		public FibonacciSequenceComputer(IComputeFibonacciNumber fibonacciNumber)
		{
			FibonacciNumber = fibonacciNumber;
		}

		public void Compute()
		{
			var Count = 10;
			var sequence = Enumerable.Range(0, Count)
				.Select(FibonacciNumber.Compute)
				.ToArray();

			Console.WriteLine($"The first {Count} numbers in the Fibonacci sequence are: ");
			Console.WriteLine(string.Join(" ", sequence));
		}
	}
}
