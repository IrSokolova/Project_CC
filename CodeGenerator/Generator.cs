using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System; 
using System.IO;
using System.Linq;
using System.Reflection;
using BindingFlags = System.Reflection.BindingFlags;
using Cecilifier.Runtime;
using ConsoleApp1.SyntaxAnalyser;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using TypeAttributes = Mono.Cecil.TypeAttributes;
using Action = ConsoleApp1.SyntaxAnalyser.Action;
using ConsoleApp1.SyntaxAnalyser;
using FieldAttributes = Mono.Cecil.FieldAttributes;
using ParameterAttributes = Mono.Cecil.ParameterAttributes;
using Type = ConsoleApp1.SyntaxAnalyser.Type;

namespace DefaultNamespace.CodeGenerator;

public class Generator
{
    // private Action _action;
    private AssemblyDefinition _asm;
    private TypeDefinition _typeDef;
    private MethodDefinition _mainModule;
    private ILProcessor _mainProc;
    
    private string _path = @"/home/tatiana/RiderProjects/Project_CC/CodeGenerator/Exe/code.exe";
    private MainRoutine? _mainRoutine;
    private Dictionary<string, VariableDefinition> _vars;
    public Generator(Action action)
    {
	    _vars = new Dictionary<string, VariableDefinition>();
	    _mainRoutine = null;
	    var mp = new ModuleParameters { Architecture = TargetArchitecture.AMD64, Kind =  ModuleKind.Console, ReflectionImporterProvider = new SystemPrivateCoreLibFixerReflectionProvider() };
	    _asm = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition("Program", Version.Parse("1.0.0.0")), Path.GetFileName(_path), mp);
	    
	    _typeDef = new TypeDefinition("", "Program", TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.Public, _asm.MainModule.TypeSystem.Object);
	    _asm.MainModule.Types.Add(_typeDef);
	    
	    _mainModule = new MethodDefinition("Main", MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig, _asm.MainModule.TypeSystem.Void);
	    _typeDef.Methods.Add(_mainModule);
	    _mainModule.Body.InitLocals = true;
	    _mainProc = _mainModule.Body.GetILProcessor();
	    
	    var mainParams = new ParameterDefinition("args", ParameterAttributes.None, _asm.MainModule.TypeSystem.String.MakeArrayType());
	    _mainModule.Parameters.Add(mainParams);

	    StartGeneration(action);
	    
	    // Assembly info = typeof(int).Assembly;
	    // Console.WriteLine(info);
        example();
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
				    // todo
			    }
		    }
		    else if (action._declaration._typeDeclaration != null)
		    {
			    // todo
		    }
		    else if (action._declaration._variableDeclaration != null)
		    {
			    GenerateVarDecl(action._declaration._variableDeclaration);
		    }
	    }
	    else if (action._statement != null)
	    {
		    // todo
	    } // else error
    }

    public void GenerateVarDecl(VariableDeclaration varDecl)
    {
	    string? name = varDecl._identifier._name;
	    Type type = varDecl._type;
	    Value? value = varDecl._value;

	    Double valCostil = Double.Parse(value._expressions._expression._relation._operation._operand._single._value);
	    OpCode typeOpCodeCostil = OpCodes.Ldc_R8;
	    TypeReference typeRefCostil = _asm.MainModule.TypeSystem.Double;
	    
	    
	    var varDef = new VariableDefinition(typeRefCostil);
	    _mainModule.Body.Variables.Add(varDef);
	    _mainProc.Emit(typeOpCodeCostil, valCostil);
	    _mainProc.Emit(OpCodes.Stloc, varDef);
	    
	    Print(varDef, "System.Double");
	    
	    _vars.Add(name, varDef);
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
    
    
    
