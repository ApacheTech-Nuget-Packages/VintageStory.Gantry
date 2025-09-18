using Microsoft.CodeAnalysis;

namespace Gantry.Analysers.CSharp.Extensions;

/// <summary>
///     Extension methods for <see cref="IMethodSymbol"/>.
/// </summary>
internal static class MethodSymbolExtensions
{
    /// <summary>
    ///     Determines whether the specified method symbol is a Harmony patch method
    /// </summary>
    /// <param name="method">The method symbol to check</param>
    /// <returns>True if the method is a Harmony patch; otherwise, false</returns>
    internal static bool IsHarmonyPatch(this IMethodSymbol method)
    {
        var attrs = method.GetAttributes();
        if (attrs.Length == 0) return false;
        var harmonyAttributes = new[]
        {
            "HarmonyLib.HarmonyPrefix",
            "HarmonyLib.HarmonyPostfix",
            "HarmonyLib.HarmonyTranspiler",
            "HarmonyLib.HarmonyFinalizer"
        };
        return attrs.Any(a => harmonyAttributes.Contains(a.AttributeClass?.ToString()));
    }

    internal static bool IsOrDerivesFrom(this ITypeSymbol? candidate, INamedTypeSymbol? target)
    {
        if (candidate is null || target is null) return false;
        for (var t = candidate; t is not null; t = t.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(t, target)) return true;
        }
        return false;
    }

    internal static bool IsDecoratredWithAny(this IMethodSymbol method, params INamedTypeSymbol?[] typeSymbols)
    {
        if (typeSymbols.Length == 0) return false;
        var list = method.GetAttributes();
        if (list.Length == 0) return false;
        foreach (var a in list)
        {
            var attrClass = a.AttributeClass;
            foreach (var t in typeSymbols)
            {
                if (t is null) continue;
                if (attrClass.IsOrDerivesFrom(t)) return true;
            }
        }
        return false;
    }
}
