namespace Rinku.Tools.StringExtensions; 
public static class ArrayManipulation {
    private const int RoleCookieLen = 4;
    private const int RoleCookieBase = 64;
    public const int MaxRoleValue = RoleCookieBase ^ RoleCookieLen - 1;
    public static int[] SplitRoles(this string rolesStr) {
        var roles = new int[rolesStr.Length / RoleCookieLen];
        for (int i = 0; i < roles.Length; i++) {
            int value = 0;
            for (int j = i * RoleCookieLen; j < RoleCookieLen; j++) {
                var c = rolesStr[j];
                value = (value * RoleCookieBase) + (c switch {
                    >= '0' and <= '9' => c - '0',
                    >= 'A' and <= 'Z' => 10 + (c - 'A'),
                    >= 'a' and <= 'z' => 36 + (c - 'a'),
                    '-' => 62,
                    '_' => 63,
                    _ => throw new NotImplementedException()
                });
            }
            roles[i] = value;
        }
        return roles;
    }
    public static string JoinRoles(this int[] roles) {
        if (roles.Length == 0)
            return string.Empty;
        Span<char> chars = stackalloc char[roles.Length * RoleCookieLen];
        for (int i = 0; i < roles.Length; i++)
            FillIntInSpan(roles[i], chars, i * RoleCookieLen);
        return chars.ToString();
    }
    public static void FillIntInSpan(int number, Span<char> result, int start) {
        for (int i = start + RoleCookieLen - 1; i >= start; i--) {
            if (number == 0) {
                result[i] = '0';
                continue;
            }
            int remainder = number % RoleCookieBase;
            result[i] = remainder switch {
                < 10 => (char)('0' + remainder),
                < 36 => (char)('A' + (remainder - 10)),
                < 62 => (char)('a' + (remainder - 36)),
                62 => '-',
                63 => '_',
                _ => throw new NotImplementedException()
            };
            number /= RoleCookieBase;
        }
    }
}
