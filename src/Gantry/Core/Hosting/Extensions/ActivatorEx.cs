using System.Linq.Expressions;
using System.Runtime.ExceptionServices;
using ApacheTech.Common.Extensions.Reflection;

namespace Gantry.Core.Hosting.Extensions;

/// <summary>
///     Helper code for the various activator services.
/// </summary>
internal static class ActivatorEx
{
    private static readonly MethodInfo _getServiceInfo =
        GetMethodInfo<System.Func<IServiceProvider, Type, Type, bool, object>>((sp, t, r, c) => GetService(sp, t, r, c));

    /// <summary>
    ///     Creates an instance of a specified type.
    /// </summary>
    /// <param name="provider">The service provider used to resolve dependencies</param>
    /// <param name="instanceType">The type to activate</param>
    /// <param name="parameters">Constructor arguments not provided by the <paramref name="provider"/>.</param>
    /// <returns>An activated object of type instanceType</returns>
    public static object CreateInstance(IServiceProvider provider, Type instanceType, params object[] parameters)
    {
        var bestLength = -1;
        var seenPreferred = false;

        ConstructorMatcher? bestMatcher = null;

        if (!instanceType.GetTypeInfo().IsAbstract)
        {
            foreach (var constructor in instanceType
                         .GetTypeInfo()
                         .DeclaredConstructors
                         .Where(c => c is { IsStatic: false, IsPublic: true }))
            {
                var matcher = new ConstructorMatcher(constructor);

                var api = provider.GetRequiredService<ICoreAPI>();
                var isPreferred = constructor.IsSided(api.Side);
                var length = matcher.Match(parameters);

                if (isPreferred)
                {
                    if (seenPreferred)
                    {
                        ThrowMultipleConstructorsMarkedWithAttributeException();
                    }

                    if (length == -1)
                    {
                        ThrowMarkedCtorDoesNotTakeAllProvidedArguments();
                    }
                }

                if (isPreferred || bestLength < length)
                {
                    bestLength = length;
                    bestMatcher = matcher;
                }

                seenPreferred |= isPreferred;
            }
        }

        if (bestMatcher != null) return bestMatcher.CreateInstance(provider);
        var message = $"A suitable constructor for type '{instanceType}' could not be located. Ensure the type is concrete and services are registered for all parameters of a public constructor.";
        throw new InvalidOperationException(message);

    }

    /// <summary>
    ///     Creates an instance of a specified type.
    /// </summary>
    /// <typeparam name="T">The type of instance to create.</typeparam>
    /// <param name="args">The arguments to pass to the constructor.</param>
    /// <returns>T.</returns>
    public static T? CreateInstance<T>(params object?[]? args) 
        => (T?)Activator.CreateInstance(typeof(T), args);

    /// <summary>
    ///     Create a delegate that will instantiate a type with constructor arguments provided directly and/or from an instance of <see cref="IServiceProvider"/>.
    /// </summary>
    /// <param name="side"></param>
    /// <param name="instanceType">The type to activate</param>
    /// <param name="argumentTypes">
    ///     The types of objects, in order, that will be passed to the returned function as its second parameter
    /// </param>
    /// <returns>
    ///     A factory that will instantiate instanceType using an instance of <see cref="IServiceProvider"/>, and an argument array containing objects matching the types defined in argumentTypes
    /// </returns>
    public static ObjectFactory CreateSidedFactory(EnumAppSide side, Type instanceType, Type[] argumentTypes)
    {
        FindApplicableConstructor(side, instanceType, argumentTypes, out var constructor, out var parameterMap);

        var provider = Expression.Parameter(typeof(IServiceProvider), "provider");
        var argumentArray = Expression.Parameter(typeof(object[]), "argumentArray");
        var factoryExpressionBody = BuildFactoryExpression(constructor, parameterMap, provider, argumentArray);

        var factoryLambda = Expression.Lambda<System.Func<IServiceProvider, object[], object>>(
            factoryExpressionBody, provider, argumentArray);

        var result = factoryLambda.Compile();
        return result.Invoke;
    }

    /// <summary>
    ///     Instantiate a type with constructor arguments provided directly and/or from an instance of <see cref="IServiceProvider"/>.
    /// </summary>
    /// <typeparam name="T">The type to activate</typeparam>
    /// <param name="provider">The service provider used to resolve dependencies</param>
    /// <param name="parameters">Constructor arguments not provided by the <paramref name="provider"/>.</param>
    /// <returns>An activated object of type T</returns>
    public static T CreateInstance<T>(IServiceProvider provider, params object[] parameters)
    {
        return (T)CreateInstance(provider, typeof(T), parameters);
    }


