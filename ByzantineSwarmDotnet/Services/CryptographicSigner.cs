using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using ByzantineSwarmDotnet.Models;
using ByzantineSwarmDotnet.Interfaces;

namespace ByzantineSwarmDotnet.Services;

public class CryptographicSigner : ICryptographicSigner, IDisposable
{
    private readonly ILogger<CryptographicSigner> _logger;
    private readonly ECDsa _ecdsa;
    private readonly Dictionary<string, ECDsa> _publicKeys;
    private readonly object _lockObject = new();

    public CryptographicSigner(ILogger<CryptographicSigner> logger)
    {
        _logger = logger;
        _ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        _publicKeys = new Dictionary<string, ECDsa>();
    }

    public async Task<DigitalSignature> SignAsync(object message)
    {
        try
        {
            var messageJson = JsonSerializer.Serialize(message);
            var messageBytes = Encoding.UTF8.GetBytes(messageJson);
            var hash = await HashAsync(messageJson);
            
            byte[] signature;
            string publicKey;
            
            lock (_lockObject)
            {
                signature = _ecdsa.SignHash(hash);
                publicKey = Convert.ToBase64String(_ecdsa.ExportSubjectPublicKeyInfo());
            }

            return new DigitalSignature
            {
                Algorithm = "ECDSA-P256",
                SignatureData = signature,
                PublicKey = publicKey,
                SignedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error signing message");
            throw;
        }
    }

    public async Task<bool> VerifySignatureAsync(object message, DigitalSignature signature)
    {
        try
        {
            if (signature.Algorithm != "ECDSA-P256")
            {
                _logger.LogWarning("Unsupported signature algorithm: {Algorithm}", signature.Algorithm);
                return false;
            }

            var messageJson = JsonSerializer.Serialize(message);
            var hash = await HashAsync(messageJson);
            
            // Get or create ECDsa instance for the public key
            ECDsa? verifyEcdsa = null;
            
            lock (_lockObject)
            {
                if (!_publicKeys.TryGetValue(signature.PublicKey, out verifyEcdsa))
                {
                    verifyEcdsa = ECDsa.Create();
                    try
                    {
                        var publicKeyBytes = Convert.FromBase64String(signature.PublicKey);
                        verifyEcdsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);
                        _publicKeys[signature.PublicKey] = verifyEcdsa;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error importing public key");
                        verifyEcdsa?.Dispose();
                        return false;
                    }
                }
            }

            return verifyEcdsa.VerifyHash(hash, signature.SignatureData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying signature");
            return false;
        }
    }

    public async Task<string> GenerateKeyPairAsync()
    {
        try
        {
            using var tempEcdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            
            var privateKey = Convert.ToBase64String(tempEcdsa.ExportECPrivateKey());
            var publicKey = Convert.ToBase64String(tempEcdsa.ExportSubjectPublicKeyInfo());
            
            var keyPair = new
            {
                PrivateKey = privateKey,
                PublicKey = publicKey,
                CreatedAt = DateTime.UtcNow
            };
            
            return await Task.FromResult(JsonSerializer.Serialize(keyPair));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating key pair");
            throw;
        }
    }

    public async Task<byte[]> HashAsync(string data)
    {
        try
        {
            using var sha256 = SHA256.Create();
            var dataBytes = Encoding.UTF8.GetBytes(data);
            return await Task.FromResult(sha256.ComputeHash(dataBytes));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error computing hash");
            throw;
        }
    }

    public void Dispose()
    {
        lock (_lockObject)
        {
            _ecdsa?.Dispose();
            
            foreach (var ecdsa in _publicKeys.Values)
            {
                ecdsa?.Dispose();
            }
            _publicKeys.Clear();
        }
    }
}
