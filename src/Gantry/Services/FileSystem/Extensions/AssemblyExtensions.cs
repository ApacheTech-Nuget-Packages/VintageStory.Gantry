using System.Reflection;
using System.Runtime.CompilerServices;
using ApacheTech.Common.Extensions.System;
using Gantry.Core;
using Gantry.Services.FileSystem.Configuration.Consumers;
using HarmonyLib;
using JetBrains.Annotations;
using Vintagestory.API.Common;

namespace Gantry.Services.FileSystem.Extensions;

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
    public static IEnumerable<T> InstantiateAllTypesImplementing<T>(this Assembly assembly) =>
        assembly.GetAllTypesImplementing<T>().Select(Activator.CreateInstance).Select(p => p.To<T>());

    /// <summary>
    ///     Gets all types within the assembly that implement a specific interface.
    /// </summary>
    /// <typeparam name="T">The type of interface to scan the assembly for concrete implementations of.</typeparam>
    /// <param name="assembly">The assembly to scan.</param>
    /// <returns></returns>
    public static IEnumerable<Type> GetAllTypesImplementing<T>(this Assembly assembly)
        => assembly.GetTypes()
            .Where(p => !p.IsAbstract)
            .Where(p => !p.IsInterface)
            .Where(p => p.GetInterfaces().Contains(typeof(T)));

    /// <summary>
    ///     Invokes the type constructor for the specified type.
    /// </summary>
    /// <param name="type">The type of the class to invoke the class Constructor on.</param>
    public static void RunClassConstructor(this Type type) 
        => RuntimeHelpers.RunClassConstructor(type.TypeHandle);
}