namespace Gantry.Services.HarmonyPatches.Annotations;

/// <summary>
///     Extends <see cref="HarmonyPatch"/> to provide a shorthand way to define patches with argument types.
/// </summary>
public class HarmonyPatchExAttribute : HarmonyPatch
{
    /// <summary>
    ///     Initialises a new instance of the <see cref="HarmonyPatchExAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">The full name of the declaring type.</param>
    /// <param name="methodName">The name of the method to patch.</param>
    /// <param name="arguments">The argument types of the target method.</param>
    public HarmonyPatchExAttribute(string declaringType, string methodName, params Type[] arguments)
        : base(declaringType, methodName) => info.argumentTypes = arguments;
}