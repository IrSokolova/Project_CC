using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System; 
using System.IO;
using System.Linq;
using System.Reflection;
using BindingFlags = System.Reflection.BindingFlags;
using Cecilifier.Runtime;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using TypeAttributes = Mono.Cecil.TypeAttributes;

namespace DefaultNamespace.CodeGenerator;

public class Generator
{
    // private Action _action;
    public Generator()
    {
	    Assembly info = typeof(int).Assembly;
	    Console.WriteLine(info);
        // Compile("tat");
        Compile2();
    }
    
    public void Compile(string str)
    {
        var name = new AssemblyNameDefinition("SuperGreeterBinary", new Version(1, 0, 0, 0));
        var asm = AssemblyDefinition.CreateAssembly(name, "greeter.exe", ModuleKind.Console);

        asm.MainModule.Import(typeof(String));
        var void_import = asm.MainModule.Import(typeof(void));

        var method = new MethodDefinition("Main", MethodAttributes.Static | MethodAttributes.Private | MethodAttributes.HideBySig, void_import);
        var ip = method.Body.GetILProcessor();

        ip.Emit(OpCodes.Ldstr, "Hello, ");
        ip.Emit(OpCodes.Ldstr, str);
        ip.Emit(OpCodes.Call, asm.MainModule.Import(typeof(String).GetMethod("Concat", new Type[] { typeof(string), typeof(string) })));
        ip.Emit(OpCodes.Call, asm.MainModule.Import(typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) })));
        ip.Emit(OpCodes.Call, asm.MainModule.Import(typeof(Console).GetMethod("ReadLine", new Type[] { })));
        ip.Emit(OpCodes.Pop);
        ip.Emit(OpCodes.Ret);

        var type = new TypeDefinition("supergreeter", "Program", TypeAttributes.AutoClass | TypeAttributes.Public | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit, asm.MainModule.Import(typeof(object)));
        asm.MainModule.Types.Add(type);
        type.Methods.Add(method);

        asm.EntryPoint = method;

        asm.Write("greeter.exe");
    }

    public void Compile2()
    {
	    var path = @"/home/tatiana/RiderProjects/Project_CC/CodeGenerator/file.exe";
        // setup a `reflection importer` to ensure references to System.Private.CoreLib are replaced with references to `netstandard`. 
        var mp = new ModuleParameters { Architecture = TargetArchitecture.AMD64, Kind =  ModuleKind.Dll, ReflectionImporterProvider = new SystemPrivateCoreLibFixerReflectionProvider() };
        using(var assembly = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition("Foo", Version.Parse("1.0.0.0")), Path.GetFileName(path), mp))
  //       var nameDef = new AssemblyNameDefinition("CodeGeneration", new Version(1, 0, 0, 0));
		// using(var assembly = AssemblyDefinition.CreateAssembly(nameDef, "result.exe", ModuleKind.Console))
        {

			//Class : Foo
			var cls_Foo_0 = new TypeDefinition("", "Foo", TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.NotPublic, assembly.MainModule.TypeSystem.Object);
			assembly.MainModule.Types.Add(cls_Foo_0);


			//Method : Bar
			var md_Bar_1 = new MethodDefinition("Main", MethodAttributes.Private | MethodAttributes.HideBySig, assembly.MainModule.TypeSystem.Void);
			cls_Foo_0.Methods.Add(md_Bar_1);
			md_Bar_1.Body.InitLocals = true;
			var il_Bar_2 = md_Bar_1.Body.GetILProcessor();
			il_Bar_2.Emit(OpCodes.Ldstr, "Hello World!");
			il_Bar_2.Emit(OpCodes.Call, assembly.MainModule.ImportReference(TypeHelpers.ResolveMethod(typeof(System.Console), "WriteLine",System.Reflection.BindingFlags.Default|System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.Public, "System.String")));
			il_Bar_2.Emit(OpCodes.Ret);

			//** Constructor: Foo() **
			var ctor_Foo_3 = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName, assembly.MainModule.TypeSystem.Void);
			cls_Foo_0.Methods.Add(ctor_Foo_3);
			var il_ctor_Foo_4 = ctor_Foo_3.Body.GetILProcessor();
			il_ctor_Foo_4.Emit(OpCodes.Ldarg_0);
			il_ctor_Foo_4.Emit(OpCodes.Call, assembly.MainModule.ImportReference(TypeHelpers.DefaultCtorFor(cls_Foo_0.BaseType)));
			il_ctor_Foo_4.Emit(OpCodes.Ret);
			
			assembly.EntryPoint = ctor_Foo_3;

		    assembly.Write(path);

            //Writes a .runtimeconfig.json file matching the output assembly name.
			File.Copy(
				Path.ChangeExtension(typeof(Generator).Assembly.Location, ".runtimeconfig.json"),
                Path.ChangeExtension(path, ".runtimeconfig.json"),
                true);
        }
    }
    
    
    
}