using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HTMLTemplating; 
public static class Extensions {
    public static bool TryUpdate<K, V>(this Dictionary<K, V> dict, K key, V newValue) where K : notnull {
        ref var val = ref CollectionsMarshal.GetValueRefOrNullRef(dict, key);
        if (Unsafe.IsNullRef(ref val))
            return false;
        val = newValue;
        return true;
    }
    public static bool TryUpdate<K, V>(this Dictionary<K, V> dict, K key, Func<V, V> updateValue) where K : notnull {
        ref var val = ref CollectionsMarshal.GetValueRefOrNullRef(dict, key);
        if (Unsafe.IsNullRef(ref val))
            return false;
        val = updateValue(val);
        return true;
    }
}
