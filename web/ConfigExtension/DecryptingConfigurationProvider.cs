using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace web.ConfigExtension;

// this custom implementation of configuration will be a singleton lifespan in app
public class DecryptingConfigurationProvider(IConfiguration configuration) : ConfigurationProvider
{
    private readonly IConfiguration _configuration = configuration;

    //represent cache to store data
    private readonly ConcurrentDictionary<string, string> _decryptedCache = new();

    private static readonly byte[] _encryptionKey = SHA256.HashData(
        Encoding.UTF8.GetBytes("This is a long string that will be hashed to create a 32-byte key"));

    //overide the default load method to this method which saves all values into cache
    public override void Load()
    {
        foreach (var pair in _configuration.AsEnumerable())
        {
            Data[pair.Key] = pair.Value;
        }
    }

    // this is the method which runs everytime, we call any configuration method
    public override bool TryGet(string key, out string value)
    {
        if (base.TryGet(key, out var encryptedValue))
        {
            value = encryptedValue != null ? GetDecryptedValue(key, encryptedValue) : null;
            return true;
        }

        value = null;
        return false;
    }
    
    // check if any value start with ENC, if it is then it decrypt the value
    private string GetDecryptedValue(string key, string encryptedValue)
    {
        if (encryptedValue == null)
        {
            return null;
        }

        return _decryptedCache.GetOrAdd(key, _ =>
        {
            if (IsEncrypted(encryptedValue))
            {
                try
                {
                    return Decrypt(encryptedValue);
                }
                catch (Exception)
                {
                    // Log the exception
                    return encryptedValue; // Return original value if decryption fails
                }
            }
            return encryptedValue;
        });
    }

    private bool IsEncrypted(string value) => value?.StartsWith("ENC:") ?? false;

    private string Decrypt(string encryptedValue)
    {
        string cipherText = encryptedValue.Substring(4);
        byte[] fullCipherText = Convert.FromBase64String(cipherText);

        byte[] iv = new byte[16];
        byte[] encryptedData = new byte[fullCipherText.Length - 16];

        Array.Copy(fullCipherText, iv, 16);
        Array.Copy(fullCipherText, 16, encryptedData, 0, encryptedData.Length);

        using var aes = Aes.Create();
        aes.Key = _encryptionKey;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        using var ms = new MemoryStream(encryptedData);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}
