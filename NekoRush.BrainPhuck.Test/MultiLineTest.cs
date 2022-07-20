using NUnit.Framework;

namespace NekoRush.BrainPhuck.Test;

public class MultiLineTest
{
    [Test]
    public void IncAndPrint()
    {
        var output = 0;
        const string code = "+++++ # Add the ptr 5 times\n" +
                            ".     # Then print it";

        var bf = BfScript.Create(code, new() {EnableExtension = true, MemorySize = 0x4000});
        {
            bf.OnStdOut += (_, args) => output = args.Byte;
            bf.Run();
        }

        Assert.AreEqual(output, 5);
    }
}
