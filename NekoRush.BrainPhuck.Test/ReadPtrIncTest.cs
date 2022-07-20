using System;
using System.Runtime.InteropServices.ComTypes;
using NUnit.Framework;

namespace NekoRush.BrainPhuck.Test;

public class ReadPtrIncTest
{
    [Test]
    public void IncAndPrint()
    {
        var output = 0;
        const string code = "+++++.";

        var bf = BfScript.Create(code, BfScriptOptions.Default);
        {
            bf.OnStdOut += (_, args) => output = args.Byte;
            bf.Run();
        }

        Assert.AreEqual(output, 5);
    }

    [Test]
    public void ReadAndPrint()
    {
        var output = 0;
        const string code = ",.";

        var bf = BfScript.Create(code, BfScriptOptions.Default);
        {
            bf.OnStdIn += (_, args) => args.Byte = 0xCC;
            bf.OnStdOut += (_, args) => output = args.Byte;
            bf.Run();
        }

        Assert.AreEqual(output, 0xCC);
    }
    
    [Test]
    public void ReadIncAndPrint()
    {
        var output = 0;
        const string code = ",+.";

        var bf = BfScript.Create(code, BfScriptOptions.Default);
        {
            bf.OnStdIn += (_, args) => args.Byte = 0xCC;
            bf.OnStdOut += (_, args) => output = args.Byte;
            bf.Run();
        }

        Assert.AreEqual(output, 0xCD);
    }
}
