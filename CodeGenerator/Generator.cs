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
    private Dictionary<string, Type> _varsTypes;
    private Dictionary<string, VariableDefinition> _vars;
    private Dictionary<string, MethodDefinition> _funs;
    private Dictionary<string, ILProcessor> _funsProcs;
    
    public Generator(Action action)
    {
	    Processing processing = new Processing();
	    _records = processing.FindRecords(action);
	    
	    _varsTypes = new Dictionary<string, Type>();
	    _vars = new Dictionary<string, VariableDefinition>();
	    _funs = new Dictionary<string, MethodDefinition>();
	    _funsProcs = new Dictionary<string, ILProcessor>();
	    
	    _mainRoutine = null;
	    
	    var mp = new ModuleParameters { Architecture = TargetArchitecture.AMD64, Kind =  ModuleKind.Console, ReflectionImporterProvider = new SystemPrivateCoreLibFixerReflectionProvider() };
	    _asm = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition("Program", Version.Parse("1.0.0.0")), Path.GetFileName(_path), mp);
	    
	    _typeDef = new TypeDefinition("", "Program", TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.Public, _asm.MainModule.TypeSystem.Object);
	    _asm.MainModule.Types.Add(_typeDef);
	    GenerateRecords();
	    
	    _mainModule = new MethodDefinition("Main", MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig, _asm.MainModule.TypeSystem.Void);
	    _typeDef.Methods.Add(_mainModule);
	    _mainModule.Body.InitLocals = true;
	    _mainProc = _mainModule.Body.GetILProcessor();
	    
	    var mainParams = new ParameterDefinition("args", ParameterAttributes.None, _asm.MainModule.TypeSystem.String.MakeArrayType());
	    _mainModule.Parameters.Add(mainParams);
	    
	    StartGeneration(action);

        example();
    }

    public void GenerateRecords()
    {
	    foreach (var recDecl in _records)
	    {
		    var name = recDecl._identifier._name;
		    var recType = new TypeDefinition("", name, TypeAttributes.Sealed |TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.SequentialLayout, _asm.MainModule.ImportReference(typeof(System.ValueType)));
		    _typeDef.NestedTypes.Add(recType);

		    VariableDeclaration field = recDecl._type._recordType._variableDeclaration;
		    VariableDeclarations fields = recDecl._type._recordType._variableDeclarations;
		    while (fields != null)
		    {
			    ProcessField(recType, field);
			    field = fields._variableDeclaration;
			    fields = fields._variableDeclarations;
		    }
		    ProcessField(recType, field);
	    }
    }

    public void ProcessField(TypeDefinition recType, VariableDeclaration field)
    {
	    string name = field._identifier._name;
	    Type type = field._type;
	    var fld = new FieldDefinition(name, FieldAttributes.Public, GetTypeRef(type));
	    recType.Fields.Add(fld);
    }

    public void StartGeneration(Action action)
    {
	    Actions? actions = action._actions;
	    MainRoutine? mainRoutine = null;

	    while (actions != null)
	    {
		    GenerateAction(action);
		    action = actions._action;
		    actions = actions._actions;
	    }
	    GenerateAction(action);
	    if (_mainRoutine != null)
	    {
		    GenerateMainRoutine();
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
		    if (action._declaration._routineDeclaration != null)
		    {
			    if (action._declaration._routineDeclaration._mainRoutine != null)
			    {
				    _mainRoutine = action._declaration._routineDeclaration._mainRoutine;
			    } 
			    else if (action._declaration._routineDeclaration._function != null)
			    {
				    GenerateFuncDecl(action._declaration._routineDeclaration._function);
			    }
		    }
		    else if (action._declaration._typeDeclaration != null)
		    {
			    // todo
		    }
		    else if (action._declaration._variableDeclaration != null)
		    {
			    GenerateVarDecl(action._declaration._variableDeclaration, _mainModule, _mainProc, null);
		    }
	    }
	    else if (action._statement != null)
	    {
		    GenerateStmt(action._statement, null, _mainProc, _mainModule);
	    } // else error
    }
    
    public void GenerateMainRoutine()
    {
	    string name = "main";
	    Body? body = _mainRoutine!._body;
	    
	    var funModule = new MethodDefinition(name, MethodAttributes.Assembly | MethodAttributes.Static | MethodAttributes.HideBySig, _asm.MainModule.TypeSystem.Void);
	    _typeDef.Methods.Add(funModule);
	    funModule.Body.InitLocals = true;
	    var funProc = funModule.Body.GetILProcessor();

	    GenerateBody(body, null, funProc, funModule);
    }

    public void GenerateFuncDecl(Function func)
    {
	    string? name = func._identifier._name;
	    Parameters? parameters = func._parameters;
	    Type? returnType = func._routineReturnType._type; // in the fun always should be return
	    Body? body = func._routineInsights._body;
	    
	    var funModule = new MethodDefinition(name, MethodAttributes.Assembly | MethodAttributes.Static | MethodAttributes.HideBySig, GetTypeRef(returnType!));
	    _typeDef.Methods.Add(funModule);
	    funModule.Body.InitLocals = true;
	    var funProc = funModule.Body.GetILProcessor();
	    
	    while (parameters != null)
	    {
		    ParameterDeclaration? pd = parameters._parameterDeclaration;
		    parameters = parameters._parameters;
		    if (pd != null) GenerateParamDecl(pd, funModule);
	    }

	    GenerateBody(body, GetTypeRef(returnType!), funProc, funModule);
    }

    public void GenerateBody(Body body, TypeReference returnType, ILProcessor proc, MethodDefinition md)
    {
	    if (body._declaration != null)
	    {
		    if (body._declaration._routineDeclaration != null)
		    {
			    if (body._declaration._routineDeclaration._mainRoutine != null)
			    {
				    _mainRoutine = body._declaration._routineDeclaration._mainRoutine;
			    } 
			    else if (body._declaration._routineDeclaration._function != null)
			    {
				    GenerateFuncDecl(body._declaration._routineDeclaration._function);
			    }
		    }
		    else if (body._declaration._typeDeclaration != null)
		    {
			    // todo
		    }
		    else if (body._declaration._variableDeclaration != null)
		    {
			    GenerateVarDecl(body._declaration._variableDeclaration, md, proc, null);
		    }
	    }
	    else if (body._statement != null)
	    {
		    GenerateStmt(body._statement, returnType, proc, md);
	    }
	    else if (body._return != null)
	    {
		    Expression exp = body._return._expression;
		    string value = ""; // todo something with exp
		    
		    EmitValue(value, proc, returnType);
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
		    ILProcessor p = _funsProcs[stmt._routineCall._identifier._name];
		    p.Emit(OpCodes.Call, _funs[stmt._routineCall._identifier._name]);
		    proc.Emit(OpCodes.Ret);
	    }
    }

    public void GenerateAss(Assignment ass, TypeReference returnType, ILProcessor proc, MethodDefinition md)
    {
	    Variable v = ass._variable;

	    if (v._arrayType != null)
	    {
		    Expression expInd = v._arrayType._arrayType._expression; // index
		    int ind = 0; // todo
		    
		    proc.Emit(OpCodes.Ldloc, _funs[v._identifier._name]);
		    proc.Emit(OpCodes.Ldc_I4, ind);

		    GenerateRightAss(ass, proc);
		    proc.Emit(OpCodes.Stelem_Ref);
	    }
	    else
	    {
		    GenerateRightAss(ass, proc);
		    proc.Emit(OpCodes.Stloc, _vars[v._identifier._name]);
	    }
    }

    public void GenerateRightAss(Assignment ass, ILProcessor proc)
    {
	    Variable v = ass._variable;
	    
	    if (ass._expressions != null)
	    {
		    Expressions exp = ass._expressions;
		    string value = ""; // todo value if var and if no var
			    
		    Type type = _varsTypes[v._identifier._name];
		    EmitValue(value, proc, GetTypeRef(type));
	    }
	    else if (ass._routineCall != null)
	    {
		    RoutineCall rc = ass._routineCall;
		    Expressions callParams = rc._expressions; // todo callParams
		    proc.Emit(OpCodes.Call, _funs[rc._identifier._name]);
	    }
    }

    public void GenerateWhile(WhileLoop loop, TypeReference returnType, ILProcessor proc, MethodDefinition md)
    {
	    // todo
    }

    public void GenerateFor(ForLoop loop, TypeReference returnType, ILProcessor proc, MethodDefinition md)
    {
	    string name = loop._identifier._name;
	    Body body = loop._body;

	    Expression from = loop._range._from;
	    Expression to = loop._range._to;

	    var var_i = new VariableDefinition(_asm.MainModule.TypeSystem.Int32);
	    md.Body.Variables.Add(var_i);
	    proc.Emit(OpCodes.Ldc_I4, 5); // todo from
	    proc.Emit(OpCodes.Stloc, var_i);
		    
	    var lblFel = proc.Create(OpCodes.Nop);
	    var nop = proc.Create(OpCodes.Nop);
	    proc.Append(nop);
		    
	    proc.Emit(OpCodes.Ldloc, var_i);
	    proc.Emit(OpCodes.Ldc_I4, 1); // todo to
		    
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
		    
	    GenerateBody(body, returnType, proc, md);
    }

    public void GenerateIf(IfStatement stmt, TypeReference returnType, ILProcessor proc, MethodDefinition md)
    {
	    Expression exp = stmt._condition;
	    Body ifb = stmt._ifBody;
	    Body elb = stmt._elseBody;
		    
	    // todo make normal condition
	    proc.Emit(OpCodes.Ldloc, 6); // 6
	    proc.Emit(OpCodes.Ldc_I4, 6); // 6
	    proc.Emit(OpCodes.Ceq); // =
		    
	    var elseEntryPoint = proc.Create(OpCodes.Nop);
	    proc.Emit(OpCodes.Brfalse, elseEntryPoint);
		    
	    GenerateBody(ifb, returnType, proc, md);
	    var elseEnd = proc.Create(OpCodes.Nop);

	    if (elb != null)
	    {
		    var endOfIf = proc.Create(OpCodes.Br, elseEnd);
		    proc.Append(endOfIf);
		    proc.Append(elseEntryPoint);
		    // else
		    GenerateBody(elb, returnType, proc, md);
	    }
	    else
	    {
		    proc.Append(elseEntryPoint);
	    }
	    proc.Append(elseEnd);
	    md.Body.OptimizeMacros();
    }

    public void GenerateParamDecl(ParameterDeclaration pd, MethodDefinition md) // no kostil
    {
	    string? name = pd._identifier._name;
	    Type type = pd._type;
	    
	    var paramDef = new ParameterDefinition(name, ParameterAttributes.None, GetTypeRef(type));
	    md.Parameters.Add(paramDef);
    }

    public void EmitValue(string value, ILProcessor proc, TypeReference type)
    {
	    if (type.Equals(_asm.MainModule.TypeSystem.Int32))
	    {
		    proc.Emit(OpCodes.Ldc_I4, Int32.Parse(value));
	    }
	    else if (type.Equals(_asm.MainModule.TypeSystem.Double))
	    {
		    proc.Emit(OpCodes.Ldc_R8, Double.Parse(value));
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
	    else if (type._arrayType != null)
	    {
		    // todo
	    }
	    else if (type._recordType != null)
	    {
		    // todo
	    }
	    return null;
    }

    public void GenerateVarDecl(VariableDeclaration varDecl, MethodDefinition md, ILProcessor proc, string? name)
    {
	    if (name == null) { name = varDecl._identifier._name; }
	    Type type = varDecl._type;
	    Value? value = varDecl._value;
	    
	    // Double valCostil = Double.Parse(value._expressions._expression._relation._operation._operand._single._value);
	    // OpCode typeOpCodeCostil = OpCodes.Ldc_R8;
	    // TypeReference typeRefCostil = _asm.MainModule.TypeSystem.Double;
	    if (type._primitiveType != null)
	    {
		    var varDef = new VariableDefinition(GetTypeRef(type));
		    _mainModule.Body.Variables.Add(varDef);
	    
		    if (value != null)
			    GenerateExpression(value._expressions._expression, proc);
	    
		    // proc.Emit(typeOpCodeCostil, valCostil);
		    proc.Emit(OpCodes.Stloc, varDef);
	    
		    Print(varDef, "System.Int32");
	    
		    _vars.Add(name, varDef);
		    _varsTypes.Add(name, type);
	    }
	    else if (type._arrayType != null)
	    {
		    ArrayType at = type._arrayType;
		    Expression exp = at._expression; // length
		    Type t = at._type;

		    int len = 0; // todo
		    
		    var arr = new VariableDefinition(GetTypeRef(t).MakeArrayType());
		    md.Body.Variables.Add(arr);
		    proc.Emit(OpCodes.Ldc_I4, len);
		    proc.Emit(OpCodes.Newarr, GetTypeRef(t));
		    proc.Emit(OpCodes.Stloc, arr);
		    
		    _vars.Add(name, arr);
		    _varsTypes.Add(name, type);
	    }
	    else if (type._recordType != null)
	    {
		    // RecordType rt = type._recordType;
		    //
		    // VariableDeclaration v = rt._variableDeclaration;
		    // VariableDeclarations? declarations = rt._variableDeclarations;
		    // while (declarations != null)
		    // {
			   //  GenerateVarDecl(v, md, proc, name + v._identifier._name);
			   //  v = declarations._variableDeclaration;
			   //  declarations = declarations._variableDeclarations;
		    // }
		    // GenerateVarDecl(v, md, proc, name + v._identifier._name);
	    }
    }

    public void GenerateExpression(Expression exp, ILProcessor proc)
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

    public void GenerateOperand(Operand operand, ILProcessor proc)
    {
	    if (operand._expression != null)
		    GenerateOperation(operand._expression._relation._operation, proc);
	    else if (operand._single._variable != null)
	    {
		    proc.Emit(OpCodes.Ldloc, _vars[operand._single._variable._identifier._name]);
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

    public void Print(VariableDefinition varDef, string type)
    {
	    // type = "System.Double";
	    // type = "System.String";
	    
	    _mainProc.Emit(OpCodes.Ldloc, varDef);
	    _mainProc.Emit(OpCodes.Call, _asm.MainModule.ImportReference(TypeHelpers.ResolveMethod(typeof(System.Console), "WriteLine",System.Reflection.BindingFlags.Default|System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.Public, type)));
	    _mainProc.Emit(OpCodes.Ret);
    }
    
    public void example()
    {
	    var path = @"/home/tatiana/RiderProjects/Project_CC/CodeGenerator/file.exe";
	    // var path = @"C:\Users\alena\RiderProjects\compiler\Project_CC\CodeGenerator\file.exe";
	    
	    
        // setup a `reflection importer` to ensure references to System.Private.CoreLib are replaced with references to `netstandard`. 
        
        // setup a reflection importer to ensure references to System.Private.CoreLib are replaced with references to netstandard. 
 
                var mp = new ModuleParameters { Architecture = TargetArchitecture.AMD64, Kind =  ModuleKind.Console, ReflectionImporterProvider = new SystemPrivateCoreLibFixerReflectionProvider() };
    using(var assembly = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition("Program", Version.Parse("1.0.0.0")), Path.GetFileName(path), mp))
        {

      //Class : Program
      var cls_Program_0 = new TypeDefinition("", "Program", TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.Public, assembly.MainModule.TypeSystem.Object);
      assembly.MainModule.Types.Add(cls_Program_0);
      
      //Method : Main
      var md_Main_1 = new MethodDefinition("Main", MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig, assembly.MainModule.TypeSystem.Void);
      cls_Program_0.Methods.Add(md_Main_1);
      md_Main_1.Body.InitLocals = true;
      var il_Main_2 = md_Main_1.Body.GetILProcessor();

      //Parameters of 'public static void Main(string[] args)...'
      var p_args_3 = new ParameterDefinition("args", ParameterAttributes.None, assembly.MainModule.TypeSystem.String.MakeArrayType());
      md_Main_1.Parameters.Add(p_args_3);

      //int y = 2+3;
      var lv_y_4 = new VariableDefinition(assembly.MainModule.TypeSystem.Int32);
      md_Main_1.Body.Variables.Add(lv_y_4);
      il_Main_2.Emit(OpCodes.Ldc_I4, 2);
      il_Main_2.Emit(OpCodes.Ldc_I4, 3);
      il_Main_2.Emit(OpCodes.Add);
      il_Main_2.Emit(OpCodes.Stloc, lv_y_4);

      //string text = "";
      var lv_text_5 = new VariableDefinition(assembly.MainModule.TypeSystem.String);
      md_Main_1.Body.Variables.Add(lv_text_5);
      il_Main_2.Emit(OpCodes.Ldstr, "");
      il_Main_2.Emit(OpCodes.Stloc, lv_text_5);

      //Console.WriteLine("Hello");
      il_Main_2.Emit(OpCodes.Ldstr, "Hello");
      il_Main_2.Emit(OpCodes.Call, assembly.MainModule.ImportReference(TypeHelpers.ResolveMethod(typeof(System.Console), "WriteLine",System.Reflection.BindingFlags.Default|System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.Public, "System.String")));

      //void foo () {...

      //Method : <Main>g__foo|0_0
      var md_foo_6 = new MethodDefinition("<Main>g__foo|0_0", MethodAttributes.Assembly | MethodAttributes.Static | MethodAttributes.HideBySig, assembly.MainModule.TypeSystem.Void);
      cls_Program_0.Methods.Add(md_foo_6);
      md_foo_6.Body.InitLocals = true;
      var il_foo_7 = md_foo_6.Body.GetILProcessor();

      //int x = 30;
      var lv_x_8 = new VariableDefinition(assembly.MainModule.TypeSystem.Int32);
      md_foo_6.Body.Variables.Add(lv_x_8);
      il_foo_7.Emit(OpCodes.Ldc_I4, 30);
      il_foo_7.Emit(OpCodes.Stloc, lv_x_8);
      il_foo_7.Emit(OpCodes.Ret);

      //string newText = "something";
      var lv_newText_9 = new VariableDefinition(assembly.MainModule.TypeSystem.String);
      md_Main_1.Body.Variables.Add(lv_newText_9);
      il_foo_7.Emit(OpCodes.Ldstr, "something");
      il_foo_7.Emit(OpCodes.Stloc, lv_newText_9);
      il_Main_2.Emit(OpCodes.Ret);

      // Constructor: Program() 
      var ctor_Program_10 = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName, assembly.MainModule.TypeSystem.Void);
      cls_Program_0.Methods.Add(ctor_Program_10);
      var il_ctor_Program_11 = ctor_Program_10.Body.GetILProcessor();
      il_ctor_Program_11.Emit(OpCodes.Ldarg_0);
      il_ctor_Program_11.Emit(OpCodes.Call, assembly.MainModule.ImportReference(TypeHelpers.DefaultCtorFor(cls_Program_0.BaseType)));
      il_ctor_Program_11.Emit(OpCodes.Ret);
      assembly.EntryPoint = md_Main_1;
      
      assembly.Write(path);

      //Writes a .runtimeconfig.json file matching the output assembly name.
      File.Copy(
	      Path.ChangeExtension(typeof(Generator).Assembly.Location, ".runtimeconfig.json"),
	      Path.ChangeExtension(path, ".runtimeconfig.json"),
	      true);
        }
  }
}
    
    
    
