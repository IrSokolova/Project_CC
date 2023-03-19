namespace ConsoleApp1.SyntaxAnalyser;

public class Identifier
{
    // private bool _global;
    private bool _readOnly;
    private string? _type; // "Function" or type of variable
    private string? _name;

    public Identifier(bool readOnly, string? type, string? name)
    {
        _readOnly = readOnly;
        _type = type;
        _name = name;
    }

    public string ToString(string shift)
    {
        return "(" + _name + ") \n";
    }
}

public class Declaration
{
    private VariableDeclaration? _variableDeclaration;
    private TypeDeclaration? _typeDeclaration;
    private RoutineDeclaration? _routineDeclaration;

    public Declaration(VariableDeclaration? variableDeclaration, TypeDeclaration? typeDeclaration, RoutineDeclaration? routineDeclaration)
    {
        _variableDeclaration = variableDeclaration;
        _typeDeclaration = typeDeclaration;
        _routineDeclaration = routineDeclaration;
    }

    public string ToString(string shift)
    {
        string declarationToString = "──Declaration \n";
        shift += "   ";
        string variableDeclarationToString;
        string typeDeclarationToString;
        string routineDeclarationToString;
        
        if (_variableDeclaration != null)
        {
            variableDeclarationToString = _variableDeclaration.ToString(shift);
            return declarationToString + shift + "└" + variableDeclarationToString;
        }
        
        if (_typeDeclaration != null)
        {
            typeDeclarationToString = _typeDeclaration.ToString(shift);
            return declarationToString + shift + "└" + typeDeclarationToString;
        }
        
        if (_routineDeclaration != null)
        {
            routineDeclarationToString = _routineDeclaration.ToString(shift);
            return declarationToString + shift + "└" + routineDeclarationToString;
        }
        return "";
    }
}

public class VariableDeclaration
{
    private Identifier _identifier;
    private Type _type;
    private Value? _value;
    // private Expression? _expression;
    
    public VariableDeclaration(Identifier identifier, Type type, Value? value)
    {
        _identifier = identifier;
        _type = type;
        _value = value;
        // _expression = expression;
    }

    public string ToString(string shift)
    {
        string variableDeclarationToString = "──VariableDeclaration ";
        shift += "   ";
        
        string identifierToString;
        string typeToString;
        string valueToString;
        
        if (_value == null)
        {
            identifierToString = _identifier.ToString(shift);
            typeToString = _type.ToString(shift);
            return variableDeclarationToString + identifierToString + shift + "└" + typeToString;
        }
        
        identifierToString = _identifier.ToString(shift);
        typeToString = _type.ToString(shift);
        valueToString = _value.ToString(shift);
        return variableDeclarationToString + identifierToString + shift + "│" + typeToString + shift + "└" + valueToString;
    }
}

public class VariableDeclarations
{
    private VariableDeclaration _variableDeclaration;
    private VariableDeclarations _variableDeclarations;

    public VariableDeclarations(VariableDeclaration variableDeclaration, VariableDeclarations variableDeclarations)
    {
        _variableDeclaration = variableDeclaration;
        _variableDeclarations = variableDeclarations;
    }
}

public class TypeDeclaration
{
    private Identifier _identifier;
    private Type _type;
    
    public TypeDeclaration(Identifier identifier, Type type)
    {
        _identifier = identifier;
        _type = type;
    }

    public string ToString(string shift)
    {
        return "";
    }
}

public class RoutineDeclaration
{
    private MainRoutine? _mainRoutine;
    private Function? _function;

    public RoutineDeclaration(MainRoutine? mainRoutine, Function? function)
    {
        _mainRoutine = mainRoutine;
        _function = function;
    }

    public string ToString(string shift)
    {
        return "";
    }
}

public class MainRoutine
{
    private Identifier _identifier;
    private Body? _body;
    // private RoutineInsights _routineInsights;
    // private Parameters? _parameters;
    // private RoutineReturnType _routineReturnType;

