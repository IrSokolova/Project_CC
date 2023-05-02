namespace ConsoleApp1.CodeGenerator.Exe;
using ConsoleApp1.SyntaxAnalyser;

public class FunProcessing
{
    private List<Function> _functions;
    public MainRoutine _mainRoutine;

    public FunProcessing()
    {
        _functions = new List<Function>();
    }
    
    public List<Function> FindFunctions(Action action)
    {
        Actions? actions = action._actions;
        while (actions != null)
        {
            ProcessAction(action);
            action = actions._action;
            actions = actions._actions;
        }
        ProcessAction(action);
        return _functions;
    }

    public void ProcessAction(Action action)
    {
        if (action._declaration != null)
        {
		    ProcessDeclaration(action._declaration);
        }
        else if (action._statement != null)
        {
            ProcessStmt(action._statement);
        }
    }

    public void ProcessDeclaration(Declaration declaration)
    {
        if (declaration._routineDeclaration != null)
        {
            RoutineDeclaration routineDeclaration = declaration._routineDeclaration;
            if (routineDeclaration._mainRoutine != null)
            {
                _mainRoutine = routineDeclaration._mainRoutine;
            }
            else if (routineDeclaration._function != null)
            {
                _functions.Add(routineDeclaration._function);
            }
        }
    }
    
    public void ProcessBody(Body body)
    {
        while (body != null)
        {
            if (body._declaration != null)
            {
                ProcessDeclaration(body._declaration);
            }
            else if (body._statement != null)
            {
                ProcessStmt(body._statement);
            }

            body = body._body;
        }
    }

    public void ProcessStmt(Statement statement)
    {
        if (statement._whileLoop != null)
        {
            ProcessBody(statement._whileLoop._body);
        }
        else if (statement._forLoop != null)
        {
            ProcessBody(statement._forLoop._body);
        }
        else if (statement._ifStatement != null)
        {
            ProcessBody(statement._ifStatement._ifBody);
            ProcessBody(statement._ifStatement._elseBody);
        }
    }
}