using System;
using System.IO;
using System.Xml.Linq;

class Program
{
    static void Main(string[] args)
    {
        // Load configuration
        var configPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "config.xml");
        if (!File.Exists(configPath))
            configPath = Path.Combine(AppContext.BaseDirectory, "config.xml"); // fallback

        var config = XElement.Load(configPath);

        var length = int.Parse(config.Element("Length")?.Value ?? "16");
        var useUpper = bool.Parse(config.Element("UseUpper")?.Value ?? "true");
        var useLower = bool.Parse(config.Element("UseLower")?.Value ?? "true");
        var useDigits = bool.Parse(config.Element("UseDigits")?.Value ?? "true");
        var useSymbols = bool.Parse(config.Element("UseSymbols")?.Value ?? "true");
        var storeToDb = bool.Parse(config.Element("StoreToDb")?.Value ?? "false");
        var dbPath = config.Element("DatabasePath")?.Value ?? "passwords.db";

        var svc = new PasswordService(useUpper, useLower, useDigits, useSymbols);

        Console.WriteLine("Password Generator — Secure, customizable\n");

        // parse simple CLI args
        int requestedLength = length;
        if (args.Length >= 1 && int.TryParse(args[0], out var aLen))
            requestedLength = aLen;

        var pwd = svc.Generate(requestedLength);

        Console.WriteLine($"Generated password ({requestedLength} chars):\n\n{pwd}\n");

        // show entropy estimate
        double entropyBits = svc.EstimateEntropyBits(requestedLength);
        Console.WriteLine($"Estimated entropy: {Math.Round(entropyBits, 2)} bits\n");

        // optionally store (hashed) or metadata in DB
        if (storeToDb)
        {
            var db = new PasswordStore(dbPath);
            db.EnsureDatabase();
            db.InsertRecord(pwd, requestedLength, entropyBits);
            Console.WriteLine($"Stored password metadata in DB: {dbPath}\n");
        }

        Console.WriteLine("Done. Press any key to exit.");
        Console.ReadKey();
    }
}