    public MainRoutine(Identifier identifier, Body? body)
    {
        _identifier = identifier;
        _body = body;
    }
}
    
public class Function
{
    private Identifier _identifier;
    private Parameters? _parameters;
    private RoutineReturnType _routineReturnType;
    private RoutineInsights _routineInsights;

    public Function(Identifier identifier, Parameters? parameters, RoutineReturnType routineReturnType, RoutineInsights routineInsights)
    {
        _identifier = identifier;
        _parameters = parameters;
        _routineReturnType = routineReturnType;
        _routineInsights = routineInsights;
    }
}

public class Parameters
{
    private ParameterDeclaration _parameterDeclaration;
    private ParameterDeclarations? _parameterDeclarations;

    public Parameters(ParameterDeclaration parameterDeclaration, ParameterDeclarations? parameterDeclarations)
    {
        _parameterDeclaration = parameterDeclaration;
        _parameterDeclarations = parameterDeclarations;
    }
}

public class ParameterDeclaration // TYPE DEC IS THE SAME
{
    private Identifier _identifier;
    private Type _type;
    
    public ParameterDeclaration(Identifier identifier, Type type)
    {
        _identifier = identifier;
        _type = type;
    }
    
}

public class ParameterDeclarations
{
    private ParameterDeclaration _parameterDeclaration;
    private ParameterDeclarations? _parameterDeclarations;

    public ParameterDeclarations(ParameterDeclaration parameterDeclaration, ParameterDeclarations? parameterDeclarations)
    {
        _parameterDeclaration = parameterDeclaration;
        _parameterDeclarations = parameterDeclarations;
    }
}


public class Value
{
    private Expression _expression;

    public Value(Expression expression)
    {
        _expression = expression;
    }

    public string ToString(string shift)
    {
        shift += "   ";
        string expressionToString = _expression.ToString(shift);
        return "──Value \n" + shift + "└" + expressionToString;
    }
}


public class Expression
{
    private Relation _relation;
    private MultipleRelation? _multipleRelation;

    public Expression(Relation relation, MultipleRelation? multipleRelation)
    {
        _relation = relation;
        _multipleRelation = multipleRelation;
    }

    public string ToString(string shift)
    {
        string expressionToString = "──Expression \n";
        string relationToString;
        string actionsToString;

        if (_multipleRelation != null)
        {
            string shift1 = shift + "  │";
            string shift2 = shift + "  └";
            shift += "   ";
            relationToString = _relation.ToString(shift1);
            actionsToString = _multipleRelation.ToString(shift);
            return expressionToString + shift1 + relationToString + shift2 + actionsToString;
        }

        shift += "   ";
        relationToString = _relation.ToString(shift);
        return expressionToString + shift + "└" + relationToString;
    }
}

public class Expressions // WHY
{
    private Expression _expression;
    private Expressions _expressions;

    public Expressions(Expression expression, Expressions expressions)
    {
        _expression = expression;
        _expressions = expressions;
    }
}

public class Relation 
{
    private Operation? _operation;
    private Comparison? _comparison;

    public Relation(Operation? operation, Comparison? comparison)
    {
        _operation = operation;
        _comparison = comparison;
    }

    public string ToString(string shift)
    {
        string relationToString = "──Relation \n";
        string operationToString;
        string comparisonToString;

        if (_comparison != null)
        {
            string shift1 = shift + "  │";
            string shift2 = shift + "  └";
            shift += "   ";
            operationToString = _operation.ToString(shift1);
            comparisonToString = _comparison.ToString(shift);
            return relationToString + shift1 + operationToString + shift2 + comparisonToString;
        }

        shift += "   ";
        operationToString = _operation.ToString(shift);
        return relationToString + shift + "└" + operationToString;
    }
}

public class Operation
{
    private Operand _operand;
    private Operator? _operator;

    public Operation(Operand operand, Operator? @operator)
    {
        _operand = operand;
        _operator = @operator;
    }

