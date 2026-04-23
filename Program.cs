using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Keys;
using NullOps.RdpSigner;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using RSAKeyVaultProvider;

if (args.Length < 3)
{
    Console.Error.WriteLine("Usage: rdpsign <vaulturi> <keyName> <path1> [<path2> …]");
    return;
}

Uri vaultUri = new(args[0]);
string keyName = args[1];
DefaultAzureCredential credential = new();

// Get the certificate from Key Vault
var certificateClient = new CertificateClient(vaultUri, credential);
var certificateWithPolicy = await certificateClient.GetCertificateAsync(keyName).ConfigureAwait(false);
var certBytes = certificateWithPolicy.Value.Cer;
var x509 = X509CertificateLoader.LoadCertificate(certBytes);

var rsa = RSAFactory.Create(credential, certificateWithPolicy.Value.KeyId, x509);

using var signer = new RdpSigner(rsa);
foreach (var file in args[2..])
{
    signer.SignFile(file, Path.ChangeExtension(file, ".signed.rdp"));
}