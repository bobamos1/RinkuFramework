namespace Rinku.Tools.StringExtensions; 
public static class Comparators {
    public static bool Same(this string str1, string str2) {
        if (str1.Length != str2.Length)
            return false;
        for (int i = 0; i < str1.Length; i++)
            if ((str1[i] | 32) != (str2[i] | 32))
                return false;
        return true;
    }
}
