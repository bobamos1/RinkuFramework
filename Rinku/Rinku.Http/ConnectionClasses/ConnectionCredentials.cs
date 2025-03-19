namespace Rinku.Http.ConnectionClasses;

public struct ConnectionCredentials(string Username, string Password) {
    public string? Username = Username;
    public string? Password = Password;
    public readonly bool Validate(out int id) {
        if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password)) {
            id = -2;
            return false;
        }
        id = 1;
        return true;
    }
}
