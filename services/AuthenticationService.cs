using Microsoft.Data.Sqlite;

namespace MyLibrary.Services;

public class AuthenticationService
{
    private readonly string _connectionString = "Data Source=library.db";

    public bool Authenticate(string username, string password)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(1) FROM Users WHERE Username = @username AND Password = @password";
        command.Parameters.AddWithValue("@username", username);
        command.Parameters.AddWithValue("@password", password);
        var result = command.ExecuteScalar();
        return result is long count && count > 0;
    }
}
