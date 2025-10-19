namespace Gantry.Core.Helpers;

/// <summary>
///     Provides utility methods for managing scoped state changes.
/// </summary>
public class SteadyState
{
	/// <summary>
	///     Creates a synchronous scoped state. The provided <paramref name="setState"/> action is called with <paramref name="initialState"/> on creation and with the negated value on disposal.
	/// </summary>
	/// <param name="setState">Action to set the state.</param>
	/// <param name="initialState">The initial state value. Defaults to true.</param>
	/// <returns>An <see cref="IDisposable"/> that reverts the state on disposal.</returns>
	public static IDisposable Create(Action<bool> setState, bool initialState = true)
		=> new SynchronousScopedState(setState, initialState);

	/// <summary>
	///     Creates an asynchronous scoped state. The provided <paramref name="setStateAsync"/> function is called with <paramref name="initialState"/> on creation and with the negated value on disposal.
	/// </summary>
	/// <param name="setStateAsync">Asynchronous function to set the state.</param>
	/// <param name="initialState">The initial state value. Defaults to true.</param>
	/// <returns>An <see cref="IAsyncDisposable"/> that reverts the state asynchronously on disposal.</returns>
	public static async Task<IAsyncDisposable> CreateAsync(System.Func<bool, Task> setStateAsync, bool initialState = true) 
		=> await new AynchronousScopedState(setStateAsync, initialState).StartAsync();

	/// <summary>
	///     Synchronous implementation of a scoped state.
	/// </summary>
	private class SynchronousScopedState : IDisposable
	{
		private readonly Action<bool> _setState;
		private readonly bool _initialState;

		/// <summary>
		///     Initialises a new instance and sets the state.
		/// </summary>
		/// <param name="setState">Action to set the state.</param>
		/// <param name="initialState">The initial state value.</param>
		public SynchronousScopedState(Action<bool> setState, bool initialState = true)
		{
			_setState = setState;
			_initialState = initialState;
			_setState(_initialState);
		}

		/// <summary>
		///     Reverts the state when disposed.
		/// </summary>
		public void Dispose() => _setState(!_initialState);
	}

	/// <summary>
	///     Asynchronous implementation of a scoped state.
	/// </summary>
	private class AynchronousScopedState : IAsyncDisposable
	{
		private readonly System.Func<bool, Task> _setStateAsync;
		private readonly bool _initialState;

		/// <summary>
		///     Initialises a new instance for asynchronous state management.
		/// </summary>
		/// <param name="setStateAsync">Asynchronous function to set the state.</param>
		/// <param name="initialState">The initial state value.</param>
		public AynchronousScopedState(System.Func<bool, Task> setStateAsync, bool initialState)
		{
			_setStateAsync = setStateAsync;
			_initialState = initialState;
		}

		/// <summary>
		///     Starts the asynchronous state change.
		/// </summary>
		public async Task<IAsyncDisposable> StartAsync()
		{
			await _setStateAsync(_initialState);
			return this;
		}

		/// <summary>
		///     Reverts the state asynchronously when disposed.
		/// </summary>
		public async ValueTask DisposeAsync() => await _setStateAsync(!_initialState);
	}
}