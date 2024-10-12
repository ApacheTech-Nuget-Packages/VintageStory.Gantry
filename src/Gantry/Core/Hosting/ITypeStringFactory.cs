namespace Gantry.Core.Hosting;

/// <summary>
///        Creates instances of type <typeparamref name="T"/>, resolved via dependency injection.
/// </summary>
/// <typeparam name="T">The type of instance to create.</typeparam>
public interface ITypeStringFactory<out T>
{
    /// <summary>
    ///        Creates an instance of a type, resolved via dependency injection.
    /// </summary>
    /// <param name="typeName">The fully qualified name of the type to create.</param>
    /// <returns>An instance of type <typeparamref name="T"/>, resolved via dependency injection.</returns>
    T Create(string typeName);
}
