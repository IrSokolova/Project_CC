using ConsoleApp1.LexicalAnalyser;
namespace ConsoleApp1.SyntaxAnalyser;

/// <summary>
/// Очередь из токенов создаётся из списка lexicalAnalysis,
/// чтобы потом доставать в парсере по одному токену
/// </summary>
public class TokenQueue
{
    private Queue<Tuple<TokenTypes, string>> _tokenQueue;

    public TokenQueue(List<Tuple<TokenTypes, string>> lexicalAnalysis)
    {
        _tokenQueue = new Queue<Tuple<TokenTypes, string>>(lexicalAnalysis);
    }

    /// <summary>
    /// Функция пытается удалить текущий токен из очереди и возвращает его или null
    /// </summary>
    /// <returns>nextToken or null</returns>
    public Tuple<TokenTypes, string?>? GetNextToken()
    {
        var isNextToken = _tokenQueue.TryDequeue(out var nextToken);
        return isNextToken ? nextToken : null;
    }

    /// <summary>
    /// Функция возвращает текущий токен или null
    /// </summary>
    /// <returns></returns>
    public Tuple<TokenTypes, string?>? Current()
    {
        var isCurrentToken = _tokenQueue.TryPeek(out var currentToken);
        return currentToken;
    }
}
