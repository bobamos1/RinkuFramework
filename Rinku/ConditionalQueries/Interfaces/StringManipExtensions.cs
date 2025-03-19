namespace ConditionalQueries.Interfaces;
public static class StringManipExtensions {
    public const int StackallocIntBufferSizeLimit = 128;
    public static ReadOnlySpan<char> ExtractTrim(this ReadOnlySpan<char> str, int startingIndex, int endingIndex) {
        int starting = startingIndex;
        int ending = endingIndex;
        while (starting < ending && char.IsWhiteSpace(str[starting]))
            starting++;
        if (starting >= ending)
            return [];
        while (starting < ending && char.IsWhiteSpace(str[ending - 1]))
            ending--;
        if (starting >= ending)
            return [];
        return str[starting..ending];
    }
    public static bool StartingWith(this ReadOnlySpan<char> str, string look) {
        if (look.Length > str.Length)
            return false;
        if ((str[0] | 32) != (look[0] | 32))
            return false;
        for (int i = 1; i < look.Length; i++)
            if ((str[i] | 32) != (look[i] | 32))
                return false;
        return true;
    }
    public static bool StartingWith(this ReadOnlySpan<char> str, string look, int from) {
        if (look.Length > str.Length - from)
            return false;
        if ((str[0 + from] | 32) != (look[0] | 32))
            return false;
        for (int i = 1; i < look.Length; i++)
            if ((str[i + from] | 32) != (look[i] | 32))
                return false;
        return true;
    }
    public static bool EndingWith(this ReadOnlySpan<char> str, string look) {
        if (look.Length > str.Length)
            return false;
        if ((str[^1] | 32) != (look[^1] | 32))
            return false;
        for (int i = 2; i < look.Length; i++)
            if ((str[^i] | 32) != (look[^i] | 32))
                return false;
        return true;
    }
    public static bool EndingWith(this ReadOnlySpan<char> str, string look, int from) {
        if (look.Length > from)
            return false;
        if ((str[^(1 + from)] | 32) != (look[^1] | 32))
            return false;
        for (int i = 2; i < look.Length; i++)
            if ((str[^(i + from)] | 32) != (look[^i] | 32))
                return false;
        return true;
    }
    public static int FindIndex(this ReadOnlySpan<char> str, string look) {
        for (int i = 0; i < str.Length; i++)
            if (str.StartingWith(look, i))
                return i;
        return -1;
    }
    public static int FindLastIndex(this ReadOnlySpan<char> str, string look) {
        for (int i = str.Length - 1; i >= 0; i--)
            if (str.EndingWith(look, i))
                return i;
        return -1;
    }
    public static int FillIndexes(this ReadOnlySpan<char> sql, char look, Span<int> indexes, int startingPoint = 0) {
        var nbAdded = 0;
        for (int i = 0; i < sql.Length; i++)
            if (sql[i] == look) {
                indexes[startingPoint + nbAdded] = i;
                nbAdded++;
            }
        return nbAdded;
    }
    public static string[] SplitTrim(this ReadOnlySpan<char> str, char delimiter = ',') {
        Span<int> indexes = stackalloc int[StackallocIntBufferSizeLimit];
        int nb = str.GetSplitIndexes(indexes, delimiter);
        if (nb == 0)
            return [];
        if (nb == 1)
            return [new(str[indexes[0]..indexes[1]])];
        return str.ArrayFromIndexes(indexes, nb);
    }
    public static string[] ArrayFromIndexes(this ReadOnlySpan<char> str, Span<int> indexes, int nb) {
        string[] results = new string[nb];
        for (int i = 0, j = 0; i < nb; i++, j += 2)
            results[i] = new string(str[indexes[j]..indexes[j + 1]]);
        return results;
    }
    public static int GetSplitIndexes(this ReadOnlySpan<char> str, Span<int> indexes, char delimiter = ',') {
        var nb = 0;
        int firstNonSpace = -1;
        int lastNonSpace = -1;
        for (int i = 0; i < str.Length; i++) {
            if (str[i] is ' ' or '\n' or '\r' or '\t') {
                if (lastNonSpace == -1)
                    lastNonSpace = i;
                continue;
            }
            if (str[i] != delimiter) {
                if (firstNonSpace == -1)
                    firstNonSpace = i;
                lastNonSpace = -1;
                continue;
            }
            if (firstNonSpace == -1)
                continue;
            if (lastNonSpace == -1)
                lastNonSpace = i;
            indexes[nb++] = firstNonSpace;
            indexes[nb++] = lastNonSpace;
            firstNonSpace = -1;
            lastNonSpace = -1;
        }
        if (firstNonSpace != -1) {
            if (lastNonSpace == -1)
                lastNonSpace = str.Length;
            indexes[nb++] = firstNonSpace;
            indexes[nb++] = lastNonSpace;
        }
        return nb / 2;
    }
    public static int LastIndexOf(this ReadOnlySpan<char> str, char look) {
        for (int i = str.Length - 1; i >= 0; i--)
            if (str[i] == look)
                return i;
        return -1;
    }
}
