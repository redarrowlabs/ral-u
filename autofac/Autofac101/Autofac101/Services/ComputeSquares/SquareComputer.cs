using System;
using System.Linq;

namespace Autofac101.Services.ComputeSquares
{
	public class SquareComputer : IComputeSquares
	{
		public void Compute()
		{
			var squareCount = 10;
			var squares = Enumerable.Range(0, squareCount)
				.Select(i => i*i)
				.ToArray();

			Console.WriteLine($"The first {squareCount} squares are:");
			Console.WriteLine(string.Join(" ", squares));
		}
	}
}
