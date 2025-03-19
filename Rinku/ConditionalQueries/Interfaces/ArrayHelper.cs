namespace ConditionalQueries.Interfaces; 
public static class ArrayHelper {
    public static T[] MergedWith<T>(this T[] arr1, T[] arr2) {
        var res = new T[arr1.Length + arr2.Length];
        Array.Copy(arr1, res, arr1.Length);
        Array.Copy(arr2, 0, res, arr1.Length, arr2.Length);
        return res;
    }
    public static T[]? Merge<T>(T[]? arr1, T[]? arr2) {
        if (arr1 is null)
            return arr2?.Copy();
        if (arr2 is null)
            return arr1.Copy();
        return arr1.MergedWith(arr2);
    }
    public static T[] Copy<T>(this T[] arr) {
        var res = new T[arr.Length];
        Array.Copy(arr, res, arr.Length);
        return res;
    }
}
