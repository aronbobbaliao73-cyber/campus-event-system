using System.Data;
using Microsoft.Data.SqlClient;

namespace CampusEvents.Backend;

public class RegistrationService
{
    private readonly string _connectionString;

    public RegistrationService(string connectionString)
    {
        _connectionString = connectionString
            ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public string? GetUserRegistration(string inputEmail)
    {
        if (string.IsNullOrWhiteSpace(inputEmail))
        {
            throw new ArgumentException("Email is required.", nameof(inputEmail));
        }

        const string sql = @"
            SELECT TOP (1) r.Status
            FROM dbo.Registrations AS r
            INNER JOIN dbo.Users AS u ON u.UserId = r.UserId
            WHERE u.Email = @Email
            ORDER BY r.RegisteredAt DESC";

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, conn);

        cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value = inputEmail.Trim();

        conn.Open();
        object? result = cmd.ExecuteScalar();

        return result is null || result == DBNull.Value ? null : result.ToString();
    }
}