    public string ToString(string shift)
    {
        string operationToString = "──Operation \n";
        string operandToString;
        string operatorToString;

        if (_operator != null)
        {
            string shift1 = shift + "  │";
            string shift2 = shift + "  └";
            shift += "   ";
            operandToString = _operand.ToString(shift1);
            operatorToString = _operator.ToString(shift);
            return operationToString + shift1 + operandToString + shift2 + operatorToString;
        }

        shift += "   ";
        operandToString = _operand.ToString(shift);
        return operationToString + shift + "└" + operandToString;
    }
}

public class Operand 
{
    private Single? _single;
    private Expression? _expression;

    public Operand(Single? single, Expression? expression)
    {
        _single = single;
        _expression = expression;
    }
    
    public string ToString(string shift)
    {
        string operandToString = "──Operand \n";
        string singleToString;
        string expressionToString;

        shift += "   ";
        if (_single != null)
        {
            singleToString = _single.ToString(shift);
            return operandToString + shift + "└" + singleToString;
        }

        if (_expression != null)
        {
            expressionToString = _expression.ToString(shift);
            return operandToString + shift + "└" + expressionToString;
        }

        return "";
    }
}

public class Comparison
{
    private Operator _operator; // (something before) less than...
    private Operation _operation; // then the operation

    public Comparison(Operator @operator, Operation operation)
    {
        _operator = @operator;
        _operation = operation;
    }

    public string ToString(string shift)
    {
        return "";
    }
}

public class Single
{
    private Type? _type;
    private float? _value;
    private Variable? _variable;
    // private bool _isNot;
    
    public Single(Type? type, float? value, Variable? variable)
    {
        _type = type;
        _value = value;
        _variable = variable;
    }

    public string ToString(string shift)
    {
        string singleToString = "──Single (";
        string typeToString;
        string valueToString;
        string variableToString;

        if (_type != null)
        {
            typeToString = _type.ToString();
            return singleToString + typeToString + ", " + _value + ") \n";
        }

        if (_variable != null)
        {
            variableToString = _variable.ToString(shift);
            return singleToString + variableToString + ") \n";
        }

        return "";
    }
}

public class Variable
{
    private Identifier _identifier;
    // private Identifiers _identifiers;
    
    public Variable(Identifier identifier)
    {
        _identifier = identifier;
    }

    public string ToString(string shift)
    {
        string identifierToString = _identifier.ToString(shift);
        return "Variable " + identifierToString;
    }
}

public class Operator
{
    private ComparisonOperator? _comparisonOperator;
    private MathematicalOperator? _mathematicalOperator;
    private LogicalOperator? _logicalOperator;  // maybe not needed

    public Operator(ComparisonOperator? comparisonOperator, MathematicalOperator? mathematicalOperator,
        LogicalOperator? logicalOperator)
    {
        _comparisonOperator = comparisonOperator;
        _mathematicalOperator = mathematicalOperator;
        _logicalOperator = logicalOperator;
    }

    public string ToString(string shift)
    {
        string operatorToString = "──Operator \n";
        string comparisonOperatorToString;
        string mathematicalOperatorToString;
        string logicalOperatorToString;

        shift += "   ";
        if (_comparisonOperator != null)
        {
            comparisonOperatorToString = _comparisonOperator.ToString(shift);
            return operatorToString + shift + "└" + comparisonOperatorToString;
        }

        if (_mathematicalOperator != null)
        {
            mathematicalOperatorToString = _mathematicalOperator.ToString(shift);
            return operatorToString + shift + "└" + mathematicalOperatorToString;
        }
        
        if (_logicalOperator != null)
        {
            logicalOperatorToString = _logicalOperator.ToString(shift);
            return operatorToString + shift + "└" + logicalOperatorToString;
        }

        return "";
    }
}

public class ComparisonOperator
{
    private string _sign;

    public ComparisonOperator(string sign)
    {
        _sign = sign;
    }

