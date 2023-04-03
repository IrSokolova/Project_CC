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
    public static Dictionary<String, List<Tuple<String, Object>>> functions = new Dictionary<string, List<Tuple<string, object>>>();
    public static Dictionary<String, Object> localVariables = new Dictionary<string, object>();

    public Visitor(){}

    public class ActionVisitor
    {
        public void Visit(ConsoleApp1.SyntaxAnalyser.Action action)
        {
            
            if (!(action._declaration is null))
            {
                action._declaration.Accept(new DeclarationVisitor());
            }
            if (!(action._statement is null))
            {
                action._statement.Accept(new StatementVisitor());
            }
            if (!(action._actions is null))
            {
                action._actions.Accept(new ActionsVisitor());
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
        public Type Visit(RoutineInsights routineInsights)
        {
            if (!(routineInsights._body is null))
            {
                return routineInsights._body.Accept(new BodyVisitor());
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
        public string Visit(VariableDeclaration variableDeclaration)
        {
            string varName = variableDeclaration._identifier.ToString();
            if (localVariables.ContainsKey(varName))
            {
                throw new Exception(String.Format("Variable {0} already declared", varName));
            }
            
            // TODO: тут нам надо бы проверить,что тип, который мы вручную присваиваем также совпадает с настоящим типом значения
            // var n : Integer is 10 - хорошо
            // var n : Integer is "10" - плохо
            if (variableDeclaration._value != null)
            {
                String expectedVariableType = variableDeclaration._type.ToString();
                String actualVariabletype = variableDeclaration._value.Accept(new ValueVisitor()).ToString();
                
                if (!(actualVariabletype.Equals(expectedVariableType)))
                {
                    throw new Exception("Type Error in variable declaration");
                }
                return actualVariabletype;
            }
            localVariables.Add(varName, variableDeclaration._type);
            return null;
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
                
                if (typeDeclaration._type.ToString().Equals("Array"))
                {
                    String expextedArrayType = typeDeclaration._type +  " " + typeDeclaration._type._arrayType._type;
                    String actualArrayType =  typeDeclaration._type + " " + typeDeclaration._type._arrayType._expression.Accept(new ExpressionVisitor());
                    if (!(actualArrayType.Equals(expextedArrayType)))
                    {
                        throw new Exception("Type Error in array type declaration");
                    }
                    localVariables.Add(typeName, actualArrayType);
                }

                // TODO: тут нам надо бы проверить,что тип, который мы вручную присваиваем также совпадает с настоящим типом значения
                // type arr is array [1] Integer - хорошо
                // type arr is array ["1"] Integer - плохо
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
            String varName = assignment._variable._identifier._name;
            if (!localVariables.ContainsKey(varName))
            {
                throw new Exception(String.Format("The variable {0} is undefined", varName));
            }

            Type expectedType = (Type)localVariables[varName];
            Type actualType = assignment._expression.Accept(new ExpressionVisitor());

            if (!expectedType.ToString().Equals(actualType.ToString()))
            {
                throw new Exception(String.Format("Type error in assignment of variable {0}", varName));
            }
        }
    }
    
    public class WhileLoopVisitor
    {
        public void Visit(WhileLoop whileLoop)
        {
            if (whileLoop._body != null)
            {
                whileLoop._body.Accept(new BodyVisitor());
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
                forLoop._body.Accept(new BodyVisitor());
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
        public string Visit(Type type)
        {
            if (type._primitiveType != null)
            {
                return type._primitiveType.Accept(new PrimitiveTypeVisitor());
            }
            if (type._arrayType != null)
            {
                return type._arrayType.Accept(new ArrayTypeVisitor());
            }

            if (type._recordType != null)
            {
                return type._recordType.Accept(new RecordTypeVisitor());
            }

            return null;
        }
    }
    
    public class PrimitiveTypeVisitor
    {
        public string Visit(PrimitiveType primitiveType)
        {
            return primitiveType.ToString();
        }
    }
    
    public class ArrayTypeVisitor
    {
        public string Visit(ArrayType? arrayType)
        {
            return arrayType._type.Accept(new TypeVisitor());
        }
    }
    
    public class RecordTypeVisitor
    {
        public string Visit(RecordType recordType)
        {
            if (recordType._variableDeclarations != null)
            {
                return recordType._variableDeclarations.Accept(new VariableDeclarationsVisitor());
            }

            return recordType._variableDeclaration.Accept(new VariableDeclarationVisitor());
        }
    }

    public class ExpressionVisitor
    {
        public Type Visit(Expression? expression)
        {
            if (expression._multipleRelation != null)
                expression._multipleRelation.Accept(new MultipleRelationVisitor());
            return expression._relation.Accept(new RelationVisitor());
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
        public string Visit(VariableDeclarations? variableDeclarations)
        {
            if (variableDeclarations._variableDeclarations != null)
            {
                return variableDeclarations._variableDeclarations.Accept(new VariableDeclarationsVisitor());
            }

            return variableDeclarations._variableDeclaration.Accept(new VariableDeclarationVisitor());
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
            functions.Add(routineName, new List<Tuple<string, object>>());
            mainRoutine._body.Accept(new BodyVisitor());
            
            localVariables.Clear();
        }
    }
    
    public class FunctionVisitor
    {
        public void Visit(Function function)
        {
            string functionName = function._identifier.ToString();
            
            if (functions.ContainsKey(functionName))
            {
                throw new Exception(String.Format("{0} function already exists", functionName));
            }

            if (function._parameters != null)
            {
                function._parameters.Accept(new ParametersVisitor());
            }

            List<Tuple<String, Object>> parameters = new List<Tuple<string, object>>();
            foreach (var variable in localVariables)
            {
                parameters.Add(new Tuple<string, object>(variable.Key, variable.Value));
            }
            
            functions.Add(functionName, parameters);
            
            if (function._routineReturnType._type != null)
            {
                Type expectedReturnType = function._routineReturnType._type;
                Type actualReturnType = function._routineInsights.Accept(new RoutineInsightsVisitor());
                if (!expectedReturnType.ToString().Equals(actualReturnType.ToString()))
                {
                    throw new Exception(String.Format("Return type mismatch in {0} function", functionName));
                }
            }

            localVariables.Clear();
        }
    }
    
    public class BodyVisitor
    {
        public Type Visit(Body body)
        {
            if (body._declaration != null)
            {
                body._declaration.Accept(new DeclarationVisitor());
            }

            if (body._statement != null)
            {
                body._statement.Accept(new StatementVisitor());
            }
            
            if (body._body != null)
            {
                 return body._body.Accept(new BodyVisitor());
            }
            if (body._return != null)
            {
                return body._return.Accept(new ReturnVisitor());
            }
            return null;
        }
    }
    
    public class ParameterDeclarationVisitor
    {
        public void Visit(ParameterDeclaration parameterDeclaration)
        {
            string parameterName = parameterDeclaration._identifier.ToString();
            if (localVariables.ContainsKey(parameterName))
            {
                throw new Exception(String.Format("Parameter {0} already declared", parameterName));
            }
            localVariables.Add(parameterName, parameterDeclaration._type);
        }
    }
    
    public class ParametersVisitor
    {
        public void Visit(Parameters parameters)
        {
            if (parameters._parameterDeclaration != null)
            {
                parameters._parameterDeclaration.Accept(new ParameterDeclarationVisitor());
            }

            if (parameters._parameters != null)
            {
                parameters._parameters.Accept(new ParametersVisitor());
            }
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
                if (!operationType.ToString().Equals(operandType.ToString()))
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
        public Type Visit(Comparison comparison)
        {
            if (comparison._operation != null) return comparison._operation.Accept(new OperationVisitor());
            return comparison._operator.Accept(new OperatorVisitor());
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
        public Type Visit(Return? rReturn)
        {
            return rReturn._expression.Accept(new ExpressionVisitor());
        }
    }
}