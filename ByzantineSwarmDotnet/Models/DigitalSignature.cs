namespace ByzantineSwarmDotnet.Models;

public record DigitalSignature
{
    public string Algorithm { get; init; } = "ECDSA-P256";
    public byte[] SignatureData { get; init; } = Array.Empty<byte>();
    public string PublicKey { get; init; } = string.Empty;
    public DateTime SignedAt { get; init; } = DateTime.UtcNow;
}