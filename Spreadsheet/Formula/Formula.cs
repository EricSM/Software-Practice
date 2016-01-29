// Skeleton written by Joe Zachary for CS 3500, January 2015
// Revised by Joe Zachary, January 2016
// JLZ Repaired pair of mistakes, January 23, 2016

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Formulas
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  Provides a means to evaluate Formulas.  Formulas can be composed of
    /// non-negative floating-point numbers, variables, left and right parentheses, and
    /// the four binary operator symbols +, -, *, and /.  (The unary operators + and -
    /// are not allowed.)
    /// </summary>
    public class Formula
    {
        private List<string> tokenList;

        /// <summary>
        /// Creates a Formula from a string that consists of a standard infix expression composed
        /// from non-negative floating-point numbers (using C#-like syntax for double/int literals), 
        /// variable symbols (a letter followed by zero or more letters and/or digits), left and right
        /// parentheses, and the four binary operator symbols +, -, *, and /.  White space is
        /// permitted between tokens, but is not required.
        /// 
        /// Examples of a valid parameter to this constructor are:
        ///     "2.5e9 + x5 / 17"
        ///     "(5 * 2) + 8"
        ///     "x*y-2+35/9"
        ///     
        /// Examples of invalid parameters are:
        ///     "_"
        ///     "-5.3"
        ///     "2 5 + 3"
        /// 
        /// If the formula is syntacticaly invalid, throws a FormulaFormatException with an 
        /// explanatory Message.
        /// </summary>
        public Formula(String formula)
        {
            tokenList = GetTokens(formula).ToList();
            var numberOfOpeningParenthesis = 0;
            var numberOfClosingParenthesis = 0;
            var lastToken = String.Empty;

            var lpPattern = @"\(";
            var rpPattern = @"\)";
            var opPattern = @"[\+\-*/]";
            var varPattern = @"[a-zA-Z][0-9a-zA-Z]*";
            var doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";
            var ignoreSpaceOption = RegexOptions.IgnorePatternWhitespace;

            if (tokenList.Count == 0)
            {
                throw new FormulaFormatException("Formula must have at least one number or variable");
            }

            if (!Regex.IsMatch(tokenList.First(), String.Format("({0}) | ({1}) | ({2})", lpPattern, varPattern, doublePattern), 
                ignoreSpaceOption))
            {
                throw new FormulaFormatException("The first token of a formula must be a number, a variable, or an opening parenthesis");
            }

            if (!Regex.IsMatch(tokenList.Last(), String.Format("({0}) | ({1}) | ({2})", rpPattern, varPattern, doublePattern), 
                ignoreSpaceOption))
            {
                throw new FormulaFormatException("The last token of a formula must be a number, a variable, or a closing parenthesis");
            }

            foreach (string token in tokenList)
            {
                if (token.Equals(lpPattern))
                {
                    numberOfOpeningParenthesis++;
                }
                else if (token.Equals(rpPattern))
                {
                    numberOfClosingParenthesis++;
                }
                else if (!Regex.IsMatch(token, String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4})",
                    lpPattern, rpPattern, opPattern, varPattern, doublePattern), ignoreSpaceOption))
                {
                    throw new FormulaFormatException("There cannot be invalid tokens");
                }
                
                if (numberOfClosingParenthesis > numberOfOpeningParenthesis)
                {
                    throw new FormulaFormatException("All closing parentheses must have corresponding opening parenthesis");
                }

                if (!string.IsNullOrEmpty(lastToken))
                {
                    if (Regex.IsMatch(lastToken, String.Format("({0}) | ({1})", lpPattern, opPattern), ignoreSpaceOption) &&
                        !Regex.IsMatch(token, String.Format("({0}) | ({1}) | ({2})", doublePattern, varPattern, lpPattern),
                        ignoreSpaceOption))
                    {
                        throw new FormulaFormatException("An opening parenthesis or an operator must followed by either a " +
                            "number, a variable, or an opening parenthesis");
                    }
                    else if (Regex.IsMatch(lastToken, String.Format("({0}) | ({1}) | ({2})", doublePattern, varPattern, rpPattern),
                            ignoreSpaceOption) &&
                            !Regex.IsMatch(token, String.Format("({0}) | ({1})", opPattern, rpPattern), ignoreSpaceOption))
                    {
                        throw new FormulaFormatException("A number, a variable, or a closing parenthesis must followed by " +
                            "either an operator or a closing parenthesis");
                    }
                }

                lastToken = token;
            }

            if (numberOfOpeningParenthesis != numberOfClosingParenthesis)
            {
                throw new FormulaFormatException("Must have equal numbers of opening and closing parentheses");
            }
        }

        /// <summary>
        /// Evaluates this Formula, using the Lookup delegate to determine the values of variables.  (The
        /// delegate takes a variable name as a parameter and returns its value (if it has one) or throws
        /// an UndefinedVariableException (otherwise).  Uses the standard precedence rules when doing the evaluation.
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, its value is returned.  Otherwise, throws a FormulaEvaluationException  
        /// with an explanatory Message.
        /// </summary>
        public double Evaluate(Lookup lookup)
        {
            var valueStack = new Stack<double>();
            var operatorStack = new Stack<string>();
            string op;
            double number;

            foreach (string token in tokenList)
            {
                if (Double.TryParse(token, out number))
                {
                    if (operatorStack.Count == 0 || !Regex.IsMatch(operatorStack.Peek(), @"[*/]"))
                    {
                        valueStack.Push(number);
                    }
                    else
                    {
                        MultOrDivCurrentAndLastValues(ref valueStack, ref operatorStack, number);
                    }
                }
                else if (Regex.IsMatch(token, @"[a-zA-Z][0-9a-zA-Z]*"))
                {
                    try
                    {
                        number = lookup(token);

                        if (operatorStack.Count == 0 || !Regex.IsMatch(operatorStack.Peek(), @"[*/]"))
                        {
                            valueStack.Push(number);
                        }
                        else
                        {
                            MultOrDivCurrentAndLastValues(ref valueStack, ref operatorStack, number);
                        }
                    }
                    catch (UndefinedVariableException e)
                    {
                        throw new FormulaEvaluationException(e.Message);
                    }                    
                }
                else if (Regex.IsMatch(token, @"[\+\-]"))
                {
                    AddOrSubLastTwoValues(ref valueStack, ref operatorStack);
                    operatorStack.Push(token);
                }
                else if (Regex.IsMatch(token, @"[\(*/]"))
                {
                    operatorStack.Push(token);
                }
                else if (token.Equals(")"))
                {
                    AddOrSubLastTwoValues(ref valueStack, ref operatorStack);

                    operatorStack.Pop();

                    if (operatorStack.Count > 0 && Regex.IsMatch(operatorStack.Peek(), @"[*/]"))
                    {
                        if (operatorStack.Peek().Equals("*"))
                        {
                            operatorStack.Pop();
                            valueStack.Push(valueStack.Pop() * valueStack.Pop());
                        }
                        else if (operatorStack.Peek().Equals("/"))
                        {
                            valueStack.Push(1 / valueStack.Pop() * valueStack.Pop());
                        }
                    }
                }
            }

            if (operatorStack.Count == 0)
            {
                return valueStack.Pop();
            }
            else 
            {
                AddOrSubLastTwoValues(ref valueStack, ref operatorStack);
            }

            return valueStack.Pop();
        }

        private static void MultOrDivCurrentAndLastValues(ref Stack<double> valueStack, ref Stack<string> operatorStack,
            double currentValue)
        {
            if (operatorStack.Count > 0 && Regex.IsMatch(operatorStack.Peek(), @"[*/]"))
            {
                string op = operatorStack.Pop();
                if (op.Equals("*"))
                {
                    valueStack.Push(valueStack.Pop() * currentValue);
                }
                else if (op.Equals("/"))
                {
                    if (currentValue == 0)
                    {
                        throw new FormulaEvaluationException("Cannot divide by 0");
                    }
                    valueStack.Push(valueStack.Pop() / currentValue);
                }
            }
        }

        private static void AddOrSubLastTwoValues(ref Stack<Double> valueStack, ref Stack<string> operatorStack)
        {
            if (operatorStack.Count > 0 && Regex.IsMatch(operatorStack.Peek(), @"[\+\-]"))
            {
                string op = operatorStack.Pop();
                if (op.Equals("+"))
                {
                    valueStack.Push(valueStack.Pop() + valueStack.Pop());
                }
                else if (op.Equals("-"))
                {
                    valueStack.Push(valueStack.Pop() * -1 + valueStack.Pop());
                }
            }
        }

        /// <summary>
        /// Given a formula, enumerates the tokens that compose it.  Tokens are left paren,
        /// right paren, one of the four operator symbols, a string consisting of a letter followed by
        /// zero or more digits and/or letters, a double literal, and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z][0-9a-zA-Z]*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }
        }
    }
    

    /// <summary>
    /// A Lookup method is one that maps some strings to double values.  Given a string,
    /// such a function can either return a double (meaning that the string maps to the
    /// double) or throw an UndefinedVariableException (meaning that the string is unmapped 
    /// to a value. Exactly how a Lookup method decides which strings map to doubles and which
    /// don't is up to the implementation of the method.
    /// </summary>
    public delegate double Lookup(string s);

    /// <summary>
    /// Used to report that a Lookup delegate is unable to determine the value
    /// of a variable.
    /// </summary>
    public class UndefinedVariableException : Exception
    {
        /// <summary>
        /// Constructs an UndefinedVariableException containing whose message is the
        /// undefined variable.
        /// </summary>
        /// <param name="variable"></param>
        public UndefinedVariableException(String variable)
            : base(variable)
        {
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the parameter to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message) : base(message)
        {
        }
    }

    /// <summary>
    /// Used to report errors that occur when evaluating a Formula.
    /// </summary>
    public class FormulaEvaluationException : Exception
    {
        /// <summary>
        /// Constructs a FormulaEvaluationException containing the explanatory message.
        /// </summary>
        public FormulaEvaluationException(String message) : base(message)
        {
        }
    }
}
