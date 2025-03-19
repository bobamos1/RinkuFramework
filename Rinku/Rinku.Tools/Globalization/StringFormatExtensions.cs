namespace Rinku.Tools.Globalization;
public static class StringFormatExtensions {
    public static int GetNbFormatParams(this string format, int maxCount = 10) {
        Span<bool> paramsFound = stackalloc bool[maxCount];
        return format.GetNbFormatParams(paramsFound);
    }
    public static bool CheckValidFormat(this string format, int maxCount = 10, bool prefectMatch = false) {
        Span<bool> paramsFound = stackalloc bool[maxCount];
        return format.CheckValidFormat(paramsFound, prefectMatch);
    }
    public static bool CheckValidFormat(this string format, Span<bool> paramsFound, bool prefectMatch) => prefectMatch
            ? format.GetNbFormatParams(paramsFound) == paramsFound.Length
            : format.CheckAndFillSpan(paramsFound);
    public static int GetNbFormatParams(this string format, Span<bool> paramsFound) {
        if (!format.CheckAndFillSpan(paramsFound))
            return -1;
        var nbParams = 0;
        for (int i = 0; i < paramsFound.Length; i++) {
            if (paramsFound[i]) {
                nbParams++;
                paramsFound[i] = false;
            }
        }
        return nbParams;
    }
    public static bool CheckAndFillSpan(this string format, Span<bool> paramsFound) {
        for (int i = 0; i < format.Length; i++) {
            if (format[i] != '{')
                continue;
            if (i + 2 >= format.Length)
                return false;
            if (format[i + 1] == '{') {
                i++;
                continue;
            }
            var possibleDigit = ParamNumberOrEnding(format[i + 1]);
            if (possibleDigit == -1)
                return false;
            var possibleSecondDigit = ParamNumberOrEnding(format[i + 2]);
            if (possibleSecondDigit == -1)
                return false;
            if (possibleSecondDigit < 10) {
                if (i + 3 >= format.Length || ParamNumberOrEnding(format[i + 3]) != 10)
                    return false;
                possibleDigit = possibleDigit * 10 + possibleSecondDigit;
            }
            if (possibleDigit >= paramsFound.Length)
                return false;
            paramsFound[possibleDigit] = true;
            if (possibleSecondDigit == 10)
                i += 2;
            else
                i += 3;
        }
        return true;
    }
    private static int ParamNumberOrEnding(char item) {
        if (item == '}' || item == ':')
            return 10;
        if (item < '0' || item > '9')
            return -1;
        return item - '0';
    }
}
