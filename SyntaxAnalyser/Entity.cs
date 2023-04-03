using DefaultNamespace.SemantycalAnalyser;

namespace ConsoleApp1.SyntaxAnalyser;

public class Identifier
{
    // private bool _global;
    public bool _readOnly;
    public string? _type; // "Function" or type of variable
    public string? _name;

    public Identifier(bool readOnly, string? type, string? name)
    {
        _readOnly = readOnly;
        _type = type;
        _name = name;
    }
    
    public string Accept(Visitor.IdentifierVisitor identifierVisitor)
    {
        identifierVisitor.Visit(this);
        return _type.ToString();
    }

    public override string ToString()
    {
        return _name;
    }
    
    public string ToString(string shift)
    {
        return "──Identifier(" + _name + ") \n";
    }
}

public class Declaration
{
    public VariableDeclaration? _variableDeclaration;
    public TypeDeclaration? _typeDeclaration;
    public RoutineDeclaration? _routineDeclaration;

    public Declaration(VariableDeclaration? variableDeclaration, TypeDeclaration? typeDeclaration, RoutineDeclaration? routineDeclaration)
    {
        _variableDeclaration = variableDeclaration;
        _typeDeclaration = typeDeclaration;
        _routineDeclaration = routineDeclaration;
    }
    
    public void Accept(Visitor.DeclarationVisitor declarationVisitor)
    {
        declarationVisitor.Visit(this);
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
    public Identifier _identifier;
    public Type _type;
    public Value? _value;
    // private Expression? _expression;
    
    public VariableDeclaration(Identifier identifier, Type type, Value? value)
    {
        _identifier = identifier;
        _type = type;
        _value = value;
        // _expression = expression;
    }
    
    public string Accept(Visitor.VariableDeclarationVisitor declarationVisitor)
    {
        return declarationVisitor.Visit(this);
    }

    public string ToString(string shift)
    {
        string variableDeclarationToString = "──VariableDeclaration ";
        shift += "   ";
        string shift1 = shift + "  │";
        string identifierToString = _identifier.ToString();
        string typeToString;
        string valueToString;
        
        if (_value == null)
        {
            typeToString = _type.ToString(shift);
            return variableDeclarationToString + "(" + identifierToString + ") \n" + shift + "└" + typeToString;
        }
        
        typeToString = _type.ToString(shift1);
        valueToString = _value.ToString(shift);
        return variableDeclarationToString + "(" + identifierToString + ") \n" + shift + "│" + typeToString + shift + "└" + valueToString;
    }
}

public class VariableDeclarations
{
    public VariableDeclaration _variableDeclaration;
    public VariableDeclarations? _variableDeclarations;

    public VariableDeclarations(VariableDeclaration variableDeclaration, VariableDeclarations? variableDeclarations)
    {
        _variableDeclaration = variableDeclaration;
        _variableDeclarations = variableDeclarations;
    }
    
    public string Accept(Visitor.VariableDeclarationsVisitor declarationVisitor)
    {
       return declarationVisitor.Visit(this);
    }
    
    public string ToString(string shift)
    {
        string variableDeclarationsToString = "──VariableDeclarations \n";
        string variableDeclarationToString;
        string nextVariableDeclarationsToString;
        
        shift += "   ";
        if (_variableDeclarations != null)
        {
            string shift1 = shift + "  │";
            string shift2 = shift + "  └";
            variableDeclarationToString = _variableDeclaration.ToString(shift1);
            nextVariableDeclarationsToString = _variableDeclarations.ToString(shift);
            return variableDeclarationsToString + shift1 + variableDeclarationToString + shift2 + nextVariableDeclarationsToString;
        }
        
        variableDeclarationToString = _variableDeclaration.ToString(shift);
        return variableDeclarationsToString + shift + "└" + variableDeclarationToString;
    }
}

public class TypeDeclaration
{
    public Identifier _identifier;
    public Type _type;
    
    public TypeDeclaration(Identifier identifier, Type type)
    {
        _identifier = identifier;
        _type = type;
    }
    
    public void Accept(Visitor.TypeDeclarationVisitor declarationVisitor)
    {
        declarationVisitor.Visit(this);
    }

