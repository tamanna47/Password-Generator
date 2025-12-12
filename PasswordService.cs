using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public class PasswordService
{
    private readonly string _pool;
    private readonly int _poolSize;

    // Default sets
    private const string Upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Lower = "abcdefghijklmnopqrstuvwxyz";
    private const string Digits = "0123456789";
    private const string Symbols = "!@#$%^&*()-_=+[]{}|;:,.<>?/";

    public PasswordService(bool useUpper = true, bool useLower = true, bool useDigits = true, bool useSymbols = true)
    {
        var sb = new StringBuilder();
        if (useUpper) sb.Append(Upper);
        if (useLower) sb.Append(Lower);
        if (useDigits) sb.Append(Digits);
        if (useSymbols) sb.Append(Symbols);

        _pool = sb.Length > 0 ? sb.ToString() : Lower + Upper + Digits; // fallback
        _poolSize = _pool.Length;
    }

    /// <summary>Generate a cryptographically strong random password.</summary>
    public string Generate(int length = 16)
    {
        if (length <= 0) throw new ArgumentException("Length must be > 0", nameof(length));

        var result = new char[length];
        using var rng = RandomNumberGenerator.Create();
        var buffer = new byte[4];

        for (int i = 0; i < length; i++)
        {
            rng.GetBytes(buffer);
            // convert to 32-bit unsigned integer
            uint val = BitConverter.ToUInt32(buffer, 0);
            // map to pool index
            var idx = (int)(val % (uint)_poolSize);
            result[i] = _pool[idx];
        }
        return new string(result);
    }

    /// <summary>Estimate theoretical entropy in bits: length * log2(poolSize)</summary>
    public double EstimateEntropyBits(int length)
    {
        return length * Math.Log(_poolSize, 2);
    }

    /// <summary>Return pool size (for diagnostics)</summary>
    public int PoolSize => _poolSize;
}
