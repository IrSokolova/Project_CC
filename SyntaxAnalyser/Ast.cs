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

        return null;
    }

    public Declaration BuildRoutineDeclaration(
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
            Console.WriteLine("Error in BuildParameters");
            Environment.Exit(0);
        }
        RoutineReturnType routineReturnType = new RoutineReturnType(type);
        RoutineInsights insights = BuildRoutineInsights(routineInsights);

        RoutineDeclaration routineDeclaration = new RoutineDeclaration(identifier, parameters, routineReturnType, insights);
        return new Declaration(null, null, routineDeclaration);
    }

    public RoutineInsights BuildRoutineInsights(List<Tuple<TokenTypes, string>> routineInsights)
    {
        
    }

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

    public ParameterDeclaration BuildParameterDeclaration(TokenTypes tokenType, string token, bool readOnly)
    {
        Identifier identifier = new Identifier(readOnly, tokenType.ToString(), token);
        Type? type = BuildType(tokenType);
        if (type == null)
        {
            Console.WriteLine("Error in BuildParameters");
            Environment.Exit(0);
        }
        return new ParameterDeclaration(identifier, type);
    }

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