    /// <summary>
    ///     Retrieve an instance of the given type from the service provider. If one is not found then instantiate it directly.
    /// </summary>
    /// <typeparam name="T">The type of the service</typeparam>
    /// <param name="provider">The service provider used to resolve dependencies</param>
    /// <returns>The resolved service or created instance</returns>
    public static T GetServiceOrCreateInstance<T>(IServiceProvider provider)
    {
        return (T)GetServiceOrCreateInstance(provider, typeof(T));
    }

    /// <summary>
    ///     Retrieve an instance of the given type from the service provider. If one is not found then instantiate it directly.
    /// </summary>
    /// <param name="provider">The service provider</param>
    /// <param name="type">The type of the service</param>
    /// <returns>The resolved service or created instance</returns>
    public static object GetServiceOrCreateInstance(IServiceProvider provider, Type type)
    {
        return provider.GetService(type) ?? CreateInstance(provider, type);
    }

    private static MethodInfo GetMethodInfo<T>(Expression<T> expr)
    {
        var mc = (MethodCallExpression)expr.Body;
        return mc.Method;
    }

    private static object GetService(IServiceProvider sp, Type type, Type requiredBy, bool isDefaultParameterRequired)
    {
        var service = sp.GetService(type);
        if (service is not null || isDefaultParameterRequired) return service!;
        throw new InvalidOperationException($"Unable to resolve service for type '{type}' while attempting to activate '{requiredBy}'.");
    }

    private static Expression BuildFactoryExpression(
        ConstructorInfo? constructor,
        IReadOnlyList<int?>? parameterMap,
        Expression serviceProvider,
        Expression factoryArgumentArray)
    {
        ArgumentNullException.ThrowIfNull(constructor, nameof(constructor));
        ArgumentNullException.ThrowIfNull(parameterMap, nameof(parameterMap));
        var constructorParameters = constructor.GetParameters();
        var constructorArguments = new Expression[constructorParameters.Length];

        for (var i = 0; i < constructorParameters.Length; i++)
        {
            var constructorParameter = constructorParameters[i];
            var parameterType = constructorParameter.ParameterType;
            var hasDefaultValue = constructorParameter.TryGetDefaultValue(out var defaultValue);

            if (parameterMap[i] != null)
            {
                constructorArguments[i] = Expression.ArrayAccess(factoryArgumentArray, Expression.Constant(parameterMap[i]));
            }
            else
            {
                var parameterTypeExpression = new[] { serviceProvider,
                    Expression.Constant(parameterType, typeof(Type)),
                    Expression.Constant(constructor.DeclaringType, typeof(Type)),
                    Expression.Constant(hasDefaultValue) };
                constructorArguments[i] = Expression.Call(_getServiceInfo, parameterTypeExpression);
            }

            // Support optional constructor arguments by passing in the default value
            // when the argument would otherwise be null.
            if (hasDefaultValue)
            {
                var defaultValueExpression = Expression.Constant(defaultValue);
                constructorArguments[i] = Expression.Coalesce(constructorArguments[i], defaultValueExpression);
            }

            constructorArguments[i] = Expression.Convert(constructorArguments[i], parameterType);
        }

        return Expression.New(constructor, constructorArguments);
    }

    private static void FindApplicableConstructor(
        EnumAppSide side,
        Type instanceType,
        Type[] argumentTypes,
        out ConstructorInfo? matchingConstructor,
        out int?[]? parameterMap)
    {
        matchingConstructor = null;
        parameterMap = null;

        if (TryFindPreferredConstructor(side, instanceType, argumentTypes, ref matchingConstructor, ref parameterMap) ||
            TryFindMatchingConstructor(side, instanceType, argumentTypes, ref matchingConstructor, ref parameterMap)) return;
        var message = $"A suitable constructor for type '{instanceType}' could not be located. Ensure the type is concrete and services are registered for all parameters of a public constructor.";
        throw new InvalidOperationException(message);
    }

    // Tries to find constructor based on provided argument types
    private static bool TryFindMatchingConstructor(EnumAppSide side,
        Type instanceType,
        Type[] argumentTypes,
        ref ConstructorInfo? matchingConstructor,
        ref int?[]? parameterMap)
    {
        foreach (var constructor in instanceType.GetTypeInfo().DeclaredConstructors)
        {
            if (constructor.IsStatic || !constructor.IsPublic)
            {
                continue;
            }

            if (!TryCreateParameterMap(constructor.GetParameters(), argumentTypes, out var tempParameterMap))
                continue;
            if (matchingConstructor != null)
            {
                throw new InvalidOperationException($"Multiple constructors accepting all given argument types have been found in type '{instanceType}'. There should only be one applicable constructor.");
            }

            matchingConstructor = constructor;
            parameterMap = tempParameterMap;
        }

        return matchingConstructor != null;
    }