    public string ToString(string shift)
    {
        return "──ComparisonOperator (" + _sign + ") \n";
    }

}

public class MathematicalOperator
{
    private string _sign;

    public MathematicalOperator(string sign)
    {
        _sign = sign;
    }
    
    public string ToString(string shift)
    {
        return "──MathematicalOperator (" + _sign + ") \n";
    }
}

public class LogicalOperator
{
    private string _sign;

    public LogicalOperator(string sign)
    {
        _sign = sign;
    }
    
    public string ToString(string shift)
    {
        return "──LogicalOperator (" + _sign + ") \n";
    }
    
}

public class MultipleRelation
{
    private LogicalOperator _logicalOperator;
    private Relation _relation;
    private MultipleRelation? _multipleRelation;

    public MultipleRelation(Relation relation, MultipleRelation? multipleRelation, LogicalOperator logicalOperator)
    {
        _relation = relation;
        _multipleRelation = multipleRelation;
        _logicalOperator = logicalOperator;
    }

    public string ToString(string shift)
    {
        string multipleRelationToString = "──MultipleRelation \n";
        string relationToString;

        if (_multipleRelation != null)
        {
            string shift1 = shift + "  │";
            string shift2 = shift + "  └";
            shift += "   ";
            relationToString = _relation.ToString(shift1);
            var relationsToString = _multipleRelation.ToString(shift);
            return multipleRelationToString + shift1 + relationToString + shift2 + relationsToString;
        }

        shift += "   ";
        relationToString = _relation.ToString(shift);
        return multipleRelationToString + shift + "└" + relationToString;
    }
}

public class Type
{
    // private Identifier _identifier;
    
    private PrimitiveType? _primitiveType;
    private ArrayType? _arrayType;
    private RecordType? _recordType;

    public Type(PrimitiveType? primitiveType, ArrayType? arrayType, RecordType? recordType)
    {
        _primitiveType = primitiveType;
        _arrayType = arrayType;
        _recordType = recordType;
    }

    public string ToString(string shift)
    {

        string primitiveTypeToString;
        string arrayTypeToString;
        string recordTypeToString;

        if (_primitiveType != null)
        {
            primitiveTypeToString = _primitiveType.ToString();
            return "──Type (" + primitiveTypeToString + ") \n";
        }
        if (_arrayType != null)
        {
            return "──Type (Array) \n";
        }
        if (_recordType != null)
        {
            return "──Type (Record) \n";
        }

        return "";

    }
    
    public override string ToString()
    {

        string primitiveTypeToString;
        string arrayTypeToString;
        string recordTypeToString;

        if (_primitiveType != null)
        {
            primitiveTypeToString = _primitiveType.ToString();
            return primitiveTypeToString;
        }
        if (_arrayType != null)
        {
            return "Array";
        }
        if (_recordType != null)
        {
            return "Record";
        }

        return "";

    }
}

public class PrimitiveType
{
    private bool _isInt;
    private bool _isReal;
    private bool _isBoolean;

    public PrimitiveType(bool isInt, bool isReal, bool isBoolean)
    {
        _isInt = isInt;
        _isReal = isReal;
        _isBoolean = isBoolean;
    }

    public override string ToString()
    {
        if (_isInt)
            return "Integer";
        if (_isReal)
            return "Real";
        if (_isBoolean)
            return "Boolean";
        return "";
    }
}

public class ArrayType  // TODO IDONTUNDERSTAND
{
    private Expression _expression; 
    private Type _type;
    // size

    public ArrayType(Expression expression, Type type)
    {
        _expression = expression;
        _type = type;
    }
}

public class RecordType
{
    private VariableDeclaration _variableDeclaration;
    private VariableDeclarations? _variableDeclarations;

    public RecordType(VariableDeclaration variableDeclaration, VariableDeclarations? variableDeclarations)
    {
        _variableDeclaration = variableDeclaration;
        _variableDeclarations = variableDeclarations;
    }
}

