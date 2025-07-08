namespace ByzantineSwarmDotnet.Examples.SampleData;

/// <summary>
/// Byzantine swarm configuration samples
/// </summary>
public static class ByzantineConfigSamples
{
    public static Dictionary<string, object> GetBasicByzantineConfig()
    {
        return new Dictionary<string, object>
        {
            ["fault_tolerance_level"] = "byzantine",
            ["max_byzantine_faults"] = 3,
            ["required_consensus_percentage"] = 0.67,
            ["reputation_weighted_voting"] = true,
            ["cryptographic_verification"] = true,
            ["timeout_seconds"] = 300,
            ["recovery_enabled"] = true
        };
    }

    public static Dictionary<string, object> GetMaximumSecurityConfig()
    {
        return new Dictionary<string, object>
        {
            ["fault_tolerance_level"] = "byzantine",
            ["max_byzantine_faults"] = 7,
            ["required_consensus_percentage"] = 0.85,
            ["reputation_weighted_voting"] = true,
            ["cryptographic_verification"] = true,
            ["hierarchical_validation"] = true,
            ["real_time_monitoring"] = true,
            ["automatic_quarantine"] = true,
            ["timeout_seconds"] = 900,
            ["recovery_enabled"] = true,
            ["advanced_attack_detection"] = true
        };
    }

    public static Dictionary<string, object> GetAttackSimulationConfig()
    {
        return new Dictionary<string, object>
        {
            ["simulation_mode"] = true,
            ["attack_type"] = "coordinated_byzantine",
            ["malicious_agent_percentage"] = 0.30,
            ["attack_patterns"] = new[]
            {
                "contradictory_responses",
                "timing_attacks",
                "reputation_manipulation",
                "consensus_disruption"
            },
            ["monitoring_enabled"] = true,
            ["recovery_testing"] = true,
            ["resilience_validation"] = true
        };
    }
}