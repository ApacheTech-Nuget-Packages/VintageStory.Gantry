

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global

namespace Gantry.Services.HarmonyPatches.Annotations;

/// <summary>
///     Indicates that the decorated class or method should be applied as a patch, but only for a specified app-side.
/// </summary>
/// <seealso cref="HarmonyPatch" />
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Delegate | AttributeTargets.Method, AllowMultiple = true)]
public abstract class HarmonySidedPatchAttribute : HarmonyPatch
{
    /// <summary>
    ///     Gets the app-side to run the patch on.
    /// </summary>
    /// <value>The app-side to run the patch on.</value>
    internal EnumAppSide Side { get; }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    protected HarmonySidedPatchAttribute()
    {
        Side = EnumAppSide.Universal;
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="forSide">The app-side to run the patch on.</param>
    protected HarmonySidedPatchAttribute(EnumAppSide forSide)
    {
        Side = forSide;
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="forSide">The app-side to run the patch on.</param>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    protected HarmonySidedPatchAttribute(EnumAppSide forSide, Type declaringType) : base(declaringType)
    {
        Side = forSide;
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="forSide">The app-side to run the patch on.</param>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    protected HarmonySidedPatchAttribute(EnumAppSide forSide, Type declaringType, Type[] argumentTypes) : base(declaringType, argumentTypes)
    {
        Side = forSide;
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="forSide">The app-side to run the patch on.</param>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodName">The name of the method to patch.</param>
    protected HarmonySidedPatchAttribute(EnumAppSide forSide, Type declaringType, string methodName) : base(declaringType, methodName)
    {
        Side = forSide;
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="forSide">The app-side to run the patch on.</param>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodName">The name of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    protected HarmonySidedPatchAttribute(EnumAppSide forSide, Type declaringType, string methodName, params Type[] argumentTypes) : base(declaringType, methodName, argumentTypes)
    {
        Side = forSide;
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="forSide">The app-side to run the patch on.</param>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodName">The name of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    /// <param name="argumentVariations">The argument variations, to further identify the method to patch.</param>
    protected HarmonySidedPatchAttribute(EnumAppSide forSide, Type declaringType, string methodName, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(declaringType, methodName, argumentTypes, argumentVariations)
    {
        Side = forSide;
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="forSide">The app-side to run the patch on.</param>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodType">Type of the method to patch.</param>
    protected HarmonySidedPatchAttribute(EnumAppSide forSide, Type declaringType, MethodType methodType) : base(declaringType, methodType)
    {
        Side = forSide;
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="forSide">The app-side to run the patch on.</param>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodType">Type of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    protected HarmonySidedPatchAttribute(EnumAppSide forSide, Type declaringType, MethodType methodType, params Type[] argumentTypes) : base(declaringType, methodType, argumentTypes)
    {
        Side = forSide;
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="forSide">The app-side to run the patch on.</param>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodType">Type of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    /// <param name="argumentVariations">The argument variations, to further identify the method to patch.</param>
    protected HarmonySidedPatchAttribute(EnumAppSide forSide, Type declaringType, MethodType methodType, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(declaringType, methodType, argumentTypes, argumentVariations)
    {
        Side = forSide;
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="forSide">The app-side to run the patch on.</param>
    /// <param name="declaringType">Type of the declaring method to patch.</param>
    /// <param name="methodName">The name of the method to patch.</param>
    /// <param name="methodType">Type of the method to patch.</param>
    protected HarmonySidedPatchAttribute(EnumAppSide forSide, Type declaringType, string methodName, MethodType methodType) : base(declaringType, methodName, methodType)
    {
        Side = forSide;
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="forSide">The app-side to run the patch on.</param>
    /// <param name="methodName">The name of the method to patch.</param>
    protected HarmonySidedPatchAttribute(EnumAppSide forSide, string methodName) : base(methodName)
    {
        Side = forSide;
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="forSide">The app-side to run the patch on.</param>
    /// <param name="methodName">The name of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    protected HarmonySidedPatchAttribute(EnumAppSide forSide, string methodName, params Type[] argumentTypes) : base(methodName, argumentTypes)
    {
        Side = forSide;
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="forSide">The app-side to run the patch on.</param>
    /// <param name="methodName">The name of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    /// <param name="argumentVariations">The argument variations, to further identify the method to patch.</param>
    protected HarmonySidedPatchAttribute(EnumAppSide forSide, string methodName, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(methodName, argumentTypes, argumentVariations)
    {
        Side = forSide;
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="forSide">The app-side to run the patch on.</param>
    /// <param name="methodName">The name of the method to patch.</param>
    /// <param name="methodType">Type of the method to patch.</param>
    protected HarmonySidedPatchAttribute(EnumAppSide forSide, string methodName, MethodType methodType) : base(methodName, methodType)
    {
        Side = forSide;
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="forSide">The app-side to run the patch on.</param>
    /// <param name="methodType">Type of the method to patch.</param>
    protected HarmonySidedPatchAttribute(EnumAppSide forSide, MethodType methodType) : base(methodType)
    {
        Side = forSide;
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="forSide">The app-side to run the patch on.</param>
    /// <param name="methodType">Type of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    protected HarmonySidedPatchAttribute(EnumAppSide forSide, MethodType methodType, params Type[] argumentTypes) : base(methodType, argumentTypes)
    {
        Side = forSide;
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="forSide">The app-side to run the patch on.</param>
    /// <param name="methodType">Type of the method to patch.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    /// <param name="argumentVariations">The argument variations, to further identify the method to patch.</param>
    protected HarmonySidedPatchAttribute(EnumAppSide forSide, MethodType methodType, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(methodType, argumentTypes, argumentVariations)
    {
        Side = forSide;
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="forSide">The app-side to run the patch on.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    protected HarmonySidedPatchAttribute(EnumAppSide forSide, Type[] argumentTypes) : base(argumentTypes)
    {
        Side = forSide;
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="HarmonySidedPatchAttribute"/> class.
    /// </summary>
    /// <param name="forSide">The app-side to run the patch on.</param>
    /// <param name="argumentTypes">The argument types, to further identify the method to patch.</param>
    /// <param name="argumentVariations">The argument variations, to further identify the method to patch.</param>
    protected HarmonySidedPatchAttribute(EnumAppSide forSide, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(argumentTypes, argumentVariations)
    {
        Side = forSide;
    }
}