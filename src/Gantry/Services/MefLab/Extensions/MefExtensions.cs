using System.ComponentModel.Composition.Hosting;
using System.Reflection;

namespace Gantry.Services.MefLab.Extensions;

/// <summary>
///     Extension methods to extend the functionality of MEF containers and catalogues.
/// </summary>
public static class MefExtensions
{
    /// <summary>
    ///     Resolves an incoming contract to a container.
    /// </summary>
    public static CompositionContainer Containerise(this CompositionDataPacket packet)
    {
        var bytes = packet.Data.ToArray();
        var assembly = Assembly.Load(bytes);
        var catalogue = new AssemblyCatalog(assembly);
        return new CompositionContainer(catalogue);
    }
}
