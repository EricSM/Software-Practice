// Skeleton written by Joe Zachary for CS 3500, January 2015
// Revised by Joe Zachary, January 2016
// JLZ Repaired pair of mistakes, January 23, 2016

// Name: Eric Miramontes
// uNID: u0801584

using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <summary>
        /// List of tokens derived from the GetTokens method with invalid tokens removed.
        /// Tokens are left paren, right paren, one of the four operator symbols, a string
        /// consisting of a letter followed by zero or more digits and/or letters, and 
        /// double literals
        /// </summary>
        private List<string> _tokenList;

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
            _tokenList = GetTokens(formula).ToList(); // Use GetTokens method to get list of tokens in formula.
            var numberOfOpeningParenthesis = 0;
            var numberOfClosingParenthesis = 0;
            var lastToken = String.Empty; // Previous token processed in loop.

            // Patterns for individual tokens
            var lpPattern = @"\("; 
            var rpPattern = @"\)"; 
            var opPattern = @"[\+\-*/]";
            var varPattern = @"[a-zA-Z][0-9a-zA-Z]*";
            var doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";


            // Store IgnorePatternWhitespace regex option in a variable for later use.  
            // Since there can be no whitespace, this will be used in all future regex statements.
            var ignoreSpaceOption = RegexOptions.IgnorePatternWhitespace;

            // Check if the token list is empty
            if (_tokenList.Count == 0) 
            {
                throw new FormulaFormatException("Formula must have at least one number or variable");
            }

            // Check to make sure first token is a number, a variable, or an opening parenthesis.
            if (!Regex.IsMatch(_tokenList.First(), String.Format("({0}) | ({1}) | ({2})", lpPattern, varPattern, doublePattern), 
                ignoreSpaceOption)) 
            {
                throw new FormulaFormatException("The first token of a formula must be a number, a variable, or an opening parenthesis");
            }

            // Check to make sure last token is a number, a variable, or a closing parenthesis.
            if (!Regex.IsMatch(_tokenList.Last(), String.Format("({0}) | ({1}) | ({2})", rpPattern, varPattern, doublePattern), 
                ignoreSpaceOption)) 
            {
                throw new FormulaFormatException("The last token of a formula must be a number, a variable, or a closing parenthesis");
            }

            // Iterate through tokens to make sure formula is valid.
            foreach (string token in _tokenList) 
            {
                // Check for opening parenthesis.
                if (Regex.IsMatch(token, lpPattern))
                {
                    numberOfOpeningParenthesis++;
                }
                // Check for closing parenthesis.
                else if (Regex.IsMatch(token, rpPattern))
                { 
                    numberOfClosingParenthesis++;
                }
                // Check for invalid tokens with regex.
                else if (!Regex.IsMatch(token, String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4})",
                    lpPattern, rpPattern, opPattern, varPattern, doublePattern), ignoreSpaceOption))
                {
                    throw new FormulaFormatException("There cannot be invalid tokens");
                }

                // Make sure each closing parenthesis has its own opening parenthesis somewhere before it.
                if (numberOfClosingParenthesis > numberOfOpeningParenthesis) 
                {
                    throw new FormulaFormatException("All closing parentheses must have corresponding opening parenthesis");
                }
                // TODO fix
                // If this token is not the first one:
                if (!string.IsNullOrEmpty(lastToken))
                {
                    // Make sure operators and opening parentheses are followed by a number or opening parenthesis.
                    if (Regex.IsMatch(lastToken, string.Format("({0}) | ^({1})$", lpPattern, opPattern), ignoreSpaceOption) &&
                        !Regex.IsMatch(token, string.Format("({0}) | ({1}) | ({2})", doublePattern, varPattern, lpPattern),
                        ignoreSpaceOption))
                    {
                        throw new FormulaFormatException("An opening parenthesis or an operator must followed by either a " +
                            "number, a variable, or an opening parenthesis");
                    }
                    // Make sure numbers and closing parentheses are followed by operators or closing parentheses.
                    else if (Regex.IsMatch(lastToken, string.Format("({0}) | ({1}) | ({2})", doublePattern, varPattern, rpPattern),
                            ignoreSpaceOption) &&
                            !Regex.IsMatch(token, string.Format("^({0})$ | ({1})", opPattern, rpPattern), ignoreSpaceOption))
                    {
                        throw new FormulaFormatException("A number, a variable, or a closing parenthesis must followed by " +
                            "either an operator or a closing parenthesis");
                    }
                }

                // Finished vetting token, now loop moves on to next and this one becomes the "lastToken".
                lastToken = token;
            }

            // Make sure opening and closing parentheses are balanced.
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
            var valueStack = new Stack<double>(); // Stack of variables and doubles
            var operatorStack = new Stack<string>(); // Stack of operators.
            double number; // Current value of token if it is a double or variable.


            // Iterate through tokens.
            foreach (string token in _tokenList)
            {
                // Check to see what type of token current one is.

                // Check if current token is a double.
                if (Double.TryParse(token, out number))
                {
                    // See if most recent operator is not a multiplier or if operator stack is empty.
                    if (operatorStack.Count == 0 || !Regex.IsMatch(operatorStack.Peek(), @"[*/]"))
                    {
                        // If so, simply push current number to stack.
                        valueStack.Push(number);
                    }
                    else
                    {
                        // If most recent operator is multiplier (* or /), apply operator to both 
                        // current and previous value.
                        MultOrDivCurrentAndLastValues(ref valueStack, ref operatorStack, number);
                    }
                }
                // Check if current token is a variable.
                else if (Regex.IsMatch(token, @"[a-zA-Z][0-9a-zA-Z]*"))
                {
                    try
                    {
                        // Try to pull numerical value of variable with passed in lookup delegate.
                        number = lookup(token);

                        // Like when token was a double, see if most recent operator is not a multiplier 
                        // or if operator stack is empty.  If so, simply push current number to stack.
                        // If most recent operator is multiplier (* or /), apply operator to both 
                        // current and previous value.
                        if (operatorStack.Count == 0 || !Regex.IsMatch(operatorStack.Peek(), @"[*/]"))
                        {
                            valueStack.Push(number);
                        }
                        else
                        {
                            MultOrDivCurrentAndLastValues(ref valueStack, ref operatorStack, number);
                        }
                    }
                    // Throw FormulaEvaluationException if lookup delegate was unsuccessful.
                    catch (UndefinedVariableException e)
                    {
                        throw new FormulaEvaluationException(e.Message);
                    }                    
                }
                // Check if current token is an additive operator (+ or -).
                else if (Regex.IsMatch(token, @"[\+\-]"))
                {
                    // Try to add or subtract last two values on stack based on previous operator.
                    AddOrSubLastTwoValues(ref valueStack, ref operatorStack);

                    // Push current operator onto the stack.
                    operatorStack.Push(token);
                }
                // Check if current token is a multiplicative operator (* or /) or an opening parenthesis.
                else if (Regex.IsMatch(token, @"[\(*/]"))
                {
                    operatorStack.Push(token);
                }
                else if (token.Equals(")"))
                {
                    // Try to add or subtract last two values on stack based on previous operator.
                    AddOrSubLastTwoValues(ref valueStack, ref operatorStack);

                    // Pop opening parenthesis from operator stack.
                    operatorStack.Pop();

                    // Check if a multiplicative operator (* or /) is on top of stack. If it is,
                    // apply operator to last two values on stack and push result to value stack.
                    if (operatorStack.Count > 0 && Regex.IsMatch(operatorStack.Peek(), @"[*/]"))
                    {
                        if (operatorStack.Peek().Equals("*"))
                        {
                            operatorStack.Pop();
                            valueStack.Push(valueStack.Pop() * valueStack.Pop());
                        }
                        else if (operatorStack.Peek().Equals("/"))
                        {
                            // Make sure divisor is not 0.
                            if (valueStack.Peek() == 0)
                            {
                                throw new FormulaEvaluationException("");
                            }
                            valueStack.Push(1 / valueStack.Pop() * valueStack.Pop());
                        }
                    }
                }
            }

            // If there are no more operators to process, return last remaining value on stack.
            if (operatorStack.Count == 0)
            {
                return valueStack.Pop();
            }
            // If there is an operator left, it is additive (+ or -).  Apply it to last two 
            // remaining values on stack and return result.
            else 
            {
                AddOrSubLastTwoValues(ref valueStack, ref operatorStack);
            }
            return valueStack.Pop();
        }

        /// <summary>
        /// Helper method for when method Evaluate needs to multiply or divide top value on stack by current token.
        /// Two stacks with at least one value and operator, respectively, and the current numerical value
        /// of the double or variable token being processed are passed in as parameters. If * or / is at the top 
        /// of the operator stack, pops the value stack, pops the operator stack, and applies the popped operator 
        /// to current token and the popped number. Pushes the result onto the value stack. 
        /// </summary>
        /// <param name="valueStack">Pass in values stack by reference</param>
        /// <param name="operatorStack">Pass in operators stack by reference</param>
        /// <param name="currentValue">Pass in current token's numerical value</param>
        private void MultOrDivCurrentAndLastValues(ref Stack<double> valueStack, ref Stack<string> operatorStack,
            double currentValue)
        {
            // Checks if top of operator stack is a * or /.
            if (operatorStack.Count > 0 && Regex.IsMatch(operatorStack.Peek(), @"[*/]"))
            {
                // Pop operator stack.
                string op = operatorStack.Pop();

                // Check if multiplication sign.
                if (op.Equals("*"))
                {
                    // Multiply top two values on value stack and push result to stack.
                    valueStack.Push(valueStack.Pop() * currentValue);
                }
                // Check if division sign.
                else if (op.Equals("/"))
                {
                    // Make sure divisor is not zero.
                    if (currentValue == 0)
                    {
                        throw new FormulaEvaluationException("Cannot divide by 0");
                    }
                    // Divide penultimate value by last value on value stack and push result to stack.
                    valueStack.Push(valueStack.Pop() / currentValue);
                }
            }
        }

        /// <summary>
        /// Helper method for when method Evaluate needs to add or subtract top two values on stack.
        /// Two stacks with at least two values and one operator, respectively, are passed in as parameters.  
        /// If + or - is at the top of the operator stack, pops the value stack twice and the operator stack 
        /// once. Applies the popped operator to the popped numbers. Pushes the result onto the value stack.
        /// </summary>
        /// <param name="valueStack">Pass in values stack by reference</param>
        /// <param name="operatorStack">Pass in operators stack by reference</param>
        private void AddOrSubLastTwoValues(ref Stack<Double> valueStack, ref Stack<string> operatorStack)
        {
            // Checks if top of operator stack is a + or -.
            if (operatorStack.Count > 0 && Regex.IsMatch(operatorStack.Peek(), @"[\+\-]"))
            {
                // Pop operator stack.
                string op = operatorStack.Pop();

                // Check if plus sign.
                if (op.Equals("+"))
                {
                    // Add top two values on value stack and push result to stack.
                    valueStack.Push(valueStack.Pop() + valueStack.Pop());
                }
                // Check if minus sign.
                else if (op.Equals("-"))
                {
                    // Subtract last value from penultimate value on value stack and push result to stack.
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
