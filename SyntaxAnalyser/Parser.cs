using ConsoleApp1.LexicalAnalyser;
using System.Globalization;

namespace ConsoleApp1.SyntaxAnalyser;

public class Parser
{
    private static List<Tuple<TokenTypes, string>> _lexicalAnalysis;
    private readonly TokenQueue _tokens;
    //private Action _action;

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
    public Declaration? BuildDeclaration()
    {
        VariableDeclaration? variableDeclaration = null;
        TypeDeclaration? typeDeclaration = null;
        RoutineDeclaration? routineDeclaration = null;

        if (_tokens.Current() != null)
        {
            var nextToken = _tokens.GetNextToken();
            switch (nextToken!.Item1)
            {
                case TokenTypes.Var:
                    variableDeclaration = BuildVariableDeclaration();
                    break;
                case TokenTypes.Type:
                    typeDeclaration = BuildTypeDeclaration();
                    break;
                case TokenTypes.Routine:
                    routineDeclaration = BuildRoutineDeclaration();
                    break;
                default:
                    Console.WriteLine(
                        "Error in BuildDeclaration. Expected Var, Type or Routine tokens: Found " +
                        nextToken.Item1 +
                        " Token");
                    Environment.Exit(0);
                    break;
            }

            return new Declaration(variableDeclaration, typeDeclaration, routineDeclaration);
        }

        return null;
    }

    /// <summary>
    /// По порядку считываем: Identifier, Assign(":"), Type, Expression
    /// Проверяем для каждого, соответствует ли ожидаемому токену
    /// и, если нет, выводим ошибку ("Expected ___: Found ___")
    /// </summary>
    public VariableDeclaration? BuildVariableDeclaration()
    {

        string name = null;
        string type = null;
        bool readOnly = false;

        Type varType = null;
        Expression? expression = null;
        Value? value = null;

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

        // Get Colon Token
        nextToken = _tokens.GetNextToken();
        if (nextToken == null)
        {
            Console.WriteLine("Error in BuildVariableDeclaration. Expected \":\" token: Found end of Tokens");
            Environment.Exit(0);
        }

        if (nextToken.Item1 is not TokenTypes.Colon)
        {
            Console.WriteLine("Error in BuildVariableDeclaration. Expected \":\" token: Found " + nextToken.Item1 + " Token");
            Environment.Exit(0);
        }

        // Get Type
        nextToken = _tokens.Current();
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
            nextToken = _tokens.Current();
            if (nextToken == null)
            {
                Console.WriteLine(
                    "Error in BuildVariableDeclaration. Expected some Expression: Found end of Tokens");
                Environment.Exit(0);
            }

            expression = BuildExpression();
            value = new Value(expression);
            return new VariableDeclaration(identifier, varType, value);
        }

