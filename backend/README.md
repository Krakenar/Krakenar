# 🐙 Krakenar by Logitar

Krakenar is a tool suite aiming at handling non-business software requirements, allowing developers to focus on real, domain business requirements.

## Environment Variables

All the following environment variables are optional.

- `CACHING_ACTOR_LIFETIME`: the lifetime of cached actors. A string representing a [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan?view=net-9.0), ex.: `3.00:00:00` (3 days) or `00:15:00` (15 minutes).
- `DATABASE_PROVIDER`: the database provider to use. Its value should be one of the `DatabaseProvider` enumeration value.
- `DEFAULT_LOCALE`: the default locale code of the system. A string representing a [CultureInfo](https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo?view=net-9.0), ex.: `en` (English) or `fr-CA` (Canadian French).
- `DEFAULT_PASSWORD`: the default password of the admin user, ex.: `P@s$W0rD`.
- `DEFAULT_USERNAME`: the default username of the admin user, ex.: `admin`.
- `ENCRYPTION_KEY`: the encryption key used by the platform. It should be 32-characters long (256 bits), including lowercase and uppercase letters, digits and special characters as well.
- `EXPOSE_ERROR_DETAIL`: a boolean value indicating whether or not expose detail for `500 Internal Server Error`. Should not be enabled in Production environment for security purposes.
- `PASSWORDS_PBKDF2_ALGORITHM`: the hashing algorithm for PBKDF2 passwords. Defaults to `HMACSHA256`.
- `PASSWORDS_PBKDF2_HASH_LENGTH`: the hash length (in bytes) for PBKDF2. When not specified, will defaut to salt length.
- `PASSWORDS_PBKDF2_ITERATIONS`: the hashing iterations for PBKDF2 passwords. Defaults to 600000.
- `PASSWORDS_PBKDF2_SALT_LENGTH`: the salt length (in bytes) for PBKDF2 passwords. Defaults to 32 (256 bits).
- `SQLCONNSTR_Krakenar`: the Microsoft SQL Server connection stirng.
