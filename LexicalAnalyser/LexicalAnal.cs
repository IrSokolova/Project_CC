
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
        

        public List<Tuple<TokenTypes, string>> SplitToTokens(string text)
        {
                List<Tuple<TokenTypes, Regex>> tokens = new List<Tuple<TokenTypes, Regex>>();
                List<Tuple<TokenTypes, string>> result = new List<Tuple<TokenTypes, string>>();
                // string remainingText = Text;

                tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.DeclarationSeparators, new Regex(@"^\\s*(;)([\\s\\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));

                tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Keywords, new Regex(
                        @"^\\s*(var|is|end|in|reverse|while|for|from|loop|if|then|else|real|boolean|integer|type|record|routine|array|return)(?![a-zA-Z0-9_])([\\s\\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));

                tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.BooleanLiterals, new Regex(
                        @"^\\s*(false|true)(?![a-zA-Z0-9_])([\\s\\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));

                tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Operators, new Regex(
                        @"^\\s*((?:not|and|x?or)(?![a-zA-Z0-9_])|(?:[:><\\/])=?|(?:[\\*\\+\\-=%]))([\\s\\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));

                tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Punctuators, new Regex(
                        @"^\\s*(\\.{1,2}|[\\()\\[\\]:,])([\\s\\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));

                tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Identifiers, new Regex(
                        @"^\\s*([a-zA-Z_][a-zA-Z0-9_]*)([\\s\\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));

                tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.FloatingLiterals, new Regex(
                        @"^\\s*([0-9]+\\.(?:[0-9])*|\\.[0-9]+)(?![a-zA-Z0-9_\\.])([\\s\\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));

                tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.IntegerLiterals, new Regex(
                        @"^\\s*([0-9]+)(?![a-zA-Z0-9_\\.])([\\s\\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));

                tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Undefined, new Regex(
                        @"^\\s*(\\S*)([\\s\\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));

                string[] splitedText = text.Split();

                foreach (string str in splitedText)
                {
                        foreach (Tuple<TokenTypes, Regex> token in tokens)
                        {
                                Match m = token.Item2.Match(str);
                                if (m.Success)
                                {
                                        result.Add(new Tuple<TokenTypes, string>(token.Item1, str));
                                        Console.WriteLine(result.Last());
                                }
                        }
                }

                return result;
        }
}


