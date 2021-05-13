using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SimpleCalculator.Common.Exceptions;
using SimpleCalculator.Common.IO;
using SimpleCalculator.Common.MessageTemplates;
using SimpleCalculator.Common.Models;

namespace SimpleCalculator.Common.Processors
{
	public abstract class CalculatorBase
	{
		private const string Pattern = @"^,|^\.|,$|\.$|[,.]{2,}|\([^\d\-\(]|[^\d\)]\)|[^,\.\d+\-\*\/\(\)]|[+\/\*]-{2,}|-[+\/\*]|^[+\/\*]+|[+\/\*]{2,}|-{3,}|^-{2,}|\(\)|^\)|\($|\\|^\d+$|\d\(|\)\d";
		protected readonly ICommunicationService _communicationService;

		protected CalculatorBase(ICommunicationService communicationService)
		{
			this._communicationService =
				communicationService ?? throw new ArgumentNullException(nameof(communicationService));
		}

		public abstract void Run();

		protected virtual void ValidateExpression(string expression)
		{
			if (string.IsNullOrEmpty(expression)
			    || Regex.IsMatch(expression, Pattern, RegexOptions.IgnoreCase))
			{
				throw new InvalidExpressionException(Constants.INVALID_EXPRESSION);
			}
		}

		protected double ParseExpression(ref List<char> expression)
		{
			var multiplicationIndex = expression.IndexOf((char) CalculatorOperators.Multiplication);
			var divisionIndex = expression.IndexOf((char) CalculatorOperators.Division);
			int position;
			CalculatorOperators op;

			if (multiplicationIndex != -1 && (multiplicationIndex < divisionIndex || divisionIndex == -1))
			{
				op = CalculatorOperators.Multiplication;
				position = multiplicationIndex;
			}
			else if (divisionIndex != -1 && (divisionIndex < multiplicationIndex || multiplicationIndex == -1))
			{
				op = CalculatorOperators.Division;
				position = divisionIndex;
			}
			else
			{
				var addition = expression.IndexOf((char)CalculatorOperators.Addition);
				var subtraction = expression.IndexOf((char)CalculatorOperators.Subtraction, 1);

				if (addition == -1 && subtraction == -1 && expression.Any())
				{
					return Convert.ToDouble(new string(expression.ToArray()), CultureInfo.InvariantCulture);
				}

				op = (addition < subtraction || subtraction == -1) && addition != -1
					? CalculatorOperators.Addition
					: CalculatorOperators.Subtraction;
				position = op == CalculatorOperators.Addition ? addition : subtraction;
			}

			double result = GetChunkResult(ref expression, position, op);

			return expression.Any() ? ParseExpression(ref expression) : result;
		}

		private double GetChunkResult(ref List<char> expression, int position, CalculatorOperators op)
		{
			var rightPart = expression.GetRange(position + 1, expression.Count - position - 1);
			expression = expression.GetRange(0, position);
			var right = ExtractNumber(ref rightPart, false);
			var left = ExtractNumber(ref expression);
			var tempResult = Calculate(right, left, op);

			if (expression.Any() || rightPart.Any())
			{
				rightPart.InsertRange(0, tempResult.ToString());
				expression.AddRange(rightPart);
			}

			return tempResult;
		}

		private double ExtractNumber(ref List<char> part, bool isLeft = true)
		{
			var result = new StringBuilder();

			while (part.Any())
			{
				var ch = isLeft ? part.Last() : part.First();
				int position = isLeft ? part.Count - 2 : 1;

				if (char.IsDigit(ch) || ch == '.' || ch == ',' || ch == (char)CalculatorOperators.Subtraction 
					&& (isLeft && !char.IsDigit(part.ElementAtOrDefault(position))
					|| !isLeft && result.Length == 0))
				{
					if (isLeft)
					{
						result.Insert(0, ch);
						position += 1;
					}
					else
					{
						result.Append(ch);
						position -= 1;
					}

					part.RemoveAt(position);
					continue;
				}

				break;
			}

			return Convert.ToDouble(result.ToString(), CultureInfo.InvariantCulture);
		}

		private double Calculate(double right, double left, CalculatorOperators op)
		{
			switch (op)
			{
				case CalculatorOperators.Multiplication:
					return left * right;
				case CalculatorOperators.Division:
					if (right == 0)
					{
						throw new DivideByZeroException(Constants.ZERO_DIVISION);
					}

					return left / right;
				case CalculatorOperators.Subtraction:
					return left - right;
				case CalculatorOperators.Addition:
					return left + right;
				default:
					throw new ArgumentOutOfRangeException(nameof(op), op, Constants.INVALID_OPERATOR);
			}
		}
	}
}