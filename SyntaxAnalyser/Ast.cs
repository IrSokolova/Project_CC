using ConsoleApp1.LexicalAnalyser;
using ConsoleApp1.SyntaxAnalyser;
using Action = ConsoleApp1.SyntaxAnalyser.Action;

namespace ConsoleApp1.SyntaxAnalyser;

public class Ast
{
    private List<Tuple<TokenTypes, string>> _lexicalAnalysis;
    private Action _action;
    
    public Ast(List<Tuple<TokenTypes, string>> lexicalAnalysis)
    {
        _lexicalAnalysis = lexicalAnalysis;
    }

    /// <summary>
    /// функция немного обрабатывает _lexicalAnalysis и вызывает BuildMainRoutineDeclaration, которая потом
    /// вызывает другие функции. Такая вот рекурсия
    /// </summary>
    /// <returns> AST. </returns>
    public Action ComposeTree()
    {
        // _action = new Action(null!, null!);
        for (int i = 0; i < _lexicalAnalysis.Count; i++)
        {
            var tokenType = _lexicalAnalysis[i].Item1;
            var token = _lexicalAnalysis[i].Item2;
            
            if (tokenType == TokenTypes.Routine)
            {
                // Routine actions
            }
        }
        foreach (var (tokenType, token) in _lexicalAnalysis)
        {
            if (tokenType == TokenTypes.Routine)
            {
                // Routine actions
            }
        }

        return null!;
    }

    /// <summary>
    /// routineInsights до вызова этой функции берутся из функции ExtractRoutineInsights, а эта функция
    /// вызывает BuildRoutineBody (которая собирает body для MainRoutineDeclaration), потом собирает 
    /// объявление main рутины и возвращает его.
    /// </summary>
    /// <param name="routineInsights"></param>
    /// <returns></returns>
    public Declaration BuildMainRoutineDeclaration(List<Tuple<TokenTypes, string>> routineInsights)
    {
        Identifier identifier = new Identifier(true, "Function", "main");
        Body body = BuildRoutineBody();

        MainRoutine? mainRoutine = new MainRoutine(identifier, body);
        RoutineDeclaration routineDeclaration = new RoutineDeclaration(mainRoutine, null);
        return new Declaration(null, null, routineDeclaration);
    }
    
    /// <summary>
    /// как BuildMainRoutineDeclaration только с параметрами и ретерном
    /// </summary>
    /// <param name="name"></param>
    /// <param name="parametersList"></param>
    /// <param name="returnType"></param>
    /// <param name="routineInsights"></param>
    /// <returns></returns>
    public Declaration BuildFunctionDeclaration(
        string name, 
        List<Tuple<TokenTypes, string>> parametersList, 
        TokenTypes returnType,
        List<Tuple<TokenTypes, string>> routineInsights)
    {
        Identifier identifier = new Identifier(true, "Function", name);
        Parameters? parameters = BuildParameters(parametersList);
        Type? type = BuildType(returnType);
        if (type == null)
        {
            Console.WriteLine("Error in BuildFunctionDeclaration");
            Environment.Exit(0);
        }
        RoutineReturnType routineReturnType = new RoutineReturnType(type);
        RoutineInsights insights = BuildRoutineInsights(routineInsights);

        Function function = new Function(identifier, parameters, routineReturnType, insights);
        RoutineDeclaration routineDeclaration = new RoutineDeclaration(null, function);
        return new Declaration(null, null, routineDeclaration);
    }

    /// <summary>
    /// эта функция вызывается для BuildMainRoutineDeclaration (и возможно в будущем для BuildFunctionDeclaration). Она
    /// должна вернуть body согласно его структуре (Declaration - Statement - Body)
    /// </summary>
    /// <returns></returns>
    public Body BuildRoutineBody()
    {
        return null!;
    }

    /// <summary>
    /// в этой функции можно собрать body функцией BuildRoutineBody, а потом придумать что делать с ретерном и
    /// собрать RoutineInsights согласно его структуре (Body - Return)
    /// </summary>
    /// <param name="routineInsights"></param>
    /// <returns></returns>
    public RoutineInsights BuildRoutineInsights(List<Tuple<TokenTypes, string>> routineInsights)
    {
        // private Body _body;
        // private Return _return; -> private Expression _expression;
        return null!;
    }

