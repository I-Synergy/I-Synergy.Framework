// ISynergy.Framework.Synchronization depends on Dotmim.Sync and MessagePack with
// ContractlessStandardResolver, both of which are not compatible with trimming or Native AOT.
//
// AOT warnings are propagated to consuming applications via [RequiresUnreferencedCode] and
// [RequiresDynamicCode] attributes on DefaultMessagePackSerializer, MessagePackSerializerFactory,
// and SyncSetupExtensions.WithTenantFilter.
//
// The IL2026 / IL3050 suppressions in the project file apply only to warnings originating
// from Dotmim.Sync internals that cannot be fixed within this library.
//
// Applications publishing with <PublishAot>true</PublishAot> should not use this library until
// Dotmim.Sync gains AOT support. Monitor https://github.com/Mimetis/Dotmim.Sync for updates.
