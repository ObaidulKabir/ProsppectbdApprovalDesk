using System.Security.Cryptography;
using System.Text;
using ProspectbdApprovalDesk.Application.Security;

namespace ProspectbdApprovalDesk.Infrastructure.Security;

public sealed class AesGcmEncryptionService : IEncryptionService
{
    private readonly byte[] _key;

    public AesGcmEncryptionService(string keyBase64)
    {
        _key = Convert.FromBase64String(keyBase64);
        if (_key.Length is not (16 or 24 or 32))
            throw new InvalidOperationException("Encryption key must be 128/192/256-bit (base64).");
    }

    public string EncryptToBase64(string plaintext)
    {
        var nonce = RandomNumberGenerator.GetBytes(12);
        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        var ciphertext = new byte[plaintextBytes.Length];
        var tag = new byte[16];

        using var aes = new AesGcm(_key, tag.Length);
        aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);

        var payload = new byte[nonce.Length + tag.Length + ciphertext.Length];
        Buffer.BlockCopy(nonce, 0, payload, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, payload, nonce.Length, tag.Length);
        Buffer.BlockCopy(ciphertext, 0, payload, nonce.Length + tag.Length, ciphertext.Length);

        return Convert.ToBase64String(payload);
    }

    public string DecryptFromBase64(string ciphertextBase64)
    {
        var payload = Convert.FromBase64String(ciphertextBase64);
        if (payload.Length < 12 + 16)
            throw new CryptographicException("Invalid ciphertext.");

        var nonce = payload.AsSpan(0, 12).ToArray();
        var tag = payload.AsSpan(12, 16).ToArray();
        var ciphertext = payload.AsSpan(28).ToArray();
        var plaintext = new byte[ciphertext.Length];

        using var aes = new AesGcm(_key, tag.Length);
        aes.Decrypt(nonce, ciphertext, tag, plaintext);

        return Encoding.UTF8.GetString(plaintext);
    }
}

