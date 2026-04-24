# NullOps.RDPSigner

A C# implementation of rdpsign.exe that uses Azure Key Vault for code signing.

This .Net port is based off the amazing work done by Norbert Federa at https://github.com/nfedera/rdpsign

## Prerequisites

- An active Azure login (e.g., `az login`) is required for authentication via `DefaultAzureCredential`
- A certificate with a corresponding key stored in Azure Key Vault

## Usage

```cmd
rdpsign <vaulturi> <keyName> <path1> [<path2> …]
```

- `vaulturi` — Azure Key Vault URI (e.g., `https://myvault.vault.azure.net/`)
- `keyName` — name of the certificate/key in Key Vault
- `path1…` — one or more `.rdp` files to sign

Each input file is signed and written to `<filename>.signed.rdp`.

## Required Key Vault Permissions

The identity used (from `az login` or otherwise) needs the following Key Vault permissions:

- **Keys**: `Get`, `Sign`
- **Certificates**: `Get`

These can be granted via Azure Portal, CLI, or PowerShell using access policies or RBAC.
