namespace ConsoleApp1.SyntaxAnalyser;

public class Identifier
{
    // private bool _global;
    private bool _readOnly;
    private string _type; // "Function" or type of variable
    private string _name;

    public Identifier(bool readOnly, string type, string name)
    {
        _readOnly = readOnly;
        _type = type;
        _name = name;
    }
}

public class Declaration
{
    private VariableDeclaration? _variableDeclaration;
    private TypeDeclaration? _typeDeclaration;
    private RoutineDeclaration? _routineDeclaration;

    public Declaration(VariableDeclaration? variableDeclaration, TypeDeclaration? typeDeclaration, RoutineDeclaration routineDeclaration)
    {
        _variableDeclaration = variableDeclaration;
        _typeDeclaration = typeDeclaration;
        _routineDeclaration = routineDeclaration;
    }
}

public class VariableDeclaration
{
    private Identifier _identifier;
    private Type _type;
    private Value _value;  // why we need Value if we can replace it with expression
    // private Expression _expression;
    
    public VariableDeclaration(Identifier identifier, Type type, Value value)
    {
        _identifier = identifier;
        _type = type;
        _value = value;
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
}

public class MainRoutine
{
    private Identifier _identifier;
    private Body _body;
    // private RoutineInsights _routineInsights;
    // private Parameters? _parameters;
    // private RoutineReturnType _routineReturnType;

    public MainRoutine(Identifier identifier, Body body)
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
}

public class Expression
{
    private Relation _relation;
    private MultipleRelation _multipleRelation;

    public Expression(Relation relation, MultipleRelation multipleRelation)
    {
        _relation = relation;
        _multipleRelation = multipleRelation;
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
    private Operation _operation;
    private Comparison _comparison;
}

public class Operation 
{
    private Operator _operator;
    private Operand _operand;
}

public class Operand 
{
    private Single _single;
    private Expression _expression;

    public Operand(Single single, Expression expression)
    {
        _single = single;
        _expression = expression;
    }
}

public class Comparison
{
    private Operator _operator; // (something before) less then...
    private Operation _operation; // then the operation

    public Comparison(Operator @operator, Operation operation)
    {
        _operator = @operator;
        _operation = operation;
    }
}

public class Single
{
    private Type _type;
    private Value _value;
    private Variable _variable;
    // private bool _isNot;
    
    public Single(Type type, Value value, Variable variable)
    {
        _type = type;
        _value = value;
        _variable = variable;
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
}

public class Operator
{
    private ComparisonOperator _comparisonOperator;
    private MathematicalOperator _mathematicalOperator;
    private LogicalOperator _logicalOperator;  // maybe not needed

    public Operator(ComparisonOperator comparisonOperator, MathematicalOperator mathematicalOperator, LogicalOperator logicalOperator)
    {
        _comparisonOperator = comparisonOperator;
        _mathematicalOperator = mathematicalOperator;
        _logicalOperator = logicalOperator;
    }
}

public class ComparisonOperator
{
    private string _sign;

    public ComparisonOperator(string sign)
    {
        _sign = sign;
    }
}

public class MathematicalOperator
{
    private string _sign;

    public MathematicalOperator(string sign)
    {
        _sign = sign;
    }
}

public class LogicalOperator
{
    private string _sign;

    public LogicalOperator(string sign)
    {
        _sign = sign;
    }
}

public class MultipleRelation
{
    private LogicalOperator _logicalOperator;
    private Relation _relation;
    private MultipleRelation _multipleRelation;

    public MultipleRelation(Relation relation, MultipleRelation multipleRelation, LogicalOperator logicalOperator)
    {
        _relation = relation;
        _multipleRelation = multipleRelation;
        _logicalOperator = logicalOperator;
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
    private VariableDeclarations _variableDeclarations;

    public RecordType(VariableDeclaration variableDeclaration, VariableDeclarations variableDeclarations)
    {
        _variableDeclaration = variableDeclaration;
        _variableDeclarations = variableDeclarations;
    }
}

public class Action
{
    private Declaration _declaration;
    private Actions _actions;

    public Action(Declaration declaration, Actions actions)
    {
        _declaration = declaration;
        _actions = actions;
    }
}

public class Actions
{
    private Action _action;
    private Actions _actions;

    public Actions(Action action, Actions actions)
    {
        _action = action;
        _actions = actions;
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
    private Return _return;

    public RoutineInsights(Body body, Return @return)
    {
        _body = body;
        _return = @return;
    }
}

public class Return
{
    private Expression _expression;

    public Return(Expression expression)
    {
        _expression = expression;
    }
}

public class Body
{
    private Declaration _declaration;
    private Statement _statement;
    private Body _body;

    public Body(Declaration declaration, Statement statement, Body body)
    {
        _declaration = declaration;
        _statement = statement;
        _body = body;
    }
}

public class Statement
{
    private Assignment _assignment;
    private WhileLoop _whileLoop;
    private ForLoop _forLoop;
    private IfStatement _ifStatement;
    private RoutineCall _routineCall;

    public Statement(Assignment assignment, WhileLoop whileLoop, ForLoop forLoop, IfStatement ifStatement, RoutineCall routineCall)
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
    private Expressions _expressions; // MAYBE ACTION // TODO EXPLAIN

    public RoutineCall(Identifier identifier, Expressions expressions)
    {
        _identifier = identifier;
        _expressions = expressions;
    }
}

public class WhileLoop
{
    private Expression _expression;
    private Body _body;

    public WhileLoop(Expression expression, Body body)
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
    private Body _body;

    public ForLoop(bool reverse, Range range, Body body)
    {
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
