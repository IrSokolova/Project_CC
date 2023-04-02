using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography.X509Certificates;
using ConsoleApp1.SyntaxAnalyser;
using Action = System.Action;
using Range = ConsoleApp1.SyntaxAnalyser.Range;
using Single = ConsoleApp1.SyntaxAnalyser.Single;
using Type = ConsoleApp1.SyntaxAnalyser.Type;

namespace DefaultNamespace.SemantycalAnalyser;

public class Visitor
{
    public static Dictionary<String, Tuple<Object, Object>> functions;
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
                routineDeclaration._mainRoutine.Accept(new RelationVisitor.MainRoutineVisitor());
            }
            if (!(routineDeclaration._function is null))
            {
                routineDeclaration._function.Accept(new RelationVisitor.FunctionVisitor());
            }
        }
    }
    
    public class RoutineInsightsVisitor
    {
        public Object Visit(RoutineInsights routineInsights)
        {
            if (!(routineInsights._body is null))
            {
                // return routineInsights._body.Accept(new BodyVisitor());
            }

            return null;
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
                Type actualVariableRtpe = variableDeclaration._value.Accept(new RelationVisitor.ValueVisitor());
                
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
            ifStatement._ifBody.Accept(new RelationVisitor.BodyVisitor());
            if (!(ifStatement._elseBody is null))
            {
                ifStatement._elseBody.Accept(new RelationVisitor.BodyVisitor());
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
        public void Visit(WhileLoop whileLoop)
        {
            if (whileLoop._body != null)
            {
                whileLoop._body.Accept(new RelationVisitor.BodyVisitor());
            }
            // TODO: Экспрешионы всегда должны быть типа Bool?
            whileLoop._expression.Accept(new ExpressionVisitor());
        }
    }
    
    public class ForLoopVisitor
    {
        public void Visit(ForLoop forLoop)
        {
            String identifierType = forLoop._identifier.Accept(new IdentifierVisitor());
            if (identifierType != "Integer")
            {
                throw new Exception(String.Format("Identifier can not be of type {0}", identifierType));
            }
            
            if (forLoop._body != null)
            {
                forLoop._body.Accept(new RelationVisitor.BodyVisitor());
            }
            
            // TODO: проверить Range
            if (forLoop._reverse)
            {
                
            }
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
        public string? Visit(Identifier? identifier)
        {
            if (identifier != null)
            {
                return identifier._type;
            }

            return null;
        }
    }
    
    public class TypeVisitor
    {
        public Type Visit(Type type)
        {
            // TODO: Выглядит странно, но ладно
            // TODO: написать accept для type
            return type;
        }
    }
    
    public class PrimitiveTypeVisitor
    {
        public void Visit(PrimitiveType? primitiveType)
        {
            // return primitiveType.GetType();
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
        public Type Visit(Expression? expression)
        {
            if (expression != null)
            {
                return expression._relation.Accept(new RelationVisitor());
            }

            if (expression._multipleRelation != null)
                    expression._multipleRelation.Accept(new RelationVisitor.MultipleRelationVisitor());
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
        public Type Visit(Relation? relation)
        {
            if (relation._operation != null)
            {
                return relation._operation.Accept(new OperationVisitor());
            }

            return relation._comparison.Accept(new ComparisonVisitor());
        }
    }

    public class ValueVisitor
    {
        public Type Visit(Value? value)
        {
            return value._expression.Accept(new ExpressionVisitor());
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
        public void Visit(MainRoutine mainRoutine)
        {
            string routineName = mainRoutine._identifier.ToString();
            if (functions.ContainsKey(routineName))
            {
                throw new Exception(String.Format("{0} routine already exists", routineName));
            }
        }
    }
    
    public class FunctionVisitor
    {
        public void Visit(Function function)
        {
            string functionName = function._identifier.ToString();
            if (function._routineReturnType._type != null)
            {
                Type expectedReturnType = function._routineReturnType._type;
            }

            // Type actualReturnType = function._routineInsights.Accept(new RoutineInsightsVisitor());

            if (functions.ContainsKey(functionName))
            {
                throw new Exception(String.Format("{0} function already exists", functionName));
            }
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
        public Type Visit(Operation? operation)
        {
            // Operation - Value1, Operator - sign(+, - ...), Operand - Value2
            Type? operationType = null;
            Type? operatorType = null;
            Type? operandType;
            
            operandType = operation._operand.Accept(new OperandVisitor());
            if (operation._operation != null)
            {
                operationType = operation._operation.Accept(new OperationVisitor());
            }

            if (operation._operator != null)
            {
                if (operation._operator._comparisonOperator != null)
                {
                    if (!(operationType._primitiveType._isInt && operandType._primitiveType._isInt ||
                        operationType._primitiveType._isReal && operandType._primitiveType._isReal))
                    {
                        throw new Exception("Type Error for the comparison operator");
                    } 
                }
                
                if (operation._operator._logicalOperator != null)
                {
                    if (!(operationType._primitiveType._isBoolean && operandType._primitiveType._isBoolean))
                    {
                        throw new Exception("Type Error for the logical operator");
                    } 
                }
                operatorType = operation._operator.Accept(new OperatorVisitor());
            }

            
            
            if (operationType != null)
            {
                if (!operationType.Equals(operandType))
                {
                    throw new Exception("Type Error");
                }
            }
            
            if (operatorType != null)
            {
                return operatorType;
            }

            return operandType;
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
        public Type? Visit(Operand operand)
        {
            if (operand._single._variable != null)
            {
                string key = operand._single._variable._identifier.ToString();
                if (localVariables.ContainsKey(key))
                {
                    return (Type) localVariables[key];
                }
                throw new Exception(String.Format("Variable {0} undefined", key));
            }
            return operand._single._type;
        }
    }
    
    public class OperatorVisitor
    {
        public Type? Visit(Operator? oOperator)
        {
            if (oOperator._comparisonOperator != null || oOperator._logicalOperator != null)
            {
                return new Type(new PrimitiveType(false, false, true), null, null);
            }

            return null;
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