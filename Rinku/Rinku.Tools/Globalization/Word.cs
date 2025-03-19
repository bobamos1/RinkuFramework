using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Rinku.Tools.Globalization;
public class Word : Dictionary<CultureInfo, string> {
    public Word(int capacity) : base(capacity) { }
    public static string CurrentCulture => CultureInfo.CurrentCulture.Name;
    public static CultureInfo DefaultCulture { get; set; } = CultureInfo.InvariantCulture;
    public static string NotSupported { get; set; } = "NotSupported";
    #region transformations
    public static Word operator +(Word word, string str) {
        var newWord = new Word();
        foreach (var kvp in word)
            newWord[kvp.Key] = kvp.Value + str;
        return newWord;
    }
    public static Word operator +(string str, Word word) {
        var newWord = new Word();
        foreach (var kvp in word)
            newWord[kvp.Key] = str + kvp.Value;
        return newWord;
    }
    public static Word Format(Word word, params object?[] values) {
        var formattedWord = new Word();
        foreach (var kvp in word)
            formattedWord[kvp.Key] = string.Format(kvp.Value, values);
        return formattedWord;
    }
    public bool CheckValidFormat(int expectedCount, bool prefectMatch = false) {
        Span<bool> paramsFound = stackalloc bool[expectedCount];
        foreach (var format in this.Values)
            if (!format.CheckValidFormat(paramsFound, prefectMatch))
                return false;
        return true;
    }
    public static Word operator +(Word word1, Word word2) {
        var newWord = new Word();
        foreach (var kvp in word1)
            newWord[kvp.Key] = kvp.Value + word2.GetTranslation(kvp.Key);
        return newWord;
    }
    public static Word Format(Word word1, Word word2) {
        var formattedWord = new Word();
        foreach (var kvp in word1)
            formattedWord[kvp.Key] = string.Format(kvp.Value, word2.GetTranslation(kvp.Key));
        return formattedWord;
    }
    #endregion
    #region initialization
    public Word() { }
    public Word(string traduction) {
        Add(CultureInfo.InvariantCulture, traduction);
    }
    public Word(IDictionary<CultureInfo, string> translations) : base(translations) => Assert();
    public void Assert() {
        if (!IsValid())
            throw new ArgumentException("Needs at least a invariantCulture");
    }
    public bool IsValid() => ContainsKey(DefaultCulture) || ContainsKey(CultureInfo.InvariantCulture);
    public static Word New(IDictionary<CultureInfo, string> traductions) => new(traductions);
    public static Word New(Dictionary<CultureInfo, string> traductions) => new(traductions);
    public static implicit operator Word(string translation) => new(translation);
    #endregion
    #region usage
    [return: NotNullIfNotNull(nameof(word))]
    public static implicit operator string?(Word? word) => word?.ToString();
    public override string ToString() => GetTranslation(CultureInfo.CurrentCulture);
    public string this[string key] {
        get => GetTranslation(new CultureInfo(key));
        set => base[new CultureInfo(key)] = value;
    }
    public new string this[CultureInfo key] {
        get => GetTranslation(key);
        set => base[key] = value;
    }
    public string GetTranslation(CultureInfo culture) {
        if (TryGetValue(culture, out var result))
            return result;
        if (TryGetValue(culture.Parent, out result))
            return result;
        if (TryGetValue(DefaultCulture, out result))
            return result;
        if (TryGetValue(CultureInfo.InvariantCulture, out result))
            return result;
        return Values.FirstOrDefault() ?? NotSupported;
    }
    #endregion
}