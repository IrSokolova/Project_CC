using ConsoleApp1.LexicalAnalyser;

namespace ConsoleApp1.SyntaxAnalyser;

public class Parser
{
    private static List<Tuple<TokenTypes, string>> _lexicalAnalysis;
    private readonly TokenQueue _tokens;
    private Action _action;

    /// <summary>
    /// Создааём парсер и передаём ему лист токенов, чтобы создать TokenQueue
    /// </summary>
    /// <param name="lexicalAnalysis"></param>
    public Parser(List<Tuple<TokenTypes, string>> lexicalAnalysis)
    {
        _lexicalAnalysis = lexicalAnalysis;
        _tokens = new TokenQueue(_lexicalAnalysis);
    }
    
    /// <summary>
    /// Создаём Action, который состоит из Declaration и Actions
    /// </summary>
    /// <returns></returns>
    public Action? BuildAction()
    {
        if (_tokens.Current() != null)
        {
            Declaration? declaration = BuildDeclaration();
            Actions? actions = BuildActions();
            return new Action(declaration, actions);
        }

        return null;
    }

    /// <summary>
    /// Создаём Actions.
    /// Если action в нём отсутствует, то Actions = null
    /// </summary>
    /// <returns></returns>
    public Actions? BuildActions()
    {
        Action? action = BuildAction();
        if (action == null)
            return null;
        Actions? actions = BuildActions();
        return new Actions(action, actions);
    }

    /// <summary>
    /// Создаём Declaration.
    /// Чекаем, является ли токен "var", "type" или "routine",
    /// И вызываем соответствующую функцию.
    /// Или, если ни один токен не подошёл, возвращаем null
    /// </summary>
    /// <returns></returns>
    public Declaration? BuildDeclaration()
    {
        VariableDeclaration? variableDeclaration = null;
        TypeDeclaration? typeDeclaration = null;
        RoutineDeclaration? routineDeclaration = null;

        if (_tokens.Current() != null)
        {
            var nextToken = _tokens.Current();
            switch (nextToken.Item1)
            {
                case TokenTypes.Var:
                    _tokens.GetNextToken();
                    variableDeclaration = BuildVariableDeclaration();
                    break;
                case TokenTypes.Type:
                    _tokens.GetNextToken();
                    typeDeclaration = BuildTypeDeclaration();
                    break;
                case TokenTypes.Routine:
                    _tokens.GetNextToken();
                    routineDeclaration = BuildRoutineDeclaration();
                    break;
            }

            return (variableDeclaration == null && typeDeclaration == null && routineDeclaration == null)
                ? null
                : new Declaration(variableDeclaration, typeDeclaration, routineDeclaration);
        }

        return null;
    }

    /// <summary>
    /// По порядку считываем: Identifier, Assign(":"), Type, Expression
    /// Проверяем для каждого, соответствует ли ожидаемому токену
    /// и, если нет, выводим ошибку ("Expected ___: Found ___")
    /// </summary>
    /// <returns></returns>
    public VariableDeclaration? BuildVariableDeclaration()
    {

        string name = null;
        string type = null;
        bool readOnly = false;

        Type varType = null;
        Expression? expression = null;

        // Get Identifiers Token
        var nextToken = _tokens.GetNextToken();
        if (nextToken == null)
        {
            Console.WriteLine("Error in BuildVariableDeclaration. Expected Identifier: Found end of Tokens");
            Environment.Exit(0);
        }

        if (nextToken.Item1 is TokenTypes.Identifiers)
            name = nextToken.Item2;
        else
        {
            Console.WriteLine("Error in BuildVariableDeclaration. Expected Identifier: Found " + nextToken.Item1 +
                              " Token");
            Environment.Exit(0);
        }

        // Get Assign Token
        nextToken = _tokens.GetNextToken();
        if (nextToken == null)
        {
            Console.WriteLine("Error in BuildVariableDeclaration. Expected \":\": Found end of Tokens");
            Environment.Exit(0);
        }

        if (nextToken.Item1 is not TokenTypes.Assign)
        {
            Console.WriteLine("Error in BuildVariableDeclaration. Expected \":\": Found " + nextToken.Item1 + " Token");
            Environment.Exit(0);
        }

        // Get Type Token
        nextToken = _tokens.GetNextToken();
        if (nextToken == null)
        {
            Console.WriteLine("Error in BuildVariableDeclaration. Expected some Type: Found end of Tokens");
            Environment.Exit(0);
        }

        varType = BuildType();
        type = nextToken.Item2;
        
        Identifier identifier = new Identifier(readOnly, type, name);

        // Get "is" and Expression
        nextToken = _tokens.GetNextToken();

        if (nextToken?.Item1 is TokenTypes.Is)
        {
            nextToken = _tokens.GetNextToken();
            if (nextToken == null)
            {
                Console.WriteLine(
                    "Error in BuildVariableDeclaration. Expected some Expression: Found end of Tokens");
                Environment.Exit(0);
            }

            expression = BuildExpression();
            return new VariableDeclaration(identifier, varType, expression);
        }

        return new VariableDeclaration(identifier, varType, null);
    }

    
    //============================================================================
    // TODO доделать остальные методы
    public TypeDeclaration? BuildTypeDeclaration()
    {
        return null;
    }

    public RoutineDeclaration? BuildRoutineDeclaration()
    {
        return null;
    }

    public Type? BuildType()
    {
        PrimitiveType primitiveType = new PrimitiveType(true, false, false);
        return new Type(primitiveType, null, null);
    }

    public Expression? BuildExpression()
    {
        return null;
    }

}