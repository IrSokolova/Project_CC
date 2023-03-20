using System.Text.RegularExpressions;

namespace ConsoleApp1.LexicalAnalyser;

public enum TokenTypes
{
        Undefined = 0,
        Identifiers,
        FloatingLiterals,
        IntegerLiterals,
        While,
        Loop,
        End,
        For,
        In,
        Reverse,
        Var,
        Is,
        To,
        Return,
        If,
        Then,
        Else,
        Array,
        Real,
        Boolean,
        Integer,
        Type,
        Record,
        Routine,
        Function,
        True,
        False,
        Plus,
        Minus,
        Mult,
        Div,
        Remainder,
        Not,
        And,
        Or,
        Xor,
        Eq,
        Greater,
        Less,
        GreaterEq,
        LessEq,
        NotEq,
        Assign,
        Colon,
        ParenthesesL,
        ParenthesesR,
        BracketsL,
        BracketsR,
};

public class LexicalAnal
{
        public List<Tuple<TokenTypes, Regex>> _Tokens = new List<Tuple<TokenTypes, Regex>>();

        public void AddTokens()
        {
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Else, new Regex(
                        @"^\s*(else)(?![a-zA-Z0-9_])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Real, new Regex(
                        @"^\s*(real)(?![a-zA-Z0-9_])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Integer, new Regex(
                        @"^\s*(integer)(?![a-zA-Z0-9_])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Boolean, new Regex(
                        @"^\s*(boolean)(?![a-zA-Z0-9_])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Type, new Regex(
                        @"^\s*(type)(?![a-zA-Z0-9_])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Record, new Regex(
                        @"^\s*(record)(?![a-zA-Z0-9_])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Routine, new Regex(
                        @"^\s*(routine)(?![a-zA-Z0-9_])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Array, new Regex(
                        @"^\s*(array)(?![a-zA-Z0-9_])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Function, new Regex(
                        @"^\s*(function)(?![a-zA-Z0-9_])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Var, new Regex(
                        @"^\s*(var)(?![a-zA-Z0-9_])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Is, new Regex(
                        @"^\s*(is)(?![a-zA-Z0-9_])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.To, new Regex(
                        @"^\s*(to)(?![a-zA-Z0-9_])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Return, new Regex(
                        @"^\s*(return)(?![a-zA-Z0-9_])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.If, new Regex(
                        @"^\s*(if)(?![a-zA-Z0-9_])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Then, new Regex(
                        @"^\s*(then)(?![a-zA-Z0-9_])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.While, new Regex(
                        @"^\s*(while)(?![a-zA-Z0-9_])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Loop, new Regex(
                        @"^\s*(loop)(?![a-zA-Z0-9_])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.End, new Regex(
                        @"^\s*(end)(?![a-zA-Z0-9_])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.For, new Regex(
                        @"^\s*(for)(?![a-zA-Z0-9_])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Reverse, new Regex(
                        @"^\s*(reverse)(?![a-zA-Z0-9_])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.In, new Regex(
                        @"^\s*(in)(?![a-zA-Z0-9_])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));

                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.True, new Regex(
                        @"^\s*(true)(?![a-zA-Z0-9_])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.False, new Regex(
                        @"^\s*(false)(?![a-zA-Z0-9_])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Not, new Regex(
                        @"^\s*((?:not)(?![a-zA-Z0-9_]))([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.And, new Regex(
                        @"^\s*((?:and)(?![a-zA-Z0-9_]))([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Or, new Regex(
                        @"^\s*((?:or)(?![a-zA-Z0-9_]))([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Xor, new Regex(
                        @"^\s*((?:xor)(?![a-zA-Z0-9_]))([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Plus, new Regex(
                        @"^\s*((?:[+]))([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Minus, new Regex(
                        @"^\s*((?:[-]))([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Div, new Regex(
                        @"^\s*((?:[\/]))([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Mult, new Regex(
                        @"^\s*((?:[*]))([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));

                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Remainder, new Regex(
                        @"^\s*((?:[%]))([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Assign, new Regex(
                        @"^\s*((?::=))([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Colon, new Regex(
                        @"^\s*((?:[:]))([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.GreaterEq, new Regex(
                        @"^\s*((?:>=))([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Greater, new Regex(
                        @"^\s*((?:[>]))([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.LessEq, new Regex(
                        @"^\s*((?:<=))([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));

                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Less, new Regex(
                        @"^\s*((?:[<]))([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.Eq, new Regex(
                        @"^\s*((?:==))([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.NotEq, new Regex(
                        @"^\s*((?:!=))([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));

                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.ParenthesesL, new Regex(
                        @"^\s*(\\.{1,2}|[(])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.BracketsL, new Regex(
                        @"^\s*(\\.{1,2}|[[])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));
                
                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.ParenthesesR, new Regex(
                        @"^\s*(\\.{1,2}|[)])([\s\S]*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase)));

                _Tokens.Add(new Tuple<TokenTypes, Regex>(TokenTypes.BracketsR, new Regex(
                        @"^\s*(\\.{1,2}|[]])([\s\S]*)",
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
        }

        public List<Tuple<TokenTypes, string>> SplitToTokens(string text)
        {
                List<Tuple<TokenTypes, string>> result = new List<Tuple<TokenTypes, string>>();

                string[] splitedText = text.Split();

                foreach (string str in splitedText)
                {
                        List<string> splitedStr = new List<string>();
                        if (str.Contains('(') && str.Contains(')') || str.Contains('[') && str.Contains(']'))
                        {
                                splitedStr.Add(str[0].ToString());
                                splitedStr.Add(str.Substring(1, str.Length - 2));
                                splitedStr.Add(str.Last().ToString());
                        }
                        else if (str.Contains('(') || str.Contains('['))
                        {
                                splitedStr.Add(str[0].ToString());
                                splitedStr.Add(str.Substring(1));
                                
                        }
                        else if (str.Contains(')') || str.Contains(']'))
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
                                return resToken;
                        }
                }

                return new Tuple<TokenTypes, string>(TokenTypes.Undefined, str);
        }
}


