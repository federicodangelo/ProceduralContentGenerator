using System;
using System.Collections.Generic;
using System.Reflection;

namespace PCG
{
    public class FunctionsParser
    {
        static private Dictionary<String, Type> functionMap;

        static FunctionsParser()
        {
            functionMap = new Dictionary<string, Type>();
            functionMap.Add(ConstantMatrix.NAME.ToLowerInvariant(), typeof(ConstantMatrix));
            functionMap.Add(ConstantNumber.NAME.ToLowerInvariant(), typeof(ConstantNumber));
            functionMap.Add(MidPointMatrix.NAME.ToLowerInvariant(), typeof(MidPointMatrix));
            functionMap.Add(PerlinNoiseMatrix.NAME.ToLowerInvariant(), typeof(PerlinNoiseMatrix));
            functionMap.Add(RandomMatrix.NAME.ToLowerInvariant(), typeof(RandomMatrix));
            functionMap.Add(VoronoiNoiseMatrix.NAME.ToLowerInvariant(), typeof(VoronoiNoiseMatrix));
            functionMap.Add(AddMatrix.NAME.ToLowerInvariant(), typeof(AddMatrix));
            functionMap.Add(MultiplyMatrix.NAME.ToLowerInvariant(), typeof(MultiplyMatrix));
        }

        static public Function Parse(string s)
        {
            parsingTokens = Tokenize(s);
            parsingTokensOffset = 0;

            return ParseFunctionCall();
        }

        static private Token[] parsingTokens;
        static private int parsingTokensOffset;

        static private Token CurrentToken
        {
            get
            {
                if (parsingTokensOffset < parsingTokens.Length)
                    return parsingTokens[parsingTokensOffset];
                else
                    return new Token();
            }
        }

        static private void DiscardCurrentToken()
        {
            parsingTokensOffset++;
        }

