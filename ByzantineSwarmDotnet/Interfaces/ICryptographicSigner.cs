using ByzantineSwarmDotnet.Models;

namespace ByzantineSwarmDotnet.Interfaces;

public interface ICryptographicSigner
{
    Task<DigitalSignature> SignAsync(object message);
    Task<bool> VerifySignatureAsync(object message, DigitalSignature signature);
    Task<string> GenerateKeyPairAsync();
    Task<byte[]> HashAsync(string data);
}