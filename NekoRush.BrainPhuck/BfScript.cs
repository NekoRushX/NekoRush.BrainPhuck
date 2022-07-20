using System.Text;
using NekoRush.BrainPhuck.EventArgs;
using NekoRush.BrainPhuck.Exception;

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable UnusedType.Global

namespace NekoRush.BrainPhuck;

public class BfScript
{
    public delegate void BfEvent<in TArgs>(BfScript sender, TArgs args);

    /// <summary>
    /// On output data.
    /// </summary>
    public event BfEvent<InOutEventArgs> OnStdOut;

    /// <summary>
    /// On read data.
    /// </summary>
    public event BfEvent<InOutEventArgs> OnStdIn;

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

            // Set syscall handler
            script._cpu.SetSysCall(Engine.Cpu.SysCallIndex.Output, (_, o) => {
                script.OnStdOut.Invoke(script, new((byte) o[0]));
                return 0;
            });
            script._cpu.SetSysCall(Engine.Cpu.SysCallIndex.Input, (_, o) =>
            {
                var args = new InOutEventArgs(0);
                script.OnStdIn.Invoke(script, args);
                return args.Byte;
            });

            if (script._cpu.Initialize()) return script;
        }

        // oops
        throw new CpuFaultException("Cpu initialization failed");
    }

    /// <summary>
    /// Run the script.
    /// </summary>
    /// <exception cref="CpuFaultException"></exception>
    public void Run()
        => _cpu!.Run();

    /// <summary>
    /// Step the script.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="CpuFaultException"></exception>
    public bool Step()
        => _cpu!.Step();
}
