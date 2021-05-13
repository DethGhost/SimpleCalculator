using System;
using System.Linq;
using SimpleCalculator.Common.Exceptions;
using SimpleCalculator.Common.IO;
using SimpleCalculator.Common.MessageTemplates;

namespace SimpleCalculator.Common.Processors
{
	public class ConsoleCalculator: CalculatorBase
	{
		public ConsoleCalculator(ICommunicationService communicationService) : base(communicationService)
		{
		}

		public override void Run()
		{
			while (true)
			{
				string expression = GetExpressionFromUser();
				string result = string.Empty;

				try
				{
					var exprList = expression.ToList();
					result = $"{expression}={ParseExpression(ref exprList)}";
					Console.ForegroundColor = ConsoleColor.Green;
				}
				catch (Exception e)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					result = e.Message;
				}
				finally
				{
					this._communicationService.WriteResponse(result);
				}
			}
		}

		private string GetExpressionFromUser()
		{
			string expression = this._communicationService.GetInput(Constants.ASK_EXPRESSION);

			try
			{
				ValidateExpression(expression);
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				this._communicationService.WriteResponse(e.Message);

				return GetExpressionFromUser();
			}

			return expression;
		}

		protected override void ValidateExpression(string expression)
		{
			base.ValidateExpression(expression);

			if (expression.IndexOfAny(new[] { '(', ')' }) != -1)
			{
				throw new InvalidExpressionException(Constants.INVALID_EXPRESSION);
			}
		}
	}
}