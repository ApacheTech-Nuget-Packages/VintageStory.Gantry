using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Reflection;
using Gantry.Core;
using Gantry.Services.MefLab.Abstractions;

namespace Gantry.Services.MefLab;

/// <summary>
///     
/// </summary>
public class MefLabService : IMefLabService
{
    private readonly AggregateCatalog _catalogue;

    /// <summary>
    ///     Initialises a new instance of the <see cref="MefLabService" /> class.
    /// </summary>
    public MefLabService()
    {   
        _catalogue = new AggregateCatalog();
        AddAssembly(ModEx.ModAssembly);
    }

    /// <summary>
    ///     Adds a directory to the aggregate Catalogue.
    /// </summary>
    /// <param name="path">The absolute path to the directory to add.</param>
    public DirectoryCatalog AddDirectory(string path)
    {
        var catalogue = _catalogue.Catalogs
            .OfType<DirectoryCatalog>()
            .FirstOrDefault(p => p.FullPath == path);
        if (catalogue is not null) return catalogue;
        catalogue = new DirectoryCatalog(path);
        _catalogue.Catalogs.Add(catalogue);
        return catalogue;
    }

    /// <summary>
    ///     Removes a directory from the aggregate Catalogue.
    /// </summary>
    /// <param name="path">The absolute path to the directory to remove.</param>
    public void RemoveDirectory(string path)
    {
        var catalogue = _catalogue.Catalogs
            .OfType<DirectoryCatalog>()
            .FirstOrDefault(p => p.FullPath == path);
        if (catalogue is not null) Remove(catalogue);
    }

    /// <summary>
    ///     Adds an assembly to the aggregate Catalogue.
    /// </summary>
    /// <param name="assembly">The assembly to add.</param>
    public AssemblyCatalog AddAssembly(Assembly assembly)
    {
        var catalogue = _catalogue.Catalogs
            .OfType<AssemblyCatalog>()
            .FirstOrDefault(p => p.Assembly == assembly);
        if (catalogue is not null) return catalogue;
        catalogue = new AssemblyCatalog(assembly);
        _catalogue.Catalogs.Add(catalogue);
        return catalogue;
    }

    /// <summary>
    ///     Removes an assembly from the aggregate Catalogue.
    /// </summary>
    /// <param name="assembly">The assembly to remove.</param>
    public void RemoveAssembly(Assembly assembly)
    {
        var catalogue = _catalogue.Catalogs
            .OfType<AssemblyCatalog>()
            .FirstOrDefault(p => p.Assembly == assembly);
        if (catalogue is not null) Remove(catalogue);
    }

    /// <summary>
    ///     Removes the specified constituent catalogue from the aggregate Catalogue.
    /// </summary>
    /// <typeparam name="TCatalogue">The type of the catalogue to remove.</typeparam>
    /// <param name="catalogue">The constituent catalogue instance that currently resides in the aggregate Catalogue.</param>
    public void Remove<TCatalogue>(TCatalogue catalogue) where TCatalogue : ComposablePartCatalog
    {
        _catalogue.Catalogs.Remove(catalogue);
    }

    /// <summary>
    ///     Creates composable parts from an array of attributed objects.
    /// </summary>
    /// <param name="attributedParts">An array of attributed parts to compose.</param>
    public void ComposeParts(params object[] attributedParts)
    {
        var container = new CompositionContainer(_catalogue);
        container.ComposeParts(attributedParts);
    }
}
