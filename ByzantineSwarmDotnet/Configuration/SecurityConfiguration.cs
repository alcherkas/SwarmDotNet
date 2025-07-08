namespace ByzantineSwarmDotnet.Configuration;

public class SecurityConfiguration
{
    public bool EnableMessageSigning { get; set; } = true;
    public bool EnableAgentAuthentication { get; set; } = true;
    public bool CertificateValidation { get; set; } = true;
    public bool EnableSecureEnclaves { get; set; } = false;
    public bool AuditLogEnabled { get; set; } = true;
    public int SessionTimeout { get; set; } = 3600000;
}