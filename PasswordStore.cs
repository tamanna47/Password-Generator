using System;
using System.Data.SQLite;
using System.IO;

public class PasswordStore
{
    private readonly string _dbPath;
    private readonly string _connString;

    public PasswordStore(string dbPath = "passwords.db")
    {
        _dbPath = dbPath;
        _connString = $"Data Source={_dbPath};Version=3;";
    }

    public void EnsureDatabase()
    {
        var exists = File.Exists(_dbPath);
        if (!exists)
        {
            SQLiteConnection.CreateFile(_dbPath);
        }

        using var conn = new SQLiteConnection(_connString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS password_records(
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                generated_at TEXT,
                length INTEGER,
                entropy REAL,
                password TEXT
            );";
        cmd.ExecuteNonQuery();
    }

    // NOTE: storing plaintext passwords is dangerous. This method stores them for demo/audit only.
    // Consider storing only salted hash / metadata in production.
    public void InsertRecord(string password, int length, double entropy)
    {
        using var conn = new SQLiteConnection(_connString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO password_records (generated_at, length, entropy, password)
            VALUES (@t, @len, @entropy, @pwd);";
        cmd.Parameters.AddWithValue("@t", DateTime.UtcNow.ToString("o"));
        cmd.Parameters.AddWithValue("@len", length);
        cmd.Parameters.AddWithValue("@entropy", entropy);
        cmd.Parameters.AddWithValue("@pwd", password); // production: do NOT store plaintext
        cmd.ExecuteNonQuery();
    }
}
