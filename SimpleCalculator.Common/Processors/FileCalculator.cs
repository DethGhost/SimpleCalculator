using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCalculator.Common.Exceptions;
using SimpleCalculator.Common.IO;
using SimpleCalculator.Common.MessageTemplates;

namespace SimpleCalculator.Common.Processors
{
	public class FileCalculator: CalculatorBase
	{
		private string _path;

		public FileCalculator(ICommunicationService communicationService, string path) : base(communicationService)
		{
			_path = path;
		}

		public override void Run()
		{
			try
			{
				string expressions = this._communicationService.GetInput(_path);
				this._communicationService.WriteResponse(ProcessFile(expressions));
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(e);
			}

			Run();
		}

		private string ProcessFile(string expressions)
		{
			var expressionsList = expressions.Split(Constants.LINE_SEPARATOR).ToList();

			for (var index = 0; index < expressionsList.Count; index++)
			{
				var expression = expressionsList[index].ToList();

				try
				{
					ValidateExpression(expressionsList[index]);
					expression = ClearExpressionFromBrackets(expression);
					expressionsList[index] += $"={ParseExpression(ref expression)}";
				}
				catch (Exception e)
				{
					expressionsList[index] += $"={e.Message}";
				}
			}

			return string.Join('\n', expressionsList);
		}

		private List<char> ClearExpressionFromBrackets(List<char> expression)
		{
			while (expression.Any(x => x == '(' || x == ')'))
			{
				var openBraceIndex = expression.LastIndexOf('(');
				var closeBraceIndex = expression.IndexOf(')');
				var expr = expression.GetRange(openBraceIndex + 1, closeBraceIndex - openBraceIndex - 1);

				var result = ParseExpression(ref expr);
				expression.RemoveRange(openBraceIndex, closeBraceIndex - openBraceIndex + 1);
				expression.InsertRange(openBraceIndex, result.ToString());

			}

			return expression;
		}

		protected override void ValidateExpression(string expression)
		{
			base.ValidateExpression(expression);

			if(expression.Count(x => x == '(') != expression.Count(x => x == ')'))
			{
				throw new InvalidExpressionException(Constants.INVALID_EXPRESSION);
			}
		}
	}
}