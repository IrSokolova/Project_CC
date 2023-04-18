using System.Text;
using ConsoleApp1.LexicalAnalyser;      
using ConsoleApp1.SyntaxAnalyser;
using DefaultNamespace.SemantycalAnalyser;

namespace DefaultNamespace;

internal class Program 
{
    public static void Main(string[] args)
    {
        // ADD YOUR PATH HERE
        // string path = @"C:\Users\alena\RiderProjects\Project_CC\Project_CC\program.txt";
        // string path = @"C:\Users\User\RiderProjects\Compiler Construction\СС_Project\program.txt";
        string path = @"C:\Users\79953\RiderProjects\Project_CC\program.txt"; // do not delete please
        string text = "";

        using (FileStream stream = File.OpenRead(path))
        {
            int totalBytes = (int)stream.Length;
            byte[] bytes = new byte[totalBytes];
            int bytesRead = 0;
 
            while (bytesRead < totalBytes)
            {
                int len = stream.Read(bytes, bytesRead, totalBytes);
                bytesRead += len;
            }
 
            text = Encoding.UTF8.GetString(bytes);
        }
        string[] splitedText = text.Split('\n');
        
        LexicalAnal lexicalAnal = new LexicalAnal();
        lexicalAnal.AddTokens();
        List<Tuple<TokenTypes, string>> lexicalAnalysisResult = new List<Tuple<TokenTypes, string>>();
        foreach (string str in splitedText)
        {
            text = str.Replace("\t", String.Empty);
            text = text.Replace("\b", String.Empty);
            lexicalAnalysisResult.AddRange(lexicalAnal.SplitToTokens(text));
        }
        List<Tuple<TokenTypes, string>> lexicalAnalysis = new List<Tuple<TokenTypes, string>>();
        foreach (Tuple<TokenTypes, string> token in lexicalAnalysisResult)
        {
            if (!(token.Item1 == TokenTypes.Undefined && token.Item2 == ""))
            {
                lexicalAnalysis.Add(token);
                Console.WriteLine(token);
            }
        }

        Parser parser = new Parser(lexicalAnalysis);
        var action = parser.BuildAction();
        Console.WriteLine("=========================");
        Console.WriteLine("        *  AST  *        ");
        Console.WriteLine("=========================");
        Console.WriteLine("└" + action.ToString(""));

        SemanticAnal semanticAnal = new SemanticAnal(action);

    }
}

