// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace NekoRush.BrainPhuck.EventArgs;

public class InOutEventArgs : System.EventArgs
{
    public byte Byte { get; set; }

    public InOutEventArgs(byte b)
        => Byte = b;
}
