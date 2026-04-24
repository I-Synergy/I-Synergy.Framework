# AOT Compatibility — All Tiers

**Status:** DONE

**Goal:** Add `<IsAotCompatible>true</IsAotCompatible>` to every eligible library and resolve all IL trim/AOT warnings to 0.

## Completed (Tiers 1–3)

- [x] `ISynergy.Framework.Core`
- [x] `ISynergy.Framework.CQRS`
- [x] `ISynergy.Framework.MessageBus`
- [x] `ISynergy.Framework.MessageBus.Azure`
- [x] `ISynergy.Framework.MessageBus.RabbitMQ`
- [x] `ISynergy.Framework.OpenTelemetry`
- [x] `ISynergy.Framework.EventSourcing.EntityFramework`

## Tier 4 — Domain/utility (minimal reflection) ✅ COMPLETE

- [x] `ISynergy.Framework.EventSourcing`
- [x] `ISynergy.Framework.Financial`
- [x] `ISynergy.Framework.Geography`
- [x] `ISynergy.Framework.Mathematics`
- [x] `ISynergy.Framework.Physics`
- [x] `ISynergy.Framework.IO`
- [x] `ISynergy.Framework.Automations`
- [x] `ISynergy.Framework.Synchronization`
- [x] `ISynergy.Framework.Documents`

## Tier 5 — Infrastructure/backend ✅ COMPLETE

- [x] `ISynergy.Framework.EntityFramework`
- [x] `ISynergy.Framework.Mail`
- [x] `ISynergy.Framework.Mail.Microsoft365`
- [x] `ISynergy.Framework.Mail.SendGrid`
- [x] `ISynergy.Framework.Storage`
- [x] `ISynergy.Framework.Storage.Azure`
- [x] `ISynergy.Framework.Storage.S3`
- [x] `ISynergy.Framework.KeyVault`
- [x] `ISynergy.Framework.KeyVault.Azure`
- [x] `ISynergy.Framework.KeyVault.OpenBao`
- [x] `ISynergy.Framework.Monitoring`
- [x] `ISynergy.Framework.Monitoring.Client`
- [x] `ISynergy.Framework.OpenTelemetry.ApplicationInsights`
- [x] `ISynergy.Framework.OpenTelemetry.Sentry`
- [x] `ISynergy.Framework.AspNetCore`
- [x] `ISynergy.Framework.AspNetCore.Authentication`
- [x] `ISynergy.Framework.AspNetCore.Globalization`
- [x] `ISynergy.Framework.AspNetCore.Monitoring`
- [x] `ISynergy.Framework.AspNetCore.MultiTenancy`

## Tier 6 — MVVM/UI (reflection-heavy, may need partial AOT or suppressions) ✅ COMPLETE

- [x] `ISynergy.Framework.Mvvm`
- [x] `ISynergy.Framework.UI`
- [x] `ISynergy.Framework.UI.WPF` (note: 1–2 warnings remain in XAML-generated obj/*.g.cs files — unfixable)
- [x] `ISynergy.Framework.UI.WinUI` (note: 1 warning remains in CommunityToolkit XamlTypeInfo.g.cs — unfixable)
- [x] `ISynergy.Framework.UI.Maui`
- [x] `ISynergy.Framework.UI.Blazor`
- [x] `ISynergy.Framework.AspNetCore.Blazor`
- [x] `ISynergy.Framework.Documents.Syncfusion`
- [x] `ISynergy.Framework.Printer.Label`
- [x] `ISynergy.Framework.Printer.Label.Dymo`

## Skipped (compile-time tools, not runtime)

- `ISynergy.Framework.CQRS.SourceGenerator`
- `ISynergy.Framework.UI.SourceGenerator`
- `ISynergy.Framework.AspNetCore.Blazor.SourceGenerator`
