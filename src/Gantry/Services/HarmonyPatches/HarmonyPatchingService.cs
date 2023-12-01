using System.Reflection;
using ApacheTech.Common.Extensions.Harmony;
using ApacheTech.Common.Extensions.Reflection;
using Gantry.Core;
using Gantry.Services.HarmonyPatches.Annotations;
using HarmonyLib;
using Vintagestory.API.Common;

namespace Gantry.Services.HarmonyPatches;

/// <summary>
///     Provides methods of applying Harmony patches to the game.
/// </summary>
/// <remarks>
///     By default, all annotated [HarmonyPatch] classes in the executing assembly will
///     be processed at launch. Manual patches can be processed later on at runtime.
/// </remarks>
public class HarmonyPatchingService : IHarmonyPatchingService
{
    private readonly Dictionary<string, Harmony> _instances = new();
    private readonly string _defaultInstanceName;
        
    /// <summary>
    ///     Initialises a new instance of the <see cref="HarmonyPatchingService"/> class.
    /// </summary>
    public HarmonyPatchingService() : this(HarmonyPatchingServiceOptions.Default)
    {

    }

    /// <summary>
    ///     Initialises a new instance of the <see cref="HarmonyPatchingService"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public HarmonyPatchingService(HarmonyPatchingServiceOptions options)
    {
        _defaultInstanceName = options.DefaultInstanceName;
        if (options.AutoPatchModAssembly) PatchModAssembly();
    }

    /// <summary>
    ///     Creates a new patch host, if one with the specified ID doesn't already exist.
    /// </summary>
    /// <param name="harmonyId">The identifier to use for the patch host.</param>
    /// <returns>A <see cref="Harmony" /> patch host.</returns>
    public Harmony CreateOrUseInstance(string harmonyId)
    {
        if (_instances.TryGetValue(harmonyId, out var instance))
        {
            return instance;
        }

        var harmony = new Harmony(harmonyId);
        _instances.Add(harmonyId, harmony);
        return harmony;
    }

    /// <summary>
    /// By default, all annotated [HarmonyPatch] classes in the executing assembly will
    /// be processed at launch. Manual patches can be processed later on at runtime.
    /// </summary>
    public void PatchModAssembly()
    {
        PatchAssembly(ModEx.ModAssembly);
    }

    /// <summary>
    /// By default, all annotated [HarmonyPatch] classes in the executing assembly will
    /// be processed at launch. Manual patches can be processed later on at runtime.
    /// </summary>
    public void PatchAssembly(Assembly assembly)
    {
        try
        {
            var harmony = CreateOrUseInstance(assembly.FullName);
            PatchAll(harmony, assembly);
            var patches = harmony.GetPatchedMethods().ToList();
            if (!patches.Any()) return;
            ApiEx.Current.Logger.VerboseDebug($"\t[Gantry] {assembly.GetName()} - Patched Methods:"); // ApiEx.Current is null???
            foreach (var method in patches)
            {
                ApiEx.Current.Logger.VerboseDebug($"\t\t{method.FullDescription()}");
            }
        }
        catch (Exception ex)
        {
            ApiEx.Current.Logger.Error($"[Gantry] {ex}");
        }
    }

    /// <summary>
    ///     Gets the default harmony instance for the mod.
    /// </summary>
    /// <value>
    /// The default Harmony instance for the mod, with the mod assembly's full name as the instance ID.
    /// </value>
    public Harmony Default => CreateOrUseInstance(_defaultInstanceName);

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        UnpatchAll();
        _instances.Clear();
        DisposeAllPatchClasses(ModEx.ModAssembly);
    }

    /// <summary>
    ///     Runs all patches within classes decorated with the <see cref="HarmonySidedPatchAttribute" /> attribute, for the given side.
    /// </summary>
    /// <param name="instance">The harmony instance for which to run the patches for.</param>
    /// <param name="assembly">The assembly that hold the annotated patch classes to process.</param>
    private static void PatchAll(Harmony instance, Assembly assembly)
    {
        var sidedPatches = assembly.GetTypesWithAttribute<HarmonySidedPatchAttribute>();
        foreach (var (type, attribute) in sidedPatches)
        {
            if (attribute.Side is not EnumAppSide.Universal && attribute.Side != ApiEx.Side) continue;
            ApiEx.Current.Logger.VerboseDebug($"[Gantry] Patching {type}");
            var processor = instance.CreateClassProcessor(type);
            processor.Patch();
        }
    }

    /// <summary>
    ///     Un-patches all methods, within all patch host instances being handled by this service.
    /// </summary>
    public void UnpatchAll()
    {
        foreach (var harmony in _instances)
        {
            harmony.Value.UnpatchAll(harmony.Key);
        }
    }

    private static void DisposeAllPatchClasses(Assembly assembly)
    {
        var sidedPatches = assembly?.GetTypesWithAttribute<HarmonySidedPatchAttribute>()
                           ?? new List<(Type Type, HarmonySidedPatchAttribute Attribute)>();
        foreach (var (type, attribute) in sidedPatches)
        {
            if (attribute.Side is not EnumAppSide.Universal && attribute.Side != ApiEx.Side) continue;
            type.CallMethod("Dispose");
        }
    }
}