    /// <summary>
    /// эта функция делит lexicalAnalysis лист (аргумент) на 2 листа: 1) внутренности функции (после is и до end)
    /// 2) после end. Все должно работать, endы которые встречаются в if/loop/etc учтены
    /// </summary>
    /// <param name="lexicalAnalysis"></param>
    /// <returns> RoutineInsights (code between "is" and "end") and code after the end of the function. </returns>
    public (List<Tuple<TokenTypes, string>>?, List<Tuple<TokenTypes, string>>?) ExtractRoutineInsights(List<Tuple<TokenTypes, string>> lexicalAnalysis)
    {
        int numberOfEnds = 0;
        List<Tuple<TokenTypes, string>> routineInsights = new List<Tuple<TokenTypes, string>>();

        foreach (var pair in lexicalAnalysis)
        {
            if (pair.Item1 == TokenTypes.End && numberOfEnds == 0)
            {
                int zeroIndexOfRest = routineInsights.Count() + 1;
                List<Tuple<TokenTypes, string>> restOfCode = lexicalAnalysis.GetRange(
                    zeroIndexOfRest, lexicalAnalysis.Count() - zeroIndexOfRest);
                
                return (routineInsights, restOfCode);
            }
            
            routineInsights.Add(pair);
            switch (pair.Item1)
            {
                case TokenTypes.End:
                    numberOfEnds -= 1;
                    break;
                case TokenTypes.For:
                case TokenTypes.If:
                case TokenTypes.While:
                    numberOfEnds += 1;
                    break;
            }
        }
        
        Console.WriteLine("Error in ExtractRoutineInsights");
        Environment.Exit(0);
        return (null, null);
    }

    /// <summary>
    /// в эту функцию идет аргумент List(Tuple(TypeOfVar, NameOfVar)) (!не путать с оригинальным выводом
    /// в _lexicalAnalysis! его надо перебрать и вытащить из него переменные). Функция выводит null если параметров
    /// нет, в остальных случаях выводит объект с типом Parameters
    /// </summary>
    /// <param name="parametersList"></param>
    /// <returns></returns>
    public Parameters? BuildParameters(List<Tuple<TokenTypes, string>> parametersList)
    {
        if (parametersList.Count == 0)
        {
            return null;
        }
        if (parametersList.Count == 1)
        {
            ParameterDeclaration parameterDeclaration = BuildParameterDeclaration(parametersList[0].Item1, parametersList[0].Item2, true);
            return new Parameters(parameterDeclaration, null);
        }

        ParameterDeclaration declaration;
        ParameterDeclarations? declarations = null;
        for (int i = 1; i < parametersList.Count - 1; i++)
        {
            declaration = BuildParameterDeclaration(parametersList[i].Item1, parametersList[i].Item2, true);
            declarations = new ParameterDeclarations(declaration, declarations);
        }
        declaration = BuildParameterDeclaration(parametersList[^1].Item1, parametersList[^1].Item2, true);
        return new Parameters(declaration, declarations);
    }

    /// <summary>
    /// эта функция строит ParameterDeclaration из TypeOfVar, NameOfVar и readOnly. Это просто функция для упрощения
    /// кода
    /// </summary>
    /// <param name="tokenType"></param>
    /// <param name="token"></param>
    /// <param name="readOnly"></param>
    /// <returns></returns>
    public ParameterDeclaration BuildParameterDeclaration(TokenTypes tokenType, string token, bool readOnly)
    {
        Identifier identifier = new Identifier(readOnly, tokenType.ToString(), token);
        Type? type = BuildType(tokenType);
        if (type == null)
        {
            Console.WriteLine("Error in BuildParameterDeclaration");
            Environment.Exit(0);
        }
        return new ParameterDeclaration(identifier, type);
    }

    /// <summary>
    /// эта функция выводит null если что-то пошло не так. Иначе она находит нужный тип переменной и собирает переменную
    /// вида Type()
    /// </summary>
    /// <param name="tokenType"></param>
    /// <returns></returns>
    public Type? BuildType(TokenTypes tokenType)
    {
        switch (tokenType)
        {
            case TokenTypes.Integer:
            {
                PrimitiveType primitiveType = new PrimitiveType(true, false, false);
                return new Type(primitiveType, null, null);
            }
            case TokenTypes.Real:
            {
                PrimitiveType primitiveType = new PrimitiveType(false, true, false);
                return new Type(primitiveType, null, null);
            }
            case TokenTypes.Boolean:
            {
                PrimitiveType primitiveType = new PrimitiveType(false, false, true);
                return new Type(primitiveType, null, null);
            }
            case TokenTypes.Array: // TODO Array
                // ArrayType arrayType = new ArrayType();
                // return new Type(null, arrayType, null);
                break;
            case TokenTypes.Record: // TODO Record
                // RecordType recordType = new RecordType();
                // return new Type(null, null, recordType);
                break;
        }
        return null;
    }
}