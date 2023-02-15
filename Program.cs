using System.Text;
using ConsoleApp1.LexicalAnalyser;
using Tuple = System.Tuple;

namespace DefaultNamespace;
using System.Text.RegularExpressions;

internal class Program 
{
    public static void Main(string[] args)
    {
        string path = @"C:\Users\User\RiderProjects\Compiler Construction\СС_Project\program.txt";
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
            Console.WriteLine(text);
        }
        string[] splitedText = text.Split('\n');
        
        LexicalAnal lexicalAnal = new LexicalAnal();
        List<Tuple<TokenTypes, string>> lexicalAnalysisResult = new List<Tuple<TokenTypes, string>>(); 
        foreach (string str in splitedText)
        {
            text = str.Replace("\t", String.Empty);
            text = text.Replace("\n", String.Empty);
            text = text.Replace("\b", String.Empty);
            lexicalAnalysisResult.AddRange(lexicalAnal.SplitToTokens(text));
            List<Tuple<TokenTypes, string>> lexicalAnalysis = new List<Tuple<TokenTypes, string>>();
            foreach (Tuple<TokenTypes, string> token in lexicalAnalysisResult)
            {
                if (!(token.Item1 == TokenTypes.Undefined && token.Item2 == ""))
                {
                    lexicalAnalysis.Add(token);
                    Console.WriteLine(token);
                }
            }
        }
    }
}

