using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Keys;
using NullOps.RdpSigner;
using RSAKeyVaultProvider;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace NullOps.RdpSigner;

public class SignCommand : Command<SignSettings>
{
    public override int Execute(CommandContext context, SignSettings settings)
    {
        // Basic argument validation
        if (string.IsNullOrWhiteSpace(settings.VaultUri))
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Vault URI is required.");
            return 1;
        }

        if (string.IsNullOrWhiteSpace(settings.KeyName))
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Key name is required.");
            return 1;
        }

        if (settings.Files == null)
        {
            AnsiConsole.MarkupLine("[red]Error:[/] At least one file must be specified.");
            return 1;
        }

        // Build credential based on auth type
        TokenCredential credential = settings.AuthType switch
        {
            AuthType.Default => new DefaultAzureCredential(),
            AuthType.AzureCli => new AzureCliCredential(),
            AuthType.VisualStudio => new VisualStudioCredential(),
            AuthType.VisualStudioCode => new VisualStudioCodeCredential(),
            AuthType.InteractiveBrowser => new InteractiveBrowserCredential(),
            AuthType.ManagedIdentity => new ManagedIdentityCredential(),
            AuthType.Environment => new EnvironmentCredential(),
            AuthType.ClientSecret => new ClientSecretCredential(
                settings.TenantId ?? throw new InvalidOperationException("TenantId required for client secret authentication"),
                settings.ClientId ?? throw new InvalidOperationException("ClientId required for client secret authentication"),
                settings.ClientSecret ?? throw new InvalidOperationException("ClientSecret required for client secret authentication")),
            _ => throw new InvalidOperationException($"Unsupported auth type: {settings.AuthType}")
        };

        // If using certificate auth, create a separate credential and replace
        if (settings.AuthType == AuthType.Certificate)
        {
            if (string.IsNullOrWhiteSpace(settings.CertificatePath))
            {
                AnsiConsole.MarkupLine("[red]Error:[/] Certificate path is required for certificate authentication.");
                return 1;
            }
            if (!File.Exists(settings.CertificatePath))
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] Certificate not found: {settings.CertificatePath}");
                return 1;
            }

            var cert = new X509Certificate2(settings.CertificatePath, settings.CertificatePassword ?? string.Empty);
            credential = new ClientCertificateCredential(
                settings.TenantId ?? throw new InvalidOperationException("TenantId required for certificate authentication"),
                settings.ClientId ?? throw new InvalidOperationException("ClientId required for certificate authentication"),
                cert);
        }

        try
        {
            Uri vaultUri = new(settings.VaultUri);
            var certificateClient = new CertificateClient(vaultUri, credential);

            // Retrieve certificate from Key Vault
            var certWithPolicy = certificateClient.GetCertificateAsync(settings.KeyName).GetAwaiter().GetResult();
            var x509 = X509CertificateLoader.LoadCertificate(certWithPolicy.Value.Cer);

            var rsa = RSAFactory.Create(credential, certWithPolicy.Value.KeyId, x509);
            using var signer = new RdpSigner(x509, rsa);

            foreach (var file in settings.Files)
            {
                if (!File.Exists(file))
                {
                    AnsiConsole.MarkupLine($"[yellow]Warning:[/] File not found, skipping: {file}");
                    continue;
                }
                var output = Path.ChangeExtension(file, ".signed.rdp");
                signer.SignFile(file, output);
                AnsiConsole.MarkupLine($"Signed [green]{file}[/] -> [green]{output}[/]");
            }

            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return 1;
        }
    }
}
