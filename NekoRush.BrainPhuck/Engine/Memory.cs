// ReSharper disable MemberCanBePrivate.Global

namespace NekoRush.BrainPhuck.Engine;

internal class Memory
{
    /// <summary>
    /// Memory
    /// </summary>
    public readonly byte[] Data;
    
    public Memory(int size)
    {
        Data = new byte[size];
    }

    /// <summary>
    /// Read data
    /// </summary>
    /// <param name="ptr"></param>
    /// <returns></returns>
    public byte Read(int ptr) => Data[ptr];

    /// <summary>
    /// Write memory data
    /// </summary>
    /// <param name="ptr"></param>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public int Write(int ptr, byte[] bytes)
    {
        Array.Copy(bytes, 0, Data, ptr, bytes.Length);
        return bytes.Length;
    }
    
    /// <summary>
    /// Write memory data
    /// </summary>
    /// <param name="ptr"></param>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public int Write(int ptr, byte bytes)
    {
        Data[ptr] = bytes;
        return 1;
    }
}
