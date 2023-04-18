using Mono.Cecil;
using Mono.Cecil.Cil;

namespace DefaultNamespace.CodeGenerator;

public class Generator
{
    private Action _action;
    public Generator(Action action)
    {
        _action = action;
        var name = new AssemblyNameDefinition("SuperGreeterBinary", new Version(1, 0, 0, 0));
        var asm = AssemblyDefinition.CreateAssembly(name, "greeter.exe", ModuleKind.Console);

        // импортируем в библиотеку типы string и void
        asm.MainModule.Import(typeof(String));
        var void_import = asm.MainModule.Import(typeof(void));

        // создаем метод Main, статический, приватный, возвращающий void
        var method = new MethodDefinition("Main", MethodAttributes.Static | MethodAttributes.Private | MethodAttributes.HideBySig, void_import);
        // сохраняем короткую ссылку на генератор кода
        var ip = method.Body.GetILProcessor();

        // магия ленор!
        ip.Emit(OpCodes.Ldstr, "Hello, ");
    }
    
    
}