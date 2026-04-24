# NullOps.RDPSigner

A C# implementation of `rdpsign.exe` that uses Azure Key Vault for code signing.

This .NET port is based off the amazing work done by Norbert Federa at https://github.com/nfedera/rdpsign.

## Prerequisites

- An active Azure login (e.g., `az login`) is required for authentication via `DefaultAzureCredential` or other methods.
- A certificate with a corresponding key stored in Azure Key Vault.

## Usage

```
rdpsign --vault <vaulturi> --key <keyName> [options] <file1> [<file2> …]
```

### Options

| Option | Description |
|--------|-------------|
| `-v, --vault <URI>` | Azure Key Vault URI (required) |
| `-k, --key <NAME>` | Certificate/key name in Key Vault (required) |
| `--auth <TYPE>` | Authentication method: `Default`, `AzureCli`, `VisualStudio`, `VisualStudioCode`, `InteractiveBrowser`, `ManagedIdentity`, `Environment`, `ClientSecret`, `Certificate` (default: `Default`) |
| `--client-id <ID>` | Client ID for service principal (required for `ClientSecret` auth) |
| `--client-secret <SECRET>` | Client secret for service principal (required for `ClientSecret` auth) |
| `--tenant-id <ID>` | Tenant ID for service principal (required for `ClientSecret` and `Certificate` auth) |
| `--cert <PATH>` | Path to a PFX certificate file (required for `Certificate` auth) |
| `--cert-pass <PASSWORD>` | Password for the certificate file (optional) |

Each input file is signed and written to `<filename>.signed.rdp`.

## Authentication

By default, the tool uses `DefaultAzureCredential`, which tries multiple authentication methods (environment variables, managed identity, Visual Studio, Azure CLI, etc.). Use `--auth` to select a specific method.

Most `--auth` options rely on an existing Azure login. For example, when using `Default` or `AzureCli`, you should run `az login` first. For `ClientSecret` or `Certificate` you need the corresponding service principal credentials.

## Required Key Vault Permissions

The identity you use needs the following Key Vault permissions:

- **Keys**: `Get`, `Sign`
- **Certificates**: `Get`

These can be granted via Azure Portal, CLI, or PowerShell using access policies or RBAC.

## Examples

Sign files using the logged-in Azure CLI user:

```cmd
rdpsign --vault https://myvault.vault.azure.net/ --key mycert file1.rdp file2.rdp
```

Use a service principal:

```cmd
rdpsign --vault https://myvault.vault.azure.net/ --key mycert \
  --auth ClientSecret \
  --tenant-id <tenant> --client-id <id> --client-secret <secret> \
  file.rdp
```

Use a local certificate file for authentication:

```cmd
rdpsign --vault https://myvault.vault.azure.net/ --key mycert \
  --auth Certificate \
  --tenant-id <tenant> --client-id <id> \
  --cert /path/to/cert.pfx --cert-pass <password> \
  file.rdp
```