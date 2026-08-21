---
name: mobile-release
description: Publishing .NET MAUI apps to the Apple App Store and Google Play. Use when building AAB/IPA release artifacts, signing, uploading via Google Play / App Store Connect, configuring release pipelines, or debugging store rejections.
---

# Mobile Release Skill

Expert for publishing .NET MAUI apps to the Apple App Store and Google Play, including CI/CD release pipelines and store-submission gotchas.

## When to Use

- Publishing a MAUI app to Google Play or the Apple App Store
- Configuring `AndroidPackageFormat` / `AndroidKeyStore` MSBuild properties
- Building release artifacts (`.aab`, `.ipa`, `.pkg`) locally or in CI
- Wiring the Google Play / App Store Connect upload steps
- Debugging store rejections (`ITMS-90332`, "draft app", locale errors)

## Android Publishing (AAB)

Set these MSBuild properties **UNCONDITIONALLY** — not just under a `Release` condition:

```xml
<PropertyGroup>
  <AndroidPackageFormat>aab</AndroidPackageFormat>
  <AndroidKeyStore>true</AndroidKeyStore>
</PropertyGroup>
```

If you scope them to `Release`, PR/Debug builds emit an `.apk` instead, and the upload step finds no AAB. Always publish with an explicit `-c Release`, then assert the artifact exists and exit non-zero if it doesn't:

```powershell
dotnet publish src/App.Mobile/App.Mobile.csproj -c Release -f net10.0-android
$AAB = Get-ChildItem -Recurse -Filter *.aab | Select-Object -First 1
if (-not $AAB) { Write-Error "No .aab produced"; exit 1 }
```

## Google Play Service Account

Pass the service-account JSON via an environment variable and read it in the script. **Never macro-expand a JSON secret inline** — it breaks quoting and leaks key fragments into logs via argparse errors.

```yaml
# Pass the secret by name; the value lives in the pipeline's secret store
env:
  GOOGLE_PLAY_SA_JSON: $(GOOGLE_PLAY_SA_JSON)
```

The upload script reads `$GOOGLE_PLAY_SA_JSON` from the environment and writes it to a temp file for the client — the secret never appears in the command line or logs.

**Deployment jobs need `- checkout: self` explicitly** — they do not checkout by default:

```yaml
jobs:
- deployment: Publish
  environment: production
  steps:
  - checkout: self        # required — deployment jobs do not checkout by default
  - script: ./publish-android.sh
    env:
      GOOGLE_PLAY_SA_JSON: $(GOOGLE_PLAY_SA_JSON)
```

**Draft app rejection:** an app that has never been published can be rejected on first track submission as a "draft app". Catch that rejection and retry with the track set as a draft:

```python
try:
    edit.tracks().update(package_name=pkg, track="production", body={"releases": [...]})
except HttpError as e:
    if "draft" in str(e):
        edit.tracks().update(package_name=pkg, track="production",
                             body={"releases": [...]}, status="draft")
```

**Locale filtering:** Play's Arabic locale is region-less `ar`, not `ar-SA`. Filter metadata dirs with `^[a-z]{2,3}(-[A-Za-z0-9]{2,8})?$` and skip non-locale dirs:

```bash
for dir in fastlane/metadata/android/*/; do
  lang=$(basename "$dir")
  [[ "$lang" =~ ^[a-z]{2,3}(-[A-Za-z0-9]{2,8})?$ ]] || continue
  # upload "$dir" for locale "$lang"
done
```

## Apple App Store (macCatalyst / iOS)

- iOS/macCatalyst Release builds are fully AOT — see `.ai/reference/aot-and-trimming.md`.
- Re-sign macCatalyst bundles with Hardened Runtime and only `allow-jit` + `allow-unsigned-executable-memory` entitlements (never `disable-library-validation`).
- Strip dangling XCFramework symlinks before packaging with a NUL-delimited `find` loop (never `xargs`), or the upload fails with `ITMS-90332`.

## Self-Hosted CI Agents

Always run `- checkout: self` + `clean: true` as the first step:

```yaml
steps:
- checkout: self
  clean: true
```

The default checkout only overwrites tracked files, so stale untracked outputs — e.g. a framework containing a dangling symlink — survive across runs and crash later steps. `clean: true` wipes untracked files.

## Secrets Policy

**Never paste `.p8` private keys, keystore material, or service-account JSON into a runbook or the repo.** Reference the pipeline secret variable name and put a placeholder in the doc:

```yaml
# CORRECT — secrets referenced by name
env:
  GOOGLE_PLAY_SA_JSON: $(GOOGLE_PLAY_SA_JSON)
  APP_STORE_P8: $(APP_STORE_API_KEY_P8)

# WRONG — never inline the actual key material
env:
  GOOGLE_PLAY_SA_JSON: '{ "type": "service_account", ... }'   # LEAK
```

## Checklist

- [ ] `AndroidPackageFormat=aab` + `AndroidKeyStore=true` set unconditionally (not Release-only)
- [ ] Publish uses `-c Release` and asserts a `.aab` exists (non-zero exit if missing)
- [ ] Service-account JSON passed via env var, never macro-expanded inline
- [ ] Deployment job has `- checkout: self`
- [ ] Self-hosted agents run `checkout: self` + `clean: true` first
- [ ] Arabic metadata uses region-less `ar`; non-locale dirs filtered out
- [ ] Draft-app rejection handled with a `status="draft"` retry
- [ ] No `.p8`/keystore/service-account material in the repo or runbooks
