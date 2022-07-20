using NekoRush.BrainPhuck.Exception;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable InconsistentNaming
// ReSharper disable FunctionNeverReturns
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable EmptyEmbeddedStatement

namespace NekoRush.BrainPhuck.Engine;

public class Cpu
{
    /// <summary>
    /// ROM
    /// </summary>
    public Rom Rom { get; private set; }

    /// <summary>
    /// Memory
    /// </summary>
    public Memory Memory { get; private set; }

    /// <summary>
    /// Register PC
    /// </summary>
    public Register PC { get; private set; }

    /// <summary>
    /// Register End of Program
    /// </summary>
    public Register PCEND { get; private set; }

    /// <summary>
    /// Register Pointer
    /// </summary>
    public Register PTR { get; private set; }

    /// <summary>
    /// Register Breakpoint
    /// </summary>
    public Register BKPT { get; private set; }

    /// <summary>
    /// Register Stack
    /// </summary>
    public Register SS { get; private set; }

    private readonly bool _extension;

    /// <summary>
    /// Create cpu
    /// </summary>
    /// <param name="memory"></param>
    /// <param name="rom"></param>
    /// <param name="extension"></param>
    public Cpu(Memory memory, Rom rom, bool extension = false)
    {
        Rom = rom;
        Memory = memory;
        PC = new Register();
        PCEND = new Register();
        PTR = new Register();
        BKPT = new Register();
        SS = new Register();
        _extension = extension;
    }

    /// <summary>
    /// Initialize cpu
    /// </summary>
    /// <returns></returns>
    public bool Initialize()
    {
        // Zero the registers
        PTR.Value = 0x00;
        BKPT.Value = 0x00;
        
        // The stack start at 0x1000
        SS.Value = 0x1000;
        
        // Set pc to 0x1000
        PC.Value = 0x2000;

        // Copy Rom data into memory
        PCEND.Value = PC.Value + Memory.Write(PC.Value, Rom.Data);
        return PCEND.Value == Rom.Data.Length;
    }

    /// <summary>
    /// CPU step
    /// </summary>
    /// <returns></returns>
    /// <exception cref="CpuFaultException"></exception>
    public bool Step()
    {
        // Check breakpoint
        if (PC.Value == BKPT.Value)
            throw new CpuFaultException("Breakpoint hit");

        if (PC.Value == PCEND.Value)
            return false;

        var opcode = Memory.Read(PC.Value);
        switch ((OpCode) opcode)
        {
            case OpCode.PtrInc:
                ++PTR.Value;
                break;
            case OpCode.PtrDec:
                --PTR.Value;
                break;
            case OpCode.ReadPtrInc:
                Memory.Write(PTR.Value, (byte) (Memory.Read(PTR.Value) + 1));
                break;
            case OpCode.ReadPtrDec:
                Memory.Write(PTR.Value, (byte) (Memory.Read(PTR.Value) - 1));
                break;
            case OpCode.PutChar:
                break;
            case OpCode.GetChar:
                break;
            case OpCode.LoopStart:
                break;
            case OpCode.LoopEnd:
                break;

            default:

                // Is extension enabled
                if (_extension)
                {
                    switch ((OpCode) opcode)
                    {
                        case OpCode.PushStack:
                            break;
                        case OpCode.PopStack:
                            break;
                        case OpCode.ReadPtrJmp:
                            break;
                        case OpCode.SysCall:
                            break;
                        default:
                            throw new CpuFaultException($"Invalid instruction {opcode} at address {PC.Value}");
                    }
                }

                // Invalid instruction
                else throw new CpuFaultException($"Invalid instruction {opcode} at address {PC.Value}");

                break;
        }

        // Increment PC
        ++PC;

        return true;
    }

    /// <summary>
    /// Cpu run
    /// </summary>
    /// <exception cref="CpuFaultException"></exception>
    public void Run()
    {
        while (Step());
    }

    /// <summary>
    /// Set breakpoint
    /// </summary>
    /// <param name="ptr"></param>
    public void SetBreakPoint(int ptr)
        => BKPT.Value = ptr;

    /// <summary>
    /// Clear breakpoint
    /// </summary>
    public void ClearBreakPoint()
        => BKPT.Value = 0x00;

    public class Register
    {
        public int Value { get; set; }

        public static Register operator ++(Register register)
        {
            register.Value++;
            return register;
        }

        public static Register operator --(Register register)
        {
            register.Value--;
            return register;
        }
    }

    public enum OpCode
    {
        // Standard opcodes
        PtrInc = 62,
        PtrDec = 60,
        ReadPtrInc = 43,
        ReadPtrDec = 45,
        PutChar = 46,
        GetChar = 44,
        LoopStart = 91,
        LoopEnd = 93,

        // Extended opcodes
        PushStack = 40,
        PopStack = 41,
        ReadPtrJmp = 64,
        SysCall = 38
    }
}
