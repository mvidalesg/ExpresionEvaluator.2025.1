using System.Globalization;
using System.Text;

namespace Evaluator.Logic;

public class FunctionEvaluator
{
    public static double Evalute(string infix)
    {
        var postfix = ToPostfix(infix);
        return Calculate(postfix);
    }

    private static double Calculate(List<string> postfix)
    {
        var stack = new Stack<double>();

        foreach (var token in postfix)
        {
            if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out double number)) // se agrega
            {
                stack.Push(number);
            }
            else if (IsOperator(token))
            {
                var operand2 = stack.Pop();
                var operand1 = stack.Pop();
                stack.Push(Result(operand1, token, operand2));
            }
        }
        return stack.Pop();
    }

    private static double Result(double operand1, string op, double operand2)
    {
        return op switch
        {
            "+" => operand1 + operand2,
            "-" => operand1 - operand2,
            "*" => operand1 * operand2,
            "/" => operand1 / operand2,
            "^" => Math.Pow(operand1, operand2),
            _ => throw new Exception("Expresión inválida"),
        };
    }

    private static List<string> ToPostfix(string infix) // se modifica
    {
        var stack = new Stack<string>();
        var postfix = new List<string>();
        var numberBuilder = new StringBuilder();

        foreach (var ch in infix)
        {
            if (char.IsDigit(ch) || ch == '.')
            {
                numberBuilder.Append(ch);
            }
            else
            {
                if (numberBuilder.Length > 0)
                {
                    postfix.Add(numberBuilder.ToString());
                    numberBuilder.Clear();
                }

                if (IsOperator(ch.ToString()))
                {
                    while (stack.Count > 0 && PriorityExpression(ch.ToString()) <= PriorityStack(stack.Peek()))
                    {
                        postfix.Add(stack.Pop());
                    }
                    stack.Push(ch.ToString());
                }
                else if (ch == '(')
                {
                    stack.Push(ch.ToString());
                }
                else if (ch == ')')
                {
                    while (stack.Peek() != "(")
                    {
                        postfix.Add(stack.Pop());
                    }
                    stack.Pop();
                }
            }
        }

        if (numberBuilder.Length > 0)
        {
            postfix.Add(numberBuilder.ToString());
        }

        while (stack.Count > 0)
        {
            postfix.Add(stack.Pop());
        }

        return postfix;
    }

    private static int PriorityStack(string op) => op switch
    {
        "^" => 3,
        "*" => 2,
        "/" => 2,
        "+" => 1,
        "-" => 1,
        "(" => 0,
        _ => throw new Exception("Expresión inválida."),
    };

    private static int PriorityExpression(string op) => op switch
    {
        "^" => 4,
        "*" => 2,
        "/" => 2,
        "+" => 1,
        "-" => 1,
        "(" => 5,
        _ => throw new Exception("Expresión inválida."),
    };

    private static bool IsOperator(string op) => "+-*/^".Contains(op);
}


