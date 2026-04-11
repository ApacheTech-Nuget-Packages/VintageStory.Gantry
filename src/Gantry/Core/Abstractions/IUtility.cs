namespace Gantry.Core.Abstractions;

/// <summary>
///    Represents a utility that can be run.
/// </summary>
public interface IUtility
{
    /// <summary>
    ///     Runs the utility.
    /// </summary>
    void Run();
}

/// <summary>
///     Defines methods for objects that are managed by the host.
/// </summary>
public interface IHostedService
{
    /// <summary>
    ///     Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous Start operation.</returns>
    Task StartAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous Stop operation.</returns>
    Task StopAsync(CancellationToken cancellationToken);
}

/// <summary>
///     Base class for implementing a long running <see cref="IHostedService"/>.
/// </summary>
public abstract class BackgroundService : IHostedService, IDisposable
{
    private Task? _executeTask;
    private CancellationTokenSource? _stoppingCts;

    /// <summary>
    ///     Gets the Task that executes the background operation.
    /// </summary>
    /// <remarks>
    /// Will return <see langword="null"/> if the background operation hasn't started.
    /// </remarks>
    public virtual Task? ExecuteTask => _executeTask;

    /// <summary>
    ///     This method is called when the <see cref="IHostedService"/> starts. The implementation should return a task that represents
    ///     the lifetime of the long running operation(s) being performed.
    /// </summary>
    /// <param name="stoppingToken">Triggered when <see cref="IHostedService.StopAsync(CancellationToken)"/> is called.</param>
    /// <returns>A <see cref="Task"/> that represents the long running operations.</returns>
    /// <remarks>See <see href="https://learn.microsoft.com/dotnet/core/extensions/workers">Worker Services in .NET</see> for implementation guidelines.</remarks>
    protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

    /// <summary>
    ///     Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous Start operation.</returns>
    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
        // Create linked token to allow cancelling executing task from provided token
        _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        // Execute all of ExecuteAsync asynchronously, and store the task we're executing so that we can wait for it later.
        _executeTask = Task.Run(() => ExecuteAsync(_stoppingCts.Token), _stoppingCts.Token);

        // Always return a completed task.  Any result from ExecuteAsync will be handled by the Host.
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous Stop operation.</returns>
    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        // Stop called without start
        if (_executeTask == null)
        {
            return;
        }

        try
        {
            // Signal cancellation to the executing method
            _stoppingCts!.Cancel();
        }
        finally
        {
            await _executeTask.WaitAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
        }

    }

    /// <inheritdoc />
    public virtual void Dispose()
    {
        _stoppingCts?.Cancel();
    }
}

/// <summary>
///     Extension methods for adding hosted services to an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionHostedServiceExtensions
{
    /// <summary>
    ///     Add an <see cref="IHostedService"/> registration for the given type.
    /// </summary>
    /// <typeparam name="THostedService">An <see cref="IHostedService"/> to register.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to register with.</param>
    /// <returns>The original <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddHostedService<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THostedService>(this IServiceCollection services)
        where THostedService : class, IHostedService
    {
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, THostedService>());
        return services;
    }

    /// <summary>
    ///     Add an <see cref="IHostedService"/> registration for the given type.
    /// </summary>
    /// <typeparam name="THostedService">An <see cref="IHostedService"/> to register.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to register with.</param>
    /// <param name="implementationFactory">A factory to create new instances of the service implementation.</param>
    /// <returns>The original <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddHostedService<THostedService>(this IServiceCollection services, System.Func<IServiceProvider, THostedService> implementationFactory)
        where THostedService : class, IHostedService
    {
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService>(implementationFactory));
        return services;
    }
}