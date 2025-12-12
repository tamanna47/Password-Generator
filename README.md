# Password Generator

C#, .NET (console) project: a cryptographically-secure password generator with configurable options (XML), optional SQLite storage for demo/audit, and entropy estimation.

## Features
- Uses `RandomNumberGenerator` for strong randomness
- Configurable via `config.xml` (length, include char sets)
- Entropy estimate: bits = length * log2(poolSize)
- Optional SQLite storage (demo only — do NOT store plaintext in production)

## Quick start
1. Install .NET SDK (6.0 or 7.0)
2. Clone repo
3. `cd src`
4. `dotnet restore`
5. `dotnet run -- <length>`  (e.g. `dotnet run -- 20`)

## Security note
This sample demonstrates generation. Do **not** store plaintext passwords or use insecure storage in production. Store hashed secrets only (Argon2 / PBKDF2) or use a secret manager.

## License
MIT
