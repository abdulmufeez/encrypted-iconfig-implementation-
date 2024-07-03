using System.Security.Cryptography;
using System.Text;

namespace console.ConfigEncryption;

// Encryption will be AES
public static class ConfigEncryption
{
    private static readonly byte[] _encryptionKey = SHA256.HashData(
        Encoding.UTF8.GetBytes("This is a long string that will be hashed to create a 32-byte key"));
    public static string Encrypt(string value)
    {
        using var aes = Aes.Create();
        aes.Key = _encryptionKey;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        ms.Write(aes.IV, 0, aes.IV.Length);
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(value);
        }
        return Convert.ToBase64String(ms.ToArray());
    }
}
