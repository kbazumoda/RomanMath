using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RomanMath.Impl
{
	public static class Service
	{
        // Регулярные выражения для ведения расчета
        private const string RegexBr = "\\(([1234567890\\.\\+\\-\\*\\/^%]*)\\)";    // Скобки
        private const string RegexNum = "[-]?\\d+\\.?\\d*";                         // Числа
        private const string RegexMulOp = "[\\*\\/^%]";                             // Первоприоритетные числа
        private const string RegexAddOp = "[\\+\\-]";                               // Второприоритетные числа

        public static int Parse(string str)
        {
            // Парсинг скобок для определения очередности расчета 
            var matchSk = Regex.Match(str, RegexBr);
            if (matchSk.Groups.Count > 1)
            {
                string inner = matchSk.Groups[0].Value.Substring(1, matchSk.Groups[0].Value.Trim().Length - 2);
                string left = str.Substring(0, matchSk.Index);
                string right = str.Substring(matchSk.Index + matchSk.Length);

                return Parse(left + Parse(inner).ToString(CultureInfo.InvariantCulture) + right);
            }

            // Парсинг действий разбор очередности расчета с полученного текста
            var matchMulOp = Regex.Match(str, string.Format("({0})\\s?({1})\\s?({2})\\s?", RegexNum, RegexMulOp, RegexNum));
            var matchAddOp = Regex.Match(str, string.Format("({0})\\s?({1})\\s?({2})\\s?", RegexNum, RegexAddOp, RegexNum));
            var match = matchMulOp.Groups.Count > 1 ? matchMulOp : matchAddOp.Groups.Count > 1 ? matchAddOp : null;
            if (match != null)
            {
                string left = str.Substring(0, match.Index);
                string right = str.Substring(match.Index + match.Length);
                return Parse(left + ParseAct(match).ToString(CultureInfo.InvariantCulture) + right);
            }

            // Парсинг числа
            try
            {
                return int.Parse(str, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                throw new FormatException(string.Format("Неверная входная строка '{0}'", str));
            }
        }
        private static double ParseAct(Match match)
        {
            double a = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            double b = double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);

            switch (match.Groups[2].Value)
            {
                case "+": return a + b;
                case "-": return a - b;
                case "*": return a * b;
                case "/": return a / b;
                
                default: throw new FormatException($"Неверная входная строка '{match.Value}'");
            }
        }
        private static Dictionary<string, int> RomanMap = new Dictionary<string, int>()
        {
            {"IV", 4},
            {"III", 3},
            {"II", 2},
            {"I", 1},
            {"VI", 6},
            {"VII", 7},
            {"VIII", 9},
            {"V", 5},
            {"IX", 9},
            {"X", 10},
            {"L", 50},
            {"C", 100},
            {"D", 500},
            {"M", 1000}
        };

        
		public static int Evaluate(string expression)
        {
            if (expression.Length != 0)
            {
                int number = 0;
                foreach (var d in RomanMap)
                {
                    expression = expression.Replace($"{d.Key}", d.Value.ToString());
                } 
                number = Convert.ToInt32(Parse(expression));
                return number;
            }
            else
            {
                throw new NotImplementedException();
            }
            
		}
        
	}
}
