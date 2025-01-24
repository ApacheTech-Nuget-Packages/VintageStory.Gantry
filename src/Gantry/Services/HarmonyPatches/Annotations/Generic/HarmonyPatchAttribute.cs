namespace Gantry.Services.HarmonyPatches.Annotations.Generic;

/// <summary>
///     Annotation to define your Harmony patch methods.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Delegate | AttributeTargets.Method, AllowMultiple = true)]
public class HarmonyPatchAttribute<T> : HarmonyPatch where T : class
{
    /// <summary>
    ///     Initialises a new instance of the <see cref="HarmonyPatchAttribute{T}"/> class.
    /// </summary>
    public HarmonyPatchAttribute()
    {
        info.declaringType = typeof(T);
    }

    /// <summary>An annotation that specifies a method, property or constructor to patch</summary>
    /// <param name="methodName">The name of the method, property or constructor to patch</param>
    ///
    public HarmonyPatchAttribute(string methodName)
    {
        info.declaringType = typeof(T);
        info.methodName = methodName;
    }

    /// <summary>An annotation that specifies a method, property or constructor to patch</summary>
    /// <param name="methodName">The name of the method, property or constructor to patch</param>
    /// <param name="argumentTypes">An array of argument types to target overloads</param>
    ///
    public HarmonyPatchAttribute(string methodName, params Type[] argumentTypes)
    {
        info.declaringType = typeof(T);
        info.methodName = methodName;
        info.argumentTypes = argumentTypes;
    }

    /// <summary>An annotation that specifies a method, property or constructor to patch</summary>
    /// <param name="methodName">The name of the method, property or constructor to patch</param>
    /// <param name="argumentTypes">An array of argument types to target overloads</param>
    /// <param name="argumentVariations">An array of <see cref="ArgumentType"/></param>
    ///
    public HarmonyPatchAttribute(string methodName, Type[] argumentTypes, ArgumentType[] argumentVariations)
    {
        info.declaringType = typeof(T);
        info.methodName = methodName;
        ParseSpecialArguments(argumentTypes, argumentVariations);
    }

    /// <summary>An annotation that specifies a method, property or constructor to patch</summary>
    /// <param name="methodName">The name of the method, property or constructor to patch</param>
    /// <param name="methodType">The <see cref="MethodType"/></param>
    ///
    public HarmonyPatchAttribute(string methodName, MethodType methodType)
    {
        info.declaringType = typeof(T);
        info.methodName = methodName;
        info.methodType = methodType;
    }

    /// <summary>An annotation that specifies a method, property or constructor to patch</summary>
    /// <param name="methodType">The <see cref="MethodType"/></param>
    ///
    public HarmonyPatchAttribute(MethodType methodType)
    {
        info.declaringType = typeof(T);
        info.methodType = methodType;
    }

    /// <summary>An annotation that specifies a method, property or constructor to patch</summary>
    /// <param name="methodType">The <see cref="MethodType"/></param>
    /// <param name="argumentTypes">An array of argument types to target overloads</param>
    ///
    public HarmonyPatchAttribute(MethodType methodType, params Type[] argumentTypes)
    {
        info.declaringType = typeof(T);
        info.methodType = methodType;
        info.argumentTypes = argumentTypes;
    }

    /// <summary>An annotation that specifies a method, property or constructor to patch</summary>
    /// <param name="methodType">The <see cref="MethodType"/></param>
    /// <param name="argumentTypes">An array of argument types to target overloads</param>
    /// <param name="argumentVariations">An array of <see cref="ArgumentType"/></param>
    ///
    public HarmonyPatchAttribute(MethodType methodType, Type[] argumentTypes, ArgumentType[] argumentVariations)
    {
        info.declaringType = typeof(T);
        info.methodType = methodType;
        ParseSpecialArguments(argumentTypes, argumentVariations);
    }

    /// <summary>An annotation that specifies a method, property or constructor to patch</summary>
    /// <param name="argumentTypes">An array of argument types to target overloads</param>
    ///
    public HarmonyPatchAttribute(Type[] argumentTypes)
    {
        info.declaringType = typeof(T);
        info.argumentTypes = argumentTypes;
    }

    /// <summary>An annotation that specifies a method, property or constructor to patch</summary>
    /// <param name="argumentTypes">An array of argument types to target overloads</param>
    /// <param name="argumentVariations">An array of <see cref="ArgumentType"/></param>
    ///
    public HarmonyPatchAttribute(Type[] argumentTypes, ArgumentType[] argumentVariations)
    {
        info.declaringType = typeof(T);
        ParseSpecialArguments(argumentTypes, argumentVariations);
    }

    /// <summary>An annotation that specifies a method, property or constructor to patch</summary>
    /// <param name="typeName">The full name of the declaring class/type</param>
    /// <param name="methodName">The name of the method, property or constructor to patch</param>
    /// <param name="methodType">The <see cref="MethodType"/></param>
    ///
    public HarmonyPatchAttribute(string typeName, string methodName, MethodType methodType = MethodType.Normal)
    {
        info.declaringType = typeof(T);
        info.declaringType = AccessTools.TypeByName(typeName);
        info.methodName = methodName;
        info.methodType = methodType;
    }

    private void ParseSpecialArguments(Type[] argumentTypes, ArgumentType[] argumentVariations)
    {
        if (argumentVariations is null || argumentVariations.Length == 0)
        {
            info.argumentTypes = argumentTypes;
            return;
        }

        if (argumentTypes.Length < argumentVariations.Length)
            throw new ArgumentException("argumentVariations contains more elements than argumentTypes", nameof(argumentVariations));

        var types = new List<Type>();
        for (var i = 0; i < argumentTypes.Length; i++)
        {
            var type = argumentTypes[i];
            switch (argumentVariations[i])
            {
                case ArgumentType.Normal:
                    break;
                case ArgumentType.Ref:
                case ArgumentType.Out:
                    type = type.MakeByRefType();
                    break;
                case ArgumentType.Pointer:
                    type = type.MakePointerType();
                    break;
                default:
                    throw new UnreachableException(nameof(argumentVariations));
            }
            types.Add(type);
        }
        info.argumentTypes = types.ToArray();
    }
}