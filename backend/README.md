# 🐙 Krakenar by Logitar

Krakenar is a tool suite aiming at handling non-business software requirements, allowing developers to focus on real, domain business requirements.

## Environment Variables

All the following environment variables are optional.

- `CACHING_ACTOR_LIFETIME`: the lifetime of cached actors. A string representing a [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan?view=net-9.0), ex.: `3.00:00:00` (3 days) or `00:15:00` (15 minutes).
- `DEFAULT_LOCALE`: the default locale code of the system. A string representing a [CultureInfo](https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo?view=net-9.0), ex.: `en` (English) or `fr-CA` (Canadian French).
- `DEFAULT_PASSWORD`: the default password of the admin user, ex.: `P@s$W0rD`.
- `DEFAULT_USERNAME`: the default username of the admin user, ex.: `admin`.
- `SQLCONNSTR_Krakenar`: the Microsoft SQL Server connection stirng.
