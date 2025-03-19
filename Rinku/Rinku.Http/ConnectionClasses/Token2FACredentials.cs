namespace Rinku.Http.ConnectionClasses;

public struct Token2FACredentials(int ID, string Token) {
    public int ID = ID;
    public string Token = Token;
    public readonly bool Validate() {
        return !string.IsNullOrEmpty(Token);
    }
    public static bool WriteToken(int id) {
        return true;
    }
}
