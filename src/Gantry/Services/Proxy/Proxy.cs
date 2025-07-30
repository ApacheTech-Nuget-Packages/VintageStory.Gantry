//using ApacheTech.Common.Extensions.Harmony;

//namespace Gantry.Services.Proxy;

//internal class DynamicProxy<TProxy, TTarget> : IDisposable
//    where TProxy : Proxy<TTarget>, new()
//    where TTarget : class
//{
//    private readonly Harmony _harmony;
//    private readonly TProxy _proxyInstance;
//    private readonly TTarget _targetInstance;
//    private readonly BindingFlags _bindingFlags = 
//        BindingFlags.Instance | 
//        BindingFlags.Public |
//        BindingFlags.NonPublic | 
//        BindingFlags.FlattenHierarchy;

//    private DynamicProxy(TTarget targetInstance)
//    {
//        _harmony = new Harmony(GetHashCode().ToString());
//        _proxyInstance = new TProxy();
//        _targetInstance = targetInstance;
//        PatchDisposeMethod();

//        var proxy = Traverse.Create<TProxy>();
        

//        var methods = typeof(TProxy)
//            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
//            .Where(p => p.Name != nameof(IDisposable.Dispose));

//        foreach (var method in methods)
//        {
//            var targetName = method.Name;
//            var targetType = TargetType.Method;
//            if (method.GetCustomAttribute<MasksAttribute>() is MasksAttribute masksAttribute)
//            {
//                targetName = masksAttribute.MemberName;
//                targetType = masksAttribute.TargetType;
//            }
//        }


//        var properties = typeof(TProxy)
//            .GetProperties(BindingFlags.Instance | BindingFlags.Public);

//    }

//    private void PatchDisposeMethod()
//    {
//        var proxyDisposeMethod = typeof(TProxy).GetMethod(nameof(IDisposable.Dispose));
//        var thisDisposeMethod = typeof(DynamicProxy<TProxy, TTarget>).GetMethod(nameof(Dispose));
//        _harmony.Patch(proxyDisposeMethod, postfix: new HarmonyMethod(thisDisposeMethod));
//    }

//    public static TProxy Create(TTarget targetInstance)
//        => new DynamicProxy<TProxy, TTarget>(targetInstance)._proxyInstance!;

//    private T GetValue<T>(TargetType type, string name, object[]? args) => type switch
//    {
//        TargetType.Method => _targetInstance.CallMethod<T>(name, args),
//        TargetType.Field => _targetInstance.GetField<T>(name),
//        TargetType.Property => _targetInstance.GetProperty<T>(name),
//        _ => throw new UnreachableException(),
//    };

//    private void SetValue<T>(TargetType type, string name, T value)
//    {
//        switch (type)
//        {
//            case TargetType.Field: _targetInstance.SetField(name, value); break;
//            case TargetType.Property: _targetInstance.SetProperty(name, value); break;
//            default: throw new UnreachableException();
//        };
//    }

//    public void Dispose()
//    {
//        _harmony.UnpatchAll();
//    }
//}

//internal class Test
//{
//    public Test()
//    {
//        var target = new TargetClass();

//        using var proxy = DynamicProxy<ProxyClass, TargetClass>.Create(target);
//        proxy.NotSoSecretNumber = 42;

//    }
//}

//internal enum TargetType
//{
//    Field,
//    Property,
//    Method
//}

//[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
//internal class MasksAttribute : Attribute
//{
//    public required TargetType TargetType { get; set; }
//    public required string MemberName { get; set; }
//}

//internal class TargetClass
//{
//    private int _secretNumber = 69;
//}

//internal class ProxyClass : Proxy<TargetClass>
//{
//    [Masks(TargetType = TargetType.Field, MemberName = "_secretNumber")]
//    public int NotSoSecretNumber { get; set; }
//}

//internal abstract class Proxy<T> : IDisposable
//{
//    public virtual void Dispose()
//    {
//        // INTENTIONALLY BLANK
//    }
//}