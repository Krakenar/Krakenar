# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Unreleased

Nothing yet.

## [2.0.0] - 2026-01-24

This release replaces several internal infrastructure components with dedicated Logitar libraries, updates configuration behavior, and fixes issues related to the .NET 10 upgrade, dependency updates, messaging, and Swagger security.

### Changed

- Internal `EventBus` implementation replaced by [Logitar.EventSourcing](https://github.com/Logitar/EventSourcing/blob/main/lib/Logitar.EventSourcing.Infrastructure/EventBus.cs).
- Internal CQRS implementation replaced by [Logitar.CQRS](https://github.com/Logitar/CQRS).
- Internal `EnvironmentHelper` replaced by [Logitar.NET](https://github.com/Logitar/Logitar.NET/blob/main/src/Logitar/EnvironmentHelper.cs).
- Replaced `SilentAuthenticatedEvent` setting by `EnableAuthenticatedEventSourcing` (reversed behaviour).

### Fixed

- .NET10 upgrade.
- NuGet upgrades.
- New year upgrade (2026).
- Issues with message senders.
- Swagger security issues introduced by .NET10 upgrade.

## [1.1.0] - 2025-07-13

### Added

- Retry mechanism.
- `AUTHENTICATION_SILENT_AUTHENTICATED_EVENT` flag to remove authenticated events (`ApiKeyAuthenticated` and `UserAuthenticated`) from Event Sourcing.

## [1.0.1] - 2025-06-28

### Fixed

- Content publishing after draft saved.
- Implemented `LanguageFilter.IsDefault`.
- Content constructors.

## [1.0.0] - 2025-06-01

Initial release. 🚀

[unreleased]: https://github.com/Krakenar/Krakenar/compare/v2.0.0...HEAD
[2.0.0]: https://github.com/Krakenar/Krakenar/compare/v1.1.0...v2.0.0
[1.1.0]: https://github.com/Krakenar/Krakenar/compare/v1.0.1...v1.1.0
[1.0.1]: https://github.com/Krakenar/Krakenar/compare/v1.0.0...v1.0.1
[1.0.0]: https://github.com/Krakenar/Krakenar/compare/v0.1.0...v1.0.0
[0.1.0]: https://github.com/Krakenar/Krakenar/releases/tag/v0.1.0
