using System;
using System.Reflection;
using DataTable.Parsers;

namespace DataTable;
public static class Registerer {
    private static readonly Type NullableType = typeof(Nullable<>);
    private readonly static Dictionary<Type, Type> ParserTypes = [];
    private readonly static Dictionary<Type, Type> NullableParserTypes = [];
    static Registerer() {
        var types = Assembly.GetExecutingAssembly().GetTypes();
        var referingTypeName = typeof(Parser<>).Name;
        foreach (var type in types) {
            var baseType = type.BaseType;
            if (baseType is null || !baseType.IsGenericType)
                continue;
            if (baseType.Name != referingTypeName)
                continue;
            var parsingType = baseType.GetGenericArguments()[0];
            var parserToUse = ParserTypes;
            if (parsingType.IsGenericType && parsingType.GetGenericTypeDefinition() == NullableType) {
                parserToUse = NullableParserTypes;
                parsingType = parsingType.GetGenericArguments()[0];
            }
            if (type.IsGenericType) {
                if (!type.CheckAndUpdateGenericType(parsingType))
                    continue;
                parsingType = parsingType.GetGenericTypeDefinition();
            }
            parserToUse[parsingType] = type;
        }
    }
    public static bool CheckAndUpdateGenericType(this Type typeParent, Type parsingType) {
        if (!typeParent.IsGenericType || !parsingType.IsGenericType)
            return false;
        var parentArguments = typeParent.GetGenericArguments();
        var childArguments = parsingType.GetGenericArguments();
        if (parentArguments.Length != childArguments.Length)
            return false;
        for (int i = 0; i < parentArguments.Length; i++)
            if (parentArguments[i] != childArguments[i])
                return false;
        return true;
    }
    public static TRet? GetParser<T, TRet>() where TRet : class => GetParser<TRet>(typeof(T));
    private static TRet? GetParser<TRet>(Type type, bool isNull = false) where TRet : class {
        if (!type.IsGenericType) {
            return (isNull ? NullableParserTypes : ParserTypes).TryGetValue(type, out var parserType)
                ? CreateInstanceSafe<TRet>(parserType)
                : null;
        }
        var genType = type.GetGenericTypeDefinition();
        if (genType == NullableType)
            return GetParser<TRet>(type.GetGenericArguments()[0], true);
        if (!(isNull ? NullableParserTypes : ParserTypes).TryGetValue(genType, out var genParserType))
            return null;
        return CreateInstanceSafe<TRet>(genParserType.MakeGenericType(type.GetGenericArguments()));
    }
    public static T? CreateInstanceSafe<T>(Type type) where T : class {
        try {
            return type
                .GetConstructor(
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                    null, Type.EmptyTypes, null)
                ?.Invoke(null) as T;
        }
        catch (Exception) {
            return null;
        }
    }
    public static void Register<T>(this Parser<T> parser) {
        Parser<T>.Instance = parser;
        var interfaces = parser.GetType().GetInterfaces();
        var type = typeof(T);
        foreach (var interfaceType in interfaces) {
            if (!interfaceType.IsGenericType || interfaceType.GetGenericTypeDefinition() != typeof(IParser<,>))
                continue;
            var genericArguments = interfaceType.GetGenericArguments();
            if (genericArguments.Length != 2 || genericArguments[1] != type)
                continue;
            interfaceType
                .GetField("Instance", BindingFlags.Public | BindingFlags.Static)
                ?.SetValue(null, parser);
        }
    }
}
