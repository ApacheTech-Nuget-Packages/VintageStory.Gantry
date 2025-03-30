using System.Reflection;
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
    /// <param name="options">The options.</param>
    [ActivatorUtilitiesConstructor]
    public HarmonyPatchingService(HarmonyPatchingServiceOptions options)
    {
        _api = ApiEx.Current;
        _defaultInstanceName = options.DefaultInstanceName;
        if (options.AutoPatchModAssembly) PatchModAssembly();
    }

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        UnpatchModAssembly();
    }

    /// <summary>
    ///     Gets the default harmony instance for the mod.
    /// </summary>
    /// <value>
    /// The default Harmony instance for the mod, with the mod assembly's full name as the instance ID.
    /// </value>
    public Harmony Default => CreateOrUseInstance(_defaultInstanceName);

    #region Patching

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
        var side = ApiEx.Current.Side;
        try
        {
            var harmony = CreateOrUseInstance(assembly.FullName);
            PatchAll(harmony, assembly);
            var patches = harmony.GetPatchedMethods().ToList();
            if (!patches.Any()) return;
            G.Logger.VerboseDebug($"\tPatched {side} Methods:");
            foreach (var method in patches)
            {
                G.Logger.VerboseDebug($"\t\t{method.FullDescription()}");
            }
        }
        catch (Exception ex)
        {
            G.Logger.Error(ex);
        }
    }

    /// <summary>
    ///     Runs all patches within classes decorated with the <see cref="HarmonySidedPatchAttribute" /> attribute, for the given side.
    /// </summary>
    /// <param name="instance">The harmony instance for which to run the patches for.</param>
    /// <param name="assembly">The assembly that hold the annotated patch classes to process.</param>
    private void PatchAll(Harmony instance, Assembly assembly)
    {
        var side = ApiEx.Current.Side;
        var sidedPatches = assembly.GetTypesWithAttribute<HarmonySidedPatchAttribute>();
        foreach (var (type, attribute) in sidedPatches)
        {
            if (HasMissingDependencies(type)) continue;
            if (attribute.Side is not EnumAppSide.Universal && attribute.Side != _api.Side) continue;
            G.Logger.VerboseDebug($"Patching {type} [{side}]");

            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                var harmonyPatch = method.GetCustomAttribute<HarmonyPatch>();
                if (harmonyPatch is null) continue;

                var originalMethod = GetTargetMethod(harmonyPatch);
                if (originalMethod is null)
                {
                    G.Logger.VerboseDebug($" - Failed to resolve target method for {side} patch: {method.Name}");
                    G.Logger.Error($"Failed to resolve target method for {side} patch: {method.Name}. Method may have been removed, or renamed.");
                    continue;
                }

                var patchedMethod = new HarmonyMethod(method) { before = [$"{method.Name}_{side}"] };
                if (method.GetCustomAttribute<HarmonyPrefix>() is not null)
                {
                    if (originalMethod.HasPatch(HarmonyPatchType.Prefix, $"{method.Name}_{side}")) continue;
                    G.Logger.VerboseDebug($" - Applying {side} prefix patch: {method.Name}_{side}");
                    instance.Patch(originalMethod, prefix: patchedMethod);
                }
                else if (method.GetCustomAttribute<HarmonyPostfix>() is not null)
                {
                    if (originalMethod.HasPatch(HarmonyPatchType.Postfix, $"{method.Name}_{side}")) continue;
                    G.Logger.VerboseDebug($" - Applying {side} postfix patch: {method.Name}_{side}");
                    instance.Patch(originalMethod, postfix: patchedMethod);
                }
                else if (method.GetCustomAttribute<HarmonyTranspiler>() is not null)
                {
                    if (originalMethod.HasPatch(HarmonyPatchType.Transpiler, $"{method.Name}_{side}")) continue;
                    G.Logger.VerboseDebug($" - Applying {side} transpiler patch: {method.Name}_{side}");
                    instance.Patch(originalMethod, transpiler: patchedMethod);
                }
                else if (method.GetCustomAttribute<HarmonyFinalizer>() is not null)
                {
                    if (originalMethod.HasPatch(HarmonyPatchType.Finalizer, $"{method.Name}_{side}")) continue;
                    G.Logger.VerboseDebug($" - Applying {side} finaliser patch: {method.Name}_{side}");
                    instance.Patch(originalMethod, finalizer: patchedMethod);
                }
            }
        }
    }

    #endregion

    #region Unpatching

    /// <summary>
    ///     By default, all annotated [HarmonyPatch] classes in the executing assembly will
    ///     be processed at launch. Manual patches can be processed later on at runtime.
    /// </summary>
    public void UnpatchModAssembly()
    {
        foreach (var assembly in ModEx.ModAssemblies)
        {
            UnpatchAssembly(assembly);
        }
    }

    /// <summary>
    ///     By default, all annotated [HarmonyPatch] classes in the executing assembly will
    ///     be processed at launch. Manual patches can be processed later on at runtime.
    /// </summary>
    public void UnpatchAssembly(Assembly assembly)
    {
        try
        {
            var harmony = CreateOrUseInstance(assembly.FullName);
            UnpatchAll(harmony, assembly);
        }
        catch (Exception ex)
        {
            G.Logger.Error(ex);
        }
    }

    /// <summary>
    ///     Unpatches all patches within classes decorated with the <see cref="HarmonySidedPatchAttribute" /> attribute, for the given side.
    /// </summary>
    /// <param name="instance">The harmony instance to unpatch.</param>
    /// <param name="assembly">The assembly that hold the annotated patch classes to process.</param>
    private void UnpatchAll(Harmony instance, Assembly assembly)
    {
        var side = ApiEx.Current.Side;
        var sidedPatches = assembly.GetTypesWithAttribute<HarmonySidedPatchAttribute>();
        foreach (var (type, attribute) in sidedPatches)
        {
            if (HasMissingDependencies(type)) continue;
            if (attribute.Side is not EnumAppSide.Universal && attribute.Side != _api.Side) continue;
            G.Logger.VerboseDebug($"Unpatching {type} [{side}]");

            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                var harmonyPatch = method.GetCustomAttribute<HarmonyPatch>();
                if (harmonyPatch is null) continue;

                var originalMethod = GetTargetMethod(harmonyPatch);
                if (originalMethod is null)
                {
                    G.Logger.VerboseDebug($" - Failed to resolve target method for {side} patch: {method.Name}");
                    G.Logger.Error($"Failed to resolve target method for {side} patch: {method.Name}. Method may have been removed, or renamed.");
                    continue;
                }

                if (method.GetCustomAttribute<HarmonyPrefix>() is not null)
                {
                    if (!originalMethod.HasPatch(HarmonyPatchType.Prefix, $"{method.Name}_{side}")) continue;
                    G.Logger.VerboseDebug($" - Unpatching {side} prefix patch: {method.Name}");
                    instance.Unpatch(originalMethod, patch: method);
                }
                else if (method.GetCustomAttribute<HarmonyPostfix>() is not null)
                {
                    if (!originalMethod.HasPatch(HarmonyPatchType.Postfix, $"{method.Name}_{side}")) continue;
                    G.Logger.VerboseDebug($" - Unpatching {side} postfix patch: {method.Name}");
                    instance.Unpatch(originalMethod, patch: method);
                }
                else if (method.GetCustomAttribute<HarmonyTranspiler>() is not null)
                {
                    if (!originalMethod.HasPatch(HarmonyPatchType.Transpiler, $"{method.Name}_{side}")) continue;
                    G.Logger.VerboseDebug($" - Unpatching {side} transpiler patch: {method.Name}");
                    instance.Unpatch(originalMethod, patch: method);
                }
                else if (method.GetCustomAttribute<HarmonyFinalizer>() is not null)
                {
                    if (!originalMethod.HasPatch(HarmonyPatchType.Finalizer, $"{method.Name}_{side}")) continue;
                    G.Logger.VerboseDebug($" - Unpatching {side} finaliser patch: {method.Name}");
                    instance.Unpatch(originalMethod, patch: method);
                }
            }
        }
    }

    #endregion

    #region Utilities

    /// <summary>
    ///     Creates a new patch host, if one with the specified ID doesn't already exist.
    /// </summary>
    /// <param name="harmonyId">The identifier to use for the patch host.</param>
    /// <returns>A <see cref="Harmony" /> patch host.</returns>
    public Harmony CreateOrUseInstance(string? harmonyId)
    {
        harmonyId ??= string.Empty;
        if (_instances.TryGetValue(harmonyId, out var instance))
        {
            return instance;
        }

        var harmony = new Harmony(harmonyId);
        _instances.Add(harmonyId, harmony);
        return harmony;
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
            MethodType.StaticConstructor => AccessTools.Constructor(patchAttribute.info.declaringType, patchAttribute.info.argumentTypes, searchForStatic: true),
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
            G.Logger.VerboseDebug($"Skipping patches for {type.Name} due to missing dependency: {attribute.ModId}");
            return true;
        }
        return false;
    }

    #endregion
}