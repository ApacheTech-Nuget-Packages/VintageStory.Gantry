using ApacheTech.Common.BrighterSlim;

namespace Gantry.Services.Brighter.Hosting;

/// <summary>
///     Creates a message mapper from the underlying .NET IoC container
/// </summary>
internal class ServiceProviderMapperFactory : IAmAMessageMapperFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    ///     Constructs a mapper factory that uses the .NET Service Provider for implementation details
    /// </summary>
    /// <param name="serviceProvider"></param>
    public ServiceProviderMapperFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    ///     Create an instance of the message mapper type from the .NET IoC container
    ///     Note that there is no release as we assume that Mappers are never IDisposable
    /// </summary>
    /// <param name="messageMapperType">The type of mapper to instantiate</param>
    /// <returns></returns>
    public IAmAMessageMapper Create(Type messageMapperType)
    {
        var service = _serviceProvider.GetService(messageMapperType) 
            ?? throw new ArgumentNullException(nameof(messageMapperType));

        return (IAmAMessageMapper)service;
    }
}