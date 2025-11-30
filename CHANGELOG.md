# Changelog

All notable changes to the I-Synergy Framework will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed
- **[BREAKING] ISynergy.Framework.UI.UWP**: Minimum Windows version raised from 10.0.17763.0 (Version 1809) to 10.0.19041.0 (Version 2004)
  - **Location**: `src/ISynergy.Framework.UI.UWP/ISynergy.Framework.UI.UWP.csproj` line 4
  - **Old Value**: `<TargetPlatformMinVersion>10.0.17763.0</TargetPlatformMinVersion>`
  - **New Value**: `<TargetPlatformMinVersion>10.0.19041.0</TargetPlatformMinVersion>`
  - **Affected Scenarios**: Any UWP application targeting Windows 10 Version 1809 must upgrade to Version 2004 or later
  - **Technical Justification**: Windows 10 Version 2004 provides access to modern Windows SDK APIs and tooling features required for optimal compatibility with .NET 10, including enhanced platform capabilities and improved build toolchain support
  - **Migration Path**: See [UWP Migration Guide](src/ISynergy.Framework.UI.UWP/readme.md#migration-guide) in the package README

### Documentation
- Updated [src/ISynergy.Framework.UI.UWP/readme.md](src/ISynergy.Framework.UI.UWP/readme.md) with breaking change notice, migration guide, and technical justification

---

## Previous Releases

See individual package changelogs and release notes for version history prior to this centralized changelog.
