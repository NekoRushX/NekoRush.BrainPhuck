using System;
using NUnit.Framework;

namespace NekoRush.BrainPhuck.Test;

public class LoopTest
{
    [Test]
    public void Loop()
    {
        var output = 0;
        const string code = "-[+.]";

        var bf = BfScript.Create(code, BfScriptOptions.Default);
        {
            bf.OnStdOut += (_, args) => output = args.Byte;
            bf.Run();
        }

        Assert.AreEqual(0, output);
    }

    [Test]
    public void PrintString()
    {
        var output = "";
        const string code = "-[--->+<]>-------.>--[----->+<]>-.++++++.++++.+[->+++<]>++.----[-->+++<]>.--.-----------.";

        var bf = BfScript.Create(code, BfScriptOptions.Default);
        {
            bf.OnStdOut += (_, args) => output += Convert.ToChar(args.Byte);
            bf.Run();
        }

        Assert.AreEqual("NekoRush", output);
    }

    [Test]
    public void NestedLoop()
    {
        var output = 0;
        const string code = "-[+[-]]++++.";

        var bf = BfScript.Create(code, BfScriptOptions.Default);
        {
            bf.OnStdOut += (_, args) => output = args.Byte;
            bf.Run();
        }

        Assert.AreEqual(4, output);
    }
}
