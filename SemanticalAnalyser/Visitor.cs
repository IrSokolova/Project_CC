using System.Net.Mime;
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

    public static Dictionary<String, List<Tuple<String, Object>>> declaredTypes =
        new Dictionary<string, List<Tuple<string, object>>>();

    public static bool inRecord = false;
    public static String typeName = "";

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
                Console.WriteLine("Variable {0} already declared", varName);
                Environment.Exit(1);
            }
            if (!inRecord)
            {
                if (variableDeclaration._value != null)
                {
                    if (variableDeclaration._type._userType != null)
                    {
                        String expectedVariableType = variableDeclaration._type._userType.ToString();
                        if (!declaredTypes.ContainsKey(expectedVariableType))
                        {
                            Console.WriteLine("Type Error");
                            Console.WriteLine("The type {0} wasn't declared", expectedVariableType);
                            Environment.Exit(1);
                        }

                        List<Tuple<String, Object>> expectedTypesFromDeclaration = declaredTypes[expectedVariableType];
                        List<Object> expectedTypes = new List<Object>();
                        foreach (Tuple<String, Object> tuple in expectedTypesFromDeclaration)
                        {
                            expectedTypes.Add(tuple.Item2);
                        }

                        List<Type> actualTypes =
                            variableDeclaration._value._expressions.Accept(new ExpressionsVisitor());

                        if (!(expectedTypes.Count == actualTypes.Count))
                        {
                            Console.WriteLine("Type Error");
                            Console.WriteLine("The wrong number of fields for the record {0}. Expected number is {1}, but got {2}", expectedVariableType, expectedTypes.Count, actualTypes.Count);
                            Environment.Exit(1);
                        }

                        for (int i = 0; i < expectedTypes.Count; i++)
                        {
                            if (!expectedTypes[i].ToString().Equals(actualTypes[i].ToString()))
                            {
                                Console.WriteLine("Type Error");
                                Console.WriteLine("Type mismatch in declaration of variable {0}. Expected type is {1}, but got {2}", varName, expectedTypes[i], actualTypes[i]);
                                Environment.Exit(1);
                            }
                        }
                        localVariables.Add(varName, expectedVariableType);
                        return expectedVariableType;
                    }
                    else
                    {
                        String expectedVariableType = variableDeclaration._type.ToString();
                        String actualVariableType = "";
                        if (variableDeclaration._value.Accept(new ValueVisitor()).Count == 1)
                        {
                            actualVariableType = variableDeclaration._value.Accept(new ValueVisitor())[0].ToString();
                        }

                        if (!(actualVariableType.Equals(expectedVariableType)))
                        {
                            Console.WriteLine("Type Error in variable declaration {0}, expected type {1} but got {2}", varName, expectedVariableType, actualVariableType );
                            Environment.Exit(1);
                        }
                        localVariables.Add(varName, variableDeclaration._type);
                        return actualVariableType;
                    }
                }
                else
                {
                    if (variableDeclaration._type._arrayType != null)
                    {
                        if (variableDeclaration._type._arrayType._type._userType != null)
                        {
                            localVariables.Add(varName,
                                "Array " + variableDeclaration._type._arrayType._type._userType);
                            return "Array " + variableDeclaration._type._arrayType._type._userType;
                        }
                        else
                        {
                            localVariables.Add(varName, "Array " + variableDeclaration._type._arrayType._type);
                            return "Array " + variableDeclaration._type._arrayType._type;
                        }
                    }
                    localVariables.Add(varName, variableDeclaration._type);
                }
            }
            
            else
            {
                    if (variableDeclaration._value != null)
                    {
                        String expectedVariableType = variableDeclaration._type.ToString();
                        String actualVariabletype = variableDeclaration._value.Accept(new ValueVisitor()).ToString();

                        if (!(actualVariabletype.Equals(expectedVariableType)))
                        {
                            Console.WriteLine("Type Error in variable declaration {0}, expected type {1} but got {2}", varName, expectedVariableType, actualVariabletype );
                            Environment.Exit(1);
                        }

                        List<Tuple<String, Object>> insideVariables = declaredTypes[typeName];
                        insideVariables.Add(new Tuple<string, object>(varName, actualVariabletype));
                        declaredTypes[typeName] = insideVariables;
                    }
                    else
                    {
                        List<Tuple<String, Object>> someInsideVariables = declaredTypes[typeName];
                        if (variableDeclaration._type.ToString().Equals("Array"))
                        {
                            someInsideVariables.Add(new Tuple<string, object>(varName,
                                variableDeclaration._type + variableDeclaration._type._arrayType._type.ToString()));
                        }
                        else
                        {
                            someInsideVariables.Add(new Tuple<string, object>(varName, variableDeclaration._type));
                        }

                        declaredTypes[typeName] = someInsideVariables;
                    }
            }
            
            return null;
        }
    }
    
    public class TypeDeclarationVisitor
    {
        public void Visit(TypeDeclaration? typeDeclaration)
        {
            if (typeDeclaration != null)
            {
                typeName = typeDeclaration._identifier.ToString();
                if (declaredTypes.ContainsKey(typeName))
                {
                    Console.WriteLine("Type {0} already declared", typeName);
                    Environment.Exit(1);
                }
                
                if (typeDeclaration._type.ToString().Equals("Array"))
                {
                    String expextedArrayType = typeDeclaration._type +  " " + typeDeclaration._type._arrayType._type;
                    String actualArrayType =  typeDeclaration._type + " " + typeDeclaration._type._arrayType._expression.Accept(new ExpressionVisitor());
                    if (!(actualArrayType.Equals(expextedArrayType)))
                    {
                        Console.WriteLine("Type Error in array type declaration");
                        Environment.Exit(1);
                    }
                    localVariables.Add(typeName, typeDeclaration._type._arrayType._type);
                }

                if (typeDeclaration._type.ToString().Equals("Record"))
                {
                    inRecord = true;
                    declaredTypes.Add(typeName, new List<Tuple<string, object>>());
                    typeDeclaration._type._recordType.Accept(new RecordTypeVisitor());
                    inRecord = false;
                    typeName = "";
                }
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

            Type conditionType = ifStatement._condition.Accept(new ExpressionVisitor());
            if (!conditionType.ToString().Equals("Boolean"))
            {
                Console.WriteLine("Condition expected to be boolean type, not {0}", conditionType);
                Environment.Exit(1);
            }
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
                Console.WriteLine("The variable {0} is undefined", varName);
                Environment.Exit(1);
            }

            if (localVariables[varName] is string)
            {
                if (declaredTypes.ContainsKey(localVariables[varName].ToString()))
                {
                    if (assignment._expressions != null)
                    {
                        String expectedVariableType = localVariables[varName].ToString();
                        List<Tuple<String, Object>> expectedTypesFromDeclaration = declaredTypes[expectedVariableType];
                        List<Object> expectedTypes = new List<Object>();
                        foreach (Tuple<String, Object> tuple in expectedTypesFromDeclaration)
                        {
                            expectedTypes.Add(tuple.Item2);
                        }

                        List<Type> actualTypes =
                            assignment._expressions.Accept(new ExpressionsVisitor());
                        if (!(expectedTypes.Count == actualTypes.Count))
                        {
                            Console.WriteLine("Type Error");
                            Console.WriteLine(
                                "The wrong number of fields for the record {0}. Expected number is {1}, but got {2}",
                                expectedVariableType, expectedTypes.Count, actualTypes.Count);
                            Environment.Exit(1);
                        }

                        for (int i = 0; i < expectedTypes.Count; i++)
                        {
                            if (!expectedTypes[i].ToString().Equals(actualTypes[i].ToString()))
                            {
                                Console.WriteLine("Type Error");
                                Console.WriteLine(
                                    "Type mismatch in assignment of variable {0}. Expected type is {1}, but got {2}",
                                    varName, expectedTypes[i], actualTypes[i]);
                                Environment.Exit(1);
                            }
                        }
                    }

                    Console.WriteLine("Type Error");
                    Console.WriteLine("The type {0} wasn't declared", localVariables[varName]);
                    Environment.Exit(1);
                }
                else
                {
                    if (localVariables[varName].ToString().Contains("Array"))
                    {
                        if (assignment._variable._arrayType is null)
                        {
                            Console.WriteLine("Type Error");
                            Console.WriteLine("The variable {0} should have array type", varName);
                            Environment.Exit(1);
                        }

                        string arrayType = localVariables[varName].ToString().Split(' ').Last();
                        if (!declaredTypes.ContainsKey(arrayType))
                        {
                            if (!(arrayType.Equals("Integer") || arrayType.Equals("Boolean") || arrayType.Equals("Real")))
                            {
                                Console.WriteLine("Type Error");
                                Console.WriteLine("Type {0} wasn't defined", arrayType);
                                Environment.Exit(1);    
                            }
                            else
                            {
                                string actualType = assignment._expressions.Accept(new ExpressionsVisitor())[0].ToString();
                                if (!actualType.Equals(arrayType))
                                {
                                    Console.WriteLine("Type Error in assignment of variable {0}. Expected type: {1}, but got {2}", varName, arrayType, actualType);
                                    Environment.Exit(1);
                                }
                            }
                        }
                        else
                        {
                            List<Tuple<String, Object>> expectedTypesFromDeclaration = declaredTypes[arrayType];
                            List<Object> expectedTypes = new List<Object>();
                            foreach (Tuple<String, Object> tuple in expectedTypesFromDeclaration)
                            {
                                expectedTypes.Add(tuple.Item2);
                            }

                            List<Type> actualTypes =
                                assignment._expressions.Accept(new ExpressionsVisitor());
                            if (!(expectedTypes.Count == actualTypes.Count))
                            {
                                Console.WriteLine("Type Error");
                                Console.WriteLine(
                                    "The wrong number of fields for the record {0}. Expected number is {1}, but got {2}",
                                    arrayType, expectedTypes.Count, actualTypes.Count);
                                Environment.Exit(1);
                            }

                            for (int i = 0; i < expectedTypes.Count; i++)
                            {
                                if (!expectedTypes[i].ToString().Equals(actualTypes[i].ToString()))
                                {
                                    Console.WriteLine("Type Error");
                                    Console.WriteLine(
                                        "Type mismatch in assignment of variable {0}. Expected type is {1}, but got {2}",
                                        varName, expectedTypes[i], actualTypes[i]);
                                    Environment.Exit(1);
                                }
                            }
                        }
                    }
                }
            }
            

            else
            {
                if (assignment._variable._arrayType != null)
                {
                    Console.WriteLine("Type error");
                    Console.WriteLine("The variable {0} isn't array. Cannot access by the index", varName);
                    Environment.Exit(1);
                }
                Type expectedType = (Type)localVariables[varName];
                if (assignment._expressions != null)
                {
                    List<Type> actualType = assignment._expressions.Accept(new ExpressionsVisitor());
                    if (!expectedType.ToString().Equals(actualType[0].ToString()))
                    {
                        Console.WriteLine("Type Error in variable assignment {0}, expected type {1} but got {2}", varName, expectedType, actualType[0]);
                        Environment.Exit(1);
                    }
                }
                else
                {
                    String functionName = assignment._routineCall._identifier.ToString();
                    if (!functions.ContainsKey(functionName))
                    {
                        Console.WriteLine("The routine {0} is undefined", functionName);
                        Environment.Exit(1);
                    }

                    List<Tuple<String, Object>> parametersList = functions[functionName];
                    String actualType = "";
                    for (int i = 0; i < parametersList.Count; i++)
                    {
                        if (parametersList[i].Item1.Equals("return"))
                        {
                            actualType = (String)parametersList[i].Item2;
                            parametersList.Remove(parametersList[i]);
                            break;
                        }
                    }

                    List<Type> actualParameters = assignment._routineCall.Accept(new RoutineCallVisitor());
                    if (actualType.Equals(""))
                    {
                        Console.WriteLine("The variable {0} can't be assigned to result of {1}", varName, functionName);
                        Environment.Exit(1);
                    }

                    if (!expectedType.ToString().Equals(actualType.ToString()))
                    {
                        Console.WriteLine("The variable {0} can't be assigned to result of {1}", varName, functionName);
                        Environment.Exit(1);
                    }

                    if (parametersList.Count == actualParameters.Count)
                    {
                        for (int i = 0; i < parametersList.Count; i++)
                        {
                            if (!parametersList[i].Item2.ToString().Equals(actualParameters[i].ToString()))
                            {
                                Console.WriteLine("Type Error in passing parameters to a function {0}, expected parameter type {1}, but got {2}",
                                    functionName,parametersList[i].Item2, actualParameters[i]);
                                Environment.Exit(1);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("The function {0} should have {1} parameters but got {2} parameters",
                            functionName, parametersList.Count, actualParameters.Count);
                        Environment.Exit(1);
                    }
                }
            }
        }
    }
    
    public class WhileLoopVisitor
    {
        public void Visit(WhileLoop whileLoop)
        {
            Type conditionType = whileLoop._expression.Accept(new ExpressionVisitor());
            if (!conditionType.ToString().Equals("Boolean"))
            {
                Console.WriteLine("Condition expected to be boolean type, not {0}", conditionType);
            }

            if (whileLoop._body != null)
            {
                whileLoop._body.Accept(new BodyVisitor());
            }
        }
    }
    
    public class ForLoopVisitor
    {
        public void Visit(ForLoop forLoop)
        {
            String identifierType = forLoop._identifier.Accept(new IdentifierVisitor());
            if (identifierType != "Integer")
            {
                Console.WriteLine("Identifier can not be of type {0}", identifierType);
                Environment.Exit(1);
            }
            
            string expectedType = "Integer";
            Type fromActualType = forLoop._range._from.Accept(new ExpressionVisitor()); 
            Type toActualType = forLoop._range._to.Accept(new ExpressionVisitor()); 
            if (fromActualType is null || toActualType is null || !(expectedType.Equals(fromActualType.ToString()) && expectedType.Equals(toActualType.ToString()))) 
            { 
                Console.WriteLine("Type Error");
                Console.WriteLine("Wrong type of range values expected type Integer but got {0} and {1}", fromActualType, toActualType); 
                Environment.Exit(1);
            }
                        
            localVariables.Add(forLoop._identifier.ToString(), fromActualType);            
            
            if (forLoop._body != null)
            {
                forLoop._body.Accept(new BodyVisitor());
            }

            localVariables.Remove(forLoop._identifier.ToString());
        }
    }
    
    public class RoutineCallVisitor
    {
        public List<Type> Visit(RoutineCall? routineCall)
        {
            if (routineCall._expressions != null)
            {
                return routineCall._expressions.Accept(new ExpressionsVisitor());    
            }

            return null;
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
                type._recordType.Accept(new RecordTypeVisitor());
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
        public void Visit(RecordType recordType)
        {
            recordType._variableDeclaration.Accept(new VariableDeclarationVisitor());
            
            if (recordType._variableDeclarations != null)
            {
                recordType._variableDeclarations.Accept(new VariableDeclarationsVisitor());
            }
            
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
        public List<Type> actualParameters = new List<Type>();

        public void GoDeeper(Expressions? expressions)
        {
            if (expressions != null)
            {
                actualParameters.Add(expressions._expression.Accept(new ExpressionVisitor()));
                GoDeeper(expressions._expressions);
            }
        }
        public List<Type> Visit(Expressions? expressions)
        {
            GoDeeper(expressions);
            return actualParameters;
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
        public List<Type> Visit(Value? value)
        {
            return value._expressions.Accept(new ExpressionsVisitor());
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
            variableDeclarations._variableDeclaration.Accept(new VariableDeclarationVisitor());
            
            if (variableDeclarations._variableDeclarations != null)
            {
                variableDeclarations._variableDeclarations.Accept(new VariableDeclarationsVisitor());
            }
        }
    }
    
    public class MainRoutineVisitor
    {
        public void Visit(MainRoutine mainRoutine)
        {
            string routineName = mainRoutine._identifier.ToString();
            if (functions.ContainsKey(routineName))
            {
                Console.WriteLine("{0} routine already exists", routineName);
                Environment.Exit(1);
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
            int numberOfParameters = 0;
            
            if (functions.ContainsKey(functionName))
            {
                Console.WriteLine("{0} function already exists", functionName);
                Environment.Exit(1);
            }

            if (function._parameters != null)
            {
                function._parameters.Accept(new ParametersVisitor());
            }

            List<Tuple<String, Object>> parameters = new List<Tuple<string, object>>();
            foreach (var variable in localVariables)
            {
                numberOfParameters++;
                string parameterName = "parameter" + numberOfParameters;
                parameters.Add(new Tuple<string, object>(parameterName, variable.Value));
            }

            if (function._routineReturnType._type != null)
            {
                Type expectedReturnType = function._routineReturnType._type;
                Object actualReturnType = function._routineInsights.Accept(new RoutineInsightsVisitor());
                if (actualReturnType == null)
                {
                    Console.WriteLine("The function {0} doesn't have return statement", functionName);
                    Environment.Exit(1);
                }
                
                if (!expectedReturnType.ToString().Equals(actualReturnType.ToString()))
                {
                    Console.WriteLine("Return type mismatch in {0} function expected type {1} but got {2}", functionName, expectedReturnType, actualReturnType);
                    Environment.Exit(1);
                }
                parameters.Add(new Tuple<string, object>("return", actualReturnType.ToString()));
            }
            functions.Add(functionName, parameters);
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
                Console.WriteLine("Parameter {0} already declared", parameterName);
                Environment.Exit(1);
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
                    if (!(operationType.ToString().Equals("Integer") && operandType.ToString().Equals("Integer") || 
                          operationType.ToString().Equals("Real") && operandType.ToString().Equals("Real")))
                    {
                        Console.WriteLine("Type Error for the comparison operator. Expected type Integer and Integer or Real and Real but got {0} and {1}",
                        operandType, operationType);
                        Environment.Exit(1);
                    } 
                }
                
                if (operation._operator._logicalOperator != null)
                {
                    if (!(operationType.ToString().Equals("Boolean") && operandType.ToString().Equals("Boolean")))
                    {
                        Console.WriteLine("Type Error for the logical operator. Expected type Boolean and Boolean but got {0} and {1}",
                            operandType, operationType);
                        Environment.Exit(1);
                    } 
                }
                
                if (operation._operator._mathematicalOperator != null)
                {
                    if (!(operationType.ToString().Equals("Integer") && operandType.ToString().Equals("Integer") || 
                        operationType.ToString().Equals("Real") && operandType.ToString().Equals("Real")))
                    {
                        Console.WriteLine("Type Error for the mathematical operator. Expected type Integer and Integer or Real and Real but got {0} and {1}",
                            operandType, operationType);
                        Environment.Exit(1);
                    } 
                }
                
                operatorType = operation._operator.Accept(new OperatorVisitor());
            }

            
            
            if (operationType != null)
            {
                if (!operationType.ToString().Equals(operandType.ToString()))
                {
                    Console.WriteLine("Type Error");
                    Environment.Exit(1);
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
            if (operand._single != null)
            {
                if (operand._single._variable != null)
                {
                    string key = operand._single._variable._identifier.ToString();
                    if (localVariables.ContainsKey(key))
                    {
                        return (Type)localVariables[key];
                    }

                    Console.WriteLine("Variable {0} is undefined", key);
                    Environment.Exit(1);
                }

                return operand._single._type;
            }
            return operand._expression.Accept(new ExpressionVisitor());
        }
    }
    
    public class OperatorVisitor
    {
        public Type? Visit(Operator? oOperator)
        {
            if (oOperator._comparisonOperator != null || oOperator._logicalOperator != null)
            {
                return new Type(new PrimitiveType(false, false, true), null, null, null);
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
