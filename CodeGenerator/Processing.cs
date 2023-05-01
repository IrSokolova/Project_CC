namespace ConsoleApp1.CodeGenerator.Exe;
using ConsoleApp1.SyntaxAnalyser;

public class Processing
{
    private List<TypeDeclaration> _records;

    public Processing()
    {
        _records = new List<TypeDeclaration>();
    }
    
    public List<TypeDeclaration> FindRecords(Action action)
    {
        Actions? actions = action._actions;
        while (actions != null)
        {
            ProcessAction(action);
            action = actions._action;
            actions = actions._actions;
        }
        ProcessAction(action);
        return _records;
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
        if (declaration._typeDeclaration != null)
        {
            TypeDeclaration typeDeclaration = declaration._typeDeclaration;
            if (typeDeclaration._type._recordType != null)
            {
                _records.Add(typeDeclaration);
            }
        }
        else if (declaration._routineDeclaration != null)
        {
            RoutineDeclaration routineDeclaration = declaration._routineDeclaration;
            if (routineDeclaration._mainRoutine != null)
            {
                ProcessBody(routineDeclaration._mainRoutine._body);
            }
            else if (routineDeclaration._function != null)
            {
                ProcessBody(routineDeclaration._function._routineInsights._body);
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