    public string ToString(string shift)
    {
        string typeDeclarationToString = "──TypeDeclaration \n";
        string shift1 = shift + "  │";
        string shift2 = shift + "  └";
        shift += "   ";
;       string identifierToString = _identifier.ToString(shift1);
        string typeToString = _type.ToString(shift);

        return typeDeclarationToString + shift1 + identifierToString + shift2 + typeToString;
    }
}

public class RoutineDeclaration
{
    public MainRoutine? _mainRoutine;
    public Function? _function;

    public RoutineDeclaration(MainRoutine? mainRoutine, Function? function)
    {
        _mainRoutine = mainRoutine;
        _function = function;
    }
    
    public void Accept(Visitor.RoutineDeclarationVisitor declarationVisitor)
    {
        declarationVisitor.Visit(this);
    }

    public string ToString(string shift)
    {
        string routineDeclarationToString = "──RoutineDeclaration \n";
        string mainRoutineToString;
        string functionToString;
    
        shift += "   ";
        if(_mainRoutine != null)
        {
            mainRoutineToString = _mainRoutine.ToString(shift);
            return routineDeclarationToString + shift + "└" + mainRoutineToString;
        }
    
        if(_function != null)
        {
            functionToString = _function.ToString(shift);
            return routineDeclarationToString + shift + "└" + functionToString;
        }

        return routineDeclarationToString;
    }
}

public class MainRoutine
{
    public Identifier _identifier;
    public Body? _body;

    public MainRoutine(Identifier identifier, Body? body)
    {
        _identifier = identifier;
        _body = body;
    }
    
    public void Accept(Visitor.MainRoutineVisitor mainRoutineVisitor)
    {
        mainRoutineVisitor.Visit(this);
    }
    
    public string ToString(string shift)
    {
        string mainRoutineToString = "──MainRoutine \n";
        string identifierToString;
        string bodyToString;
        
        shift += "   ";
        if (_body != null)
        {
            string shift1 = shift + "  │";
            string shift2 = shift + "  └";
            identifierToString = _identifier.ToString(shift1);
            bodyToString = _body.ToString(shift);
            return mainRoutineToString + shift1 + identifierToString + shift2 + bodyToString;
        }
        identifierToString = _identifier.ToString(shift);
        return mainRoutineToString + shift + "└" + identifierToString;
    }
}
    
public class Function
{
    public Identifier _identifier;
    public Parameters? _parameters;
    public RoutineReturnType _routineReturnType;
    public RoutineInsights _routineInsights;

    public Function(Identifier identifier, Parameters? parameters, RoutineReturnType routineReturnType, RoutineInsights routineInsights)
    {
        _identifier = identifier;
        _parameters = parameters;
        _routineReturnType = routineReturnType;
        _routineInsights = routineInsights;
    }
    
    public void Accept(Visitor.FunctionVisitor functionVisitor)
    {
        functionVisitor.Visit(this);
    }

    public string ToString(string shift)
    {
        string functionToString = "──Function \n";
        string identifierToString;
        string parametersToString;

        string routineReturnTypeToString;
        string routineInsightsToString;

        shift += "   ";
        string shift1 = shift + "  │";
        string shift2 = shift + "  └";
        
        identifierToString = _identifier.ToString(shift1);
        routineReturnTypeToString = _routineReturnType.ToString(shift1);
        routineInsightsToString = _routineInsights.ToString(shift);
        
        if (_parameters != null)
        {
            parametersToString = _parameters.ToString(shift1);
            return functionToString + shift1 + identifierToString + shift1 + parametersToString + shift1 +
                   routineReturnTypeToString + shift2 + routineInsightsToString;
        }
        
        return functionToString + shift1 + identifierToString + shift1 + routineReturnTypeToString + shift2 +
               routineInsightsToString;
    }
}

public class Parameters
{
    public ParameterDeclaration? _parameterDeclaration;
    public Parameters? _parameters;

    public Parameters(ParameterDeclaration? parameterDeclaration, Parameters? parameters)
    {
        _parameterDeclaration = parameterDeclaration;
        _parameters = parameters;
    }
    
    public void Accept(Visitor.ParametersVisitor parametersVisitor)
    {
        parametersVisitor.Visit(this);
    }
    
