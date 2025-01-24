using System.Reflection;
using System.Reflection.Emit;

namespace Gantry.Core.Reflection;

/// <summary>
///     Dynamically creates a type that implements the specified interface and wraps the given instance of the original class.
/// </summary>
public static class DynamicWrapper
{
    /// <summary>
    ///     Dynamically creates a type that implements the specified interface and wraps the given instance of the original class.
    /// </summary>
    /// <typeparam name="TInterface">The interface to implement.</typeparam>
    /// <typeparam name="TOriginal">The original class to wrap.</typeparam>
    /// <param name="originalInstance">An instance of the original class to wrap.</param>
    /// <returns>An object implementing the specified interface.</returns>
    public static TInterface CreateWrapper<TInterface, TOriginal>(TOriginal originalInstance)
        where TInterface : class
        where TOriginal : class
    {
        if (originalInstance is null)
        {
            throw new ArgumentNullException(nameof(originalInstance));
        }

        var interfaceType = typeof(TInterface);
        var originalType = typeof(TOriginal);

        if (!interfaceType.IsInterface)
        {
            throw new ArgumentException($"{nameof(TInterface)} must be an interface.");
        }

        var assemblyName = new AssemblyName("DynamicWrapperAssembly");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicWrapperModule");

        var typeBuilder = moduleBuilder.DefineType(
            "DynamicWrapper_" + originalType.Name + "_To_" + interfaceType.Name,
            TypeAttributes.Public | TypeAttributes.Class);

        // Implement the interface
        typeBuilder.AddInterfaceImplementation(interfaceType);

        // Add a field to hold the original instance
        var originalField = typeBuilder.DefineField("_original", originalType, FieldAttributes.Private);

        // Define a constructor to initialise the field
        var constructorBuilder = typeBuilder.DefineConstructor(
            MethodAttributes.Public, CallingConventions.Standard, [originalType]);
        var ilCtor = constructorBuilder.GetILGenerator();
        ilCtor.Emit(OpCodes.Ldarg_0); // Load "this"
        ilCtor.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes)!); // Call base constructor
        ilCtor.Emit(OpCodes.Ldarg_0); // Load "this"
        ilCtor.Emit(OpCodes.Ldarg_1); // Load constructor argument (originalInstance)
        ilCtor.Emit(OpCodes.Stfld, originalField); // Store it in the field
        ilCtor.Emit(OpCodes.Ret);

        // Implement methods
        foreach (var interfaceMethod in interfaceType.GetMethods())
        {
            var methodBuilder = typeBuilder.DefineMethod(
                interfaceMethod.Name,
                MethodAttributes.Public | MethodAttributes.Virtual,
                interfaceMethod.ReturnType,
                Array.ConvertAll(interfaceMethod.GetParameters(), p => p.ParameterType));

            var ilMethod = methodBuilder.GetILGenerator();

            // Load the original instance
            ilMethod.Emit(OpCodes.Ldarg_0);
            ilMethod.Emit(OpCodes.Ldfld, originalField);

            // Load all arguments (skipping "this")
            for (var i = 1; i <= interfaceMethod.GetParameters().Length; i++)
            {
                ilMethod.Emit(OpCodes.Ldarg, i);
            }

            // Call the corresponding method on the original instance
            var originalMethod = originalType.GetMethod(interfaceMethod.Name, Array.ConvertAll(interfaceMethod.GetParameters(), p => p.ParameterType));
            if (originalMethod is null)
            {
                throw new InvalidOperationException($"The method {interfaceMethod.Name} is not implemented on {originalType.Name}.");
            }

            ilMethod.Emit(OpCodes.Callvirt, originalMethod);
            ilMethod.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(methodBuilder, interfaceMethod);
        }

        // Implement properties
        foreach (var interfaceProperty in interfaceType.GetProperties())
        {
            var propertyBuilder = typeBuilder.DefineProperty(
                interfaceProperty.Name,
                PropertyAttributes.None,
                interfaceProperty.PropertyType,
                Type.EmptyTypes);

            var originalProperty = originalType.GetProperty(interfaceProperty.Name);

            // Implement getter
            if (interfaceProperty.GetGetMethod() is not null)
            {
                var getterMethodBuilder = typeBuilder.DefineMethod(
                    "get_" + interfaceProperty.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                    interfaceProperty.PropertyType,
                    Type.EmptyTypes);

                var ilGetter = getterMethodBuilder.GetILGenerator();
                ilGetter.Emit(OpCodes.Ldarg_0);
                ilGetter.Emit(OpCodes.Ldfld, originalField);
                ilGetter.Emit(OpCodes.Callvirt, originalProperty!.GetGetMethod()!);
                ilGetter.Emit(OpCodes.Ret);

                typeBuilder.DefineMethodOverride(getterMethodBuilder, interfaceProperty.GetGetMethod()!);
                propertyBuilder.SetGetMethod(getterMethodBuilder);
            }

            // Implement setter
            if (interfaceProperty.GetSetMethod() is null) continue;
            var setterMethodBuilder = typeBuilder.DefineMethod(
                "set_" + interfaceProperty.Name,
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                null,
                [interfaceProperty.PropertyType]);

            var ilSetter = setterMethodBuilder.GetILGenerator();
            ilSetter.Emit(OpCodes.Ldarg_0);
            ilSetter.Emit(OpCodes.Ldfld, originalField);
            ilSetter.Emit(OpCodes.Ldarg_1);
            ilSetter.Emit(OpCodes.Callvirt, originalProperty!.GetSetMethod()!);
            ilSetter.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(setterMethodBuilder, interfaceProperty.GetSetMethod()!);
            propertyBuilder.SetSetMethod(setterMethodBuilder);
        }

        // Create the dynamic type
        var generatedType = typeBuilder.CreateType();

        // Create an instance of the dynamic type
        var instance = Activator.CreateInstance(generatedType, originalInstance);

        // Cast to the interface and return
        return instance as TInterface;
    }
}