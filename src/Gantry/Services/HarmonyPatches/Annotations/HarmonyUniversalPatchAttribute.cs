namespace Gantry.Services.HarmonyPatches.Annotations;

/// <summary>
///     Indicates that the decorated class or method should be applied as a patch, but only for the client.
/// </summary>
/// <seealso cref="HarmonySidedPatchAttribute" />
/// <seealso cref="HarmonyPatch" />
public class HarmonyUniversalPatchAttribute : HarmonySidedPatchAttribute
{
    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    public HarmonyUniversalPatchAttribute() : base(EnumAppSide.Universal)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    public HarmonyUniversalPatchAttribute(Type declaringType) : base(EnumAppSide.Universal, declaringType)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    public HarmonyUniversalPatchAttribute(Type declaringType, Type[] argumentTypes) : base(EnumAppSide.Universal, declaringType, argumentTypes)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodName">The name of the method to patch.</param>
    public HarmonyUniversalPatchAttribute(Type declaringType, string methodName) : base(EnumAppSide.Universal, declaringType, methodName)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodName">The name of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    public HarmonyUniversalPatchAttribute(Type declaringType, string methodName, params Type[] argumentTypes) : base(EnumAppSide.Universal, declaringType, methodName, argumentTypes)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodName">The name of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    /// <param name="argumentVariations">The argument variations, to further identify the method to patch.</param>
    public HarmonyUniversalPatchAttribute(Type declaringType, string methodName, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(EnumAppSide.Universal, declaringType, methodName, argumentTypes, argumentVariations)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodType">Type of the method to patch.</param>
    public HarmonyUniversalPatchAttribute(Type declaringType, MethodType methodType) : base(EnumAppSide.Universal, declaringType, methodType)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodType">Type of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    public HarmonyUniversalPatchAttribute(Type declaringType, MethodType methodType, params Type[] argumentTypes) : base(EnumAppSide.Universal, declaringType, methodType, argumentTypes)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodType">Type of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    /// <param name="argumentVariations">The argument variations, to further identify the method to patch.</param>
    public HarmonyUniversalPatchAttribute(Type declaringType, MethodType methodType, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(EnumAppSide.Universal, declaringType, methodType, argumentTypes, argumentVariations)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodName">The name of the method to patch.</param>
    /// <param name="methodType">Type of the method to patch.</param>
    public HarmonyUniversalPatchAttribute(Type declaringType, string methodName, MethodType methodType) : base(EnumAppSide.Universal, declaringType, methodName, methodType)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="methodName">The name of the method to patch.</param>
    public HarmonyUniversalPatchAttribute(string methodName) : base(EnumAppSide.Universal, methodName)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="methodName">The name of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    public HarmonyUniversalPatchAttribute(string methodName, params Type[] argumentTypes) : base(EnumAppSide.Universal, methodName, argumentTypes)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="methodName">The name of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    /// <param name="argumentVariations">The argument variations, to further identify the method to patch.</param>
    public HarmonyUniversalPatchAttribute(string methodName, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(EnumAppSide.Universal, methodName, argumentTypes, argumentVariations)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="methodName">The name of the method to patch.</param>
    /// <param name="methodType">Type of the method to patch.</param>
    public HarmonyUniversalPatchAttribute(string methodName, MethodType methodType) : base(EnumAppSide.Universal, methodName, methodType)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="methodType">Type of the method to patch.</param>
    public HarmonyUniversalPatchAttribute(MethodType methodType) : base(EnumAppSide.Universal, methodType)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="methodType">Type of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    public HarmonyUniversalPatchAttribute(MethodType methodType, params Type[] argumentTypes) : base(EnumAppSide.Universal, methodType, argumentTypes)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="methodType">Type of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    /// <param name="argumentVariations">The argument variations, to further identify the method to patch.</param>
    public HarmonyUniversalPatchAttribute(MethodType methodType, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(EnumAppSide.Universal, methodType, argumentTypes, argumentVariations)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    public HarmonyUniversalPatchAttribute(Type[] argumentTypes) : base(EnumAppSide.Universal, argumentTypes)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    /// <param name="argumentVariations">The argument variations, to further identify the method to patch.</param>
    public HarmonyUniversalPatchAttribute(Type[] argumentTypes, ArgumentType[] argumentVariations) : base(EnumAppSide.Universal, argumentTypes, argumentVariations)
    {
    }
}