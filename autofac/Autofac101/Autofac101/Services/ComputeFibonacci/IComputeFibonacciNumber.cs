using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autofac101.Services.ComputeFibonacci
{
	public interface IComputeFibonacciNumber
	{
		int Compute(int n);
	}
}