    // Tries to find constructor marked with ActivatorUtilitiesConstructorAttribute
    private static bool TryFindPreferredConstructor(
        EnumAppSide side,
        Type instanceType,
        Type[] argumentTypes,
        ref ConstructorInfo? matchingConstructor,
        ref int?[]? parameterMap)
    {
        var seenPreferred = false;
        foreach (var constructor in instanceType.GetTypeInfo().DeclaredConstructors)
        {
            if (constructor.IsStatic || !constructor.IsPublic)
            {
                continue;
            }

            if (!constructor.IsSided(side)) continue;
            if (seenPreferred)
            {
                ThrowMultipleConstructorsMarkedWithAttributeException();
            }

            if (!TryCreateParameterMap(constructor.GetParameters(), argumentTypes, out var tempParameterMap))
            {
                ThrowMarkedCtorDoesNotTakeAllProvidedArguments();
            }

            matchingConstructor = constructor;
            parameterMap = tempParameterMap;
            seenPreferred = true;
        }

        return matchingConstructor != null;
    }

    // Creates an injectable parameterMap from givenParameterTypes to assignable constructorParameters.
    // Returns true if each given parameter type is assignable to a unique; otherwise, false.
    private static bool TryCreateParameterMap(ParameterInfo[] constructorParameters, Type[] argumentTypes, out int?[] parameterMap)
    {
        parameterMap = new int?[constructorParameters.Length];

        for (var i = 0; i < argumentTypes.Length; i++)
        {
            var foundMatch = false;
            var givenParameter = argumentTypes[i].GetTypeInfo();

            for (var j = 0; j < constructorParameters.Length; j++)
            {
                if (parameterMap[j] != null)
                {
                    // This ctor parameter has already been matched
                    continue;
                }

                if (!constructorParameters[j].ParameterType.GetTypeInfo().IsAssignableFrom(givenParameter))
                {
                    continue;
                }

                foundMatch = true;
                parameterMap[j] = i;
                break;
            }

            if (!foundMatch)
            {
                return false;
            }
        }

        return true;
    }

    private class ConstructorMatcher
    {
        private readonly ConstructorInfo _constructor;
        private readonly ParameterInfo[] _parameters;
        private readonly object[] _parameterValues;
        private readonly bool[] _parameterValuesSet;

        public ConstructorMatcher(ConstructorInfo constructor)
        {
            _constructor = constructor;
            _parameters = _constructor.GetParameters();
            _parameterValuesSet = new bool[_parameters.Length];
            _parameterValues = new object[_parameters.Length];
        }

        public int Match(IReadOnlyList<object> givenParameters)
        {
            var applyIndexStart = 0;
            var applyExactLength = 0;
            for (var givenIndex = 0; givenIndex != givenParameters.Count; givenIndex++)
            {
                var givenType = givenParameters[givenIndex]?.GetType().GetTypeInfo();
                var givenMatched = false;

                for (var applyIndex = applyIndexStart; givenMatched == false && applyIndex != _parameters.Length; ++applyIndex)
                {
                    if (_parameterValuesSet[applyIndex] ||
                        !_parameters[applyIndex]
                            .ParameterType
                            .GetTypeInfo()
                            .IsAssignableFrom(givenType)) continue;
                    givenMatched = true;
                    _parameterValuesSet[applyIndex] = true;
                    _parameterValues[applyIndex] = givenParameters[givenIndex];
                    if (applyIndexStart != applyIndex) continue;
                    applyIndexStart++;
                    if (applyIndex == givenIndex)
                    {
                        applyExactLength = applyIndex;
                    }
                }

                if (givenMatched == false)
                {
                    return -1;
                }
            }
            return applyExactLength;
        }

        public object CreateInstance(IServiceProvider provider)
        {
            for (var index = 0; index != _parameters.Length; index++)
            {
                if (_parameterValuesSet[index]) continue;
                var value = provider.GetService(_parameters[index].ParameterType);
                if (value == null)
                {
                    if (!_parameters[index].TryGetDefaultValue(out var defaultValue))
                    {
                        throw new InvalidOperationException($"Unable to resolve service for type '{_parameters[index].ParameterType}' while attempting to activate '{_constructor.DeclaringType}'.");
                    }
                    _parameterValues[index] = defaultValue;
                }
                else
                {
                    _parameterValues[index] = value;
                }
            }

            try
            {
                return _constructor.Invoke(_parameterValues);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException is not null)
                    ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                // The above line will always throw, but the compiler requires we throw explicitly.
                throw;
            }
        }
    }

    private static void ThrowMultipleConstructorsMarkedWithAttributeException()
    {
        throw new InvalidOperationException("Multiple constructors were marked with a service-based constructor attribute.");
    }

    private static void ThrowMarkedCtorDoesNotTakeAllProvidedArguments()
    {
        throw new InvalidOperationException("Constructor marked with a service-based constructor attribute does not accept all given argument types.");
    }
}