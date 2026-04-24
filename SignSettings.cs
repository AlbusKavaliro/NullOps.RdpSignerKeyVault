using Spectre.Console.Cli;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System;

namespace NullOps.RdpSigner;

public enum AuthType
{
    Default,
    AzureCli,
    VisualStudio,
    VisualStudioCode,
    InteractiveBrowser,
    ManagedIdentity,
    Environment,
    ClientSecret,
    Certificate
}

public sealed class SignSettings : CommandSettings
{
    [CommandOption("-v|--vault")]
    [Description("Azure Key Vault URI")]
    public string? VaultUri { get; init; }

    [CommandOption("-k|--key")]
    [Description("Certificate/key name in Key Vault")]
    public string? KeyName { get; init; }

    [CommandOption("--auth")]
    [Description("Authentication method")]
    public AuthType AuthType { get; init; } = AuthType.Default;

    [CommandOption("--client-id")]
    [Description("Client ID for service principal")]
    public string? ClientId { get; init; }

    [CommandOption("--client-secret")]
    [Description("Client secret for service principal")]
    public string? ClientSecret { get; init; }

    [CommandOption("--tenant-id")]
    [Description("Tenant ID for service principal")]
    public string? TenantId { get; init; }

    [CommandOption("--cert")]
    [Description("Path to certificate file (PFX) for certificate auth")]
    public string? CertificatePath { get; init; }

    [CommandOption("--cert-pass")]
    [Description("Password for certificate file")]
    public string? CertificatePassword { get; init; }

    [CommandArgument(0, "<files>")]
    public required IEnumerable<string> Files { get; init; }
}
