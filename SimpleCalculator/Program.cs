using System.Linq;
using SimpleCalculator.Common.IO;
using SimpleCalculator.Common.Processors;

namespace SimpleCalculator
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Any())
			{
				new FileCalculator(new FileCommunication(), args.FirstOrDefault()).Run();
			}
			else
			{
				new ConsoleCalculator(new ConsoleCommunication()).Run();
			}
		}
	}
}
