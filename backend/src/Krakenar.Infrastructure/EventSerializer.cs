using Krakenar.Infrastructure.Converters;

namespace Krakenar.Infrastructure;

public class EventSerializer : Logitar.EventSourcing.Infrastructure.EventSerializer
{
  protected override void RegisterConverters()
  {
    base.RegisterConverters();

    SerializerOptions.Converters.Add(new ConfigurationIdConverter());
    SerializerOptions.Converters.Add(new CustomIdentifierConverter());
    SerializerOptions.Converters.Add(new DescriptionConverter());
    SerializerOptions.Converters.Add(new DisplayNameConverter());
    SerializerOptions.Converters.Add(new GenderConverter());
    SerializerOptions.Converters.Add(new IdentifierConverter());
    SerializerOptions.Converters.Add(new LanguageIdConverter());
    SerializerOptions.Converters.Add(new LocaleConverter());
    SerializerOptions.Converters.Add(new PersonNameConverter());
    SerializerOptions.Converters.Add(new RealmIdConverter());
    SerializerOptions.Converters.Add(new RoleIdConverter());
    SerializerOptions.Converters.Add(new SecretConverter());
    SerializerOptions.Converters.Add(new SessionIdConverter());
    SerializerOptions.Converters.Add(new SlugConverter());
    SerializerOptions.Converters.Add(new TimeZoneConverter());
    SerializerOptions.Converters.Add(new UniqueNameConverter());
    SerializerOptions.Converters.Add(new UrlConverter());
    SerializerOptions.Converters.Add(new UserIdConverter());
  }
}
