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
        public void Visit(ConsoleApp1.SyntaxAnalyser.Action action)
        {
            
            if (!(action._actions is null))
            {
                action._actions.Accept(new ActionsVisitor());
            }
            if (!(action._declaration is null))
            {
                action._declaration.Accept(new DeclarationVisitor());
            }
            if (!(action._statement is null))
            {
                action._statement.Accept(new StatementVisitor());
            }
        }
    }
    
    public class ActionsVisitor
    {
        public void Visit(Actions actions)
        {
            actions._action.Accept(new ActionVisitor());
            
            if (!(actions._actions is null))
            {
                actions._actions.Accept(new ActionsVisitor());
            }
        }
    }
    
    public class DeclarationVisitor
    {
        public void Visit(Declaration declaration)
        {
            if (!(declaration._variableDeclaration is null))
            {
                declaration._variableDeclaration.Accept(new VariableDeclarationVisitor());
            }
            
            if (!(declaration._typeDeclaration is null))
            {
                declaration._typeDeclaration.Accept(new TypeDeclarationVisitor());
            }
            
            if (!(declaration._routineDeclaration is null))
            {
                declaration._routineDeclaration.Accept(new RoutineDeclarationVisitor());
            }
        }
    }
    
    public class RoutineDeclarationVisitor
    {
        public void Visit(RoutineDeclaration routineDeclaration)
        {
            if (!(routineDeclaration._mainRoutine is null))
            {
                routineDeclaration._mainRoutine.Accept(new MainRoutineVisitor());
            }
            
            if (!(routineDeclaration._function is null))
            {
                routineDeclaration._function.Accept(new FunctionVisitor());
            }
        }
    }
    
    public class RoutineInsightsVisitor
    {
        public void Visit(RoutineInsights? routineInsights)
        {
            
        }
    }
    
    public class RoutineReturnTypeVisitor
    {
        public void Visit(RoutineReturnType? routineReturnType)
        {
            
        }
    }
    
    public class VariableDeclarationVisitor
    {
        public void Visit(VariableDeclaration? variableDeclaration)
        {
            
        }
    }
    
    public class TypeDeclarationVisitor
    {
        public void Visit(TypeDeclaration? typeDeclaration)
        {
            
        }
    }
    
    public class StatementVisitor
    {
        public void Visit(Statement? statement)
        {
            
        }
    }
    
    public class IfStatementVisitor
    {
        public void Visit(IfStatement? ifStatement)
        {
            
        }
    }
    
    public class AssignmentVisitor
    {
        public void Visit(Assignment? assignment)
        {
            
        }
    }
    
    public class WhileLoopVisitor
    {
        public void Visit(WhileLoop? whileLoop)
        {
            
        }
    }
    
    public class ForLoopVisitor
    {
        public void Visit(ForLoop? forLoop)
        {
            
        }
    }
    
    public class RoutineCallVisitor
    {
        public void Visit(RoutineCall? routineCall)
        {
            
        }
    }

    public class IdentifierVisitor
    {
        public void Visit(Identifier? identifier)
        {
            
        }
    }
    
    public class TypeVisitor
    {
        public void Visit(Type? type)
        {
            
        }
    }
    
    public class PrimitiveTypeVisitor
    {
        public void Visit(PrimitiveType? primitiveType)
        {
            
        }
    }
    
    public class ArrayTypeVisitor
    {
        public void Visit(ArrayType? arrayType)
        {
            
        }
    }
    
    public class RecordTypeVisitor
    {
        public void Visit(RecordType? recordType)
        {
            
        }
    }

    public class ExpressionVisitor
    {
        public void Visit(Expression? expression)
        {
            
        }
    }
    
    public class ExpressionsVisitor
    {
        public void Visit(Expressions? expressions)
        {
            
        }
    }

    public class RelationVisitor
    {
        public void Visit(Relation? relation)
        {
            
        }
    }

    public class ValueVisitor
    {
        public void Visit(Value? value)
        {
            
        }
    }

    public class MultipleRelationVisitor
    {
        public void Visit(MultipleRelation? multipleRelation)
        {
            
        }
    }
    
    public class VariableDeclarationsVisitor
    {
        public void Visit(VariableDeclarations? variableDeclarations)
        {
            
        }
    }
    
    public class MainRoutineVisitor
    {
        public void Visit(MainRoutine? mainRoutine)
        {
            
        }
    }
    
    public class FunctionVisitor
    {
        public void Visit(Function? function)
        {
            
        }
    }
    
    public class BodyVisitor
    {
        public void Visit(Body? body)
        {
            
        }
    }
    
    public class ParameterDeclarationVisitor
    {
        public void Visit(ParameterDeclaration? parameterDeclaration)
        {
            
        }
    }
    
    public class ParametersVisitor
    {
        public void Visit(Parameters? parameters)
        {
            
        }
    }
    
    public class OperationVisitor
    {
        public void Visit(Operation? operation)
        {
            
        }
    }
    
    public class ComparisonVisitor
    {
        public void Visit(Comparison? comparison)
        {
            
        }
    }
    
    public class OperandVisitor
    {
        public void Visit(Operand? operand)
        {
            
        }
    }
    
    public class OperatorVisitor
    {
        public void Visit(Operator? oOperator)
        {
            
        }
    }
    
    public class SingleVisitor
    {
        public void Visit(Single? single)
        {
            
        }
    }
    
    public class VariableVisitor
    {
        public void Visit(Variable? variable)
        {
            
        }
    }
    
    public class ComparisonOperatorVisitor
    {
        public void Visit(ComparisonOperator? comparisonOperator)
        {
            
        }
    }
    
    public class MathematicalOperatorVisitor
    {
        public void Visit(MathematicalOperator? mathematicalOperator)
        {
            
        }
    }
    
    public class LogicalOperatorVisitor
    {
        public void Visit(LogicalOperator? logicalOperator)
        {
            
        }
    }
    
    public class RangeVisitor
    {
        public void Visit(Range? range)
        {
            
        }
    }
    
    public class ReturnVisitor
    {
        public void Visit(Return? rReturn)
        {
            
        }
    }
}