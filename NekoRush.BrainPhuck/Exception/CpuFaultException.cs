namespace NekoRush.BrainPhuck.Exception;

using System;

public class CpuFaultException : Exception
{
    public CpuFaultException(string reason) : base(reason)
    {
    }
}
