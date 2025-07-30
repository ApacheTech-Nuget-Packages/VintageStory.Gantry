namespace Gantry.Core.Reflection;

/// <summary>
///     <see cref="DynamicHarmonyWrapper"/> is a wrapper around the Harmony library to allow for dynamic access.
/// </summary>
public class DynamicHarmonyWrapper : IDisposable
{
    private readonly Harmony _harmony;

    /// <summary>
    ///     Initialises a new instance of the <see cref="DynamicHarmonyWrapper"/> class.
    /// </summary>
    public DynamicHarmonyWrapper()
    {
        _harmony = new Harmony(Guid.NewGuid().ToString());
    }


    /// <inheritdoc />
    public void Dispose()
    {
        _harmony.UnpatchAll();
    }
}