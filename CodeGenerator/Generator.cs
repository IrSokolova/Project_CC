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
    private string _path = @"/home/tatiana/RiderProjects/Project_CC/CodeGenerator/Exe";
    private MainRoutine? _mainRoutine;
    public Generator(Action action)
    {
	    _mainRoutine = null;
	    var mp = new ModuleParameters { Architecture = TargetArchitecture.AMD64, Kind =  ModuleKind.Console, ReflectionImporterProvider = new SystemPrivateCoreLibFixerReflectionProvider() };
	    _asm = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition("Program", Version.Parse("1.0.0.0")), Path.GetFileName(_path), mp);

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
			    // todo
		    }
	    }
	    else if (action._statement != null)
	    {
		    // todo
	    } // else error
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

      var fld_y_1 = new FieldDefinition("y", FieldAttributes.Public, assembly.MainModule.TypeSystem.Int32);
      cls_Program_0.Fields.Add(fld_y_1);
      var fld_y1_2 = new FieldDefinition("y1", FieldAttributes.Public, assembly.MainModule.TypeSystem.Int32);
      cls_Program_0.Fields.Add(fld_y1_2);

      //Method : Main
      var md_Main_3 = new MethodDefinition("Main", MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig, assembly.MainModule.TypeSystem.Void);
      cls_Program_0.Methods.Add(md_Main_3);
      md_Main_3.Body.InitLocals = true;
      var il_Main_4 = md_Main_3.Body.GetILProcessor();

      //Parameters of 'public static void Main(string[] args)...'
      var p_args_5 = new ParameterDefinition("args", ParameterAttributes.None, assembly.MainModule.TypeSystem.String.MakeArrayType());
      md_Main_3.Parameters.Add(p_args_5);

      //string text = "";
      var lv_text_6 = new VariableDefinition(assembly.MainModule.TypeSystem.String);
      md_Main_3.Body.Variables.Add(lv_text_6);
      il_Main_4.Emit(OpCodes.Ldstr, "");
      il_Main_4.Emit(OpCodes.Stloc, lv_text_6);

      //Console.WriteLine("Hello");
      il_Main_4.Emit(OpCodes.Ldstr, "Hello");
      il_Main_4.Emit(OpCodes.Call, assembly.MainModule.ImportReference(TypeHelpers.ResolveMethod(typeof(System.Console), "WriteLine",System.Reflection.BindingFlags.Default|System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.Public, "System.String")));
      il_Main_4.Emit(OpCodes.Ret);

      //Method : foo
      var md_foo_7 = new MethodDefinition("foo", MethodAttributes.Public | MethodAttributes.HideBySig, assembly.MainModule.TypeSystem.Void);
      cls_Program_0.Methods.Add(md_foo_7);
      md_foo_7.Body.InitLocals = true;
      var il_foo_8 = md_foo_7.Body.GetILProcessor();

      //int x = 30;
      var lv_x_9 = new VariableDefinition(assembly.MainModule.TypeSystem.Int32);
      md_foo_7.Body.Variables.Add(lv_x_9);
      il_foo_8.Emit(OpCodes.Ldc_I4, 30);
      il_foo_8.Emit(OpCodes.Stloc, lv_x_9);
      il_foo_8.Emit(OpCodes.Ret);

      // Constructor: Program() 
      var ctor_Program_10 = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName, assembly.MainModule.TypeSystem.Void);
      cls_Program_0.Methods.Add(ctor_Program_10);
      var il_ctor_Program_11 = ctor_Program_10.Body.GetILProcessor();

      //public int y = 2;
      il_ctor_Program_11.Emit(OpCodes.Ldarg_0);
      il_ctor_Program_11.Emit(OpCodes.Ldc_I4, 2);
      il_ctor_Program_11.Emit(OpCodes.Stfld, fld_y_1);

      //public int y1 = 4;
      il_ctor_Program_11.Emit(OpCodes.Ldarg_0);
      il_ctor_Program_11.Emit(OpCodes.Ldc_I4, 4);
      il_ctor_Program_11.Emit(OpCodes.Stfld, fld_y1_2);
      il_ctor_Program_11.Emit(OpCodes.Ldarg_0);
      il_ctor_Program_11.Emit(OpCodes.Call, assembly.MainModule.ImportReference(TypeHelpers.DefaultCtorFor(cls_Program_0.BaseType)));
      il_ctor_Program_11.Emit(OpCodes.Ret);
      assembly.EntryPoint = md_Main_3;
      
      assembly.Write(path);

      //Writes a .runtimeconfig.json file matching the output assembly name.
      File.Copy(
	      Path.ChangeExtension(typeof(Generator).Assembly.Location, ".runtimeconfig.json"),
	      Path.ChangeExtension(path, ".runtimeconfig.json"),
	      true);
        }
  }
}
    
    
    
