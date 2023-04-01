using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using ConsoleApp1.SyntaxAnalyser;
using Action = System.Action;
using Range = ConsoleApp1.SyntaxAnalyser.Range;
using Single = ConsoleApp1.SyntaxAnalyser.Single;
using Type = ConsoleApp1.SyntaxAnalyser.Type;

namespace DefaultNamespace.SemantycalAnalyser;

public class Visitor
{
    public static Dictionary<String, Object> functions;
    public static Dictionary<String, Object> localVariables;

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
        public void Visit(RoutineInsights routineInsights)
        {
            if (!(routineInsights._body is null))
            {
                routineInsights._body.Accept(new BodyVisitor());
            }
        }
    }
    
    public class RoutineReturnTypeVisitor
    {
        public Type? Visit(RoutineReturnType routineReturnType)
        {
            return routineReturnType._type;
        }
    }
    
    public class VariableDeclarationVisitor
    {
        public void Visit(VariableDeclaration? variableDeclaration)
        {
            if (variableDeclaration != null)
            {
                string varName = variableDeclaration._identifier.ToString();
                if (localVariables.ContainsKey(varName))
                {
                    throw new Exception(String.Format("Variable {0} already declared", varName));
                }
                
                // TODO: тут нам надо бы проверить,что тип, который мы вручную присваиваем также совпадает с настоящим типом значения
                // var n : Integer is 10 - хорошо
                // var n : Integer is "10" - плохо
                Type expectedVariableType = variableDeclaration._type;
                variableDeclaration._value.Accept(new ValueVisitor());
                
                localVariables.Add(varName, variableDeclaration._type);
            }
        }
    }
    
    public class TypeDeclarationVisitor
    {
        public void Visit(TypeDeclaration? typeDeclaration)
        {
            if (typeDeclaration != null)
            {
                string typeName = typeDeclaration._identifier.ToString();
                if (localVariables.ContainsKey(typeName))
                {
                    throw new Exception(String.Format("Type {0} already declared", typeName));
                }
                
                // TODO: тут нам надо бы проверить,что тип, который мы вручную присваиваем также совпадает с настоящим типом значения
                // type arr is array [1] Integer - хорошо
                // type arr is array ["1"] Integer - плохо

                localVariables.Add(typeName, typeDeclaration._type);
            }
        }
    }
    
    public class StatementVisitor
    {
        public void Visit(Statement statement)
        {
            if (!(statement._assignment is null))
            {
                statement._assignment.Accept(new AssignmentVisitor());
            }

            if (!(statement._ifStatement is null))
            {
                statement._ifStatement.Accept(new IfStatementVisitor());
            }

            if (!(statement._forLoop is null))
            {
                statement._forLoop.Accept(new ForLoopVisitor());
            }

            if (!(statement._routineCall is null))
            {
                statement._routineCall.Accept(new RoutineCallVisitor());
            }

            if (!(statement._whileLoop is null))
            {
                statement._whileLoop.Accept(new WhileLoopVisitor());
            }
        }
    }
    
    public class IfStatementVisitor
    {
        public void Visit(IfStatement ifStatement)
        {
            // TODO надо проверить, что condition - bool
            
            ifStatement._condition.Accept(new ExpressionVisitor());
            ifStatement._ifBody.Accept(new BodyVisitor());
            if (!(ifStatement._elseBody is null))
            {
                ifStatement._elseBody.Accept(new BodyVisitor());
            }
        }
    }
    
    public class AssignmentVisitor
    {
        public void Visit(Assignment assignment)
        {
            // TODO: проверить, что мы присваеваем нужный тип к нужному типу
            // Type expressionType = assignment._expression.Accept(new ExpressionVisitor());
            
            // Надо сделать так, чтобы все accept могли что-то возвращать
            
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