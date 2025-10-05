namespace Gantry.Core.Annotation;

/// <summary>
///     Attribute to mark a property as not composable within the GUI.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
public sealed class NotComposableAttribute : Attribute;