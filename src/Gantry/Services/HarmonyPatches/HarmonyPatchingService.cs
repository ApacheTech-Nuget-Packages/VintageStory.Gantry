using System.Reflection;
using ApacheTech.Common.Extensions.Harmony;
using ApacheTech.Common.Extensions.Reflection;
using Gantry.Services.HarmonyPatches.Extensions;

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
    private readonly ICoreAPI _api;
    private readonly Dictionary<string, Harmony> _instances = [];
    private readonly string _defaultInstanceName;

    /// <summary>
    ///     Initialises a new instance of the <see cref="HarmonyPatchingService"/> class.
    /// </summary>
    /// <param name="api">The api to use for game related calls.</param>
    /// <param name="options">The options.</param>
    [ActivatorUtilitiesConstructor]
    public HarmonyPatchingService(ICoreAPI api, HarmonyPatchingServiceOptions options)
    {
        _api = api;
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
    ///     By default, all annotated [HarmonyPatch] classes in the executing assembly will
    ///     be processed at launch. Manual patches can be processed later on at runtime.
    /// </summary>
    public void PatchModAssembly()
    {
        foreach (var assembly in ModEx.ModAssemblies)
        {
            PatchAssembly(assembly);
        }
    }

    /// <summary>
    ///     By default, all annotated [HarmonyPatch] classes in the executing assembly will
    ///     be processed at launch. Manual patches can be processed later on at runtime.
    /// </summary>
    public void PatchAssembly(Assembly assembly)
    {
        try
        {
            var harmony = CreateOrUseInstance(assembly.FullName);
            PatchAll(harmony, assembly);
            var patches = harmony.GetPatchedMethods().ToList();
            if (!patches.Any()) return;
            ApiEx.Logger.VerboseDebug("\tPatched Methods:");
            foreach (var method in patches)
            {
                ApiEx.Logger.VerboseDebug($"\t\t{method.FullDescription()}");
            }
        }
        catch (Exception ex)
        {
            ApiEx.Logger.Error(ex);
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
    private void PatchAll(Harmony instance, Assembly assembly)
    {
        var sidedPatches = assembly.GetTypesWithAttribute<HarmonySidedPatchAttribute>();
        foreach (var (type, attribute) in sidedPatches)
        {
            ApiEx.Logger.VerboseDebug($"Patching {type}");
            if (HasMissingDependencies(type)) continue;
            if (attribute.Side is not EnumAppSide.Universal && attribute.Side != _api.Side) continue;

            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                var harmonyPatch = method.GetCustomAttribute<HarmonyPatch>();
                if (harmonyPatch is not null)
                {
                    var originalMethod = GetTargetMethod(harmonyPatch);
                    if (originalMethod is null)
                    {
                        ApiEx.Logger.VerboseDebug($" - Failed to resolve target method for patch: {method.Name}");
                        ApiEx.Logger.Error($"Failed to resolve target method for patch: {method.Name}. Method may have been removed, or renamed.");
                        continue;
                    }

                    var patchedMethod = new HarmonyMethod(method) { before = [method.Name] };
                    if (method.GetCustomAttribute<HarmonyPrefix>() is not null)
                    {
                        if (originalMethod.HasPatch(HarmonyPatchType.Prefix, method.Name)) continue;
                        ApiEx.Logger.VerboseDebug($" - Applying prefix patch: {method.Name}");
                        instance.Patch(originalMethod, prefix: patchedMethod);
                    }
                    else if (method.GetCustomAttribute<HarmonyPostfix>() is not null)
                    {
                        if (originalMethod.HasPatch(HarmonyPatchType.Postfix, method.Name)) continue;
                        ApiEx.Logger.VerboseDebug($" - Applying postfix patch: {method.Name}");
                        instance.Patch(originalMethod, postfix: patchedMethod);
                    }
                    else if(method.GetCustomAttribute<HarmonyTranspiler>() is not null)
                    {
                        if (originalMethod.HasPatch(HarmonyPatchType.Transpiler, method.Name)) continue;
                        ApiEx.Logger.VerboseDebug($" - Applying transpiler patch: {method.Name}");
                        instance.Patch(originalMethod, transpiler: patchedMethod);
                    }
                    else if (method.GetCustomAttribute<HarmonyFinalizer>() is not null)
                    {
                        if (originalMethod.HasPatch(HarmonyPatchType.Finalizer, method.Name)) continue;
                        ApiEx.Logger.VerboseDebug($" - Applying finaliser patch: {method.Name}");
                        instance.Patch(originalMethod, finalizer: patchedMethod);
                    }
                    else if (method.GetCustomAttribute<HarmonyReversePatch>() is not null)
                    {
                        ApiEx.Logger.VerboseDebug($" - Applying reverse patch: {method.Name}");
                        instance.CreateReversePatcher(originalMethod, patchedMethod).Patch();
                    }

                    if (method.GetCustomAttribute<HarmonyTelemetryPatchAttribute>() is not null)
                    {
                        ApiEx.Logger.VerboseDebug($" - Applying telemetry patch: {method.Name}");
                        instance.ApplyTelemetryPatch(method, originalMethod);
                    }

                }
            }
        }
    }

    /// <summary>
    ///     Resolves the target method specified by the HarmonyPatch attribute, including support for constructors.
    /// </summary>
    /// <param name="patchAttribute">The HarmonyPatch attribute.</param>
    /// <returns>The target MethodBase.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the target method cannot be resolved.</exception>
    private static MethodBase GetTargetMethod(HarmonyPatch patchAttribute)
    {
        if (patchAttribute.info.declaringType is null)
        {
            throw new InvalidOperationException("Invalid HarmonyPatch attribute; declaring type is not specified. Are internals visible to Gantry?");
        }

        return patchAttribute.info.methodType switch
        {
            MethodType.Getter => AccessTools.PropertyGetter(patchAttribute.info.declaringType, patchAttribute.info.methodName),
            MethodType.Setter => AccessTools.PropertySetter(patchAttribute.info.declaringType, patchAttribute.info.methodName),
            MethodType.Constructor => AccessTools.Constructor(patchAttribute.info.declaringType, patchAttribute.info.argumentTypes),
            MethodType.StaticConstructor => AccessTools.Constructor( patchAttribute.info.declaringType, patchAttribute.info.argumentTypes, searchForStatic: true ),
            _ => AccessTools.Method(patchAttribute.info.declaringType, patchAttribute.info.methodName, patchAttribute.info.argumentTypes)
        };
    }

    /// <summary>
    ///     Checks if the specified type is missing any required dependencies.
    /// </summary>
    /// <param name="type">The type to check for missing dependencies.</param>
    /// <returns>
    ///     <c>true</c> if the type has at least one missing dependency; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    ///     This method retrieves all <see cref="RequiresModAttribute"/> attributes applied to the specified type.
    ///     If there are no such attributes, the type is considered to have no missing dependencies.
    ///     If the type has dependencies, the method verifies whether any required mods are missing using the mod loader.
    /// </remarks>
    private bool HasMissingDependencies(Type type)
    {
        var attributes = type.GetCustomAttributes<RequiresModAttribute>().ToArray();
        if (attributes.Length <= 0) return false;
        foreach (var attribute in attributes)
        {
            if (_api.ModLoader.IsModEnabled(attribute.ModId)) continue;
            ApiEx.Logger.VerboseDebug($"Skipping patches for {type.Name} due to missing dependency: {attribute.ModId}");
            return true;
        }
        return false;
    }

    /// <summary>
    ///     Un-patches all methods, within all patch host instances being handled by this service.
    /// </summary>
    public void UnpatchAll()
    {
        try
        {
            foreach (var harmony in _instances)
            {
                ApiEx.Logger.VerboseDebug($"Unpatching Harmony instance: {harmony.Key}");
                harmony.Value.UnpatchAll(harmony.Key);
            }
        }
        catch (Exception ex)
        {
            ApiEx.Logger.Error(ex);
        }
    }

    private void DisposeAllPatchClasses(Assembly assembly)
    {
        var sidedPatches = assembly?.GetTypesWithAttribute<HarmonySidedPatchAttribute>() ?? [];
        foreach (var (type, attribute) in sidedPatches)
        {
            if (attribute.Side is not EnumAppSide.Universal && attribute.Side != _api.Side) continue;
            type.CallMethod("Dispose");
        }
    }
}