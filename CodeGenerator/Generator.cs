using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System; 
using System.IO;
using System.Linq;
using System.Reflection;
using BindingFlags = System.Reflection.BindingFlags;
using Cecilifier.Runtime;
using ConsoleApp1.CodeGenerator.Exe;
using ConsoleApp1.SyntaxAnalyser;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using TypeAttributes = Mono.Cecil.TypeAttributes;
using Action = ConsoleApp1.SyntaxAnalyser.Action;
using ConsoleApp1.SyntaxAnalyser;
using Mono.CompilerServices.SymbolWriter;
using FieldAttributes = Mono.Cecil.FieldAttributes;
using ParameterAttributes = Mono.Cecil.ParameterAttributes;
using Range = ConsoleApp1.SyntaxAnalyser.Range;
using Type = ConsoleApp1.SyntaxAnalyser.Type;
using ArrayType = ConsoleApp1.SyntaxAnalyser.ArrayType;

namespace DefaultNamespace.CodeGenerator;

public class Generator
{
    // private Action _action;
    private AssemblyDefinition _asm;
    private TypeDefinition _typeDef;
    private MethodDefinition _mainModule;
    private ILProcessor _mainProc;
    
    private string _path = @"/home/tatiana/RiderProjects/Project_CC/CodeGenerator/Exe/code.exe";
    // private string _path = @"C:\Users\alena\RiderProjects\compiler\Project_CC\CodeGenerator\Exe\code.exe";
    
    private MainRoutine? _mainRoutine;

    private List<TypeDeclaration> _records;
    private Dictionary<string, Function> _functions;
    private Dictionary<string, Type> _typeDeclarations;
    private Dictionary<string, TypeDefinition> _recTypeDefinitions;
    private Dictionary<string, MethodDefinition> _constructors;
    private Dictionary<string, List<FieldDefinition>> _recFieldDefinitions;
    private Dictionary<string, Tuple<int, ParameterDefinition, Type>> _paramsDefinitions;
    private Dictionary<string, Type> _varsTypes;
    private Dictionary<string, VariableDefinition> _vars;
    private Dictionary<string, MethodDefinition> _funs;
    private Dictionary<string, ILProcessor> _funsProcs;
    private MethodDefinition _mainRoutineModule;

    public Generator(Action action)
    {
	    Processing processing = new Processing();
	    FunProcessing funProcessing = new FunProcessing();
	    _records = processing.FindRecords(action);
	    _mainRoutine = null;
	    _functions = funProcessing.FindFunctions(action);
	    _mainRoutine = funProcessing._mainRoutine;

	    _typeDeclarations = new Dictionary<string, Type>();
	    _paramsDefinitions = new Dictionary<string, Tuple<int, ParameterDefinition, Type>>();
	    _recTypeDefinitions = new Dictionary<string, TypeDefinition>();
	    _recFieldDefinitions = new Dictionary<string, List<FieldDefinition>>();
	    _constructors = new Dictionary<string, MethodDefinition>();
	    _varsTypes = new Dictionary<string, Type>();
	    _vars = new Dictionary<string, VariableDefinition>();
	    _funs = new Dictionary<string, MethodDefinition>();
	    _funsProcs = new Dictionary<string, ILProcessor>();

	    var mp = new ModuleParameters { Architecture = TargetArchitecture.AMD64, Kind =  ModuleKind.Console, ReflectionImporterProvider = new SystemPrivateCoreLibFixerReflectionProvider() };
	    _asm = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition("Program", Version.Parse("1.0.0.0")), Path.GetFileName(_path), mp);
	    
	    _typeDef = new TypeDefinition("", "Program", TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.Public, _asm.MainModule.TypeSystem.Object);
	    _asm.MainModule.Types.Add(_typeDef);
	    
	    _mainRoutineModule = new MethodDefinition("mainRoutineModule", MethodAttributes.Assembly | MethodAttributes.Static | MethodAttributes.HideBySig, _asm.MainModule.TypeSystem.Void);

	    GenerateRecords();
	    GenerateFunctions();
	    if (_mainRoutine != null)
	    {
		    GenerateMainRoutine(_mainRoutineModule);
	    }
	    
	    _mainModule = new MethodDefinition("Main", MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig, _asm.MainModule.TypeSystem.Void);
	    _typeDef.Methods.Add(_mainModule);
	    _mainModule.Body.InitLocals = true;
	    _mainProc = _mainModule.Body.GetILProcessor();
	    
	    var mainParams = new ParameterDefinition("args", ParameterAttributes.None, _asm.MainModule.TypeSystem.String.MakeArrayType());
	    _mainModule.Parameters.Add(mainParams);
	    