    public string ToString(string shift)
    {
        string parametersToString = "──Parameters \n";
        string parameterDeclarationToString;
        string nextParametersToString;
        
        shift += "   ";
        if (_parameters != null)
        {
            string shift1 = shift + "  │";
            string shift2 = shift + "  └";
            parameterDeclarationToString = _parameterDeclaration.ToString(shift1);
            nextParametersToString = _parameters.ToString(shift);
            return parametersToString + shift1 + parameterDeclarationToString + shift2 + nextParametersToString;
        }
        
        parameterDeclarationToString = _parameterDeclaration.ToString(shift);
        return parametersToString + shift + "└" + parameterDeclarationToString;
    }
}

public class ParameterDeclaration // TYPE DEC IS THE SAME
{
    public Identifier _identifier;
    public Type _type;
    
    public ParameterDeclaration(Identifier identifier, Type type)
    {
        _identifier = identifier;
        _type = type;
    }
    
    public void Accept(Visitor.ParameterDeclarationVisitor declarationVisitor)
    {
        declarationVisitor.Visit(this);
    }

    public string ToString(string shift)
    {
        string parameterDeclarationToString = "──ParameterDeclaration (";
        string identifierToString = _identifier.ToString();
        string typeToString = _type.ToString();

        return parameterDeclarationToString + identifierToString + ", " + typeToString + ") \n";
    }
    
}


public class Value
{
    public Expression _expression;

    public Value(Expression expression)
    {
        _expression = expression;
    }
    
