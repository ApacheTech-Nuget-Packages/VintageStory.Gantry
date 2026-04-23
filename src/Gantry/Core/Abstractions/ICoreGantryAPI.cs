using ApacheTech.Common.Mediator.Commands.Processor;
using Gantry.Core.Helpers;
using Gantry.Core.Hosting.Annotation;
using Gantry.Services.HarmonyPatches;
using Gantry.Services.IO.Abstractions.Contracts;
using Gantry.Services.IO.Configuration;

namespace Gantry.Core.Abstractions;

/// <summary>
///     Provides the core API surface for Gantry mods, exposing logging, dependency injection, localisation, mod metadata, and core services.
///     This interface is implemented by the Gantry core and injected into mod hosts and services, ensuring correct context and isolation per mod.
/// </summary>
public interface ICoreGantryAPI
{
    /// <summary>
    ///     Logger for diagnostic and debug output.
    /// </summary>
    ILogger Logger { get; }

    /// <summary>
    ///     The underlying Vintagestory API instance for the current mod.
    /// </summary>
    ICoreAPI Uapi { get; }

    /// <summary>
    ///     The underlying Vintagestory API instance for the current mod.
    /// </summary>
    ICoreClientAPI Capi => ApiEx.Client;

    /// <summary>
    ///     The underlying Vintagestory API instance for the current mod.
    /// </summary>
    ICoreServerAPI Sapi => ApiEx.Server;

    /// <summary>
    ///     The current mod, as registered with the mod manager.
    /// </summary>
    Mod Mod { get; }

    /// <summary>
    ///     The main assembly for the mod that initialised the Gantry MDK.
    /// </summary>
    Assembly ModAssembly { get; }

    /// <summary>
    ///     The main assemblies for the mod, including the Gantry MDK, if not merged into the main assembly.
    /// </summary>
    List<Assembly> ModAssemblies => [.. new[] { typeof(IModHost).Assembly, ModAssembly }.Distinct()];

    /// <summary>
    ///     Provides access to the Gantry mod API context, including side-specific helpers and utilities.
    /// </summary>
    IModApiContext ApiEx { get; }

    /// <summary>
    ///     The current application side (client, server, or universal).
    /// </summary>
    EnumAppSide Side => ApiEx.Side;

    /// <summary>
    ///     Provides localisation and translation services for the mod.
    /// </summary>
    IStringTranslator Lang { get; }

    /// <summary>
    ///     Harmony patching service for runtime method interception and patch management.
    /// </summary>
    IHarmonyPatchingService Harmony => Resolve<IHarmonyPatchingService>();

    /// <summary>
    ///     The dependency injection service provider for the mod.
    /// </summary>
    IServiceProvider Services { get; }

    /// <summary>
    ///     Resolves a service of the specified type from the mod's service provider.
    /// </summary>
    /// <typeparam name="TService">The type of service to resolve.</typeparam>
    /// <returns>The resolved service instance.</returns>
    TService Resolve<TService>()
        where TService : notnull
        => Services.GetRequiredService<TService>();

    /// <summary>
    ///     Brighter command processor for CQRS and messaging patterns.
    /// </summary>
    ICommandProcessor CommandProcessor => Resolve<ICommandProcessor>();

    /// <summary>
    ///     File system service for mod data and configuration file access.
    /// </summary>
    IFileSystemService IO => Resolve<IFileSystemService>();

    /// <summary>
    ///     Mod settings service for configuration and settings management.
    /// </summary>
    IModSettingsService Settings => Resolve<IModSettingsService>();

    /// <summary>
    ///     The cancellation token for the mod's lifetime, which is triggered when the mod is unloaded or the game is shutting down.
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    ///     Writes a verbose debug message to the mod logger.
    /// </summary>
    /// <param name="messageTemplate">The message template to log.</param>
    /// <param name="args">Arguments for formatting the message.</param>
    public void Log(string messageTemplate, params object[] args)
        => Logger.VerboseDebug(messageTemplate, args);

    /// <summary>
    ///     Injects the required services based on the specified application side.
    /// </summary>
    /// <param name="instance">The instance to inject services into.</param>
    public void Compose<T>(T instance)
        where T : notnull
    {
        if (Services is null) return;

        //
        // Instance property injection
        //

        var properties = instance
            .GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
            .Where(p => p.GetCustomAttributes(typeof(InjectAttribute), inherit: false)
                .Cast<InjectAttribute>()
                .Any(attr => attr.Side == EnumAppSide.Universal || attr.Side == Side))
            .ToList();

        foreach (var property in properties)
        {
            var propertyType = property.PropertyType;
            var service = Services.GetService(propertyType);
            if (service is null)
            {
                Logger.Error(
                    $"Failed to inject service of type {propertyType.FullName} into property {property.Name} of mod system {GetType().FullName} for side {Side}.");
                continue;
            }
            property.SetValue(instance, service);

        }

        //
        // Sided<> support
        // 

        var staticSidedProperties = instance
            .GetType()
            .GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
            .Where(p => p.GetCustomAttributes(typeof(InjectAttribute), inherit: false)
                .Cast<InjectAttribute>()
                .Any(attr => attr.Side == EnumAppSide.Universal || attr.Side == Side))
            .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(Sided<>))
            .ToList();

        foreach (var property in staticSidedProperties)
        {
            var sidedType = property.PropertyType;
            var genericArg = sidedType.GetGenericArguments().First();
            var service = Services.GetService(genericArg);
            if (service is null)
            {
                Logger.Error(
                    $"Failed to inject service of type {genericArg.FullName} into static sided property {property.Name} of mod system {GetType().FullName} for side {Side}.");
                continue;
            }
            if (property.GetValue(instance) is not ISidedInstance sidedInstance)
            {
                Logger.Error(
                    $"Failed to inject service of type {genericArg.FullName} into static sided property {property.Name} of mod system {GetType().FullName} for side {Side} because the Sided<> instance is null.");
                continue;
            }
            sidedInstance.Set(Side, service);
        }

        // ROADMAP: Add MEF composition support here in future.
    }
}