﻿namespace DefaultNamespace.SyntaxAnalyser;

public class Identifier
{
    private bool _global;
    private bool _readOnly;
    private string _type; // "Function" or type of variable
    private string _name;

    public Identifier(bool global, bool readOnly, string type, string name)
    {
        _global = global;
        _readOnly = readOnly;
        _type = type;
        _name = name;
    }
}

public class Declaration
{
    private VariableDeclaration _variableDeclaration;
    private TypeDeclaration _typeDeclaration;
    private RoutineDeclaration _routineDeclaration;

    public Declaration(VariableDeclaration variableDeclaration, TypeDeclaration typeDeclaration, RoutineDeclaration routineDeclaration)
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
    private Identifier _identifier;
    private Parameters _parameters;
    private RoutineReturnType _routineReturnType;
    private RoutineInsights _routineInsights;

    public RoutineDeclaration(Identifier identifier, Parameters parameters)
    {
        _identifier = identifier;
        _parameters = parameters;
    }
}

public class Parameters
{
    private ParameterDeclaration _parameterDeclaration;
    private ParameterDeclarations _parameterDeclarations;

    public Parameters(ParameterDeclaration parameterDeclaration, ParameterDeclarations parameterDeclarations)
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
    private ParameterDeclarations _parameterDeclarations;

    public ParameterDeclarations(ParameterDeclaration parameterDeclaration, ParameterDeclarations parameterDeclarations)
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

public class Expressions
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

public class Operation // Simple
{
    private Operator _operator;
    private Operand _operand;
}

public class Operand // Summand
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
    private Modifiable _modifiable;
    // private bool _isNot;
    
    public Single(Type type, Value value, Modifiable modifiable)
    {
        _type = type;
        _value = value;
        _modifiable = modifiable;
        
        // if (sign) value *= sign->op == "-" ? -1 : 1;
        // this->value = std::to_string(value);
    }
}

public class Modifiable
{
    private Identifier _identifier;
    // private Identifiers _identifiers;
    
    public Modifiable(Identifier identifier)
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
    private Identifier _identifier;
    private PrimitiveType _primitivetype;
    private ArrayType _arraytype;
    private RecordType _recordtype;

    public Type(Identifier identifier, PrimitiveType primitivetype, ArrayType arraytype, RecordType recordtype)
    {
        _identifier = identifier;
        _primitivetype = primitivetype;
        _arraytype = arraytype;
        _recordtype = recordtype;
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
    private Return _return; // ReturnInRoutine

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
    private Declaration _declaration; // instead of SimpleDeclaration
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
    private Modifiable _modifiable;
    private Expression _expression;

    public Assignment(Modifiable modifiable, Expression expression)
    {
        _modifiable = modifiable;
        _expression = expression;
    }
}

public class RoutineCall 
{
    private Identifier _identifier;
    private Expressions _expressions; // ExpressionInRoutineCall IS DELETED 

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

// struct Identifiers {
//     string name;
//     struct Expression* expression;
//     struct Identifiers* identifiers;
//     Identifiers(string name, struct Expression* expression, struct Identifiers* identifiers):
//             name(name),
//             expression(expression),
//             identifiers(identifiers) {};
// };

// struct Simple {
//     struct Factor* factor;
//     struct Factors* factors;
//     Simple(Factor* factor, Factors* factors):
//         factor(factor), factors(factors) {};
// };
//
// struct Factor {
//     struct Summand* summand;
//     struct Summands* summands;
//     Factor(Summand* summand, Summands* summands):
//         summand(summand),
//     summands(summands) {};
// };
//
// struct Factors {
//     struct SimpleOperator* simpleOperator;
//     struct Factor* factor;
//     struct Factors* factors;
//     Factors(SimpleOperator* simpleOperator, Factor* factor, Factors* factors):
//         simpleOperator(simpleOperator),
//     factor(factor),
//     factors(factors) {};
// };
//
// struct Sign {
//     string op;
//     Sign(string op):
//         op(op) {};
// };
//
// struct SimpleOperator {
//     string op;
//     SimpleOperator(string op):
//         op(op) {};
// };
//
// struct Summands {
//     struct Sign* sign;
//     struct Summand* summand;
//     struct Summands* summands;
//     Summands(Sign* sign, Summand* summand, Summands* summands):
//         sign(sign),
//     summand(summand),
//     summands(summands) {};
// };