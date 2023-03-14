using ConsoleApp1.LexicalAnalyser;

namespace DefaultNamespace.SyntaxAnalyser;

public class TreeNode
{
    public string Name;
    public List<TreeNode> Children;


    public TreeNode(string name, TokenTypes token, string valueTokenized)
    {
        this.Name = name;
    }
}

