namespace SimpleCalculator.Common.IO
{
	public interface ICommunicationService
	{
		string GetInput(string message = null);
		void WriteResponse(string response);
	}
}