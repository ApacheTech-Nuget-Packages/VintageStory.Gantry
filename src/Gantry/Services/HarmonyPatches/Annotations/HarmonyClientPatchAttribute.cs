namespace Gantry.Services.HarmonyPatches.Annotations;

/// <summary>
///     Indicates that the decorated class or method should be applied as a patch, but only for the client.
/// </summary>
/// <seealso cref="HarmonySidedPatchAttribute" />
/// <seealso cref="HarmonyPatch" />
public class HarmonyClientPatchAttribute : HarmonySidedPatchAttribute
{
    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonyClientPatchAttribute"/> class.
    /// </summary>
    public HarmonyClientPatchAttribute() : base(EnumAppSide.Client)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonyClientPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    public HarmonyClientPatchAttribute(Type declaringType) : base(EnumAppSide.Client, declaringType)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonyClientPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    public HarmonyClientPatchAttribute(Type declaringType, Type[] argumentTypes) : base(EnumAppSide.Client, declaringType, argumentTypes)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonyClientPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodName">The name of the method to patch.</param>
    public HarmonyClientPatchAttribute(Type declaringType, string methodName) : base(EnumAppSide.Client, declaringType, methodName)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonyClientPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodName">The name of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    public HarmonyClientPatchAttribute(Type declaringType, string methodName, params Type[] argumentTypes) : base(EnumAppSide.Client, declaringType, methodName, argumentTypes)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonyClientPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodName">The name of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    /// <param name="argumentVariations">The argument variations, to further identify the method to patch.</param>
    public HarmonyClientPatchAttribute(Type declaringType, string methodName, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(EnumAppSide.Client, declaringType, methodName, argumentTypes, argumentVariations)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonyClientPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodType">Type of the method to patch.</param>
    public HarmonyClientPatchAttribute(Type declaringType, MethodType methodType) : base(EnumAppSide.Client, declaringType, methodType)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonyClientPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodType">Type of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    public HarmonyClientPatchAttribute(Type declaringType, MethodType methodType, params Type[] argumentTypes) : base(EnumAppSide.Client, declaringType, methodType, argumentTypes)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonyClientPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodType">Type of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    /// <param name="argumentVariations">The argument variations, to further identify the method to patch.</param>
    public HarmonyClientPatchAttribute(Type declaringType, MethodType methodType, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(EnumAppSide.Client, declaringType, methodType, argumentTypes, argumentVariations)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonyClientPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodName">The name of the method to patch.</param>
    /// <param name="methodType">Type of the method to patch.</param>
    public HarmonyClientPatchAttribute(Type declaringType, string methodName, MethodType methodType) : base(EnumAppSide.Client, declaringType, methodName, methodType)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonyClientPatchAttribute"/> class.
    /// </summary>
    /// <param name="methodName">The name of the method to patch.</param>
    public HarmonyClientPatchAttribute(string methodName) : base(EnumAppSide.Client, methodName)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonyClientPatchAttribute"/> class.
    /// </summary>
    /// <param name="methodName">The name of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    public HarmonyClientPatchAttribute(string methodName, params Type[] argumentTypes) : base(EnumAppSide.Client, methodName, argumentTypes)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonyClientPatchAttribute"/> class.
    /// </summary>
    /// <param name="methodName">The name of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    /// <param name="argumentVariations">The argument variations, to further identify the method to patch.</param>
    public HarmonyClientPatchAttribute(string methodName, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(EnumAppSide.Client, methodName, argumentTypes, argumentVariations)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonyClientPatchAttribute"/> class.
    /// </summary>
    /// <param name="methodName">The name of the method to patch.</param>
    /// <param name="methodType">Type of the method to patch.</param>
    public HarmonyClientPatchAttribute(string methodName, MethodType methodType) : base(EnumAppSide.Client, methodName, methodType)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonyClientPatchAttribute"/> class.
    /// </summary>
    /// <param name="methodType">Type of the method to patch.</param>
    public HarmonyClientPatchAttribute(MethodType methodType) : base(EnumAppSide.Client, methodType)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonyClientPatchAttribute"/> class.
    /// </summary>
    /// <param name="methodType">Type of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    public HarmonyClientPatchAttribute(MethodType methodType, params Type[] argumentTypes) : base(EnumAppSide.Client, methodType, argumentTypes)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonyClientPatchAttribute"/> class.
    /// </summary>
    /// <param name="methodType">Type of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    /// <param name="argumentVariations">The argument variations, to further identify the method to patch.</param>
    public HarmonyClientPatchAttribute(MethodType methodType, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(EnumAppSide.Client, methodType, argumentTypes, argumentVariations)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonyClientPatchAttribute"/> class.
    /// </summary>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    public HarmonyClientPatchAttribute(Type[] argumentTypes) : base(EnumAppSide.Client, argumentTypes)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonyClientPatchAttribute"/> class.
    /// </summary>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    /// <param name="argumentVariations">The argument variations, to further identify the method to patch.</param>
    public HarmonyClientPatchAttribute(Type[] argumentTypes, ArgumentType[] argumentVariations) : base(EnumAppSide.Client, argumentTypes, argumentVariations)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonyClientPatchAttribute"/> class.
    /// </summary>
    /// <param name="typeName">The full name of the declaring class/type</param>
    /// <param name="methodName">The name of the method to patch</param>
    /// <param name="methodType">The type of the method to patch</param>
    public HarmonyClientPatchAttribute(string typeName, string methodName, MethodType methodType = MethodType.Normal) : base(EnumAppSide.Client, typeName, methodName, methodType)
    {
    }

    /// <summary>
    ///     Initialises a new instance of the <see cref="HarmonyClientPatchAttribute"/> class.
    /// </summary>
    /// <param name="declaringType">The full name of the declaring type.</param>
    /// <param name="methodName">The name of the method to patch.</param>
    /// <param name="arguments">The argument types of the target method.</param>
    public HarmonyClientPatchAttribute(string declaringType, string methodName, params Type[] arguments): base(EnumAppSide.Client, declaringType, methodName, arguments)
    {
    }
}