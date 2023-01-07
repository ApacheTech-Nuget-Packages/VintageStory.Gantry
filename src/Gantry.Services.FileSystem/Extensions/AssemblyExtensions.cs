using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ApacheTech.Common.Extensions.System;
using Gantry.Core;
using Gantry.Services.FileSystem.Configuration.Consumers;
using HarmonyLib;
using JetBrains.Annotations;
using Vintagestory.API.Common;

namespace Gantry.Services.FileSystem.Extensions
{
    /// <summary>
    ///     Extension methods to aid assembly level reflection.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class AssemblyExtensions
    {
        /// <summary>
        ///     Instantiates all types within the assembly that implement a specific interface.
        /// </summary>
        /// <typeparam name="T">The type of interface to scan the assembly for concrete implementations of.</typeparam>
        /// <param name="assembly">The assembly to scan.</param>
        /// <returns></returns>
        public static IEnumerable<T> InstantiateAllTypesImplementing<T>(this Assembly assembly)
        {
            return assembly
                .GetAllTypesImplementing<T>()
                .Select(Activator.CreateInstance)
                .Select(p => p.To<T>());
        }

        /// <summary>
        ///     Gets all types within the assembly that implement a specific interface.
        /// </summary>
        /// <typeparam name="T">The type of interface to scan the assembly for concrete implementations of.</typeparam>
        /// <param name="assembly">The assembly to scan.</param>
        /// <returns></returns>
        public static IEnumerable<Type> GetAllTypesImplementing<T>(this Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(p => !p.IsAbstract)
                .Where(p => !p.IsInterface)
                .Where(p => p.GetInterfaces().Contains(typeof(T)));
        }

        /// <summary>
        ///     Initialises classes that implement the <see cref="ISettingsConsumer"/> interface, within the given assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public static void InitialiseSettingsConsumers(this Assembly assembly)
        {
            var consumers = assembly.GetAllTypesImplementing<ISettingsConsumer>();
            foreach (var consumer in consumers)
            {
                var sideInfo = consumer.GetCustomAttribute<SettingsConsumerAttribute>()
                               ?? throw new InvalidOperationException($"Missing `{nameof(SettingsConsumerAttribute)}` Attribute for class: {consumer.FullName}.");

                if (sideInfo.Side == EnumAppSide.Universal || sideInfo.Side == ApiEx.Side)
                    AccessTools.Method(consumer, "Initialise")?.Invoke(null, null);
            }
        }

        /// <summary>
        ///     Invokes the type constructor for the specified type.
        /// </summary>
        /// <param name="type">The type of the class to invoke the class Constructor on.</param>
        public static void RunClassConstructor(this Type type) 
            => RuntimeHelpers.RunClassConstructor(type.TypeHandle);
    }
}