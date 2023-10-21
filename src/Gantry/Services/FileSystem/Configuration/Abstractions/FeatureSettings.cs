using System;
using JetBrains.Annotations;

namespace Gantry.Services.FileSystem.Configuration.Abstractions
{
    /// <summary>
    ///     Acts as a base class for all settings POCO Classes for a given feature.
    /// </summary>
    /// <seealso cref="IDisposable" />
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public abstract class FeatureSettings : IDisposable
    {
        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing) { }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}