using System.Text;
using NekoRush.BrainPhuck.Exception;
// ReSharper disable UnusedType.Global

namespace NekoRush.BrainPhuck;

public class BfScript
{
    private Engine.Cpu? _cpu;

    private BfScript()
    {
    }
    
    /// <summary>
    /// Create a new instance of BfScript.
    /// </summary>
    /// <param name="code"></param>
    /// <param name="opt"></param>
    /// <returns></returns>
    /// <exception cref="CpuFaultException"></exception>
    public static BfScript Create(string code, BfScriptOptions opt)
    {
        // Create bf script
        var script = new BfScript();
        {
            var memory = new Engine.Memory(opt.MemorySize);
            var rom = new Engine.Rom(Encoding.UTF8.GetBytes(code));

            // Initialize cpu
            script._cpu = new Engine.Cpu(memory, rom, opt.EnableExtension);
            if (script._cpu.Initialize()) return script;
        }

        // oops
        throw new CpuFaultException("Cpu initialization failed");
    }

    /// <summary>
    /// Run the script.
    /// </summary>
    public void Run()
    {
        _cpu!.Run();
    }
}
