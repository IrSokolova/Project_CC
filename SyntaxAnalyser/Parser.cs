using ConsoleApp1.LexicalAnalyser;
using System.Globalization;
using Microsoft.VisualBasic.CompilerServices;

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
    /// Создаём Action, который состоит из Declaration, Statement и Actions
    /// </summary>
    public Action? BuildAction()
    {
        if (_tokens.Current() != null)
        {
            Statement? statement = null;
            Declaration? declaration = BuildDeclaration();
            if (declaration == null)
            {
                statement = BuildStatement(); // !there is no check for statement inside BuildStatement!
                if (statement == null)
                {
                    return null;
                }
            }
            Actions? actions = BuildActions();
            return new Action(declaration, statement, actions);
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
    ///
    /// Если токен не подходит то очередь не меняется и декларэйшн не строится.
    /// </summary>
    public Declaration? BuildDeclaration()
    {
        VariableDeclaration? variableDeclaration = null;
        TypeDeclaration? typeDeclaration = null;
        RoutineDeclaration? routineDeclaration = null;

        if (_tokens.Current() != null)
        {
            var token = _tokens.Current();
            switch (token!.Item1)
            {
                case TokenTypes.Var:
                    _tokens.GetNextToken();
                    variableDeclaration = BuildVariableDeclaration();
                    return new Declaration(variableDeclaration, typeDeclaration, routineDeclaration);
                case TokenTypes.Type:
                    _tokens.GetNextToken();
                    typeDeclaration = BuildTypeDeclaration();
                    return new Declaration(variableDeclaration, typeDeclaration, routineDeclaration);
                case TokenTypes.Routine :
                    _tokens.GetNextToken();
                    routineDeclaration = BuildRoutineDeclaration();
                    return new Declaration(variableDeclaration, typeDeclaration, routineDeclaration);
                case TokenTypes.Function:
                    _tokens.GetNextToken();
                    routineDeclaration = BuildFunctionDeclaration();
                    return new Declaration(variableDeclaration, typeDeclaration, routineDeclaration);
                default:
                    return null;
            }
        }
        return null;
    }
    
    public Statement? BuildStatement()
    {
        WhileLoop? whileLoop = null;
        ForLoop? forLoop = null;
        IfStatement? ifStatement = null;
        
        Assignment? assignment = null;
        RoutineCall? routineCall = null;

        if (_tokens.Current() != null)
        {
            var token = _tokens.Current();
            // var token = _tokens.GetNextToken();
            switch (token!.Item1)
            {
                case TokenTypes.While:
                    _tokens.GetNextToken();
                    whileLoop = BuildWhileLoop();
                    break;
                case TokenTypes.For:
                    _tokens.GetNextToken();
                    forLoop = BuildForLoop();
                    break;
                case TokenTypes.If :
                    _tokens.GetNextToken();
                    ifStatement = BuildIfStatement();
                    break;
                case TokenTypes.Identifiers:
                    _tokens.GetNextToken();
                    string? name = token.Item2;
                    Identifier identifier = new Identifier(false, null, name);
                    
                    token = _tokens.Current();
                    CheckNull(token, TokenTypes.Assign, "BuildStatement");
                    Type? arrayType = null;
                    if (token!.Item1 == TokenTypes.BracketsL)
                    {
                        var array = BuildArrayType(false);
                        arrayType = new Type(null, array, null, null);
                    }
                    
                    token = _tokens.GetNextToken();
                    CheckNull(token, TokenTypes.Assign, "BuildStatement");
                
                    RoutineCall? rc = null;
                    Expressions? expressions = null;
                    switch (token!.Item1)
                    {
                        case TokenTypes.Assign:
                            token = _tokens.Current();
                            if (token.Item1 is TokenTypes.ParenthesesL)
                            {
                                _tokens.GetNextToken();
                                // Expression? expression = BuildExpression();
                                // nextToken = _tokens.GetNextToken();
                                // if (nextToken != null && nextToken.Item1 is TokenTypes.ParenthesesR)
                                expressions = BuildExpressions();
                                token = _tokens.GetNextToken();
                                CheckNull(token, TokenTypes.ParenthesesR, "BuildStatement");
                                CheckTokenMatch(token!.Item1, TokenTypes.ParenthesesR, "BuildStatement");
                                if (expressions._expressions == null)
                                {
                                    Operand o1 = new Operand(null, expressions._expression);
                                    Operator? o2 = BuildOperator();
                                    Operation? o3 = BuildOperation();
                                    Expression e =
                                        new Expression(new Relation(new Operation(o1, o2, o3), null), null);
                                    expressions = new Expressions(e, null);
                                }
                            }
                            else
                            {
                                Expression? exp = BuildExpression();
                                if (exp == null)
                                    expressions = null;
                                else
                                    expressions = new Expressions(exp, null);
                            }
                            
                            
                            // token = _tokens.Current();
                            Variable variable = new Variable(identifier, arrayType);
                            if (expressions == null && _tokens.Current()!.Item1 is TokenTypes.ParenthesesL)
                            {
                                _tokens.GetNextToken();
                                Identifier callIdentifier = new Identifier(false, null, token.Item2);
                                Expressions? callExpressions = BuildExpressions();
                                token = _tokens.GetNextToken();
                                CheckNull(token, TokenTypes.ParenthesesR, "BuildStatement");
                                CheckTokenMatch(token!.Item1, TokenTypes.ParenthesesR, "BuildStatement");
                                routineCall = new RoutineCall(callIdentifier, callExpressions);
                            }
                            assignment = new Assignment(variable, expressions, routineCall);
                            break;
                        case TokenTypes.ParenthesesL:
                            expressions = BuildExpressions();
                            token = _tokens.GetNextToken();
                            CheckNull(token, TokenTypes.ParenthesesR, "BuildStatement");
                            CheckTokenMatch(token!.Item1, TokenTypes.ParenthesesR, "BuildStatement");
                            routineCall = new RoutineCall(identifier, expressions);
                            break;
                    }
                    break;
            }

            if (assignment != null || 
                whileLoop != null || 
                forLoop != null || 
                ifStatement != null ||
                routineCall != null)
            {
                return new Statement(assignment, whileLoop, forLoop, ifStatement, routineCall);
            }
        }
        return null;
    }

    public WhileLoop BuildWhileLoop()
    {
        var nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.ParenthesesL, "BuildWhileLoop");
        CheckTokenMatch(nextToken!.Item1, TokenTypes.ParenthesesL, "BuildWhileLoop");
        
        Expression? expression = BuildExpression();

        nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.ParenthesesR, "BuildWhileLoop");
        CheckTokenMatch(nextToken!.Item1, TokenTypes.ParenthesesR, "BuildWhileLoop");
        
        nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.Loop, "BuildWhileLoop");
        CheckTokenMatch(nextToken!.Item1, TokenTypes.Loop, "BuildWhileLoop");

        Body? body = BuildBody();

        nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.End, "BuildWhileLoop");
        CheckTokenMatch(nextToken!.Item1, TokenTypes.End, "BuildWhileLoop");
        return new WhileLoop(expression, body);
    }
    
    public ForLoop BuildForLoop()
    {
        var nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.Identifiers, "BuildForLoop");
        CheckTokenMatch(nextToken!.Item1, TokenTypes.Identifiers, "BuildForLoop");
        Identifier identifier = new Identifier(true, "Integer", nextToken.Item2);
        
        nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.Assign, "BuildForLoop");
        CheckTokenMatch(nextToken!.Item1, TokenTypes.Assign, "BuildForLoop");
        
        Expression from = BuildExpression();
        
        nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.To, "BuildForLoop");
        CheckTokenMatch(nextToken!.Item1, TokenTypes.To, "BuildForLoop");
        
        Expression to = BuildExpression();
        Range range = new Range(from, to);
        
        nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.Reverse, "BuildForLoop");
        Boolean reverse = false;
        Body? body;
        if (nextToken!.Item1 == TokenTypes.Reverse)
        {
            reverse = true;
            nextToken = _tokens.GetNextToken();
            CheckNull(nextToken!, TokenTypes.Loop, "BuildForLoop");
        }
        CheckTokenMatch(nextToken!.Item1, TokenTypes.Loop, "BuildForLoop");
        body = BuildBody();
        nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.End, "BuildForLoop");
        CheckTokenMatch(nextToken!.Item1, TokenTypes.End, "BuildForLoop");
        return new ForLoop(identifier, reverse, range, body);
    }
    
    public IfStatement BuildIfStatement()
    {
        var nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.ParenthesesL, "BuildIfStatement");
        CheckTokenMatch(nextToken!.Item1, TokenTypes.ParenthesesL, "BuildIfStatement");
        
        Expression? expression = BuildExpression();
        
        nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.ParenthesesR, "BuildIfStatement");
        CheckTokenMatch(nextToken!.Item1, TokenTypes.ParenthesesR, "BuildIfStatement");
        
        nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.Then, "BuildIfStatement");
        CheckTokenMatch(nextToken!.Item1, TokenTypes.Then, "BuildIfStatement");

        Body? ifBody = BuildBody(); // BuildIfBody because the body ends with "Else" token BUT maybe BuildBody is ok idk
        Body? elseBody = null;
        CheckNull(_tokens.Current(), TokenTypes.Else, "BuildIfStatement");
        
        nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.End, "BuildIfStatement");
        if (nextToken!.Item1 == TokenTypes.Else)
        {
            elseBody = BuildBody();
            nextToken = _tokens.GetNextToken();
            CheckNull(nextToken, TokenTypes.End, "BuildIfStatement");
            CheckTokenMatch(nextToken!.Item1, TokenTypes.End, "BuildIfStatement");
        }
        else
        {
            CheckTokenMatch(nextToken.Item1, TokenTypes.End, "BuildIfStatement");
        }

        return new IfStatement(expression, ifBody, elseBody);
    }

    /// <summary>
    /// По порядку считываем: Identifier, Assign(":"), Type, Expression
    /// Проверяем для каждого, соответствует ли ожидаемому токену
    /// и, если нет, выводим ошибку ("Expected ___: Found ___")
    /// </summary>
    public VariableDeclaration? BuildVariableDeclaration()
    {
        string? name = null;
        string? type = null;
        bool readOnly = false;

        Type varType = null;
        Expressions? expressions = null;
        Value? value = null;

        // Get Identifiers Token
        var nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.Identifiers, "BuildVariableDeclaration");
        CheckTokenMatch(nextToken!.Item1, TokenTypes.Identifiers, "BuildVariableDeclaration");
        name = nextToken.Item2;

        // Get Colon Token
        nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.Colon, "BuildVariableDeclaration");
        CheckTokenMatch(nextToken.Item1, TokenTypes.Colon, "BuildVariableDeclaration");

        // Get Type
        nextToken = _tokens.Current();
        CheckNull(nextToken, TokenTypes.Type, "BuildVariableDeclaration"); // Expected some Type

        varType = BuildType();
        type = nextToken.Item2;
        
        Identifier identifier = new Identifier(readOnly, type, name);

        // Get "is" and Expression (or several Expressions)
        nextToken = _tokens.Current();

        if (nextToken?.Item1 is TokenTypes.Is)
        {
            _tokens.GetNextToken();
            nextToken = _tokens.Current();
            CheckNull(nextToken, "Expression", "BuildVariableDeclaration");

            if (nextToken.Item1 is TokenTypes.ParenthesesL)
            {
                _tokens.GetNextToken();
                // Expression? expression = BuildExpression();
                // nextToken = _tokens.GetNextToken();
                // if (nextToken != null && nextToken.Item1 is TokenTypes.ParenthesesR)
                expressions = BuildExpressions();
                nextToken = _tokens.GetNextToken();
                CheckNull(nextToken, TokenTypes.ParenthesesR, "BuildVariableDeclaration");
                CheckTokenMatch(nextToken!.Item1, TokenTypes.ParenthesesR, "BuildVariableDeclaration");
                if (expressions._expressions == null)
                {
                    Operand o1 = new Operand(null, expressions._expression);
                    Operator? o2 = BuildOperator();
                    Operation? o3 = BuildOperation();
                    Expression e =
                        new Expression(new Relation(new Operation(o1, o2, o3), null), null);
                    expressions = new Expressions(e, null);
                }
            }
            else
            {
                Expression? exp = BuildExpression();
                if (exp == null)
                    expressions = null;
                else
                    expressions = new Expressions(exp, null);
            }
            
            value = new Value(expressions);
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

        _tokens.GetNextToken();
        VariableDeclaration? variableDeclaration = BuildVariableDeclaration();
        VariableDeclarations? variableDeclarations = BuildVariableDeclarations();
        return new VariableDeclarations(variableDeclaration, variableDeclarations);
    }
    
    /// <summary>
    /// TypeDeclaration structure: [name] is [Type]
    /// </summary>
    public TypeDeclaration? BuildTypeDeclaration()
    {
        string? name = null;
        string? type = null;
        bool readOnly = false;

        Type varType = null;

        // Get Identifiers Token
        var nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.Identifiers, "BuildTypeDeclaration");
        CheckTokenMatch(nextToken.Item1, TokenTypes.Identifiers, "BuildTypeDeclaration");
        name = nextToken.Item2;
        
        // Get "is" token
        nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.Is, "BuildTypeDeclaration");
        CheckTokenMatch(nextToken.Item1, TokenTypes.Is, "BuildTypeDeclaration");

        // Get Type
        nextToken = _tokens.Current();
        CheckNull(nextToken, TokenTypes.Type, "BuildTypeDeclaration");

        varType = BuildType();
        type = nextToken!.Item2;
        
        Identifier identifier = new Identifier(readOnly, type, name);

        return new TypeDeclaration(identifier, varType);
    }

    /// <summary>
    /// RoutineDeclaration contains MainRoutine or Function
    /// </summary>
    public RoutineDeclaration BuildRoutineDeclaration()
    {
        CheckNull(_tokens.Current(), "Routine", "BuildRoutineDeclaration");
        
        MainRoutine? mainRoutine = BuildMainRoutine();
        if (mainRoutine == null)
        {
            Console.WriteLine("Error in BuildRoutineDeclaration. Expected Routine: Found null for both of them");
            Environment.Exit(0);
        }

        var nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.End, "BuildRoutineDeclaration");
        CheckTokenMatch(nextToken!.Item1, TokenTypes.End, "BuildRoutineDeclaration");
        return new RoutineDeclaration(mainRoutine, null);
    }
    
    public RoutineDeclaration BuildFunctionDeclaration()
    {
        CheckNull(_tokens.Current(), "Function", "BuildFunctionDeclaration");
        
        Function? function = BuildFunction();
        if (function == null)
        {
            Console.WriteLine(
                "Error in BuildFunctionDeclaration. Expected  function: Found null for both of them");
            Environment.Exit(0);
        }
        
        var nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.End, "BuildFunctionDeclaration");
        CheckTokenMatch(nextToken!.Item1, TokenTypes.End, "BuildFunctionDeclaration");
        return new RoutineDeclaration(null, function);
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
        UserType? userType = null;
        
        var nextToken = _tokens.Current();
        switch (nextToken.Item1)
        {
            case TokenTypes.Integer:
                _tokens.GetNextToken();
                isInt = true;
                break;
            case TokenTypes.Real:
                _tokens.GetNextToken();
                isReal = true;
                break;
            case TokenTypes.Boolean:
                _tokens.GetNextToken();
                isBoolean = true;
                break;
            case TokenTypes.Array:
                _tokens.GetNextToken();
                arrayType = BuildArrayType(true);
                break;
            case TokenTypes.Record:
                _tokens.GetNextToken();
                recordType = BuildRecordType();
                break;
            case TokenTypes.Identifiers:
                _tokens.GetNextToken();
                userType = new UserType(nextToken.Item2);
                break;
        }
        
        if (isInt || isReal || isBoolean)
            primitiveType = new PrimitiveType(isInt, isReal, isBoolean);

        if (primitiveType == null && arrayType == null && recordType == null && userType == null)
        {
            return null;
        }
        return new Type(primitiveType, arrayType, recordType, userType);
    }
    
    /// <summary>
    /// Structure of Array: array '[' [Expression] ']' Type
    /// </summary>
    public ArrayType? BuildArrayType(Boolean isDeclaration)
    {
        Expression? expression;
        Type? type = null;
        
        var nextToken = _tokens.GetNextToken();
        
        // Get BracketsL Token
        CheckNull(nextToken, TokenTypes.BracketsL, "BuildArrayType");
        CheckTokenMatch(nextToken.Item1, TokenTypes.BracketsL, "BuildArrayType");

        // Get Expression
        nextToken = _tokens.Current();
        CheckNull(nextToken, "Expression", "BuildArrayType");
        expression = BuildExpression();
        
        // Get BracketsR Token
        nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.BracketsR, "BuildArrayType");
        CheckTokenMatch(nextToken.Item1, TokenTypes.BracketsR, "BuildArrayType");

        // Get Type
        if (isDeclaration)
        {
            nextToken = _tokens.Current();
            CheckNull(nextToken, TokenTypes.Type, "BuildArrayType");
            type = BuildType();
        }
        
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
        CheckNull(nextToken, TokenTypes.FigureBracketsL, "BuildRecordType");
        CheckTokenMatch(nextToken!.Item1, TokenTypes.FigureBracketsL, "BuildRecordType");
        
        // Get VariableDeclarations
        nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.Var, "BuildRecordType");
        CheckTokenMatch(nextToken!.Item1, TokenTypes.Var, "BuildRecordType");
        
        variableDeclaration = BuildVariableDeclaration();
        variableDeclarations = BuildVariableDeclarations();

        // Get End token
        nextToken = _tokens.Current();
        CheckNull(nextToken, TokenTypes.FigureBracketsR, "BuildRecordType");
        CheckTokenMatch(nextToken!.Item1, TokenTypes.FigureBracketsR, "BuildRecordType");
        _tokens.GetNextToken();
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
        // TODO бесполезная штука вроде
        // MultipleRelation? multipleRelation = BuildMultipleRelation();
        MultipleRelation? multipleRelation = null;

        return new Expression(relation, multipleRelation);
    }

    public Expressions? BuildExpressions()
    {
        Expression? expression = BuildExpression();
        Expressions? expressions;
        if (expression != null)
        {
            expressions = BuildExpressions();
            return new Expressions(expression, expressions);
        }

        return null;
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
        
        // comparison = BuildComparison();
        return new Relation(operation, comparison);
    }

    // TODO Implement logicalOperatorBuild
    /// <summary>
    /// MultipleRelation contains one or more Relations
    /// Otherwise, return null
    /// </summary>
    public MultipleRelation? BuildMultipleRelation()
    {
        Relation? relation = BuildRelation();

        if (relation == null)
            return null;
        
        MultipleRelation? multipleRelation = BuildMultipleRelation();
        return new MultipleRelation(relation, multipleRelation);
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
        Operation? operation = null;
        if (@operator != null)
            operation = BuildOperation();
        
        return new Operation(operand, @operator, operation);
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
        
        var nextToken = _tokens.Current();
        if (single != null && nextToken != null && nextToken.Item1 is TokenTypes.ParenthesesL)
        {
            return null;
        }
        
        // If it's not Single, try to build Expression
        if (single == null && nextToken != null)
        {
            if (nextToken.Item1 is TokenTypes.ParenthesesL)
            {
                _tokens.GetNextToken();
                expression = BuildExpression();
                
                // Check if parentheses are closed
                nextToken = _tokens.Current();
                CheckNull(nextToken, TokenTypes.ParenthesesR, "BuildOperand");
                CheckTokenMatch(nextToken!.Item1, TokenTypes.ParenthesesR, "BuildOperand");
                
                _tokens.GetNextToken();
            }
        }
        // else if (nextToken != null && 
        //          nextToken.Item1 != TokenTypes.BracketsR)
        //     _tokens.GetNextToken();

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
        string? value = null;
        Variable? variable = null;
        Identifier? identifier = null;
        PrimitiveType? primitiveType = null;
        
        var nextToken = _tokens.Current();
        if (nextToken == null)
            return null;
        
        // If Single is a variable
        if (nextToken.Item1 is TokenTypes.Identifiers)
        {
            _tokens.GetNextToken();
            identifier = new Identifier(true, null, nextToken.Item2);
            var token = _tokens.Current();
            // CheckNull(token, TokenTypes.Assign, "BuildSingle");
            Type? arrayType = null;
            if (token != null && token.Item1 == TokenTypes.BracketsL)
            {
                var array = BuildArrayType(false);
                arrayType = new Type(null, array, null, null);
            }
            variable = new Variable(identifier, arrayType);
            return new Single(type, value, variable);
        }

        // If Single is Float
        if (nextToken.Item1 is TokenTypes.FloatingLiterals)
        {
            _tokens.GetNextToken();
            primitiveType = new PrimitiveType(false, true, false);
            type = new Type(primitiveType, null, null, null);
            string floatValue = nextToken.Item2;
            return new Single(type, floatValue, null);
        }

        // If Single is Integer
        if (nextToken.Item1 is TokenTypes.IntegerLiterals)
        {
            _tokens.GetNextToken();
            primitiveType = new PrimitiveType(true, false, false);
            type = new Type(primitiveType, null, null, null);
            string intValue = nextToken.Item2;
            return new Single(type, intValue, null);
        }
        
        // If Single is Boolean
        if (nextToken.Item1 is TokenTypes.True || nextToken.Item1 is TokenTypes.False)
        {
            var nextT = _tokens.GetNextToken();
            primitiveType = new PrimitiveType(false, false, true);
            type = new Type(primitiveType, null, null, null);
            string boolValue = nextToken.Item2;
            return new Single(type, boolValue, null);
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
        LogicalOperator? logicalOperator = BuildLogicalOperator();
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
    
    // not and or xor
    public LogicalOperator? BuildLogicalOperator()
    {
        string op = "";
        var nextToken = _tokens.Current();
        if (nextToken == null)
            return null;
        switch (nextToken.Item1)
        {
            case TokenTypes.And:
                _tokens.GetNextToken();
                op = "and";
                break;
            case TokenTypes.Or:
                _tokens.GetNextToken();
                op = "or";
                break;
            case TokenTypes.Xor:
                _tokens.GetNextToken();
                op = "xor";
                break;
            default:
                return null;
        }

        return new LogicalOperator(op);
    }

    public MainRoutine? BuildMainRoutine()
    {
        var nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.Identifiers, "BuildMainRoutine");
        CheckTokenMatch(nextToken!.Item1, TokenTypes.Identifiers, "BuildMainRoutine");

        string? name = nextToken.Item2;
        
        nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.ParenthesesL, "BuildMainRoutine");
        CheckTokenMatch(nextToken!.Item1, TokenTypes.ParenthesesL, "BuildMainRoutine");
        
        nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.ParenthesesR, "BuildMainRoutine");
        CheckTokenMatch(nextToken!.Item1, TokenTypes.ParenthesesR, "BuildMainRoutine");
        
        nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.Is, "BuildMainRoutine");
        CheckTokenMatch(nextToken!.Item1, TokenTypes.Is, "BuildMainRoutine");
        
        Identifier identifier = new Identifier(true, "Function", name);
        Body? body = BuildBody();
        return new MainRoutine(identifier, body);
    }

    public void CheckNull(Tuple<TokenTypes, string?>? token, TokenTypes expected, string where)
    {
        if (token == null)
        {
            Console.WriteLine("Error in " + where + ". Expected " + expected + ": Found end of Tokens");
            Environment.Exit(0);
        }
    }
    
    public void CheckNull(Tuple<TokenTypes, string?>? token, string expected, string where)
    {
        if (token == null)
        {
            Console.WriteLine("Error in " + where + ". Expected " + expected + ": Found end of Tokens");
            Environment.Exit(0);
        }
    }

    public void CheckTokenMatch(TokenTypes token, TokenTypes expected, string where)
    {
        if (token != expected)
        {
            Console.WriteLine("Error in " + where + ". Expected " + expected + ": Found " + token + " Token");
            Environment.Exit(0);
        }
    }

    public Function? BuildFunction()
    {
        Type? type = BuildType();
        
        var nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.Identifiers, "BuildFunction");
        CheckTokenMatch(nextToken!.Item1, TokenTypes.Identifiers, "BuildFunction");

        string? name = nextToken.Item2;
        
        nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.ParenthesesL, "BuildFunction");
        CheckTokenMatch(nextToken!.Item1, TokenTypes.ParenthesesL, "BuildFunction");
        
        Parameters? parameters = BuildParameters();
        
        nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.ParenthesesR, "BuildFunction");
        CheckTokenMatch(nextToken!.Item1, TokenTypes.ParenthesesR, "BuildFunction");

        nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.Is, "BuildFunction");
        CheckTokenMatch(nextToken!.Item1, TokenTypes.Is, "BuildFunction");
        
        Identifier identifier = new Identifier(true, "Function", name);
        Body? body = BuildBody();
        RoutineReturnType? routineReturnType = new RoutineReturnType(type);
        RoutineInsights? routineInsights = new RoutineInsights(body);
        return new Function(identifier, parameters, routineReturnType, routineInsights);
    }

    public Parameters? BuildParameters()
    {
        ParameterDeclaration? parameterDeclaration = BuildParameterDeclaration();

        if (parameterDeclaration == null)
            return null;
        Parameters? parameters = BuildParameters();
        return new Parameters(parameterDeclaration, parameters);
    }

    public ParameterDeclaration? BuildParameterDeclaration()
    {
        Type? type = BuildType();
        if (type == null)
            return null;
        var nextToken = _tokens.GetNextToken();
        CheckNull(nextToken, TokenTypes.Identifiers, "BuildParameterDeclaration");
        CheckTokenMatch(nextToken!.Item1, TokenTypes.Identifiers, "BuildParameterDeclaration");
        Identifier? identifier = new Identifier(true, null, nextToken.Item2);

        return new ParameterDeclaration(identifier, type);
    }

    /// <summary>
    /// эта функция вызывается для BuildMainRoutineDeclaration (и возможно в будущем для BuildFunctionDeclaration). Она
    /// должна вернуть body согласно его структуре (Declaration - Statement - Body)
    /// </summary>
    /// <returns></returns>
    public Body? BuildBody()
    {
        Return? @return = null;
        Statement? statement = null;

        var token = _tokens.Current();
        // CheckNull(token, TokenTypes.Return, "BuildReturn");
        Declaration? declaration = BuildDeclaration();
        if (declaration == null)
        {
            @return = BuildReturn();
            if (@return == null)
            {
                if (_tokens.Current().Item1 != TokenTypes.End) 
                {
                    statement = BuildStatement(); // !there is no check for statement inside BuildStatement!
                    if (statement == null)
                    {
                        return null;
                    }
                }
                else
                {
                    // _tokens.GetNextToken();
                    return null;
                }
            }
        }
        Body? body = BuildBody();
        return new Body(declaration, statement, body, @return);
    }

    /// <summary>
    /// проверка на ретерн потом построение ретерна
    /// </summary>
    /// <returns></returns>
    public Return? BuildReturn()
    {
        CheckNull(_tokens.Current(), TokenTypes.Return, "BuildReturn");
        if (_tokens.Current()!.Item1 == TokenTypes.Return)
        {
            _tokens.GetNextToken();
            Expression? expression = BuildExpression();
            return new Return(expression);
        }

        return null;
    }
}

