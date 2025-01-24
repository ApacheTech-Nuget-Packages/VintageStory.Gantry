namespace Gantry.Core.Hosting;

/// <summary>
///        Creates instances of type <typeparamref name="T"/>, resolved via dependency injection.
/// </summary>
/// <typeparam name="T">The type of instance to create.</typeparam>
public class TypeStringFactory<T> : ITypeStringFactory<T>
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    ///        Instantiates a new instance of the <see cref="TypeStringFactory{T}"/> class.
    /// </summary>
    public TypeStringFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    ///        Creates an instance of a type, resolved via dependency injection.
    /// </summary>
    /// <param name="typeName">The fully qualified name of the type to create.</param>
    /// <returns>An instance of type <typeparamref name="T"/>, resolved via dependency injection.</returns>
    public T Create(string typeName)
    {
        try
        {
            var type = Type.GetType(typeName);
            var instance = ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, type);
            return (T)instance;
        }
        catch (Exception)
        {
            throw;
        }
    }
}