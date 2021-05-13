using System;

namespace SimpleCalculator.Common.IO
{
	public class ConsoleCommunication: ICommunicationService
	{
		public string GetInput(string message = null)
		{
			WriteResponse(message);

			return Console.ReadLine();
		}

		public void WriteResponse(string response)
		{
			Console.WriteLine(response);
			Console.ResetColor();
		}
	}
}