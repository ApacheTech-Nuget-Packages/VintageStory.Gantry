using System.Reflection;
using System.Threading;
using Gantry.Core.Diagnostics;
using Gantry.Core.Extensions.Helpers;
using JetBrains.Annotations;
using Vintagestory.API.Common;

namespace Gantry.Core
{
    /// <summary>
    ///     Provides global access to mod information, and metadata.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class ModEx
    {
        /// <summary>
        ///     The main assembly for the mod that initialised the Gantry MDK.
        /// </summary>
        /// HACK: Potential Integration Bug: Is this global, static object shared among disparate mods?
        public static Assembly ModAssembly { get; private set; }

        // NB:  My thinking here, is that if this dll is loaded once by the game,
        //      then only one static value will be held here. Last in, first out.
        //      However, if the game creates aliases, and loads each unpack directory
        //      separately, then there will be no issues. The VMods Core seemed to
        //      work as intended, so this might not be an issues. However, it is
        //      something to perform integration testing on, once we get to a
        //      stable, and testable state.

        /// <summary>
        ///     The mod's metadata.
        /// </summary>
        public static ModInfoAttribute ModInfo { get; private set; }

        /// <summary>
        ///     Gets the side designated within the mod information.
        /// </summary>
        /// <exception cref="GantryException">Cannot determine app-side before `ApiEx` is intialised.</exception>
        public static EnumAppSide ModAppSide { get; private set; }

        internal static void Initialise(Assembly modAssembly)
        {
            ModAssembly = modAssembly;
            ModInfo = modAssembly.GetCustomAttribute<ModInfoAttribute>();

            if (ModInfo is null)
            {
                throw new GantryException($"Could not find `ModInfoAttribute` within assembly: {ModAssembly.FullName}");
            }

            ModAppSide = EnumEx.Parse<EnumAppSide>(ModInfo.Side);
        }

        /// <summary>
        ///     Determines whether a given mod is installed, and enabled, on the current app-side.
        /// </summary>
        /// <param name="modId">The mod identifier.</param>
        /// <returns><c>true</c> if the mod is enabled; otherwise, <c>false</c>.</returns>
        public static bool IsModEnabled(string modId)
        {
            return ApiEx.Current.ModLoader.IsModEnabled(modId);
        }

        /// <summary>
        ///     Determines whether the current code block is running on the main thread. See remarks.
        /// </summary>
        /// <remarks>
        ///     Within a Single-Player game, the server will never run on the main application thread.
        ///     Single-Player servers are run as a background thread, within the client application.
        /// </remarks>
        /// <returns><c>true</c> if the code is currently running on the main application thread; otherwise <c>false</c>.</returns>
        public static bool IsCurrentlyOnMainThread()
        {
            var thread = Thread.CurrentThread;
            return thread.GetApartmentState() == ApartmentState.STA
                   && !thread.IsBackground
                   && !thread.IsThreadPoolThread
                   && thread.IsAlive;
        }
    }
}