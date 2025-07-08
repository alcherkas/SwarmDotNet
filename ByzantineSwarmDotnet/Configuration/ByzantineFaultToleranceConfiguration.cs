namespace ByzantineSwarmDotnet.Configuration;

public class ByzantineFaultToleranceConfiguration
{
    public bool EnableBFT { get; set; } = true;
    public string ConsensusAlgorithm { get; set; } = "PBFT";
    public int MinConsensusParticipants { get; set; } = 10;
    public int MaxConsensusTime { get; set; } = 15000;
    public bool MessageVerificationEnabled { get; set; } = true;
    public int RequiredSignatures { get; set; } = 7;
    public string SignatureAlgorithm { get; set; } = "ECDSA-P256";
    public bool EnableGossipProtocol { get; set; } = true;
    public int GossipFanout { get; set; } = 3;
    public int GossipInterval { get; set; } = 2000;
}