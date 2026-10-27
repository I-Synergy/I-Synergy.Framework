# WinUI 3 MSIX Packaging

Packaging, signing, CI/CD, and packaging troubleshooting. Load this when producing a distributable MSIX or fixing a package-install failure.
---


## MSIX Packaging

```powershell
# Build release
.\BuildAndRun.ps1 /p:Configuration=Release -SkipRun

# Generate dev certificate (one-time, matches manifest Publisher)
winapp cert generate --manifest .

# Trust certificate (one-time, requires admin)
winapp cert install ./devcert.pfx

# Package and sign
winapp package <build-output-dir> --cert ./devcert.pfx

# With timestamp (required for production — without it signature expires with cert)
winapp package <build-output-dir> --cert prod.pfx --timestamp http://timestamp.digicert.com

# Self-contained (bundles Windows App SDK runtime)
winapp package <build-output-dir> --cert ./devcert.pfx --self-contained
```

### Key Packaging Rules
- Publisher must match between certificate and manifest `Identity.Publisher` — use `winapp cert generate --manifest`
- `cert install` requires admin elevation
- Default PFX password is `password` — override with `--password`

### CI/CD (GitHub Actions)

```yaml
name: Build and Package
on: [push]
jobs:
  build:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v4
      - uses: microsoft/setup-WinAppCli@v0.1
      - name: Build
        run: dotnet build -c Release -p:Platform=x64
      - name: Package
        run: |
          winapp cert generate --if-exists skip --quiet
          winapp package ./bin/x64/Release/ --cert ./devcert.pfx --quiet
      - name: Upload artifact
        uses: actions/upload-artifact@v4
        with:
          name: msix-package
          path: "*.msix"
```

### Packaging Troubleshooting

| Error | Solution |
|-------|----------|
| Publisher mismatch | `winapp cert generate --manifest` |
| Certificate not trusted | `winapp cert install ./devcert.pfx` (admin) |
| appxmanifest.xml not found | `winapp init` or pass `--manifest <path>` |
| Package install failed | Trust cert first; remove stale: `Get-AppxPackage <name> \| Remove-AppxPackage` |
