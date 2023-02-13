using ConsoleApp1.LexicalAnalyser;

namespace DefaultNamespace;
using System.Text.RegularExpressions;

internal class Program 
{
    public static void Main(string[] args)
    {
        string text = "var b : Integer is 6 routine main() is var c : Real is 8.7 if (b < c) then b := 10 end end";
        LexicalAnal lexicalAnal = new LexicalAnal();
        List<Tuple<TokenTypes, string>> aaaa = lexicalAnal.SplitToTokens(text);
    }
    
}

