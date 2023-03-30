using System.Runtime.InteropServices.JavaScript;
using ConsoleApp1.SyntaxAnalyser;
using Action = System.Action;
using Range = ConsoleApp1.SyntaxAnalyser.Range;
using Single = ConsoleApp1.SyntaxAnalyser.Single;
using Type = ConsoleApp1.SyntaxAnalyser.Type;

namespace DefaultNamespace.SemantycalAnalyser;

public class Visitor
{
    private Dictionary<String, Object> functions;
    private Dictionary<String, Object> localVariables;

    public Visitor(){}

    public class ActionVisitor
    {
        public void visit(ConsoleApp1.SyntaxAnalyser.Action? action)
        {
            
        }
    }
    
    public class ActionsVisitor
    {
        public void visit(Actions? actions)
        {
            
        }
    }
    
    public class DeclarationVisitor
    {
        public void visit(Declaration? declaration)
        {
            
        }
    }
    
    public class RoutineDeclarationVisitor
    {
        public void visit(RoutineDeclaration? routineDeclaration)
        {
            
        }
    }
    
    public class RoutineInsightsVisitor
    {
        public void visit(RoutineInsights? routineInsights)
        {
            
        }
    }
    
    public class RoutineReturnTypeVisitor
    {
        public void visit(RoutineReturnType? routineReturnType)
        {
            
        }
    }
    
    public class VariableDeclarationVisitor
    {
        public void visit(VariableDeclaration? variableDeclaration)
        {
            
        }
    }
    
    public class TypeDeclarationVisitor
    {
        public void visit(TypeDeclaration? typeDeclaration)
        {
            
        }
    }
    
    public class StatementVisitor
    {
        public void visit(Statement? statement)
        {
            
        }
    }
    
    public class IfStatementVisitor
    {
        public void visit(IfStatement? ifStatement)
        {
            
        }
    }
    
    public class AssignmentVisitor
    {
        public void visit(Assignment? assignment)
        {
            
        }
    }
    
    public class WhileLoopVisitor
    {
        public void visit(WhileLoop? whileLoop)
        {
            
        }
    }
    
    public class ForLoopVisitor
    {
        public void visit(ForLoop? forLoop)
        {
            
        }
    }
    
    public class RoutineCallVisitor
    {
        public void visit(RoutineCall? routineCall)
        {
            
        }
    }

    public class IdentifierVisitor
    {
        public void visit(Identifier? identifier)
        {
            
        }
    }
    
    public class TypeVisitor
    {
        public void visit(Type? type)
        {
            
        }
    }
    
    public class PrimitiveTypeVisitor
    {
        public void visit(PrimitiveType? primitiveType)
        {
            
        }
    }
    
    public class ArrayTypeVisitor
    {
        public void visit(ArrayType? arrayType)
        {
            
        }
    }
    
    public class RecordTypeVisitor
    {
        public void visit(RecordType? recordType)
        {
            
        }
    }

    public class ExpressionVisitor
    {
        public void visit(Expression? expression)
        {
            
        }
    }
    
    public class ExpressionsVisitor
    {
        public void visit(Expressions? expressions)
        {
            
        }
    }

    public class RelationVisitor
    {
        public void visit(Relation? relation)
        {
            
        }
    }

    public class ValueVisitor
    {
        public void visit(Value? value)
        {
            
        }
    }

    public class MultipleRelationVisitor
    {
        public void visit(MultipleRelation? multipleRelation)
        {
            
        }
    }
    
    public class VariableDeclarationsVisitor
    {
        public void visit(VariableDeclarations? variableDeclarations)
        {
            
        }
    }
    
    public class MainRoutineVisitor
    {
        public void visit(MainRoutine? mainRoutine)
        {
            
        }
    }
    
    public class FunctionVisitor
    {
        public void visit(Function? function)
        {
            
        }
    }
    
    public class BodyVisitor
    {
        public void visit(Body? body)
        {
            
        }
    }
    
    public class ParameterDeclarationVisitor
    {
        public void visit(ParameterDeclaration? parameterDeclaration)
        {
            
        }
    }
    
    public class ParametersVisitor
    {
        public void visit(Parameters? parameters)
        {
            
        }
    }
    
    public class OperationVisitor
    {
        public void visit(Operation? operation)
        {
            
        }
    }
    
    public class ComparisonVisitor
    {
        public void visit(Comparison? comparison)
        {
            
        }
    }
    
    public class OperandVisitor
    {
        public void visit(Operand? operand)
        {
            
        }
    }
    
    public class OperatorVisitor
    {
        public void visit(Operator? oOperator)
        {
            
        }
    }
    
    public class SingleVisitor
    {
        public void visit(Single? single)
        {
            
        }
    }
    
    public class VariableVisitor
    {
        public void visit(Variable? variable)
        {
            
        }
    }
    
    public class ComparisonOperatorVisitor
    {
        public void visit(ComparisonOperator? comparisonOperator)
        {
            
        }
    }
    
    public class MathematicalOperatorVisitor
    {
        public void visit(MathematicalOperator? mathematicalOperator)
        {
            
        }
    }
    
    public class LogicalOperatorVisitor
    {
        public void visit(LogicalOperator? logicalOperator)
        {
            
        }
    }
    
    public class RangeVisitor
    {
        public void visit(Range? range)
        {
            
        }
    }
    
    public class ReturnVisitor
    {
        public void visit(Return? rReturn)
        {
            
        }
    }
}