// using Mono.Cecil;
// using Mono.Cecil.Cil;
// using Mono.Cecil.Rocks;
// using System; 
// using System.IO;
// using System.Linq;
// using BindingFlags = System.Reflection.BindingFlags;
// using Cecilifier.Runtime;
//                
// public class SnippetRunner
// {
// 	public static void Main(string[] args)
// 	{
//         // setup a `reflection importer` to ensure references to System.Private.CoreLib are replaced with references to `netstandard`. 
//         var mp = new ModuleParameters { Architecture = TargetArchitecture.AMD64, Kind =  ModuleKind.Dll, ReflectionImporterProvider = new SystemPrivateCoreLibFixerReflectionProvider() };
// 		using(var assembly = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition("Foo", Version.Parse("1.0.0.0")), Path.GetFileName(args[0]), mp))
//         {
//
// 			//Class : Foo
// 			var cls_Foo_0 = new TypeDefinition("", "Foo", TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.NotPublic, assembly.MainModule.TypeSystem.Object);
// 			assembly.MainModule.Types.Add(cls_Foo_0);
//
//
// 			//Method : Bar
// 			var md_Bar_1 = new MethodDefinition("Bar", MethodAttributes.Private | MethodAttributes.HideBySig, assembly.MainModule.TypeSystem.Void);
// 			cls_Foo_0.Methods.Add(md_Bar_1);
// 			md_Bar_1.Body.InitLocals = true;
// 			var il_Bar_2 = md_Bar_1.Body.GetILProcessor();
// 			il_Bar_2.Emit(OpCodes.Ldstr, "Hello World!");
// 			il_Bar_2.Emit(OpCodes.Call, assembly.MainModule.ImportReference(TypeHelpers.ResolveMethod(typeof(System.Console), "WriteLine",System.Reflection.BindingFlags.Default|System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.Public, "System.String")));
// 			il_Bar_2.Emit(OpCodes.Ret);
//
// 			//** Constructor: Foo() **
// 			var ctor_Foo_3 = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName, assembly.MainModule.TypeSystem.Void);
// 			cls_Foo_0.Methods.Add(ctor_Foo_3);
// 			var il_ctor_Foo_4 = ctor_Foo_3.Body.GetILProcessor();
// 			il_ctor_Foo_4.Emit(OpCodes.Ldarg_0);
// 			il_ctor_Foo_4.Emit(OpCodes.Call, assembly.MainModule.ImportReference(TypeHelpers.DefaultCtorFor(cls_Foo_0.BaseType)));
// 			il_ctor_Foo_4.Emit(OpCodes.Ret);
//
// 		    assembly.Write(args[0]);
//
//             //Writes a .runtimeconfig.json file matching the output assembly name.
// 			File.Copy(
// 				Path.ChangeExtension(typeof(SnippetRunner).Assembly.Location, ".runtimeconfig.json"),
//                 Path.ChangeExtension(args[0], ".runtimeconfig.json"),
//                 true);
//         }
// 	}
// }