        return new VariableDeclaration(identifier, varType, null);
    }
    
    /// <summary>
    /// Checks for more VariableDeclarations.
    /// Used in BuildRecordType()
    /// </summary>
    public VariableDeclarations? BuildVariableDeclarations()
    {
        var nextToken = _tokens.Current();
        if(nextToken?.Item1 is not TokenTypes.Var)
            return null;
        
        VariableDeclaration? variableDeclaration = BuildVariableDeclaration();
        VariableDeclarations? variableDeclarations = BuildVariableDeclarations();
        return new VariableDeclarations(variableDeclaration, variableDeclarations);
    }
    
    /// <summary>
    /// TypeDeclaration structure: [name] is [Type]
    /// </summary>
    public TypeDeclaration? BuildTypeDeclaration()
    {
        string name = null;
        string type = null;
        bool readOnly = false;

        Type varType = null;

        // Get Identifiers Token
        var nextToken = _tokens.GetNextToken();
        if (nextToken == null)
        {
            Console.WriteLine("Error in BuildTypeDeclaration. Expected Identifier: Found end of Tokens");
            Environment.Exit(0);
        }

        if (nextToken.Item1 is TokenTypes.Identifiers)
            name = nextToken.Item2;
        else
        {
            Console.WriteLine("Error in BuildTypeDeclaration. Expected Identifier: Found " + nextToken.Item1 +
                              " Token");
            Environment.Exit(0);
        }
        
        // Get "is" token
        nextToken = _tokens.GetNextToken();

        if (nextToken == null)
        {
            Console.WriteLine("Error in BuildTypeDeclaration. Expected \"is\" token: Found end of Tokens");
            Environment.Exit(0);
        }
        
        if (nextToken?.Item1 is not TokenTypes.Is)
        {
            Console.WriteLine(
                    "Error in BuildTypeDeclaration. Expected \"is\" token: Found " + nextToken.Item1 +
                    " Token");
            Environment.Exit(0);
        }

        // Get Type
        nextToken = _tokens.Current();
        if (nextToken == null)
        {
            Console.WriteLine("Error in BuildTypeDeclaration. Expected some Type: Found end of Tokens");
            Environment.Exit(0);
        }

        varType = BuildType();
        type = nextToken.Item2;
        
        Identifier identifier = new Identifier(readOnly, type, name);

        return new TypeDeclaration(identifier, varType);
    }

    /// <summary>
    /// RoutineDeclaration contains MainRoutine or Function
    /// </summary>
    public RoutineDeclaration? BuildRoutineDeclaration()
    {
        if (_tokens.Current() == null)
        {
            Console.WriteLine("Error in BuildRoutineDeclaration. Expected mainRoutine or function: Found end of Tokens");
            Environment.Exit(0);
        }
        else
        {
            MainRoutine? mainRoutine = BuildMainRoutine();
            Function? function = BuildFunction();
            if (mainRoutine == null && function == null)
            {
                Console.WriteLine(
                    "Error in BuildRoutineDeclaration. Expected mainRoutine or function: Found null for both of them");
                Environment.Exit(0);
            }

            return new RoutineDeclaration(mainRoutine, function);
        }

        return null;
    }

    /// <summary>
    /// Checks for one of a primitive types or builds ArrayType or RecordType
    /// </summary>
    public Type? BuildType()
    {
        bool isInt = false;
        bool isReal = false;
        bool isBoolean = false;

        PrimitiveType? primitiveType = null;
        ArrayType? arrayType = null;
        RecordType? recordType = null;
        
        var nextToken = _tokens.GetNextToken();
        switch (nextToken.Item1)
        {
            case TokenTypes.Integer:
                isInt = true;
                break;
            case TokenTypes.Real:
                isReal = true;
                break;
            case TokenTypes.Boolean:
                isBoolean = true;
                break;
            case TokenTypes.Array:
                arrayType = BuildArrayType();
                break;
            case TokenTypes.Record:
                recordType = BuildRecordType();
                break;
        }
        
        if (isInt || isReal || isBoolean)
            primitiveType = new PrimitiveType(isInt, isReal, isBoolean);
        
        return new Type(primitiveType, arrayType, recordType);
    }
    
    /// <summary>
    /// Structure of Array: array '[' [Expression] ']' Type
    /// </summary>
    public ArrayType? BuildArrayType()
    {
        Expression? expression;
        Type? type;
        
        var nextToken = _tokens.GetNextToken();
        
        // Get BracketsL Token
        if (nextToken == null)
        {
            Console.WriteLine("Error in BuildArrayType. Expected \"[\" token: Found end of Tokens");
            Environment.Exit(0);
        }

        if (nextToken.Item1 is not TokenTypes.BracketsL)
        {
            Console.WriteLine("Error in BuildArrayType. Expected \"[\" token: Found " + nextToken.Item1 + " Token");
            Environment.Exit(0);
        }
        
        // Get Expression
        nextToken = _tokens.Current();
        if (nextToken == null)
        {
            Console.WriteLine(
                "Error in BuildArrayType. Expected some Expression: Found end of Tokens");
            Environment.Exit(0);
        }
        expression = BuildExpression();
        
        // Get BracketsR Token
        nextToken = _tokens.GetNextToken();
        if (nextToken == null)
        {
            Console.WriteLine("Error in BuildArrayType. Expected \"]\" token: Found end of Tokens");
            Environment.Exit(0);
        }

        if (nextToken.Item1 is not TokenTypes.BracketsR)
        {
            Console.WriteLine("Error in BuildArrayType. Expected \"]\" token: Found " + nextToken.Item1 + " Token");
            Environment.Exit(0);
        }
        
        // Get Type
        nextToken = _tokens.Current();
        if (nextToken == null)
        {
            Console.WriteLine("Error in BuildArrayType. Expected some Type: Found end of Tokens");
            Environment.Exit(0);
        }
        type = BuildType();
        
        return new ArrayType(expression, type);
    }
    
    /// <summary>
    /// Structure of Record: record [VariableDeclarations] end
    /// </summary>
    public RecordType? BuildRecordType()
    {
        VariableDeclaration? variableDeclaration;
        VariableDeclarations? variableDeclarations;
        var nextToken = _tokens.GetNextToken();
        
        // Get VariableDeclarations
        if (nextToken == null)
        {
            Console.WriteLine("Error in BuildRecordType. Expected Var token: Found end of Tokens");
            Environment.Exit(0);
        }

        if (nextToken.Item1 is not TokenTypes.Var)
        {
            Console.WriteLine("Error in BuildRecordType. Expected Var token: Found " + nextToken.Item1 + " Token");
            Environment.Exit(0);
        }

        variableDeclaration = BuildVariableDeclaration();
        variableDeclarations = BuildVariableDeclarations();

        // Get End token
        nextToken = _tokens.Current();
        if (nextToken == null)
        {
            Console.WriteLine("Error in BuildRecordType. Expected End token: Found end of Tokens");
            Environment.Exit(0);
        }

        if (nextToken.Item1 is not TokenTypes.End)
        {
            Console.WriteLine("Error in BuildRecordType. Expected End token: Found " + nextToken.Item1 + " Token");
            Environment.Exit(0);
        }
        
        return new RecordType(variableDeclaration, variableDeclarations);
    }
    
    /// <summary>
    /// Expression contains one or more Relations
    /// </summary>
    public Expression? BuildExpression()
    {
        // Try to build Relation
        Relation? relation = BuildRelation();
        if (relation == null)
            return null;
        
        // Try to build more Relations
        MultipleRelation? multipleRelation= BuildMultipleRelation();
        
        return new Expression(relation, multipleRelation);
    }

    /// <summary>
    /// Relation contains Operation and maybe Comparison following it
    /// </summary>
    public Relation? BuildRelation()
    {
        Operation? operation = BuildOperation();
        Comparison? comparison = null;
        if (operation == null)
            return null;
        
        comparison = BuildComparison();
        return new Relation(operation, comparison);
    }

    // TODO Implement logicalOperatorBuild
    /// <summary>
    /// MultipleRelation contains one or more Relations
    /// Otherwise, return null
    /// </summary>
    public MultipleRelation? BuildMultipleRelation()
    {
        LogicalOperator? logicalOperator = null;
        Relation? relation = BuildRelation();

        if (relation == null)
            return null;
        
        MultipleRelation? multipleRelation = null;
        multipleRelation = BuildMultipleRelation();
        return new MultipleRelation(relation, multipleRelation, logicalOperator);
    }

    /// <summary>
    /// Operation contains Operand and maybe Operator (sign)
    /// </summary>
    public Operation? BuildOperation()
    {
        Operand? operand = BuildOperand();
        if (operand == null)
            return null;
        Operator? @operator = BuildOperator();
        
        
        return new Operation(operand, @operator);
    }

    /// <summary>
    /// Comparison contains a ComparisonOperator and Operation following it
    /// </summary>
    public Comparison? BuildComparison()
    {
        // Try to build operator, if falling - return null
        Operator? @operator = BuildOperator();
        if(@operator == null)
            return null;
        
        // Try to build operation, if falling - error
        Operation? operation = BuildOperation();
        if (operation == null)
        {
            Console.WriteLine("Error in BuildComparison. Expected Operation after ComparisonOperator: Found null");
            Environment.Exit(0);
        }
        return new Comparison(@operator, operation);
    }

    /// <summary>
    /// Operand is either a Single or an Expression
    /// </summary>
    public Operand? BuildOperand()
    {
        // Try to build Single
        Single? single = BuildSingle();
        Expression? expression = null;
        
        // If it's not Single, try to build Expression
        var nextToken = _tokens.Current();
        if (single == null && nextToken != null)
        {
            if (nextToken.Item1 is TokenTypes.ParenthesesL)
            {
                _tokens.GetNextToken();
                expression = BuildExpression();
                
                // Check if parentheses are closed
                nextToken = _tokens.Current();
                if (nextToken == null)
                {
                    Console.WriteLine("Error in BuildOperand. Expected \")\" token: Found end of Tokens");
                    Environment.Exit(0);
                }

                if (nextToken.Item1 is not TokenTypes.ParenthesesR)
                {
                    Console.WriteLine("Error in BuildOperand. Expected \")\" token: Found " + nextToken.Item1 +
                                      " Token");
                    Environment.Exit(0);
                }
                _tokens.GetNextToken();
            }
        }
        else if (nextToken != null)
            _tokens.GetNextToken();

        if (single == null && expression == null)
            return null;
        return new Operand(single, expression);
    }

    /// <summary>
    /// Single is either a variable or a number (Float or Integer)
    /// </summary>
    public Single? BuildSingle()
    {
        Type? type = null;
        float? value = null;
        Variable? variable = null;
        Identifier? identifier = null;
        PrimitiveType? primitiveType = null;
        
        var nextToken = _tokens.Current();
        if (nextToken == null)
            return null;
        
        // If Single is a variable
        if (nextToken.Item1 is TokenTypes.Identifiers)
        {
            identifier = new Identifier(true, null, nextToken.Item2);
            variable = new Variable(identifier);
            return new Single(type, value, variable);
        }

        // If Single is Float
        if (nextToken.Item1 is TokenTypes.FloatingLiterals)
        {
            primitiveType = new PrimitiveType(false, true, false);
            type = new Type(primitiveType, null, null);
            float floatValue = float.Parse(nextToken.Item2, CultureInfo.InvariantCulture.NumberFormat);
            return new Single(type, floatValue, null);
        }

        // If Single is Integer
        if (nextToken.Item1 is TokenTypes.IntegerLiterals)
        {
            primitiveType = new PrimitiveType(true, false, false);
            type = new Type(primitiveType, null, null);
            int intValue = Int16.Parse(nextToken.Item2);
            return new Single(type, intValue, null);
        }

        return null;
    }

    /// <summary>
    /// Checks for one of the operators: comparison, mathematical or logical
    /// </summary>
    public Operator? BuildOperator()
    {
        ComparisonOperator? comparisonOperator = BuildComparisonOperator();
        MathematicalOperator? mathematicalOperator = BuildMathematicalOperator();
        LogicalOperator? logicalOperator = null;
        if (comparisonOperator == null && mathematicalOperator == null && logicalOperator == null)
            return null;
        return new Operator(comparisonOperator, mathematicalOperator, logicalOperator);
    }
    
    /// <summary>
    /// Checks for > < >= <= != == tokens
    /// </summary>
    public ComparisonOperator? BuildComparisonOperator()
    {
        string sign = "";
        var nextToken = _tokens.Current();
        if (nextToken == null)
            return null;
        switch (nextToken.Item1)
        {
            case TokenTypes.Greater:
                _tokens.GetNextToken();
                sign = ">";
                break;
            case TokenTypes.GreaterEq:
                _tokens.GetNextToken();
                sign = ">=";
                break;
            case TokenTypes.Less:
                _tokens.GetNextToken();
                sign = "<";
                break;
            case TokenTypes.LessEq:
                _tokens.GetNextToken();
                sign = "<=";
                break;
            case TokenTypes.Eq:
                _tokens.GetNextToken();
                sign = "==";
                break;
            case TokenTypes.NotEq:
                _tokens.GetNextToken();
                sign = "!=";
                break;
            default:
                return null;
                break;
        }

        return new ComparisonOperator(sign);
    }
    
    /// <summary>
    /// Checks for + - * / % tokens
    /// </summary>
    public MathematicalOperator? BuildMathematicalOperator()
    {
        string sign = "";
        var nextToken = _tokens.Current();
        if (nextToken == null)
            return null;
        switch (nextToken.Item1)
        {
            case TokenTypes.Plus:
                _tokens.GetNextToken();
                sign = "+";
                break;
            case TokenTypes.Minus:
                _tokens.GetNextToken();
                sign = "-";
                break;
            case TokenTypes.Div:
                _tokens.GetNextToken();
                sign = "/";
                break;
            case TokenTypes.Mult:
                _tokens.GetNextToken();
                sign = "*";
                break;
            case TokenTypes.Remainder:
                _tokens.GetNextToken();
                sign = "%";
                break;
            default:
                return null;
                break;
        }

        return new MathematicalOperator(sign);
    }

    
    //============================================================================
    // TODO доделать остальные методы
    
    // not and or xor
    public LogicalOperator? BuildLogicalOperator()
    {
        return null;
    }

    public MainRoutine? BuildMainRoutine()
    {
        return null;
    }
    
    public Function? BuildFunction()
    {
        return null;
    }

}