	    StartGeneration(action);
    }

    public void GenerateRecords()
    {
	    foreach (var recDecl in _records)
	    {
		    var name = recDecl._identifier._name;
		    var recType = new TypeDefinition("", name, TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit, _asm.MainModule.TypeSystem.Object);
		    _typeDef.NestedTypes.Add(recType);
		    
		    VariableDeclaration field = recDecl._type._recordType._variableDeclaration;
		    VariableDeclarations fields = recDecl._type._recordType._variableDeclarations;
		    _recFieldDefinitions.Add(name, new List<FieldDefinition>());
		    while (fields != null)
		    {
		     ProcessField(recType, field, name);
		     field = fields._variableDeclaration;
		     fields = fields._variableDeclarations;
		    }
		    ProcessField(recType, field, name);
		    _recTypeDefinitions.Add(name, recType);
		    
		    var ctor_C_4 = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName, _asm.MainModule.TypeSystem.Void);
		    recType.Methods.Add(ctor_C_4);
		    var il_ctor_C_5 = ctor_C_4.Body.GetILProcessor();
		    il_ctor_C_5.Emit(OpCodes.Ldarg_0);
		    il_ctor_C_5.Emit(OpCodes.Call, _asm.MainModule.ImportReference(TypeHelpers.DefaultCtorFor(recType.BaseType)));
		    il_ctor_C_5.Emit(OpCodes.Ret);
		    _constructors.Add(name, ctor_C_4);
	    }
    }

    public void GenerateFunctions()
    {
	    foreach (var funDecl in _functions)
	    {
		    GenerateFuncDecl(funDecl.Value);
	    }
    }

    public void ProcessField(TypeDefinition recType, VariableDeclaration field, string recName)
    {
	    string name = field._identifier._name;
	    Type type = field._type;
	    var fld = new FieldDefinition(name, FieldAttributes.Public, GetTypeRef(type));
	    recType.Fields.Add(fld);
	    _recFieldDefinitions[recName].Add(fld);
    }

    public void StartGeneration(Action action)
    {
	    Actions? actions = action._actions;
	    // MainRoutine? mainRoutine = null;

	    while (actions != null)
	    {
		    GenerateAction(action);
		    action = actions._action;
		    actions = action._actions;
	    }
	    GenerateAction(action);

	    if (_mainRoutine != null)
	    {
		    CallMainRoutine(_mainRoutineModule);
	    }

	    _mainProc.Emit(OpCodes.Ret);

	    var ctorMethod = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName, _asm.MainModule.TypeSystem.Void);
	    _typeDef.Methods.Add(ctorMethod);
	    var ctorProc = ctorMethod.Body.GetILProcessor();
	    ctorProc.Emit(OpCodes.Ldarg_0);
	    ctorProc.Emit(OpCodes.Call, _asm.MainModule.ImportReference(TypeHelpers.DefaultCtorFor(_typeDef.BaseType)));
	    ctorProc.Emit(OpCodes.Ret);
	    _asm.EntryPoint = _mainModule;
      
	    _asm.Write(_path);
	    File.Copy(
		    Path.ChangeExtension(typeof(Generator).Assembly.Location, ".runtimeconfig.json"),
		    Path.ChangeExtension(_path, ".runtimeconfig.json"),
		    true);
    }

    public void GenerateAction(Action action)
    {
	    if (action._declaration != null)
	    {
		    if (action._declaration._variableDeclaration != null)
		    {
			    GenerateVarDecl(action._declaration._variableDeclaration, _mainModule, _mainProc, null);
		    }
		    else if (action._declaration._typeDeclaration != null)
		    {
			    _typeDeclarations.Add(action._declaration._typeDeclaration._identifier._name, action._declaration._typeDeclaration._type);
		    }
	    }
	    else if (action._statement != null)
	    {
		    GenerateStmt(action._statement, null, _mainProc, _mainModule);
	    } // else error
    }

    public void GenerateMainRoutine(MethodDefinition funModule)
    {
	    string name = "main";
	    Body? body = _mainRoutine!._body;
	    
	    // var funModule = new MethodDefinition(name, MethodAttributes.Assembly | MethodAttributes.Static | MethodAttributes.HideBySig, _asm.MainModule.TypeSystem.Void);
	    _typeDef.Methods.Add(funModule);
	    funModule.Body.InitLocals = true;
	    var funProc = funModule.Body.GetILProcessor();
	    
	    _funs.Add(name, funModule);
	    _funsProcs.Add(name, funProc);
	    
	    while (body != null)
	    {
		    GenerateBody(body, null, funProc, funModule);
		    body = body._body;
	    }
	    funProc.Emit(OpCodes.Ret);
    }

    public void CallMainRoutine(MethodDefinition funModule)
    {
	    _mainProc.Emit(OpCodes.Call, funModule);
    }

    public void GenerateFuncDecl(Function func)
    {
	    string? name = func._identifier._name;
	    Parameters? parameters = func._parameters;
	    Type? returnType = func._routineReturnType._type; // in the fun always should be return
	    Body? body = func._routineInsights._body;
	    
	    // var funModule = new MethodDefinition(name, MethodAttributes.Assembly | MethodAttributes.Static | MethodAttributes.HideBySig, GetTypeRef(returnType!));
	    var funModule = new MethodDefinition(name, MethodAttributes.Static | MethodAttributes.Public | MethodAttributes.HideBySig, GetTypeRef(returnType!));
	    _typeDef.Methods.Add(funModule);
	    funModule.Body.InitLocals = true;
	    var funProc = funModule.Body.GetILProcessor();
	   
	    _funs.Add(name, funModule);
	    _funsProcs.Add(name, funProc);

	    int i = 0;
	    while (parameters != null)
	    {
		    ParameterDeclaration? pd = parameters._parameterDeclaration;
		    parameters = parameters._parameters;
		    if (pd != null) GenerateParamDecl(pd, funModule, name, i);
		    i++;
	    }

	    while (body != null)
	    {
		    GenerateBody(body, GetTypeRef(returnType!), funProc, funModule);
		    body = body._body;
	    }
    }

    public void GenerateBody(Body body, TypeReference returnType, ILProcessor proc, MethodDefinition md)
    {
	    if (body._declaration != null)
	    {
		    if (body._declaration._variableDeclaration != null)
		    {
			    GenerateVarDecl(body._declaration._variableDeclaration, md, proc, null);
		    }
		    else if (body._declaration._typeDeclaration != null)
		    {
			    _typeDeclarations.Add(body._declaration._typeDeclaration._identifier._name, body._declaration._typeDeclaration._type);
		    }
	    }
	    else if (body._statement != null)
	    {
		    GenerateStmt(body._statement, returnType, proc, md);
	    }
	    else if (body._return != null)
	    {
		    Expression exp = body._return._expression;
		    GenerateExpression(exp, proc);
		    
		    // EmitValue(value, proc, returnType);
		    proc.Emit(OpCodes.Ret);
	    } // else error
    }

    public void GenerateStmt(Statement stmt, TypeReference returnType, ILProcessor proc, MethodDefinition md)
    {
	    if (stmt._assignment != null)
	    {
		    GenerateAss(stmt._assignment, returnType, proc, md);
	    }
	    else if (stmt._whileLoop != null)
	    {
		    GenerateWhile(stmt._whileLoop, returnType, proc, md);
	    }
	    else if (stmt._forLoop != null)
	    {
		    GenerateFor(stmt._forLoop, returnType, proc, md);
	    }
	    else if (stmt._ifStatement != null)
	    {
		    GenerateIf(stmt._ifStatement, returnType, proc, md);
	    }
	    else if (stmt._routineCall != null)
	    {
		    RoutineCall rc = stmt._routineCall;
		    Expressions callParams = rc._expressions;
		    GenerateExpressions(callParams, proc);
		    
		    proc.Emit(OpCodes.Call, _funs[rc._identifier._name]);
		    if (_functions[rc._identifier._name]._routineReturnType != null)
		    {
			    proc.Emit(OpCodes.Pop);
			    // proc.Emit(OpCodes.Ret);
		    }
	    }
    }

    public void GenerateAss(Assignment ass, TypeReference returnType, ILProcessor proc, MethodDefinition md)
    {
	    Variable v = ass._variable;

	    if (v._arrayType != null)
	    {
		    Expression expInd = v._arrayType._arrayType._expression; // index

		    proc.Emit(OpCodes.Ldloc, _vars[v._identifier._name]);
		    GenerateExpression(expInd, proc);

		    GenerateRightAss(ass, proc);
		    Type type = _varsTypes[v._identifier._name]._arrayType._type;
		    if (type._primitiveType._isBoolean)
		    {
			    proc.Emit(OpCodes.Stelem_Any, _asm.MainModule.TypeSystem.Boolean);
		    }
		    else if (type._primitiveType._isInt)
		    {
			    proc.Emit(OpCodes.Stelem_I4);
		    }
		    else if (type._primitiveType._isReal)
		    {
			    proc.Emit(OpCodes.Stelem_R8);
		    }
		    
		    if (_varsTypes[v._identifier._name]._arrayType._type._primitiveType._isInt)
		    {
			    proc.Emit(OpCodes.Ldloc, _vars[v._identifier._name]);
			    GenerateExpression(expInd, proc);
			    proc.Emit(OpCodes.Ldelem_I4);
			    proc.Emit(OpCodes.Call, _asm.MainModule.ImportReference(TypeHelpers.ResolveMethod(typeof(System.Console), "WriteLine",System.Reflection.BindingFlags.Default|System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.Public, "System.Int32")));
		    }
		    else if (_varsTypes[v._identifier._name]._arrayType._type._primitiveType._isReal)
		    {
			    // PrintElement( _vars[v._identifier._name], "System.Double", 1);
			    proc.Emit(OpCodes.Ldloc, _vars[v._identifier._name]);
			    GenerateExpression(expInd, proc);
			    proc.Emit(OpCodes.Ldelem_R8);
			    proc.Emit(OpCodes.Call, _asm.MainModule.ImportReference(TypeHelpers.ResolveMethod(typeof(System.Console), "WriteLine",System.Reflection.BindingFlags.Default|System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.Public, "System.Double")));

			    // PrintElement( _vars[v._identifier._name], "System.Double", 0);
		    }
		    else if (_varsTypes[v._identifier._name]._arrayType._type._primitiveType._isBoolean)
		    {
			    proc.Emit(OpCodes.Ldloc, _vars[v._identifier._name]);
			    GenerateExpression(expInd, proc);
			    proc.Emit(OpCodes.Ldelem_U1);
			    proc.Emit(OpCodes.Call, _asm.MainModule.ImportReference(TypeHelpers.ResolveMethod(typeof(System.Console), "WriteLine",System.Reflection.BindingFlags.Default|System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.Public, "System.Boolean")));
		    }
	    }
	    else
	    {
		    GenerateRightAss(ass, proc);
		    if (_vars.Keys.Contains(v._identifier._name))
		    {
			    proc.Emit(OpCodes.Stloc, _vars[v._identifier._name]);
		    }
		    else
		    {
			    proc.Emit(OpCodes.Starg_S, _paramsDefinitions[v._identifier._name].Item2);
		    }
		    
		    if (_varsTypes[v._identifier._name]._primitiveType._isInt)
		    {
			    Print(_vars[v._identifier._name], "System.Int32", proc);
		    }
		    else if (_varsTypes[v._identifier._name]._primitiveType._isReal)
		    {
			    Print(_vars[v._identifier._name], "System.Double", proc);
		    }
		    
		    // Print( _vars[v._identifier._name], "System.Int32");
	    }
    }

    public void GenerateRightAss(Assignment ass, ILProcessor proc)
    {
	    Variable v = ass._variable;
	    
	    if (ass._expressions != null)
	    {
		    Expressions exp = ass._expressions;
		    GenerateExpression(exp._expression, proc);
			    
		    Type type = _varsTypes[v._identifier._name];
		    // EmitValue(value, proc, GetTypeRef(type));
		    
	    }
	    else if (ass._routineCall != null)
	    {
		    RoutineCall rc = ass._routineCall;
		    Expressions callParams = rc._expressions;
		    GenerateExpressions(callParams, proc);
		    
		    proc.Emit(OpCodes.Call, _funs[rc._identifier._name]);
		    if (_functions[rc._identifier._name]._routineReturnType != null)
		    {
			    // proc.Emit(OpCodes.Pop);
			    // proc.Emit(OpCodes.Ret);
		    }
	    }
    }

    public void GenerateWhile(WhileLoop loop, TypeReference returnType, ILProcessor proc, MethodDefinition md)
    {
	    var condDef = new VariableDefinition(_asm.MainModule.TypeSystem.Boolean);
	    md.Body.Variables.Add(condDef);
	    GenerateCondition(loop._expression, proc, condDef);
	    
	    // int i = 0;
	    var var_i = new VariableDefinition(_asm.MainModule.TypeSystem.Int32);
	    md.Body.Variables.Add(var_i);
	    proc.Emit(OpCodes.Ldc_I4, 0);
	    proc.Emit(OpCodes.Stloc, var_i);

	    var lblFel = proc.Create(OpCodes.Nop);
	    var nop = proc.Create(OpCodes.Nop);
	    proc.Append(nop);
	    
	    proc.Emit(OpCodes.Ldloc, condDef); // while condDef is not false
	    proc.Emit(OpCodes.Brfalse, lblFel);

	    Body body = loop._body;
	    while (body != null)
	    {
		    GenerateBody(body, returnType, proc, md);
		    GenerateCondition(loop._expression, proc, condDef);
		    body = body._body;
	    }
	    
	    proc.Emit(OpCodes.Ldloc, var_i);
	    proc.Emit(OpCodes.Dup);
	    proc.Emit(OpCodes.Ldc_I4_1);
	    proc.Emit(OpCodes.Add);
	    proc.Emit(OpCodes.Stloc, var_i);
	    proc.Emit(OpCodes.Pop);
	    proc.Emit(OpCodes.Br, nop);
	    proc.Append(lblFel);
    }

    public void GenerateCondition(Expression exp, ILProcessor proc, VariableDefinition condDef)
    {
	    // generate condition
	    // proc.Emit(OpCodes.Ldc_I4, 3); // 3 
	    // proc.Emit(OpCodes.Ldc_I4, 6); // 6 
	    // proc.Emit(OpCodes.Cgt); // > 
	    GenerateExpression(exp, proc);
	    proc.Emit(OpCodes.Stloc, condDef);
    }

    public void GenerateFor(ForLoop loop, TypeReference returnType, ILProcessor proc, MethodDefinition md)
    {
	    string name = loop._identifier._name;
	    Body body = loop._body;

	    Expression from = loop._range._from;
	    Expression to = loop._range._to;

	    var var_i = new VariableDefinition(_asm.MainModule.TypeSystem.Int32);
	    md.Body.Variables.Add(var_i);
	    GenerateExpression(from, proc);
	    proc.Emit(OpCodes.Stloc, var_i);
		    
	    var lblFel = proc.Create(OpCodes.Nop);
	    var nop = proc.Create(OpCodes.Nop);
	    proc.Append(nop);
		    
	    proc.Emit(OpCodes.Ldloc, var_i);
	    GenerateExpression(to, proc);
		    
	    if (loop._reverse)
	    {
		    proc.Emit(OpCodes.Cgt);
	    }
	    else
	    {
		    proc.Emit(OpCodes.Clt);
	    }
	    proc.Emit(OpCodes.Brfalse, lblFel);
		    
	    _vars.Add(name, var_i);
	    _varsTypes.Add(name, new Type(new PrimitiveType(true, false, false), null, null, null));
		    
	    while (body != null)
	    {
			GenerateBody(body, returnType, proc, md);
		    body = body._body;
	    }
	    proc.Emit(OpCodes.Ldloc, var_i);
	    proc.Emit(OpCodes.Dup);
	    proc.Emit(OpCodes.Ldc_I4_1);
	    if (loop._reverse)
	    {
		    proc.Emit(OpCodes.Sub);
	    }
	    else
	    {
		    proc.Emit(OpCodes.Add);
	    }
	    proc.Emit(OpCodes.Stloc, var_i);
	    proc.Emit(OpCodes.Pop);
	    proc.Emit(OpCodes.Br, nop);
	    proc.Append(lblFel);
	    // proc.Emit(OpCodes.Ret);
    }

    public void GenerateIf(IfStatement stmt, TypeReference returnType, ILProcessor proc, MethodDefinition md)
    {
	    Expression exp = stmt._condition;
	    Body ifb = stmt._ifBody;
	    Body elb = stmt._elseBody;

	    GenerateExpression(exp, proc);
		    
	    var elseEntryPoint = proc.Create(OpCodes.Nop);
	    proc.Emit(OpCodes.Brfalse, elseEntryPoint);
	    while (ifb != null)
	    {
			GenerateBody(ifb, returnType, proc, md);
		    ifb = ifb._body;
	    }
	    var elseEnd = proc.Create(OpCodes.Nop);

	    if (elb != null)
	    {
		    var endOfIf = proc.Create(OpCodes.Br, elseEnd);
		    proc.Append(endOfIf);
		    proc.Append(elseEntryPoint);
		    // else
		    while (elb != null)
		    {
			    GenerateBody(elb, returnType, proc, md);
			    elb = elb._body;
		    }
	    }
	    else
	    {
		    proc.Append(elseEntryPoint);
	    }
	    proc.Append(elseEnd);
	    md.Body.OptimizeMacros();
    }

    public void GenerateParamDecl(ParameterDeclaration pd, MethodDefinition md, string funName, int ind)
    {
	    string? name = pd._identifier._name;
	    Type type = pd._type;
	    
	    var paramDef = new ParameterDefinition(name, ParameterAttributes.None, GetTypeRef(type));
	    md.Parameters.Add(paramDef);
	    _paramsDefinitions.Add(name, new Tuple<int, ParameterDefinition, Type>(ind, paramDef, type));
    }

    public void EmitValue(string value, ILProcessor proc, TypeReference type)
    {
	    if (type.Equals(_asm.MainModule.TypeSystem.Int32))
	    {
		    proc.Emit(OpCodes.Ldc_I4, Int32.Parse(value));
	    }
	    else if (type.Equals(_asm.MainModule.TypeSystem.Double))
	    {
		    proc.Emit(OpCodes.Ldc_R8, Double.Parse(value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo));
	    }
	    else if (type.Equals(_asm.MainModule.TypeSystem.Boolean))
	    {
		    if (value.Equals("true"))
		    {
			    proc.Emit(OpCodes.Ldc_I4, 1);
		    }
		    else if (value.Equals("false"))
		    {
			    proc.Emit(OpCodes.Ldc_I4, 0);
		    }
	    }
    }

    public TypeReference GetTypeRef(Type type)
    {
	    if (type._primitiveType != null)
	    {
		    if (type._primitiveType._isInt)
		    {
			    return _asm.MainModule.TypeSystem.Int32;
		    }
	
		    if (type._primitiveType._isReal)
		    {
			    return _asm.MainModule.TypeSystem.Double;
		    }
		    if (type._primitiveType._isBoolean)
		    {
			    return _asm.MainModule.TypeSystem.Boolean;
		    }
	    } 
	    // else if (type._arrayType != null)
	    // {
	    // }
	    // else if (type._recordType != null)
	    // {
	    // }
	    return null;
    }

    public void GenerateVarDecl(VariableDeclaration varDecl, MethodDefinition md, ILProcessor proc, string? name)
    {
	    if (name == null) { name = varDecl._identifier._name; }
	    Type type = varDecl._type;
	    Value? value = varDecl._value;
	    
	    if (type._primitiveType != null)
	    {
		    var varDef = new VariableDefinition(GetTypeRef(type));
		    md.Body.Variables.Add(varDef);

		    if (value != null)
		    {
			    GenerateExpression(value._expressions._expression, proc);
			    proc.Emit(OpCodes.Stloc, varDef);

			    
			    if (type._primitiveType._isInt || type._primitiveType._isBoolean)
			    {
				    Print(varDef, "System.Int32", proc);
			    }
			    else if (type._primitiveType._isReal)
			    {
				    Print(varDef, "System.Double", proc);
			    }
		    }

		    _vars.Add(name, varDef);
		    _varsTypes.Add(name, type);
		    
		    // Print(varDef, "System.Double");
	    }
	    else if (type._arrayType != null)
	    {
		    ArrayType at = type._arrayType;
		    Expression exp = at._expression; // length
		    Type t = at._type;

		    var arr = new VariableDefinition(GetTypeRef(t).MakeArrayType());
		    md.Body.Variables.Add(arr);
		    GenerateExpression(exp, proc);
		    proc.Emit(OpCodes.Newarr, GetTypeRef(t));
		    proc.Emit(OpCodes.Stloc, arr);
		    
		    _vars.Add(name, arr);
		    _varsTypes.Add(name, type);
	    }
	    else if (type._userType != null && _typeDeclarations.Keys.Contains(type._userType._name))
	    {
		    VariableDeclaration vewVarDecl = new VariableDeclaration(varDecl._identifier,
			    _typeDeclarations[type._userType._name], varDecl._value);
		    GenerateVarDecl(vewVarDecl, md, proc, name);
	    }
	    else if (type._userType != null)
	    {
		    RecordType recordType = null;
		    Type t = null;
		    foreach (var record in _records)
		    {
			    if (record._identifier._name.Equals(type._userType._name))
			    {
				    recordType = record._type._recordType;
				    t = record._type;
				    break;
			    }
		    }
		    
		    var recordDefinition = new VariableDefinition(_recTypeDefinitions[type._userType._name]);
		    md.Body.Variables.Add(recordDefinition);
		    proc.Emit(OpCodes.Newobj, _constructors[type._userType._name]);
		    proc.Emit(OpCodes.Stloc, recordDefinition);
		    
		    // var recordDefinition = new VariableDefinition(_recTypeDefinitions[type._userType._name]);
		    // md.Body.Variables.Add(recordDefinition);
		    if (value != null)
		    {
			    int i = 0;
			    VariableDeclaration vd = recordType._variableDeclaration;
			    VariableDeclarations vds = recordType._variableDeclarations;
			    Expressions expressions = value._expressions;
			    Expression expression = null;
			    List<FieldDefinition> fieldDefs = _recFieldDefinitions[type._userType._name];
			    while (expressions != null)
			    {
				    expression = expressions._expression;
				    expressions = expressions._expressions;
				    ProcessField(expression, recordDefinition, proc, fieldDefs[i]);
			    
				    if (vd._type._primitiveType._isInt || vd._type._primitiveType._isBoolean)
				    {
					    proc.Emit(OpCodes.Ldloc, recordDefinition);
					    proc.Emit(OpCodes.Ldfld, fieldDefs[i]);
					    proc.Emit(OpCodes.Call, _asm.MainModule.ImportReference(TypeHelpers.ResolveMethod(typeof(System.Console), "WriteLine",System.Reflection.BindingFlags.Default|System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.Public, "System.Int32")));
				    }
				    else if (vd._type._primitiveType._isReal)
				    {
					    proc.Emit(OpCodes.Ldloc, recordDefinition);
					    proc.Emit(OpCodes.Ldfld, fieldDefs[i]);
					    proc.Emit(OpCodes.Call, _asm.MainModule.ImportReference(TypeHelpers.ResolveMethod(typeof(System.Console), "WriteLine",System.Reflection.BindingFlags.Default|System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.Public, "System.Double")));
				    }

				    if (vds != null)
				    {
					    vd = vds._variableDeclaration;
					    vds = vds._variableDeclarations;
				    }
				    i++;
			    }
		    }
		    
		    _varsTypes.Add(name, t);
		    _vars.Add(name, recordDefinition);
	    }
    }

    public void ProcessField(Expression field, VariableDefinition def, ILProcessor proc, FieldDefinition fieldDefinition)
    {
	    proc.Emit(OpCodes.Ldloc, def);
	    GenerateExpression(field, proc);
	    proc.Emit(OpCodes.Stfld, fieldDefinition);
    }

    public void GenerateExpressions(Expressions? exp, ILProcessor proc)
    {
	    if (exp != null)
	    {
		    Expression expression = exp._expression;
		    Expressions? expressions = exp._expressions;
		    while (expressions != null)
		    {
			    GenerateExpression(expression, proc);
			    expression = expressions._expression;
			    expressions = expressions._expressions;
		    }
		    GenerateExpression(expression, proc);
	    }
    }
    public void GenerateExpression(Expression? exp, ILProcessor proc)
    {
	    if (exp != null)
		    GenerateOperation(exp._relation._operation, proc);
    }

    public void GenerateOperation(Operation op, ILProcessor proc)
    {
	    GenerateOperand(op._operand, proc);

	    if (op._operation != null)
	    {
		    if (op._operation._operator == null)
		    {
			    GenerateOperation(op._operation, proc);
			    GenerateOperator(op._operator, proc);
		    }
		    else
		    {
			    GenerateOperand(op._operation._operand, proc);
			    GenerateOperator(op._operator, proc);
			    
			    GenerateOperation(op._operation._operation, proc);
			    GenerateOperator(op._operation._operator, proc);
		    }
	    }
    }

    public void GenerateLDARG(int i, ILProcessor proc)
    {
	    if (i == 0)
	    {
		    proc.Emit(OpCodes.Ldarg_0);
	    }
	    else if (i == 1)
	    {
		    proc.Emit(OpCodes.Ldarg_1);
	    }
	    else if (i == 2)
	    {
		    proc.Emit(OpCodes.Ldarg_2);
	    }
	    else if (i == 3)
	    {
		    proc.Emit(OpCodes.Ldarg_3);
	    }
	    else
	    {
		    proc.Emit(OpCodes.Ldarg, i);
	    }
    }

    public void GenerateOperand(Operand operand, ILProcessor proc)
    {
	    if (operand._expression != null)
		    GenerateOperation(operand._expression._relation._operation, proc);
	    else if (operand._single._variable != null)
	    {
		    string name = operand._single._variable._identifier._name;
		    if (_vars.Keys.Contains(operand._single._variable._identifier._name))
		    {
			    proc.Emit(OpCodes.Ldloc, _vars[name]);
		    }
		    else
		    {
			    GenerateLDARG(_paramsDefinitions[name].Item1, proc);
		    }
		    
		    if (_varsTypes.Keys.Contains(name) && _varsTypes[name]._arrayType != null)
		    {
			    Expression index = operand._single._variable._arrayType._arrayType._expression;
			    //Expression indedex = _varsTypes[operand._single._variable._identifier._name]._arrayType._expression;
			    GenerateExpression(index, proc);
			    Type type = _varsTypes[name]._arrayType._type;
			    if (type._primitiveType._isInt)
			    {
				    proc.Emit(OpCodes.Ldelem_I4);
			    }
			    else if (type._primitiveType._isBoolean)
			    {
				    proc.Emit(OpCodes.Ldelem_U1);
			    }
			    else if (type._primitiveType._isReal)
			    {
				    proc.Emit(OpCodes.Ldelem_R8);
			    }
		    }
		    else if (_paramsDefinitions.Keys.Contains(name) && _paramsDefinitions[name].Item3._arrayType != null)
		    {
			    Expression index = operand._single._variable._arrayType._arrayType._expression;
			    //Expression indedex = _varsTypes[operand._single._variable._identifier._name]._arrayType._expression;
			    GenerateExpression(index, proc);
			    Type type = _paramsDefinitions[name].Item3._arrayType._type;
			    if (type._primitiveType._isInt)
			    {
				    proc.Emit(OpCodes.Ldelem_I4);
			    }
			    else if (type._primitiveType._isBoolean)
			    {
				    proc.Emit(OpCodes.Ldelem_U1);
			    }
			    else if (type._primitiveType._isReal)
			    {
				    proc.Emit(OpCodes.Ldelem_R8);
			    }
		    }
	    }
	    else
		    EmitValue(operand._single._value, proc, GetTypeRef(operand._single._type));
	    
    }

    public void GenerateOperator(Operator op, ILProcessor proc)
    {
	    if (op._mathematicalOperator != null)
		    GenerateMathOp(op._mathematicalOperator, proc);
	    else if (op._comparisonOperator != null)
		    GenerateCompOp(op._comparisonOperator, proc);
	    else if(op._logicalOperator != null)
		    GenerateLogicOp(op._logicalOperator, proc);
	    
    }

    public void GenerateMathOp(MathematicalOperator mathOp, ILProcessor proc)
    {
	    string sign = mathOp._sign;
	    switch (sign)
	    {
		    case "+":
			    proc.Emit(OpCodes.Add);
			    break;
		    case "-":
			    proc.Emit(OpCodes.Sub);
			    break;
		    case "*":
			    proc.Emit(OpCodes.Mul);
			    break;
		    case "/":
			    proc.Emit(OpCodes.Div);
			    break;
		    case "%":
			    proc.Emit(OpCodes.Rem);
			    break;
	    }
    }

    public void GenerateCompOp(ComparisonOperator compOp, ILProcessor proc)
    {
	    string sign = compOp._sign;
	    switch (sign)
	    {
		    case ">":
			    proc.Emit(OpCodes.Cgt);
			    break;
		    case ">=":
			    proc.Emit(OpCodes.Clt);
			    proc.Emit(OpCodes.Ldc_I4_0);
			    proc.Emit(OpCodes.Ceq);
			    break;
		    case "<":
			    proc.Emit(OpCodes.Clt);
			    break;
		    case "<=":
			    proc.Emit(OpCodes.Cgt);
			    proc.Emit(OpCodes.Ldc_I4_0);
			    proc.Emit(OpCodes.Ceq);
			    break;
		    case "==":
			    proc.Emit(OpCodes.Ceq);
			    break;
		    case "!=":
			    proc.Emit(OpCodes.Ceq);
			    proc.Emit(OpCodes.Ldc_I4_0);
			    proc.Emit(OpCodes.Ceq);
			    break;
	    }
    }
    
    public void GenerateLogicOp(LogicalOperator logicOp, ILProcessor proc)
    {
	    string sign = logicOp._sign;
	    switch (sign)
	    {
		    case "and":
			    proc.Emit(OpCodes.And);
			    break;
		    case "or":
			    proc.Emit(OpCodes.Or);
			    break;
		    case "xor":
			    proc.Emit(OpCodes.Xor);
			    break;
	    }
    }

    public void Print(VariableDefinition varDef, string type, ILProcessor proc)
    {
	    // type = "System.Double";
	    // type = "System.String";
	    
	    proc.Emit(OpCodes.Ldloc, varDef);
	    proc.Emit(OpCodes.Call, _asm.MainModule.ImportReference(TypeHelpers.ResolveMethod(typeof(System.Console), "WriteLine",System.Reflection.BindingFlags.Default|System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.Public, type)));
    }
    
    public void PrintElement(VariableDefinition arrDef, string type, int index)
    {
	    //Console.WriteLine(a[0]);
	    _mainProc.Emit(OpCodes.Ldloc, arrDef);
	    _mainProc.Emit(OpCodes.Ldc_I4, index);
	    _mainProc.Emit(OpCodes.Ldelem_I4);
	    _mainProc.Emit(OpCodes.Call, _asm.MainModule.ImportReference(TypeHelpers.ResolveMethod(typeof(System.Console), "WriteLine",System.Reflection.BindingFlags.Default|System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.Public, type)));
    }
}
    
    
    
