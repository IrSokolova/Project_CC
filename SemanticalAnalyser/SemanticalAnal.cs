using Action = ConsoleApp1.SyntaxAnalyser.Action;

namespace DefaultNamespace.SemantycalAnalyser;

public class SemanticAnal
{
    private static Action? syntaxAnalysis;

    public SemanticAnal(Action? action)
    {
        syntaxAnalysis = action;
    }
}