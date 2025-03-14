#nullable enable
using System.Reflection;

namespace Gantry.Services.HarmonyPatches.Extensions;

/// <summary>
///    Provides extension methods for the Harmony library.
/// </summary>
public static class HarmonyExtensions
{
    /// <summary>
    ///     Checks if the specified method has a patch of the given patch type, optionally filtered by a unique ID.
    /// </summary>
    /// <param name="originalMethod">The method to check for patches on.</param>
    /// <param name="patchType">The type of patch to check for (e.g., Transpiler, Prefix, Finaliser, Postfix).</param>
    /// <param name="uniqueId">An optional unique ID to filter the patches by.</param>
    /// <returns>True if there is a patch of the given type, optionally with the specified unique ID.</returns>
    public static bool HasPatch(this MethodBase originalMethod, HarmonyPatchType patchType, string? uniqueId = null)
    {
        var patchInfo = Harmony.GetPatchInfo(originalMethod);
        return patchType switch
        {
            HarmonyPatchType.Transpiler => HasPatchOfType(patchInfo?.Transpilers),
            HarmonyPatchType.Prefix => HasPatchOfType(patchInfo?.Prefixes),
            HarmonyPatchType.Postfix => HasPatchOfType(patchInfo?.Postfixes),
            HarmonyPatchType.Finalizer => HasPatchOfType(patchInfo?.Finalizers),
            _ => false
        };
        bool HasPatchOfType<T>(ICollection<T>? patches) where T : Patch
        {
            return patches is { Count: > 0 } && (uniqueId is null || patches.Any(patch => patch.before?.Contains(uniqueId) == true));
        }
    }

    /// <summary>
    ///     Retrieves all unique owners associated with patches on the specified method.
    /// </summary>
    /// <param name="method">The method for which to retrieve patch owners.</param>
    /// <returns>A collection of unique owner IDs representing Harmony instances that have patched the method.</returns>
    public static IEnumerable<string> GetPatchOwners(this MethodBase method)
    {
        var patchInfo = Harmony.GetPatchInfo(method);
        if (patchInfo is null) return [];

        // Combine all patches (Prefixes, Postfixes, Transpilers, Finalizers) into one collection
        var allPatches = patchInfo.Prefixes
            .Concat(patchInfo.Postfixes)
            .Concat(patchInfo.Transpilers)
            .Concat(patchInfo.Finalizers);

        // Extract unique owners
        return allPatches
            .Select(patch => patch.owner)
            .Where(owner => !string.IsNullOrWhiteSpace(owner)) // Exclude null/empty owners
            .Distinct(); // Ensure uniqueness
    }

    /// <summary>
    ///     Retrieves the maximum priority of all patches of the specified type on the given method.
    /// </summary>
    /// <param name="method">The method to inspect for patches.</param>
    /// <param name="patchType">The type of patch to consider (Prefix, Postfix, Transpiler, Finalizer).</param>
    /// <returns>The highest priority value, or <c>null</c> if no patches of the specified type exist.</returns>
    public static int? GetMaxPriority(this MethodBase method, HarmonyPatchType patchType)
    {
        var patchInfo = Harmony.GetPatchInfo(method);
        if (patchInfo is null)
        {
            return null;
        }

        return patchType switch
        {
            HarmonyPatchType.Prefix => patchInfo.Prefixes?.Max(patch => (int?)patch.priority),
            HarmonyPatchType.Postfix => patchInfo.Postfixes?.Max(patch => (int?)patch.priority),
            HarmonyPatchType.Transpiler => patchInfo.Transpilers?.Max(patch => (int?)patch.priority),
            HarmonyPatchType.Finalizer => patchInfo.Finalizers?.Max(patch => (int?)patch.priority),
            _ => null
        };
    }

    /// <summary>
    ///     Retrieves the minimum priority of all patches of the specified type on the given method.
    /// </summary>
    /// <param name="method">The method to inspect for patches.</param>
    /// <param name="patchType">The type of patch to consider (Prefix, Postfix, Transpiler, Finalizer).</param>
    /// <returns>The lowest priority value, or <c>null</c> if no patches of the specified type exist.</returns>
    public static int? GetMinPriority(this MethodBase method, HarmonyPatchType patchType)
    {
        var patchInfo = Harmony.GetPatchInfo(method);
        if (patchInfo is null)
        {
            return null;
        }

        return patchType switch
        {
            HarmonyPatchType.Prefix => patchInfo.Prefixes?.Min(patch => (int?)patch.priority),
            HarmonyPatchType.Postfix => patchInfo.Postfixes?.Min(patch => (int?)patch.priority),
            HarmonyPatchType.Transpiler => patchInfo.Transpilers?.Min(patch => (int?)patch.priority),
            HarmonyPatchType.Finalizer => patchInfo.Finalizers?.Min(patch => (int?)patch.priority),
            _ => null
        };
    }

    /// <summary>
    ///     Attempts to resolve the target method that the specified patch method is patching.
    /// </summary>
    /// <param name="patchMethod">The method to resolve the target for.</param>
    /// <param name="method">The resolved method.</param>
    /// <returns>The resolved target method that the patch method is patching, or null if no target can be resolved.</returns>
    public static bool TryResolvePatchTarget(this MethodInfo patchMethod, out MethodBase? method)
    {
        method = null;
        var harmonyPatchAttribute = patchMethod.GetCustomAttribute<HarmonyPatch>();
        if (harmonyPatchAttribute is null) return false;
        if (harmonyPatchAttribute.info.declaringType is not null && harmonyPatchAttribute.info.methodName is not null)
        {
            method = AccessTools.Method(
                harmonyPatchAttribute.info.declaringType,
                harmonyPatchAttribute.info.methodName,
                harmonyPatchAttribute.info.argumentTypes
            );
        }

        return method is not null;
    }

    /// <summary>
    ///     Applies a telemetry patch to the specified method using the Harmony library, adding logging and execution timing.
    /// </summary>
    /// <param name="instance">The Harmony instance used to apply the patch.</param>
    /// <param name="method">The method that serves as the reference for naming and categorisation in telemetry.</param>
    /// <param name="originalMethod">The original method to which the telemetry patch will be applied.</param>
    /// <remarks>
    ///     This method adds a prefix and postfix to the <paramref name="originalMethod"/>. The prefix logs the method's 
    ///     execution and starts a stopwatch to measure execution time, while the postfix stops the stopwatch and logs 
    ///     the execution duration in milliseconds. Both prefix and postfix methods are categorised under "Telemetry".
    /// </remarks>
    public static void ApplyTelemetryPatch(this Harmony instance, MethodInfo method, MethodBase originalMethod)
    {
        var prefix = new HarmonyMethod(Prefix)
        {
            before = [method.Name],
            priority = Priority.First,
            category = "Telemetry"
        };
        var postfix = new HarmonyMethod(Postfix)
        {
            before = [method.Name],
            priority = Priority.Last,
            category = "Telemetry"
        };
        instance.Patch(originalMethod, prefix: prefix, postfix: postfix);

        // Prefix method to log the start of method execution and initialise a stopwatch for timing.
        void Prefix(out Stopwatch __state)
        {
            G.Logger.VerboseDebug($"Running method: {method.Name}");
            __state = Stopwatch.StartNew();
        }

        // Postfix method to log the method's execution time upon completion.
        void Postfix(Stopwatch __state)
        {
            __state.Stop();
            G.Logger.VerboseDebug($"Method {method.Name} took {__state.ElapsedMilliseconds} ms to execute.");
        }
    }
}