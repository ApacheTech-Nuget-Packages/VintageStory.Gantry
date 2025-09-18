namespace Gantry.Services.HarmonyPatches.Annotations;

/// <summary>
///     Indicates that the decorated class or method should be applied as a patch, but only for the server.
/// </summary>
/// <seealso cref="HarmonySidedPatchAttribute" />
/// <seealso cref="HarmonyPatch" />
public class HarmonyServerPatchAttribute : HarmonySidedPatchAttribute
{
    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    public HarmonyServerPatchAttribute() : base(EnumAppSide.Server)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    public HarmonyServerPatchAttribute(Type declaringType) : base(EnumAppSide.Server, declaringType)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    public HarmonyServerPatchAttribute(Type declaringType, Type[] argumentTypes) : base(EnumAppSide.Server, declaringType, argumentTypes)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodName">The name of the method to patch.</param>
    public HarmonyServerPatchAttribute(Type declaringType, string methodName) : base(EnumAppSide.Server, declaringType, methodName)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodName">The name of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    public HarmonyServerPatchAttribute(Type declaringType, string methodName, params Type[] argumentTypes) : base(EnumAppSide.Server, declaringType, methodName, argumentTypes)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodName">The name of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    /// <param name="argumentVariations">The argument variations, to further identify the method to patch.</param>
    public HarmonyServerPatchAttribute(Type declaringType, string methodName, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(EnumAppSide.Server, declaringType, methodName, argumentTypes, argumentVariations)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodType">Type of the method to patch.</param>
    public HarmonyServerPatchAttribute(Type declaringType, MethodType methodType) : base(EnumAppSide.Server, declaringType, methodType)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodType">Type of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    public HarmonyServerPatchAttribute(Type declaringType, MethodType methodType, params Type[] argumentTypes) : base(EnumAppSide.Server, declaringType, methodType, argumentTypes)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodType">Type of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    /// <param name="argumentVariations">The argument variations, to further identify the method to patch.</param>
    public HarmonyServerPatchAttribute(Type declaringType, MethodType methodType, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(EnumAppSide.Server, declaringType, methodType, argumentTypes, argumentVariations)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodName">The name of the method to patch.</param>
    /// <param name="methodType">Type of the method to patch.</param>
    public HarmonyServerPatchAttribute(Type declaringType, string methodName, MethodType methodType) : base(EnumAppSide.Server, declaringType, methodName, methodType)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="methodName">The name of the method to patch.</param>
    public HarmonyServerPatchAttribute(string methodName) : base(EnumAppSide.Server, methodName)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="methodName">The name of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    public HarmonyServerPatchAttribute(string methodName, params Type[] argumentTypes) : base(EnumAppSide.Server, methodName, argumentTypes)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="methodName">The name of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    /// <param name="argumentVariations">The argument variations, to further identify the method to patch.</param>
    public HarmonyServerPatchAttribute(string methodName, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(EnumAppSide.Server, methodName, argumentTypes, argumentVariations)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="methodName">The name of the method to patch.</param>
    /// <param name="methodType">Type of the method to patch.</param>
    public HarmonyServerPatchAttribute(string methodName, MethodType methodType) : base(EnumAppSide.Server, methodName, methodType)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="methodType">Type of the method to patch.</param>
    public HarmonyServerPatchAttribute(MethodType methodType) : base(EnumAppSide.Server, methodType)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="methodType">Type of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    public HarmonyServerPatchAttribute(MethodType methodType, params Type[] argumentTypes) : base(EnumAppSide.Server, methodType, argumentTypes)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="methodType">Type of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    /// <param name="argumentVariations">The argument variations, to further identify the method to patch.</param>
    public HarmonyServerPatchAttribute(MethodType methodType, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(EnumAppSide.Server, methodType, argumentTypes, argumentVariations)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    public HarmonyServerPatchAttribute(Type[] argumentTypes) : base(EnumAppSide.Server, argumentTypes)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    /// <param name="argumentVariations">The argument variations, to further identify the method to patch.</param>
    public HarmonyServerPatchAttribute(Type[] argumentTypes, ArgumentType[] argumentVariations) : base(EnumAppSide.Server, argumentTypes, argumentVariations)
    {
    }
}