using System.Reflection;

namespace Data;
public class TypeHandler : List<object> {
    public TypeHandler(Type type) {
        Add(type);
    }
    public IParser<TDel, TType>? TryGetParser<TDel, TType>(Type lookType) {
        foreach (var item in this)
            if (item is IParser<TDel, TType> pars)
                return pars;
        foreach (var item in this)
            if (item is Type type) {

            }
        return null;
    }
}
public static class Retriver {
    public static readonly Dictionary<Type, TypeHandler> ParsersDict = [];
    public static readonly Dictionary<Type, TypeHandler> NullParsersDict = [];
    public static readonly Type ParserType = typeof(IParser<,>);
    static Retriver() {
        AddTypesFromAssembly(Assembly.GetExecutingAssembly());
    }
    public static void AddTypesFromAssembly(Assembly assembly) {
        var types = assembly.GetTypes();
        foreach (var type in types) {
            if (!type.IsClass || type.IsAbstract)
                continue;
            var i = 0;
            var interfaces = type.GetInterfaces();
            Type? tType;
            while ((tType = TryGetTType(interfaces, ref i)) is not null) {
                tType = GetLookType(tType, out var isNullable);
                var dictToUse = isNullable ? NullParsersDict : ParsersDict;
                if (dictToUse.TryGetValue(tType, out var handler))
                    handler.Add(type);
                else
                    dictToUse[tType] = new TypeHandler(type);
            }
        }
    }
    private static Type? TryGetTType(Type[] interfaces, ref int i) {
        for (; i < interfaces.Length; i++) { 
            var type = interfaces[i];
            if (!type.IsGenericType || type.GetGenericTypeDefinition() != ParserType)
                continue;
            var args = type.GetGenericArguments();
            if (args.Length != 2)
                continue;
            i++;
            return args[1];
        }
        return null;
    }
    public static IParser<TDel, TType>? TryGetParser<TDel, TType>() {
        var lookType = GetLookType(typeof(TType), out var isNullable);
        if ((isNullable ? NullParsersDict : ParsersDict).TryGetValue(lookType, out var handler))
            return handler.TryGetParser<TDel, TType>(lookType);
        return null;
    }
    private static Type GetLookType(Type type, out bool isNullable) {
        isNullable = false;
        if (!type.IsGenericType)
            return type;
        var lookType = type.GetGenericTypeDefinition();
        if (lookType != DataTable.NullableType)
            return lookType;
        isNullable = true;
        lookType = type.GenericTypeArguments[0];
        if (!lookType.IsGenericType)
            return lookType;
        return lookType.GetGenericTypeDefinition();
    }
}
