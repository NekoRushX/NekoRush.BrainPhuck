// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace NekoRush.BrainPhuck;

public class BfScriptOptions
{
    /// <summary>
    /// Enables instruction extension
    /// </summary>
    public bool EnableExtension { get; set; }

    /// <summary>
    /// Memory size in bytes
    /// </summary>
    public int MemorySize { get; set; }

    /// <summary>
    /// Default configuration
    /// </summary>
    public static BfScriptOptions Default => new()
    {
        EnableExtension = false,
        MemorySize = 0x2000
    };
}
