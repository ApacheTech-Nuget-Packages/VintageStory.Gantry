using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;

// ReSharper disable SuspiciousTypeConversion.Global

namespace Gantry.Core.Extensions.DotNet
{
    /// <summary>
    ///     Extension methods to aid the cleaning up of code, when exiting a game world.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class OrphanedStaticsExtensions
    {
        /// <summary>
        ///     Nullifies any orphaned static members within a given assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public static void NullifyOrphanedStaticMembers(this Assembly assembly)
        {
            assembly?.GetTypes().Do(NullifyStaticClassMembers);
        }

        private static void NullifyStaticClassMembers(Type type)
        {
            type.GetProperties(BindingFlags.Static | BindingFlags.SetProperty).Do(NullifyStaticProperty);
            type.GetFields(BindingFlags.Static | BindingFlags.SetField).Do(NullifyStaticField);
        }

        private static void NullifyStaticField(FieldInfo fieldInfo)
        {
            if (fieldInfo.Attributes == FieldAttributes.InitOnly) return;
            if (fieldInfo.FieldType is IDisposable)
            {
                var disposable = (IDisposable)fieldInfo.GetValue(null);
                disposable.Dispose();
            }
            fieldInfo.SetValue(null, null);
        }

        private static void NullifyStaticProperty(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanWrite) return;
            if (propertyInfo.PropertyType is IDisposable)
            {
                var disposable = (IDisposable)propertyInfo.GetMethod.Invoke(null, null);
                disposable.Dispose();
            }
            propertyInfo.SetMethod.Invoke(null, null);
        }
    }
}