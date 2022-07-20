using NUnit.Framework;

namespace NekoRush.BrainPhuck.Test;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        const string code = "+++++";
        var bf = BfScript.Create(code, BfScriptOptions.Default);
        {
            bf.Run();
        }

        Assert.Pass();
    }
}
