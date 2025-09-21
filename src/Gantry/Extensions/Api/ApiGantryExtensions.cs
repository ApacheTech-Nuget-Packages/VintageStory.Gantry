using Gantry.Core.Abstractions;
using Gantry.Services.IO.Configuration.Abstractions;

namespace Gantry.Extensions.Api;

/// <summary>
///     Additional extension methods for working with the Gantry Core API.
/// </summary>
public static class ApiGantryExtensions
{
    /// <summary>
    ///     Converts a side-agnostic API to a Gantry Core API instance.
    /// </summary>
    /// <param name="api">The core game API.</param>
    public static ICoreGantryAPI GantryCore(this ICoreAPI api) 
        => api.DynamicProperties().GantryCore
           ?? throw new InvalidOperationException("The Gantry Core API is not available. Ensure that the mod is correctly set up to use Gantry.");

    /// <summary>
    ///     Returns a service of the specified type from the Gantry service provider.
    /// </summary>
    /// <param name="api">The core game API.</param>
    public static T GetRequiredService<T>(this ICoreAPI api) 
        => api.GantryCore().Services.GetRequiredService<T>();

    /// <summary>
    ///     Gets the settings instance for the specified <see cref="FeatureSettings{T}"/> type.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="FeatureSettings{T}"/> to retrieve.</typeparam>
    /// <param name="api">The core game API.</param>
    /// <returns>An instance of the specified settings type.</returns>
    public static T Settings<T>(this ICoreAPI api)
        where T : FeatureSettings<T>, new() 
        => api.GantryCore().Services.GetRequiredService<T>();
}