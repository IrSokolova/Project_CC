using System.Text.RegularExpressions;

namespace ConsoleApp1.LexicalAnalyser;

public enum TokenTypes
{
        Undefined = 0,
        DeclarationSeparators,
        Keywords,
        BooleanLiterals,
        Operators,
        Punctuators,
        Identifiers,
        FloatingLiterals,
        IntegerLiterals
};

public class LexicalAnal
{
        public List<Tuple<TokenTypes, Regex>> _Tokens = new List<Tuple<TokenTypes, Regex>>();

        public List<Tuple<TokenTypes, string>> SplitToTokens(string text)
        {
                List<Tuple<TokenTypes, string>> result = new List<Tuple<TokenTypes, string>>();
                // string remainingText = Text;

                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.DeclarationSeparators, new Regex(@"^\s*(;)([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));

                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Keywords, new Regex(
                        @"^\s*(var|is|end|in|reverse|while|for|from|loop|if|then|else|real|boolean|integer|type|record|routine|array|return)(?![a-zA-Z0-9_])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));

                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.BooleanLiterals, new Regex(
                        @"^\s*(false|true)(?![a-zA-Z0-9_])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));

                // TODO Add + -
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Operators, new Regex(
                        @"^\s*((?:not|and|x?or)(?![a-zA-Z0-9_])|(?:[:><\/])=?|(?:[\*\+\[-]=%]))([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));

                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Punctuators, new Regex(
                        @"^\s*(\\.{1,2}|[\()\[\]:,])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));

                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Identifiers, new Regex(
                        @"^\s*([a-zA-Z_][a-zA-Z0-9_]*)([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));

                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.FloatingLiterals, new Regex(
                        @"^\s*([0-9]+\.(?:[0-9])*|\.[0-9]+)(?![a-zA-Z0-9_\.])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));

                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.IntegerLiterals, new Regex(
                        @"^\s*([0-9]+)(?![a-zA-Z0-9_\.])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));

                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Undefined, new Regex(
                        @"^\s*(\S*)([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));

                string[] splitedText = text.Split();

                foreach (string str in splitedText)
                {
                        List<string> splitedStr = new List<string>();
                        if (str.Contains("()"))
                        {
                                splitedStr.Add(str.Substring(0,str.Length - 2));
                                splitedStr.Add(str[str.Length - 2].ToString());
                                splitedStr.Add(str.Last().ToString());
                        }
                        else if (str.Contains('('))
                        {
                                splitedStr.Add(str[0].ToString());
                                splitedStr.Add(str.Substring(1));
                                
                        }
                        else if (str.Contains(')'))
                        {
                                splitedStr.Add(str.Substring(0,str.Length - 1));
                                splitedStr.Add(str.Last().ToString());
                        }

                        if (splitedStr.Count > 0)
                        {
                                foreach (string newStr in splitedStr)
                                {
                                        result.Add(MatchPattern(newStr));
                                }
                        }
                        else
                        {
                                result.Add(MatchPattern(str));
                        }
                }

                return result;
        }

        public Tuple<TokenTypes, string> MatchPattern(string str)
        {
                foreach (Tuple<TokenTypes, Regex> token in _Tokens)
                {
                        Match m = token.Item2.Match(str);
                        if (m.Success)
                        {
                                Tuple<TokenTypes, string> resToken = new Tuple<TokenTypes, string>(token.Item1, str);
                                // Console.WriteLine(resToken);
                                return resToken;
                        }
                }

                return new Tuple<TokenTypes, string>(TokenTypes.Undefined, str);
        }
}