public class Action
{
    private Declaration? _declaration;
    private Actions? _actions;

    public Action(Declaration? declaration, Actions? actions)
    {
        _declaration = declaration;
        _actions = actions;
    }

    public string ToString(string shift)
    {
        string actionToString = shift + "└──Action \n";
        string declarationToString;
        string actionsToString;

        if (_actions != null)
        {
            string shift1 = shift + "  │";
            string shift2 = shift + "  └";
            shift += "   ";
            declarationToString = _declaration.ToString(shift1);
            actionsToString = _actions.ToString(shift);
            return actionToString + shift1 + declarationToString + shift2 + actionsToString;
        }

        shift += "   ";
        declarationToString = _declaration.ToString(shift);
        return actionToString + shift + "└" + declarationToString;
    }
}

public class Actions
{
    private Action _action;
    private Actions? _actions;

    public Actions(Action action, Actions? actions)
    {
        _action = action;
        _actions = actions;
    }

    public string ToString(string shift)
    {
        return "";
    }
}

public class RoutineReturnType
{
    private Type _type;

    public RoutineReturnType(Type type)
    {
        _type = type;
    }
}

public class RoutineInsights
{
    private Body _body;

    public RoutineInsights(Body body)
    {
        _body = body;
    }
}

public class Return
{
    private Expression? _expression;

    public Return(Expression? expression)
    {
        _expression = expression;
    }
}

public class Body
{
    private Declaration? _declaration;
    private Statement? _statement;
    private Return? _return;
    private Body? _body;

    public Body(Declaration? declaration, Statement? statement, Body? body, Return? @return)
    {
        _declaration = declaration;
        _statement = statement;
        _body = body;
        _return = @return;
    }
}

public class Statement
{
    private Assignment? _assignment;
    private WhileLoop? _whileLoop;
    private ForLoop? _forLoop;
    private IfStatement? _ifStatement;
    private RoutineCall? _routineCall;

    public Statement(Assignment? assignment, WhileLoop? whileLoop, ForLoop? forLoop, IfStatement? ifStatement, RoutineCall? routineCall)
    {
        _assignment = assignment;
        _whileLoop = whileLoop;
        _forLoop = forLoop;
        _ifStatement = ifStatement;
        _routineCall = routineCall;
    }
}

public class Assignment
{
    private Variable _variable;
    private Expression _expression;

    public Assignment(Variable variable, Expression expression)
    {
        _variable = variable;
        _expression = expression;
    }
}

public class RoutineCall 
{
    private Identifier _identifier;
    private Expressions? _expressions; // MAYBE ACTION // TODO EXPLAIN

    public RoutineCall(Identifier identifier, Expressions? expressions)
    {
        _identifier = identifier;
        _expressions = expressions;
    }
}

public class WhileLoop
{
    private Expression _expression;
    private Body? _body;

    public WhileLoop(Expression expression, Body? body)
    {
        _expression = expression;
        _body = body;
    }
}

public class ForLoop
{
    // private string _name;
    private Identifier _identifier;
    private bool _reverse;
    private Range _range;
    private Body? _body;

    public ForLoop(Identifier identifier, bool reverse, Range range, Body? body)
    {
        _identifier = identifier;
        _reverse = reverse;
        _range = range;
        _body = body;
    }
}

public class Range
{
    private Expression _from;
    private Expression _to;

    public Range(Expression from, Expression to)
    {
        _from = from;
        _to = to;
    }
}

public class IfStatement
{
    private Expression _condition;
    private Body _ifBody;
    private Body _elseBody;
    // private ElseStatement _elseStatement;

    public IfStatement(Expression condition, Body ifBody, Body elseBody)
    {
        _condition = condition;
        _ifBody = ifBody;
        _elseBody = elseBody;
    }
}

// public class ElseStatement {private Body _Body;public ElseStatement(Body body){_Body = body;}}
