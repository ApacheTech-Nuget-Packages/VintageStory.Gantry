using ApacheTech.Common.Extensions.Reflection;
using Gantry.Core.Abstractions;
using Gantry.Core.Annotation;
using Gantry.Services.HarmonyPatches.Abstractions;
using Gantry.Services.HarmonyPatches.Annotations;
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
    private readonly ICoreGantryAPI _gantry;
    private readonly Dictionary<string, Harmony> _instances = [];
    private readonly HarmonyPatchingServiceOptions _options;

    /// <summary>
    ///     Initialises a new instance of the <see cref="HarmonyPatchingService"/> class.
    /// </summary>
    [ActivatorUtilitiesConstructor]
    public HarmonyPatchingService(ICoreGantryAPI gantry, HarmonyPatchingServiceOptions options)
    {
        gantry.Log("Starting Harmony Patching Service");
        _api = gantry.Uapi;
        _gantry = gantry;
        _options = options;
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
    public Harmony Default => CreateOrUseInstance(_options.DefaultInstanceName);

    #region Patching

    /// <summary>
    ///     By default, all annotated [HarmonyPatch] classes in the executing assembly will
    ///     be processed at launch. Manual patches can be processed later on at runtime.
    /// </summary>
    public IHarmonyPatchingService PatchModAssembly()
    {
        foreach (var assembly in _gantry.ModAssemblies)
        {
            PatchAssembly(assembly);
        }
        return this;
    }

    /// <summary>
    ///     By default, all annotated [HarmonyPatch] classes in the executing assembly will
    ///     be processed at launch. Manual patches can be processed later on at runtime.
    /// </summary>
    public void PatchAssembly(Assembly assembly)
    {
        var side = _api.Side;
        try
        {
            var n = assembly.GetName().Name;
            var harmony = (assembly == _gantry.ModAssembly)
                ? Default : CreateOrUseInstance(assembly.GetName().Name);
            PatchGantryPatchClasses(harmony, assembly);
            PatchAll(harmony, assembly);
            var patches = harmony.GetPatchedMethods().ToList();
            if (patches.Count == 0) return;
            _gantry.Log($"\tPatched {side} Methods:");
            foreach (var method in patches)
            {
                _gantry.Log($"\t\t{method.FullDescription()}");
            }
        }
        catch (Exception ex)
        {
            _gantry.Logger.Error(ex);
        }
    }

    /// <summary>
    ///     Runs all patches within classes decorated with the <see cref="HarmonySidedPatchAttribute" /> attribute, for the given side.
    /// </summary>
    /// <param name="instance">The harmony instance for which to run the patches for.</param>
    /// <param name="assembly">The assembly that hold the annotated patch classes to process.</param>
    private void PatchAll(Harmony instance, Assembly assembly)
    {
        var side = _api.Side;
        var sidedPatches = assembly.GetTypesWithAttribute<HarmonySidedPatchAttribute>();
        foreach (var (type, attribute) in sidedPatches)
        {
            if (HasMissingDependencies(type)) continue;
            if (attribute.Side is not EnumAppSide.Universal && attribute.Side != _api.Side) continue;
            PatchMethods(instance, type, side);
        }
    }

    /// <summary>
    ///     Patches all settings consumers found in the specified assembly.
    /// </summary>
    public void PatchGantryPatchClasses(Harmony harmony, Assembly assembly)
    {
        var side = _api.Side;

        var patchClasses = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Where(t => t.IsAssignableTo(typeof(IGantryPatchClass)))
            .Where(t => !HasMissingDependencies(t))
            .Select(t => (IGantryPatchClass)ActivatorUtilities.CreateInstance(_gantry.Services, t))
            .ToList();

        foreach (var patchClass in patchClasses)
        {
            // TODO: Replace with analyser once available.
            if (patchClass.GetType().GetCustomAttribute<HarmonySidedPatchAttribute>() is not null)
            {
                throw new InvalidOperationException($"Patch class {patchClass.GetType().Name} should not be annotated with HarmonySidedPatchAttribute. Use the attribute on individual patch methods instead.");
            }

            patchClass.Initialise(_gantry);
            var type = patchClass.GetType();
            PatchMethods(harmony, type, side);
        }
    }

    private static IEnumerable<MethodInfo> GetMethodsForSide(Type type, EnumAppSide side)
    {
        var patches = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.GetCustomAttribute<HarmonyPatch>() is not null)
            .Where(m => m.GetCustomAttributes().Any(attr =>
                attr is HarmonyPrefix or HarmonyPostfix or HarmonyTranspiler or HarmonyFinalizer));

        foreach (var m in patches)
        {
            var sidedAttribute = m.GetCustomAttribute<HarmonySidedPatchAttribute>();
            var runsOnAttribute = m.GetCustomAttribute<RunsOnAttribute>();
            if (sidedAttribute is null && runsOnAttribute is null)
            {
                // TODO: Replace with analyser once available.
                throw new InvalidOperationException($"Method {m.Name} in {type.Name} is missing a HarmonySidedPatch or RunsOn attribute.");
            }

            if (sidedAttribute?.Side == EnumAppSide.Universal || sidedAttribute?.Side == side
            ||  runsOnAttribute?.Side == EnumAppSide.Universal || runsOnAttribute?.Side == side)
            {
                yield return m;
            }
        }
    }

    private void PatchMethods(Harmony instance, Type type, EnumAppSide side)
    {
        _gantry.Log($"Patching {type} [{side}]");
        var methods = GetMethodsForSide(type, side).ToArray();
        foreach (var method in methods)
        {
            var harmonyPatch = method.GetCustomAttribute<HarmonyPatch>();
            if (harmonyPatch is null) continue;

            var originalMethod = GetTargetMethod(harmonyPatch);
            if (originalMethod is null)
            {
                _gantry.Log($" - Failed to resolve target method for {side} patch: {method.Name}");
                _gantry.Logger.Error($"Failed to resolve target method for {side} patch: {method.Name}. Method may have been removed, or renamed.");
                continue;
            }

            var patchedMethod = new HarmonyMethod(method) { before = [$"{method.Name}_{side}"] };
            if (method.GetCustomAttribute<HarmonyPrefix>() is not null)
            {
                if (originalMethod.HasPatch(HarmonyPatchType.Prefix, $"{method.Name}_{side}")) continue;
                _gantry.Log($" - Applying {side} prefix patch: {method.Name}_{side}");
                instance.Patch(originalMethod, prefix: patchedMethod);
            }
            else if (method.GetCustomAttribute<HarmonyPostfix>() is not null)
            {
                if (originalMethod.HasPatch(HarmonyPatchType.Postfix, $"{method.Name}_{side}")) continue;
                _gantry.Log($" - Applying {side} postfix patch: {method.Name}_{side}");
                instance.Patch(originalMethod, postfix: patchedMethod);
            }
            else if (method.GetCustomAttribute<HarmonyTranspiler>() is not null)
            {
                if (originalMethod.HasPatch(HarmonyPatchType.Transpiler, $"{method.Name}_{side}")) continue;
                _gantry.Log($" - Applying {side} transpiler patch: {method.Name}_{side}");
                instance.Patch(originalMethod, transpiler: patchedMethod);
            }
            else if (method.GetCustomAttribute<HarmonyFinalizer>() is not null)
            {
                if (originalMethod.HasPatch(HarmonyPatchType.Finalizer, $"{method.Name}_{side}")) continue;
                _gantry.Log($" - Applying {side} finaliser patch: {method.Name}_{side}");
                instance.Patch(originalMethod, finalizer: patchedMethod);
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
        foreach (var assembly in _gantry.ModAssemblies)
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
            _gantry.Logger.Error(ex);
        }
    }

    /// <summary>
    ///     Unpatches all patches within classes decorated with the <see cref="HarmonySidedPatchAttribute" /> attribute, for the given side.
    /// </summary>
    /// <param name="instance">The harmony instance to unpatch.</param>
    /// <param name="assembly">The assembly that hold the annotated patch classes to process.</param>
    private void UnpatchAll(Harmony instance, Assembly assembly)
    {
        var side = _api.Side;
        var sidedPatches = assembly.GetTypesWithAttribute<HarmonySidedPatchAttribute>();
        foreach (var (type, attribute) in sidedPatches)
        {
            if (HasMissingDependencies(type)) continue;
            if (attribute.Side is not EnumAppSide.Universal && attribute.Side != _api.Side) continue;
            _gantry.Log($"Unpatching {type} [{side}]");

            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                var harmonyPatch = method.GetCustomAttribute<HarmonyPatch>();
                if (harmonyPatch is null) continue;

                var originalMethod = GetTargetMethod(harmonyPatch);
                if (originalMethod is null)
                {
                    _gantry.Log($" - Failed to resolve target method for {side} patch: {method.Name}");
                    _gantry.Logger.Error($"Failed to resolve target method for {side} patch: {method.Name}. Method may have been removed, or renamed.");
                    continue;
                }

                if (method.GetCustomAttribute<HarmonyPrefix>() is not null)
                {
                    if (!originalMethod.HasPatch(HarmonyPatchType.Prefix, $"{method.Name}_{side}")) continue;
                    _gantry.Log($" - Unpatching {side} prefix patch: {method.Name}");
                    instance.Unpatch(originalMethod, patch: method);
                }
                else if (method.GetCustomAttribute<HarmonyPostfix>() is not null)
                {
                    if (!originalMethod.HasPatch(HarmonyPatchType.Postfix, $"{method.Name}_{side}")) continue;
                    _gantry.Log($" - Unpatching {side} postfix patch: {method.Name}");
                    instance.Unpatch(originalMethod, patch: method);
                }
                else if (method.GetCustomAttribute<HarmonyTranspiler>() is not null)
                {
                    if (!originalMethod.HasPatch(HarmonyPatchType.Transpiler, $"{method.Name}_{side}")) continue;
                    _gantry.Log($" - Unpatching {side} transpiler patch: {method.Name}");
                    instance.Unpatch(originalMethod, patch: method);
                }
                else if (method.GetCustomAttribute<HarmonyFinalizer>() is not null)
                {
                    if (!originalMethod.HasPatch(HarmonyPatchType.Finalizer, $"{method.Name}_{side}")) continue;
                    _gantry.Log($" - Unpatching {side} finaliser patch: {method.Name}");
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
        harmonyId ??= _options.DefaultInstanceName;
        if (_instances.TryGetValue(harmonyId, out var instance))
        {
            return instance;
        }

        var harmony = new Harmony(harmonyId);
        _instances.Add(harmonyId, harmony);
        _gantry.Log(" - Created Harmony Instance: {0}", harmonyId);
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
            _gantry.Log($"Skipping patches for {type.Name} due to missing dependency: {attribute.ModId}");
            return true;
        }
        return false;
    }

    #endregion
}