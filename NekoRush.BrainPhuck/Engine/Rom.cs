// ReSharper disable MemberCanBePrivate.Global

namespace NekoRush.BrainPhuck.Engine;

internal class Rom
{
    /// <summary>
    /// An array to hold the data
    /// </summary>
    public readonly byte[] Data;
    
    public Rom(byte[] binary)
    {
        // Deep copy the data
        Data = new byte[binary.Length];
        Array.Copy(binary, 0, Data, 0, binary.Length);
    }
}
