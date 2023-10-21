using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ApacheTech.Common.Extensions.Harmony;
using HarmonyLib;
using JetBrains.Annotations;

// ReSharper disable StaticMemberInGenericType
// ReSharper disable SuspiciousTypeConversion.Global
// ReSharper disable InconsistentNaming

namespace Gantry.Services.FileSystem.Configuration.ObservableFeatures
{
    /// <summary>
    ///     Notifies observers that a property value has changed within a wrapped POCO class.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of object to watch.</typeparam>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class ObservableObject<T> : IObservableObject where T: class, new()
    {
        private readonly Harmony _harmony;
        private static T _observedInstance;
        private static Action<object, string> _onObjectPropertyChanged;
        private static bool _active;
        
        /// <summary>
        ///     The instance of the object being observed.
        /// </summary>
        public object Object
        {
            get => _observedInstance;
            private set => _observedInstance = (T)value;
        }

        /// <summary>
        ///     Initialises a new instance of the <see cref="ObservableObject{T}"/> class.
        /// </summary>
        public ObservableObject(Harmony harmony) : this(new T(), harmony) { }

        /// <summary>
        /// Initialises a new instance of the <see cref="ObservableObject{T}" /> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="harmony">The harmony instance to use to patch the files.</param>
        public ObservableObject(T instance, Harmony harmony)
        {
            Object = instance;
            _harmony = harmony;
            Patch();
        }

        private void Patch()
        {
            var postfix = new HarmonyMethod(this.GetMethod(nameof(Patch_Property_SetMethod_Postfix)));
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                var original = propertyInfo.SetMethod;
                if (Harmony.GetPatchInfo(original)?.Postfixes.Any() ?? false) continue;
                _harmony.Patch(original, postfix: postfix);
            }
        }

        /// <summary>
        ///     Removes the postfix patches from the observed item.
        /// </summary>
        public void UnPatch()
        {
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                var original = propertyInfo.SetMethod;
                _harmony.Unpatch(original, HarmonyPatchType.Postfix);
            }
        }

        /// <summary>
        ///     Binds the specified feature to a POCO class object; dynamically adding an
        ///     implementation of <see cref="INotifyPropertyChanged"/>, raising an event
        ///     every time a property within the POCO class, is set.
        /// </summary>
        /// <param name="instance">The instance of the POCO class that manages the feature settings.</param>
        /// <param name="harmony">The harmony instance to use to patch the files.</param>
        /// <returns>An instance of <see cref="ObservableObject{T}"/>, which exposes the <c>PropertyChanged</c> event.</returns>
        public static ObservableObject<T> Bind(T instance, Harmony harmony)
        {
            return new ObservableObject<T>(instance, harmony);
        }

        /// <summary>
        ///     A value indicating whether this <see cref="ObservableObject{T}"/> is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if active; otherwise, <c>false</c>.
        /// </value>
        public bool Active
        {
            get => _active; 
            set => _active = value;
        }

        /// <summary>
        ///     Occurs when a property value is changed, within the observed POCO class.
        /// </summary>
        public Action<object, string> OnObjectPropertyChanged
        {
            get => _onObjectPropertyChanged;
            set => _onObjectPropertyChanged = value;
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void Patch_Property_SetMethod_Postfix(MemberInfo __originalMethod)
        {
            if (!_active) return;
            var propertyName = __originalMethod.Name.Remove(0, 4);
            _onObjectPropertyChanged(_observedInstance, propertyName);
        }
    }
}