        static private Function ParseFunctionCall()
        {
            if (CurrentToken.tokenType == TokenType.Symbol)
            {
                Type functionType;

                if (functionMap.TryGetValue(CurrentToken.symbol.ToLowerInvariant(), out functionType))
                {
                    Function function = (Function)functionType.GetConstructor(new Type[0]).Invoke(null);

                    Function[] inputValues = function.GetDefaultInputValues();

                    DiscardCurrentToken();

                    //Parse parameters (if they are available)
                    if (CurrentToken.tokenType == TokenType.StartFunctionCall)
                    {
                        DiscardCurrentToken();

                        if (CurrentToken.tokenType == TokenType.EndFunctionCall)
                        {
                            //End of function call, no parameters
                            DiscardCurrentToken();
                        }
                        else
                        {
                            //Parameters!
                            while (CurrentToken.tokenType == TokenType.Symbol)
                            {
                                String parameterName = CurrentToken.symbol;
                                DiscardCurrentToken();
                                if (CurrentToken.tokenType == TokenType.Equal)
                                {
                                    DiscardCurrentToken();

                                    if (CurrentToken.tokenType == TokenType.Number)
                                    {
                                        //Parameter value is a number

                                        int parameterNumber = CurrentToken.number;
                                        DiscardCurrentToken();

                                        int index = function.GetInputParameterIndex(parameterName);

                                        if (index >= 0)
                                        {
                                            //It's an input parameter!
                                            if (function.InputParameters[index].Type == ParameterType.Number)
                                            {
                                                inputValues[index] = new ConstantNumber(parameterNumber);
                                            }
                                            else
                                            {
                                                //Error!
                                                return null;
                                            }
                                        }
                                        else
                                        {
                                            //Try to lookup a public propery with the same name

                                            PropertyInfo property = function.GetType().GetProperty(parameterName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);

                                            if (property != null)
                                            {
                                                if (property.PropertyType == typeof(int))
                                                {
                                                    property.SetValue(function, (int)parameterNumber, null);
                                                }
                                                else if (property.PropertyType == typeof(float))
                                                {
                                                    property.SetValue(function, (int)parameterNumber, null);
                                                }
                                            }
                                        }
                                    }
                                    else if (CurrentToken.tokenType == TokenType.Symbol)
                                    {
                                        //Parameter value is anothe function call!
                                        Function parameterFunction = ParseFunctionCall();
                                        if (parameterFunction != null)
                                        {
                                            int index = function.GetInputParameterIndex(parameterName);

                                            if (index >= 0)
                                            {
                                                //It's an input parameter!
                                                if (function.InputParameters[index].Type == parameterFunction.OutputParameter.Type)
                                                {
                                                    inputValues[index] = parameterFunction;
                                                }
                                                else
                                                {
                                                    //Error!
                                                    return null;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //Error
                                        return null;
                                    }

                                    if (CurrentToken.tokenType == TokenType.Comma)
                                    {
                                        DiscardCurrentToken(); //More parameters to come!
                                    }
                                    else if (CurrentToken.tokenType == TokenType.EndFunctionCall)
                                    {
                                        DiscardCurrentToken();
                                        //End of function call
                                        break;
                                    }
                                }
                                else
                                {
                                    //Error
                                    return null;
                                }
                            }
                        }
                    }

                    function.SetInputValues(inputValues);

                    return function;
                }
            }

            return null;
        }

        static private Token[] Tokenize(string s)
        {
            List<Token> tokens = new List<Token>();

            int offset = 0;

            while(offset < s.Length)
            {
                char c = s[offset];
                offset++;

                bool skip = false;
                Token token = new Token();
                
                switch(c)
                {
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                        //Skip whitespaces
                        skip = true;
                        break;

                    case '=':
                        token.tokenType = TokenType.Equal;
                        break;

                    case '(':
                        token.tokenType = TokenType.StartFunctionCall;
                        break;

                    case ')':
                        token.tokenType = TokenType.EndFunctionCall;
                        break;

                    case '/':
                        if (offset < s.Length && s[offset] == '*')
                        {
                            //It's a /* */ comment, skip!
                            offset++;

                            while (offset < s.Length && s[offset] != '*' || offset + 1 < s.Length && s[offset + 1] != '/')
                                offset++;

                            if (offset < s.Length && s[offset] == '*' && offset + 1 < s.Length && s[offset + 1] == '/')
                                offset += 2; //skip */

                            skip = true;
                        }
                        break;

                    default:
                        //Symbol or number?
                        if (c >= '0' && c <= '9')
                        {
                            //Number!
                            int numberStart = offset - 1;

                            while (offset < s.Length && s[offset] >= '0' && s[offset] <= '9')
                                offset++;

                            int numberEnd = offset;

                            string numberStr = s.Substring(numberStart, numberEnd - numberStart);

                            token.tokenType = TokenType.Number;
                            if (!Int32.TryParse(numberStr, out token.number))
                                UnityEngine.Debug.LogError("Invalid number " + numberStr);
                        }
                        else if (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z')
                        {
                            //Symbol!
                            int symbolStart = offset - 1;

                            while (offset < s.Length && (s[offset] >= 'a' && s[offset] <= 'z' || s[offset] >= 'A' && s[offset] <= 'Z' || s[offset] >= '0' && s[offset] <= '9'))
                                offset++;

                            int symbolEnd = offset;

                            string symbolStr = s.Substring(symbolStart, symbolEnd - symbolStart);
                            token.tokenType = TokenType.Symbol;
                            token.symbol = symbolStr;
                        }
                        else
                        {
                            //Error?
                            skip = true;
                        }
                        break;
                }

                if (!skip)
                    tokens.Add(token);
            }

            return tokens.ToArray();
        }

        public enum TokenType
        {
            End, //Default last token
            Symbol, //Any string
            Number, //Any number
            StartFunctionCall, //(
            EndFunctionCall, //)
            Equal, //=
            Comma, //,
        }

        private struct Token
        {
            public TokenType tokenType;
            public string symbol;
            public int number;
        }
    }
}
