using System;

namespace SimpleCalculator.Common.Exceptions
{
	public class InvalidExpressionException : Exception
	{
		public InvalidExpressionException(string message)
			: base(message)
		{
		}
	}
}