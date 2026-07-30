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
    [Description("Azure Key Vault URI (env: VAULT_URI)")]
    public string? VaultUri { get; init; } = Environment.GetEnvironmentVariable("VAULT_URI");

    [CommandOption("-k|--key")]
    [Description("Certificate/key name in Key Vault (env: CERT_NAME)")]
    public string? KeyName { get; init; } = Environment.GetEnvironmentVariable("CERT_NAME");

    [CommandOption("--auth")]
    [Description("Authentication method")]
    public AuthType AuthType { get; init; } = AuthType.Default;

    [CommandOption("--client-id")]
    [Description("Client ID for service principal (env: CLIENT_ID)")]
    public string? ClientId { get; init; } = Environment.GetEnvironmentVariable("CLIENT_ID");

    [CommandOption("--client-secret")]
    [Description("Client secret for service principal (env: CLIENT_SECRET)")]
    public string? ClientSecret { get; init; } = Environment.GetEnvironmentVariable("CLIENT_SECRET");

    [CommandOption("--tenant-id")]
    [Description("Tenant ID for service principal (env: TENANT_ID)")]
    public string? TenantId { get; init; } = Environment.GetEnvironmentVariable("TENANT_ID");

    [CommandOption("--cert")]
    [Description("Path to certificate file (PFX) for certificate auth")]
    public string? CertificatePath { get; init; }

    [CommandOption("--cert-pass")]
    [Description("Password for certificate file (env: CERT_PASS)")]
    public string? CertificatePassword { get; init; } = Environment.GetEnvironmentVariable("CERT_PASS");

    [CommandArgument(0, "<files>")]
    public required string[] Files { get; init; }
}
