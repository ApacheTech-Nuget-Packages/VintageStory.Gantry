using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using ApacheTech.Common.Extensions.Harmony;
using Gantry.Services.FileSystem.Configuration.ObservableFeatures;
using HarmonyLib;
using JetBrains.Annotations;

// ReSharper disable SuspiciousTypeConversion.Global
// ReSharper disable InconsistentNaming

namespace Gantry.Services.FileSystem.Configuration.DynamicNotifyPropertyChanged
{
    /// <summary>
    ///     Notifies observers that a property value has changed within a wrapped POCO class.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of object to watch.</typeparam>
    /// <seealso cref="IDisposable" />
    [UsedImplicitly(ImplicitUseTargetFlags.All)]
    public class DynamicNotifyPropertyChanged<T> : IDisposable where T : class
    {
        private static DynamicNotifyPropertyChanged<T> _instance;
        private static T _observedInstance;

        private readonly Harmony _harmony;

        /// <summary>
        /// 	Initialises a new instance of the <see cref="DynamicNotifyPropertyChanged{T}" /> class.
        /// </summary>
        /// <param name="instance">The instance to watch.</param>
        private DynamicNotifyPropertyChanged(T instance)
        {
            _observedInstance = instance;
            var objectType = instance.GetType();
            _harmony = new Harmony(objectType.FullName);
            var postfix = this.GetMethod(nameof(Patch_PropertySetMethod_Postfix));
            foreach (var propertyInfo in objectType.GetProperties())
            {
                _harmony.Patch(propertyInfo.SetMethod, postfix: new HarmonyMethod(postfix));

                // ROADMAP: Experimental - List Item Addition/Removal Detection.
                if (propertyInfo.PropertyType is not IList list) continue;
                var collectionPostfix = new HarmonyMethod(this.GetMethod(nameof(Patch_ListMethod_Postfix)));
                _harmony.Patch(list.GetMethod(nameof(list.Add)), postfix: collectionPostfix);
                _harmony.Patch(list.GetMethod(nameof(list.Remove)), postfix: collectionPostfix);
                _harmony.Patch(list.GetMethod(nameof(list.Clear)), postfix: collectionPostfix);
                _harmony.Patch(list.GetMethod(nameof(list.Insert)), postfix: collectionPostfix);
            }
        }

        /// <summary>
        ///     Binds the specified feature to a POCO class object; dynamically adding an implementation of <see cref="INotifyPropertyChanged"/>, 
        ///     raising an event every time a property within the POCO class, is set.
        /// </summary>
        /// <param name="instance">The instance of the POCO class that manages the feature settings.</param>
        /// <returns>An instance of <see cref="ObservableFeature{T}"/>, which exposes the <c>PropertyChanged</c> event.</returns>
        public static DynamicNotifyPropertyChanged<T> Bind(T instance)
        {
            return _instance ??= new DynamicNotifyPropertyChanged<T>(instance);
        }

        /// <summary>
        ///     Occurs when a property value is changed, within the observed POCO class.
        /// </summary>
        public event DynamicPropertyChangedEventHandler<T> PropertyChanged;

        /// <summary>
        ///     Un-patches the dynamic property watch on the POCO class. 
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _harmony.UnpatchAll();
            _instance = null;
            _observedInstance = null;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void Patch_PropertySetMethod_Postfix(MemberInfo __originalMethod)
        {
            var propertyName = __originalMethod.Name.Remove(0, 4);
            var args = new DynamicPropertyChangedEventArgs<T>(_observedInstance, propertyName);
            _instance.PropertyChanged?.Invoke(args);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void Patch_ListMethod_Postfix()
        {
            var args = new DynamicPropertyChangedEventArgs<T>(_observedInstance, string.Empty);
            _instance.PropertyChanged?.Invoke(args);
        }
    }
}