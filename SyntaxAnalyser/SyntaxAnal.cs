using ConsoleApp1.LexicalAnalyser;
using ConsoleApp1;
using DefaultNamespace;

public class SyntaxAnal
{
    public List<Tuple<TokenTypes, string>> lexicalAnalysis;

    public SyntaxAnal(List<Tuple<TokenTypes, string>> lexicalAnalysis)
    {
        this.lexicalAnalysis = lexicalAnalysis;
    }
    
    
}