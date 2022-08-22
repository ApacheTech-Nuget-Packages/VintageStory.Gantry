using System;
using JetBrains.Annotations;

namespace Gantry.Core.Extensions.DotNet
{
    /// <summary>
    ///    Extension methods to aid with functional programming in C#.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class FunctionalExtensions
    {
        /// <summary>
        ///     Performs an operation repeatedly, until the criteria for stopping has been met.
        /// </summary>
        /// <typeparam name="T">The type of operation handler.</typeparam>
        /// <param name="this">The operation function to run.</param>
        /// <param name="createNext">A function that changes the state of the operation, between iterations.</param>
        /// <param name="finishCondition">A predicate that determines whether the criteria for stopping the iteration has been met.</param>
        /// <returns></returns>
        public static T IterateUntil<T>(this T @this, Func<T, T> createNext, Func<T, bool> finishCondition) =>
            finishCondition(@this) ? @this : createNext(@this).IterateUntil(createNext, finishCondition);
    }
}