    public Type Accept(Visitor.ValueVisitor valueVisitor)
    {
        return valueVisitor.Visit(this);
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
    public Relation _relation;
    public MultipleRelation? _multipleRelation;

    public Expression(Relation relation, MultipleRelation? multipleRelation)
    {
        _relation = relation;
        _multipleRelation = multipleRelation;
    }
    
    public Type Accept(Visitor.ExpressionVisitor expressionVisitor)
    {
        return expressionVisitor.Visit(this);
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
    public Expression _expression;
    public Expressions? _expressions;

    public Expressions(Expression expression, Expressions? expressions)
    {
        _expression = expression;
        _expressions = expressions;
    }
    
    public void Accept(Visitor.ExpressionsVisitor expressionsVisitor)
    {
        expressionsVisitor.Visit(this);
    }

    public string ToString(string shift)
    {
        string expressionsToString = "──Expressions \n";
        string expressionToString;
        string nextExpressionsToString;
        
        shift += "   ";
        if (_expressions != null)
        {
            string shift1 = shift + "  │";
            string shift2 = shift + "  └";
            expressionToString = _expression.ToString(shift1);
            nextExpressionsToString = _expressions.ToString(shift);
            return expressionsToString + shift1 + expressionToString + shift2 + nextExpressionsToString;
        }
        
        expressionToString = _expression.ToString(shift);
        return expressionsToString + shift + "└" + expressionToString;
    }
}

public class Relation 
{
    public Operation? _operation;
    public Comparison? _comparison;

    public Relation(Operation? operation, Comparison? comparison)
    {
        _operation = operation;
        _comparison = comparison;
    }
    
    public Type Accept(Visitor.RelationVisitor relationVisitor)
    {
        return relationVisitor.Visit(this);
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
    public Operand _operand;
    public Operator? _operator;
    public Operation? _operation;

    public Operation(Operand operand, Operator? @operator, Operation? operation)
    {
        _operand = operand;
        _operator = @operator;
        _operation = operation;
    }
    
    public Type Accept(Visitor.OperationVisitor operationVisitor)
    {
        return operationVisitor.Visit(this);
    }

    public string ToString(string shift)
    {
        string operationToString = "──Operation \n";
        string operandToString;
        string operatorToString;
        string nextOperationToString;

        if (_operator != null)
        {
            string shift1 = shift + "  │";
            string shift2 = shift + "  └";
            shift += "   ";
            operandToString = _operand.ToString(shift1);
            operatorToString = _operator.ToString(shift1);
            nextOperationToString = _operation.ToString(shift);
            return operationToString + shift1 + operandToString + shift1 + operatorToString + shift2 +
                   nextOperationToString;
        }

        shift += "   ";
        operandToString = _operand.ToString(shift);
        return operationToString + shift + "└" + operandToString;
    }
}

public class Operand 
{
    public Single? _single;
    public Expression? _expression;

    public Operand(Single? single, Expression? expression)
    {
        _single = single;
        _expression = expression;
    }
    
    public Type Accept(Visitor.OperandVisitor operandVisitor)
    {
        return operandVisitor.Visit(this);
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
    public Operator _operator; // (something before) less than...
    public Operation _operation; // then the operation

    public Comparison(Operator @operator, Operation operation)
    {
        _operator = @operator;
        _operation = operation;
    }
    
    public Type Accept(Visitor.ComparisonVisitor comparisonVisitor)
    {
       return comparisonVisitor.Visit(this);
    }

    public string ToString(string shift)
    {
        string comparisonToString = "──Comparison \n";
        string operatorToString;
        string operationToString;
        
        shift += "   ";
        string shift1 = shift + "  │";
        string shift2 = shift + "  └";
        operatorToString = _operator.ToString(shift1);
        operationToString = _operation.ToString(shift);
        return comparisonToString + shift1 + operatorToString + shift2 + operationToString;
    }
}

public class Single
{
    public Type? _type;
    public string? _value;
    public Variable? _variable;

    public Single(Type? type, string? value, Variable? variable)
    {
        _type = type;
        _value = value;
        _variable = variable;
    }
    
    public void Accept(Visitor.SingleVisitor singleVisitor)
    {
        singleVisitor.Visit(this);
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
            variableToString = _variable.ToString();
            return singleToString + variableToString + ") \n";
        }

        return "";
    }
}

public class Variable
{
    public Identifier _identifier;
    public Type? _arrayType;

    public Variable(Identifier identifier, Type? arrayType)
    {
        _identifier = identifier;
        _arrayType = arrayType;
    }
    
    public void Accept(Visitor.VariableVisitor variableVisitor)
    {
        variableVisitor.Visit(this);
    }

    public string ToString(string shift)
    {
        string identifierToString = _identifier.ToString();
        return "──Variable (" + identifierToString + ") \n";
    }
    
    public override string ToString()
    {
        string identifierToString = _identifier.ToString();
        return "Variable (" + identifierToString + ")";
    }
}

public class Operator
{
    public ComparisonOperator? _comparisonOperator;
    public MathematicalOperator? _mathematicalOperator;
    public LogicalOperator? _logicalOperator;  // maybe not needed

    public Operator(ComparisonOperator? comparisonOperator, MathematicalOperator? mathematicalOperator,
        LogicalOperator? logicalOperator)
    {
        _comparisonOperator = comparisonOperator;
        _mathematicalOperator = mathematicalOperator;
        _logicalOperator = logicalOperator;
    }
    
    public Type Accept(Visitor.OperatorVisitor operatorVisitor)
    {
        return operatorVisitor.Visit(this);
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
    public string _sign;

    public ComparisonOperator(string sign)
    {
        _sign = sign;
    }
    
    public void Accept(Visitor.ComparisonOperatorVisitor comparisonOperatorVisitor)
    {
        comparisonOperatorVisitor.Visit(this);
    }

    public string ToString(string shift)
    {
        return "──ComparisonOperator (" + _sign + ") \n";
    }

}

public class MathematicalOperator
{
    public string _sign;

    public MathematicalOperator(string sign)
    {
        _sign = sign;
    }
    
    public void Accept(Visitor.MathematicalOperatorVisitor mathematicalOperatorVisitor)
    {
        mathematicalOperatorVisitor.Visit(this);
    }
    
    public string ToString(string shift)
    {
        return "──MathematicalOperator (" + _sign + ") \n";
    }
}

public class LogicalOperator
{
    public string _sign;

    public LogicalOperator(string sign)
    {
        _sign = sign;
    }
    
    public void Accept(Visitor.LogicalOperatorVisitor logicalOperatorVisitor)
    {
        logicalOperatorVisitor.Visit(this);
    }
    
    public string ToString(string shift)
    {
        return "──LogicalOperator (" + _sign + ") \n";
    }
    
}

public class MultipleRelation
{
    public Relation _relation;
    public MultipleRelation? _multipleRelation;

    public MultipleRelation(Relation relation, MultipleRelation? multipleRelation)
    {
        _relation = relation;
        _multipleRelation = multipleRelation;
    }
    
    public void Accept(Visitor.MultipleRelationVisitor multipleRelationVisitor)
    {
        multipleRelationVisitor.Visit(this);
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
    
    public PrimitiveType? _primitiveType;
    public ArrayType? _arrayType;
    public RecordType? _recordType;

    public Type(PrimitiveType? primitiveType, ArrayType? arrayType, RecordType? recordType)
    {
        _primitiveType = primitiveType;
        _arrayType = arrayType;
        _recordType = recordType;
    }

    public bool Equals(Type t)
    {
        return _primitiveType == t._primitiveType && _arrayType == t._arrayType &&
                _recordType == t._recordType;
    }
    
    public string Accept(Visitor.TypeVisitor typeVisitor)
    {
        return typeVisitor.Visit(this);
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
            string shift1 = shift + "  └";
            shift += "   ";
            arrayTypeToString = _arrayType.ToString(shift);
            return "──Type \n" + shift1 + arrayTypeToString;
        }
        if (_recordType != null)
        {
            string shift1 = shift + "  └";
            shift += "   ";
            recordTypeToString = _recordType.ToString(shift);
            return "──Type \n" + shift1 + recordTypeToString;
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
    public bool _isInt;
    public bool _isReal;
    public bool _isBoolean;

    public PrimitiveType(bool isInt, bool isReal, bool isBoolean)
    {
        _isInt = isInt;
        _isReal = isReal;
        _isBoolean = isBoolean;
    }
    
    public string Accept(Visitor.PrimitiveTypeVisitor primitiveTypeVisitor)
    {
        return primitiveTypeVisitor.Visit(this);
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

public class ArrayType 
{
    public Expression _expression; 
    public Type _type;

    public ArrayType(Expression expression, Type type)
    {
        _expression = expression;
        _type = type;
    }
    
    public string Accept(Visitor.ArrayTypeVisitor arrayTypeVisitor)
    {
        return arrayTypeVisitor.Visit(this);
    }
    
    public string ToString(string shift)
    {
        string arrayTypeToString = "──ArrayType \n";
        string typeToString;
        string expressionToString;
        
        string shift1 = shift + "  │";
        string shift2 = shift + "  └";
        shift += "   ";
        typeToString = _type.ToString(shift1);
        expressionToString = _expression.ToString(shift);
        return arrayTypeToString + shift1 + typeToString + shift2 + expressionToString;
    }
}

public class RecordType
{
    public VariableDeclaration _variableDeclaration;
    public VariableDeclarations? _variableDeclarations;

    public RecordType(VariableDeclaration variableDeclaration, VariableDeclarations? variableDeclarations)
    {
        _variableDeclaration = variableDeclaration;
        _variableDeclarations = variableDeclarations;
    }
    
    public string Accept(Visitor.RecordTypeVisitor recordTypeVisitor)
    {
        return recordTypeVisitor.Visit(this);
    }
    
    public string ToString(string shift)
    {
        string recordTypeToString = "──RecordType \n";
        string variableDeclarationToString;
        string nextVariableDeclarationsToString;
        
        shift += "   ";
        if (_variableDeclarations != null)
        {
            string shift1 = shift + "  │";
            string shift2 = shift + "  └";
            variableDeclarationToString = _variableDeclaration.ToString(shift1);
            nextVariableDeclarationsToString = _variableDeclarations.ToString(shift);
            return recordTypeToString + shift1 + variableDeclarationToString + shift2 + nextVariableDeclarationsToString;
        }
        
        variableDeclarationToString = _variableDeclaration.ToString(shift);
        return recordTypeToString + shift + "└" + variableDeclarationToString;
    }
}

public class Action
{
    public Declaration? _declaration;
    public Statement? _statement;
    public Actions? _actions;

    public Action(Declaration? declaration, Statement? statement, Actions? actions)
    {
        _statement = statement;
        _declaration = declaration;
        _actions = actions;
    }

    public void Accept(Visitor.ActionVisitor actionVisitor)
    {
        actionVisitor.Visit(this);
    }
    
    public string ToString(string shift)
    {
        string actionToString = "──Action \n";
        string declarationToString;
        string statementToString;
        string actionsToString;
        
        if (_actions != null)
        {
            string shift1 = shift + "  │";
            string shift2 = shift + "  └";
            shift += "   ";
            if(_declaration != null)
            {
                declarationToString = _declaration.ToString(shift1);
                actionsToString = _actions.ToString(shift);
                return actionToString + shift1 + declarationToString + shift2 + actionsToString;
            }
            if (_statement != null)
            {
                statementToString = _statement.ToString(shift1); // TODO UNCOMMENT
                actionsToString = _actions.ToString(shift);
                return actionToString + shift1 + statementToString + shift2 + actionsToString;
            }
        }
        if(_declaration != null)
        {
            shift += "   ";
            declarationToString = _declaration.ToString(shift);
            return actionToString + shift + "└" + declarationToString;
        }
        if(_statement != null)
        {
            shift += "   ";
            statementToString = _statement.ToString(shift);  // TODO UNCOMMENT
            return actionToString + shift + "└" + statementToString;
        }

        return actionToString;
    }
}

public class Actions
{
    public Action _action;
    public Actions? _actions;

    public Actions(Action action, Actions? actions)
    {
        _action = action;
        _actions = actions;
    }
    
    public void Accept(Visitor.ActionsVisitor actionsVisitor)
    {
        actionsVisitor.Visit(this);
    }

    public string ToString(string shift)
    {
        string actionsToString = "──Actions \n";
        string actionToString;
        string nextActionsToString;
        
        shift += "   ";
        if (_actions != null)
        {
            string shift1 = shift + "  │";
            string shift2 = shift + "  └";
            actionToString = _action.ToString(shift1);
            nextActionsToString = _actions.ToString(shift);
            return actionsToString + shift1 + actionToString + shift2 + nextActionsToString;
        }
        
        actionToString = _action.ToString(shift);
        return actionsToString + shift + "└" + actionToString;
    }
}

public class RoutineReturnType
{
    public Type? _type;

    public RoutineReturnType(Type? type)
    {
        _type = type;
    }
    
    public void Accept(Visitor.RoutineReturnTypeVisitor routineReturnTypeVisitor)
    {
        routineReturnTypeVisitor.Visit(this);
    }
    
    public string ToString(string shift)
    {
        string routineReturnTypeToString = "──RoutineReturnType (";
        string typeToString;

        typeToString = _type.ToString();
        return routineReturnTypeToString + typeToString + ") \n";
    }
}

public class RoutineInsights
{
    public Body? _body;

    public RoutineInsights(Body? body)
    {
        _body = body;
    }
    
    public Type Accept(Visitor.RoutineInsightsVisitor routineInsightsVisitor)
    {
        return routineInsightsVisitor.Visit(this);
    }

    public string ToString(string shift)
    {
        string routineInsightsToString = "──RoutineInsights \n";
        string bodyToString;
        
        shift += "   ";
        if (_body != null)
        {
            string shift1 = shift + "  └";
            bodyToString = _body.ToString(shift);
            return routineInsightsToString + shift1 + bodyToString;
        }
        return routineInsightsToString;
    }
}

public class Return
{
    public Expression? _expression;

    public Return(Expression? expression)
    {
        _expression = expression;
    }
    
    public Type Accept(Visitor.ReturnVisitor returnVisitor)
    {
        return returnVisitor.Visit(this);
    }
    
    public string ToString(string shift)
    {
        string returnToString = "──Return \n";
        shift += "   ";
        string expressionToString = _expression.ToString(shift);
        return returnToString + shift + "└" + expressionToString;
    }
}

public class Body
{
    public Declaration? _declaration;
    public Statement? _statement;
    public Return? _return;
    public Body? _body;

    public Body(Declaration? declaration, Statement? statement, Body? body, Return? @return)
    {
        _declaration = declaration;
        _statement = statement;
        _body = body;
        _return = @return;
    }
    
    public Type Accept(Visitor.BodyVisitor bodyVisitor)
    {
        return bodyVisitor.Visit(this);
    }

    public string ToString(string shift)
    {
        string bodyToString = "──Body \n";
        string declarationToString;
        string statementToString;
        string returnToString;
        string nextBodyToString;
        
        shift += "   ";

        if (_body != null)
        {
            string shift1 = shift + "  │";
            string shift2 = shift + "  └";
            
            if (_declaration != null)
            {
                declarationToString = _declaration.ToString(shift1);
                nextBodyToString = _body.ToString(shift);
                return bodyToString + shift1 + declarationToString + shift2 + nextBodyToString;
            }
            
            if(_statement != null)
            {
                statementToString = _statement.ToString(shift1);
                nextBodyToString = _body.ToString(shift);
                return bodyToString + shift1 + statementToString + shift2 + nextBodyToString;
            }
        
            if(_return != null)
            {
                returnToString = _return.ToString(shift1);
                nextBodyToString = _body.ToString(shift);
                return bodyToString + shift1 + returnToString + shift2 + nextBodyToString;
            }
        }

        if(_declaration != null)
        {
            declarationToString = _declaration.ToString(shift);
            return bodyToString + shift + "└" + declarationToString;
        }
        
        if(_statement != null)
        {
            statementToString = _statement.ToString(shift);
            return bodyToString + shift + "└" + statementToString;
        }
        
        if(_return != null)
        {
            returnToString = _return.ToString(shift);
            return bodyToString + shift + "└" + returnToString;
        }

        return bodyToString;
    }
}

public class Statement
{
    public Assignment? _assignment;
    public WhileLoop? _whileLoop;
    public ForLoop? _forLoop;
    public IfStatement? _ifStatement;
    public RoutineCall? _routineCall;

    public Statement(Assignment? assignment, WhileLoop? whileLoop, ForLoop? forLoop, IfStatement? ifStatement, RoutineCall? routineCall)
    {
        _assignment = assignment;
        _whileLoop = whileLoop;
        _forLoop = forLoop;
        _ifStatement = ifStatement;
        _routineCall = routineCall;
    }
    
    public void Accept(Visitor.StatementVisitor statementVisitor)
    {
        statementVisitor.Visit(this);
    }

    public string ToString(string shift)
    {
        string statementToString = "──Statement \n";
        string assignmentToString;
        string whileLoopToString;
        string forLoopToString;
        string ifStatementToString;
        string routineCallToString;
        
        shift += "   ";
        if(_assignment != null)
        {
            assignmentToString = _assignment.ToString(shift);
            return statementToString + shift + "└" + assignmentToString;
        }
        
        if(_whileLoop != null)
        {
            whileLoopToString = _whileLoop.ToString(shift);
            return statementToString + shift + "└" + whileLoopToString;
        }
        
        if(_forLoop != null)
        {
            forLoopToString = _forLoop.ToString(shift);
            return statementToString + shift + "└" + forLoopToString;
        }
        
        if(_ifStatement != null)
        {
            ifStatementToString = _ifStatement.ToString(shift);
            return statementToString + shift + "└" + ifStatementToString;
        }
        
        if(_routineCall != null)
        {
            routineCallToString = _routineCall.ToString(shift);
            return statementToString + shift + "└" + routineCallToString;
        }

        return statementToString;
    }
}

public class Assignment
{
    public Variable _variable;
    public Expression _expression;

    public Assignment(Variable variable, Expression expression)
    {
        _variable = variable;
        _expression = expression;
    }
    
    public void Accept(Visitor.AssignmentVisitor assignmentVisitor)
    {
        assignmentVisitor.Visit(this);
    }

    public string ToString(string shift)
    {
        string assignmentToString = "──Assignment \n";
        string variableToString;
        string expressionToString;
        
        string shift1 = shift + "  │";
        string shift2 = shift + "  └";
        shift += "   ";
        
        variableToString = _variable.ToString(shift1);
        expressionToString = _expression.ToString(shift);
        return assignmentToString + shift1 + variableToString + shift2 + expressionToString;
    }
}

public class RoutineCall 
{
    public Identifier _identifier;
    public Expressions? _expressions; // MAYBE ACTION // TODO EXPLAIN

    public RoutineCall(Identifier identifier, Expressions? expressions)
    {
        _identifier = identifier;
        _expressions = expressions;
    }
    
    public void Accept(Visitor.RoutineCallVisitor routineCallVisitor)
    {
        routineCallVisitor.Visit(this);
    }
    
    public string ToString(string shift)
    {
        string routineCallToString = "──RoutineCall \n";
        string ifentifierToString;
        string expressionToString;
        
        shift += "   ";
        if (_expressions != null)
        {
            string shift1 = shift + "  │";
            string shift2 = shift + "  └";
            ifentifierToString = _identifier.ToString(shift1);
            expressionToString = _expressions.ToString(shift);
            return routineCallToString + shift1 + ifentifierToString + shift2 + expressionToString;
        }
        ifentifierToString = _identifier.ToString(shift);
        return routineCallToString + shift + "└" + ifentifierToString;
    }
}

public class WhileLoop
{
    public Expression _expression;
    public Body? _body;

    public WhileLoop(Expression expression, Body? body)
    {
        _expression = expression;
        _body = body;
    }
    
    public void Accept(Visitor.WhileLoopVisitor whileLoopVisitor)
    {
        whileLoopVisitor.Visit(this);
    }
    
    public string ToString(string shift)
    {
        string whileLoopToString = "──WhileLoop \n";
        string expressionToString;
        string bodyToString;
        
        shift += "   ";
        if (_body != null)
        {
            string shift1 = shift + "  │";
            string shift2 = shift + "  └";
            expressionToString = _expression.ToString(shift1);
            bodyToString = _body.ToString(shift);
            return whileLoopToString + shift1 + expressionToString + shift2 + bodyToString;
        }
        expressionToString = _expression.ToString(shift);
        return whileLoopToString + shift + "└" + expressionToString;
    }
}

public class ForLoop
{
    // private string _name;
    public Identifier _identifier;
    public bool _reverse;
    public Range _range;
    public Body? _body;

    public ForLoop(Identifier identifier, bool reverse, Range range, Body? body)
    {
        _identifier = identifier;
        _reverse = reverse;
        _range = range;
        _body = body;
    }
    
    public void Accept(Visitor.ForLoopVisitor forLoopVisitor)
    {
        forLoopVisitor.Visit(this);
    }
    
    public string ToString(string shift)
    {
        string forLoopToString = "──ForLoop ";
        string reverseToString = "(Reverse(" + _reverse + ")) \n";
        string identifierToString;
        string rangeToString;
        string bodyToString;
        
        shift += "   ";
        string shift1 = shift + "  │";
        string shift2 = shift + "  └";

        if (_body != null)
        { 
            identifierToString = _identifier.ToString(shift1);
            rangeToString = _range.ToString(shift1);
            bodyToString = _body.ToString(shift);
            return forLoopToString + reverseToString + shift1 + identifierToString + shift1 + rangeToString + shift2 +
                   bodyToString;
        }
        identifierToString = _identifier.ToString(shift1);
        rangeToString = _range.ToString(shift);
        return forLoopToString + reverseToString + shift1 + identifierToString + shift + "└" + rangeToString;
    }
}

public class Range
{
    public Expression _from;
    public Expression _to;

    public Range(Expression from, Expression to)
    {
        _from = from;
        _to = to;
    }
    
    public void Accept(Visitor.RangeVisitor rangeVisitor)
    {
        rangeVisitor.Visit(this);
    }

    public string ToString(string shift)
    {
        string rangeToString = "──Range \n";
        
        string shift1 = shift + "  │";
        string shift2 = shift + "  └";
        shift += "   ";
        string fromToString = _from.ToString(shift1);
        string toToString = _to.ToString(shift);
        
        return rangeToString + shift1 + fromToString + shift2 + toToString;
    }
}

public class IfStatement
{
    public Expression _condition;
    public Body _ifBody;
    public Body? _elseBody;
    // private ElseStatement _elseStatement;

    public IfStatement(Expression condition, Body ifBody, Body? elseBody)
    {
        _condition = condition;
        _ifBody = ifBody;
        _elseBody = elseBody;
    }
    
    public void Accept(Visitor.IfStatementVisitor ifStatementVisitor)
    {
        ifStatementVisitor.Visit(this);
    }
    
    public string ToString(string shift)
    {
        string ifStatementToString = "──IfStatement \n";
        string conditionToString;
        string ifBodyToString;
        string elseBodyToString;

        shift += "   ";
        string shift1 = shift + "  │";
        string shift2 = shift + "  └";

        if (_elseBody != null)
        { 
            conditionToString = _condition.ToString(shift1);
            // shift1 += "   ";
            ifBodyToString = "──IfBody \n" + shift1 + "   └" + _ifBody.ToString(shift1 + "   ");
            elseBodyToString = "──ElseBody \n" + shift + "   └"  + _elseBody.ToString(shift + "   ");
            return ifStatementToString + shift1 + conditionToString + shift1 + ifBodyToString + shift2 +
                   elseBodyToString;
        }
        conditionToString = _condition.ToString(shift1);
        ifBodyToString = _ifBody.ToString(shift);
        return ifStatementToString + shift1 + conditionToString + shift + "└" + ifBodyToString;
    }
    
}
