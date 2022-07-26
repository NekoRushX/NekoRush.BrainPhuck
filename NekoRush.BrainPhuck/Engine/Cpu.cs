﻿using NekoRush.BrainPhuck.Exception;

// ReSharper disable UnusedMember.Global
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable InconsistentNaming
// ReSharper disable EmptyEmbeddedStatement

namespace NekoRush.BrainPhuck.Engine;

internal class Cpu
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
    public Register<int> PC { get; private set; }

    /// <summary>
    /// Register End of Program
    /// </summary>
    public Register<int> PCEND { get; private set; }

    /// <summary>
    /// Register Pointer
    /// </summary>
    public Register<int> PTR { get; private set; }

    /// <summary>
    /// Register Breakpoint
    /// </summary>
    public Register<int> BKPT { get; private set; }

    /// <summary>
    /// Register Comment
    /// </summary>
    public Register<bool> COMT { get; private set; }

    /// <summary>
    /// Register Bracket id number
    /// </summary>
    public Register<int> BKTID { get; private set; }

    /// <summary>
    /// Register Bracket counter
    /// </summary>
    public Register<int> BKTCTR { get; private set; }

    /// <summary>
    /// Register Stack
    /// </summary>
    public Register<int> SS { get; private set; }

    private readonly bool _extension;
    private readonly Func<Cpu, object[], int>[] _syscall;
    private readonly Queue<int> _loopstack;

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

        PC = new();
        PCEND = new();
        PTR = new();
        BKPT = new();
        SS = new();
        COMT = new();
        BKTID = new();
        BKTCTR = new();

        _extension = extension;
        _syscall = new Func<Cpu, object[], int>[64];
        _loopstack = new Queue<int>();
    }

    /// <summary>
    /// Initialize cpu
    /// </summary>
    /// <returns></returns>
    public bool Initialize()
    {
        // Reset the registers
        COMT.Value = false;
        BKTID.Value = 0x00;
        BKTCTR.Value = 0x00;

        // The heap start at 0x0000
        PTR.Value = 0x0000;

        // The stack start at 0x1000
        // SS.Value = 0x1000;

        // Set pc to 0x0000
        PC.Value = 0x0000;
        PCEND.Value = Rom.Data.Length;

        return true;
    }

    /// <summary>
    /// Reset cpu
    /// </summary>
    /// <returns></returns>
    public bool Reset()
        => Initialize();

    /// <summary>
    /// CPU step
    /// </summary>
    /// <returns></returns>
    /// <exception cref="CpuFaultException"></exception>
    public bool Step()
    {
        // Check breakpoint
        if (PC.Value == BKPT.Value && BKPT.Value != 0)
            throw new CpuFaultException("Breakpoint hit");

        // This program is run to finish
        if (PC.Value == PCEND.Value)
            return false;

        // Grab next opcode from memory
        var opcode = Rom.Read(PC.Value);
        if (BKTCTR.Value > 0)
        {
            ++PC.Value;
            if (opcode != (char) OpCode.LoopEnd) return true;

            --BKTCTR.Value;
            if (BKTCTR.Value >= 0) return true;
        }

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
                SysCall(SysCallIndex.Output, Memory.Read(PTR.Value));
                break;

            case OpCode.GetChar:
                Memory.Write(PTR.Value, (byte) SysCall(SysCallIndex.Input));
                break;

            case OpCode.LoopStart:
                ++BKTID.Value;

                if (Memory.Read(PTR.Value) != 0)
                    _loopstack.Enqueue(PC.Value - 1);
                else
                    BKTCTR.Value = BKTID.Value;
                break;

            case OpCode.LoopEnd:
                --BKTID.Value;

                var pc = _loopstack.Dequeue();
                if (Memory.Read(PTR.Value) != 0) PC.Value = pc;
                break;

            default:

                // Is extension enabled
                if (_extension)
                {
                    // Ignore line comments
                    if (COMT.Value && opcode != '\n') break;

                    // Ignore white characters
                    if (opcode == '\t' || opcode == '\r' || opcode == ' ') break;

                    switch ((OpCode) opcode)
                    {
                        case OpCode.CommentEnd:
                            COMT.Value = false;
                            break;

                        case OpCode.Comment:
                            COMT.Value = true;
                            break;

                        case OpCode.PushStack:
                            break;

                        case OpCode.PopStack:
                            SS.Value = Memory.Read(PTR.Value);
                            break;

                        case OpCode.ReadPtrJmp:
                            PC.Value = Memory.Read(PTR.Value);
                            break;

                        case OpCode.SysCall:
                            break;

                        default:
                            throw new CpuFaultException($"Invalid instruction 0x{opcode:X2} at address {PC.Value}");
                    }
                }

                // Invalid instruction
                else throw new CpuFaultException($"Invalid instruction 0x{opcode:X2} at address {PC.Value}");

                break;
        }

        // Increment PC
        ++PC.Value;

        return true;
    }

    /// <summary>
    /// Cpu run
    /// </summary>
    /// <exception cref="CpuFaultException"></exception>
    public void Run()
    {
        while (Step()) ;
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

    /// <summary>
    /// Set syscall function
    /// </summary>
    /// <param name="index"></param>
    /// <param name="action"></param>
    public void SetSysCall(SysCallIndex index, Func<Cpu, object[], int> action)
        => _syscall[(int) index] = action;

    /// <summary>
    /// System call
    /// </summary>
    /// <param name="index"></param>
    /// <param name="args"></param>
    private int SysCall(SysCallIndex index, params object[] args)
        => _syscall[(int) index].Invoke(this, args);

    public class Register<T> where T : struct
    {
        public T Value { get; set; }

        public Register()
            => Value = default;
    }

    public enum OpCode
    {
        // Standard opcodes
        PtrInc = '>',
        PtrDec = '<',
        ReadPtrInc = '+',
        ReadPtrDec = '-',
        PutChar = '.',
        GetChar = ',',
        LoopStart = '[',
        LoopEnd = ']',

        // Extended opcodes
        Comment = '#',
        CommentEnd = '\n',
        PushStack = '(',
        PopStack = ')',
        ReadPtrJmp = '@',
        SysCall = '&'
    }

    public enum SysCallIndex
    {
        Output,
        Input
    }
}
