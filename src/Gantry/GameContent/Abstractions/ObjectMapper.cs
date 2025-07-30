namespace Gantry.GameContent.Abstractions;

/// <summary>
///     Abstract base class for mapping between DTO and domain objects using reflection.
/// </summary>
/// <typeparam name="TDtoObject">The type of the DTO object.</typeparam>
/// <typeparam name="TDomainObject">The type of the domain object.</typeparam>
public abstract class ObjectMapper<TDtoObject, TDomainObject>
    where TDtoObject : new()
    where TDomainObject : new()
{
    /// <summary>
    ///     Maps a domain object to a DTO object.
    /// </summary>
    /// <param name="domainObject">
    ///     The domain object to be mapped.
    /// </param>
    /// <returns>
    ///     A DTO object with properties mapped from the domain object.
    /// </returns>
    public virtual TDtoObject ToDto(TDomainObject domainObject)
    {
        return MapByReflection<TDtoObject, TDomainObject>(domainObject);
    }

    /// <summary>
    ///     Maps a DTO object to a domain object.
    /// </summary>
    /// <param name="dto">
    ///     The DTO object to be mapped.
    /// </param>
    /// <returns>
    ///     A domain object with properties mapped from the DTO object.
    /// </returns>
    public virtual TDomainObject FromDto(TDtoObject dto)
    {
        return MapByReflection<TDomainObject, TDtoObject>(dto);
    }

    /// <summary>
    ///     Asynchronously maps a domain object to a DTO object.
    /// </summary>
    /// <param name="domainObject">
    ///     The domain object to be mapped.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation, with a DTO object as the result.
    /// </returns>
    public virtual Task<TDtoObject> ToDtoAsync(TDomainObject domainObject)
    {
        return Task.FromResult(ToDto(domainObject));
    }

    /// <summary>
    ///     Asynchronously maps a DTO object to a domain object.
    /// </summary>
    /// <param name="dto">
    ///     The DTO object to be mapped.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation, with a domain object as the result.
    /// </returns>
    public virtual Task<TDomainObject> FromDtoAsync(TDtoObject dto)
    {
        return Task.FromResult(FromDto(dto));
    }

    /// <summary>
    ///     Uses reflection to map properties from one type to another.
    /// </summary>
    /// <typeparam name="TTo">
    ///     The target type for the mapping.
    /// </typeparam>
    /// <typeparam name="TFrom">
    ///     The source type for the mapping.
    /// </typeparam>
    /// <param name="from">
    ///     The source object to be mapped.
    /// </param>
    /// <returns>
    ///     A new instance of <typeparamref name="TTo"/> with properties copied from <typeparamref name="TFrom"/>.
    /// </returns>
    private static TTo MapByReflection<TTo, TFrom>(TFrom from) where TTo : new()
    {
        var to = new TTo();
        foreach (var toPropertyInfo in GetMappableProperties(typeof(TTo)))
        {
            var fromPropertyInfo =
                GetMappableProperties(typeof(TFrom))
                    .SingleOrDefault(p => p.Name == toPropertyInfo.Name);
            if (fromPropertyInfo is null) continue;
            var value = fromPropertyInfo.GetMethod?.Invoke(from, null);
            toPropertyInfo.SetMethod?.Invoke(to, new[] { value });
        }
        return to;
    }

    /// <summary>
    ///     Retrieves all mappable properties for a given type.
    /// </summary>
    /// <param name="type">
    ///     The type whose properties are to be retrieved.
    /// </param>
    /// <returns>
    ///     An enumerable of <see cref="PropertyInfo"/> representing the public instance properties of the type.
    /// </returns>
    private static IEnumerable<PropertyInfo> GetMappableProperties(IReflect type)
    {
        return type.GetProperties(
            BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.FlattenHierarchy |
            BindingFlags.SetProperty |
            BindingFlags.GetProperty);
    }
}