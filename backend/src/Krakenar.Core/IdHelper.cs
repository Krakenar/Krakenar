using Krakenar.Core.Realms;
using Logitar;
using Logitar.EventSourcing;

namespace Krakenar.Core;

public static class IdHelper
{
  public const char ComponentSeparator = ':';
  public const char Separator = '|';

  public static StreamId Construct(RealmId? realmId, string entityType, Guid entityId)
  {
    string components = string.Join(ComponentSeparator, entityType, Convert.ToBase64String(entityId.ToByteArray()).ToUriSafeBase64());
    return new StreamId(realmId.HasValue ? string.Join(Separator, realmId, components) : components);
  }
  public static Tuple<RealmId?, Guid> Deconstruct(StreamId streamId, string expectedType)
  {
    string[] values = streamId.Value.Split(Separator);
    if (values.Length < 1 || values.Length > 2)
    {
      throw new ArgumentException($"The value '{streamId}' is not a valid identifier.", nameof(streamId));
    }

    string[] components = values.Last().Split(ComponentSeparator);
    if (components.Length != 2)
    {
      throw new ArgumentException($"The entity component in '{streamId}' is not valid.", nameof(streamId));
    }
    else if (components.First() != expectedType)
    {
      throw new ArgumentException($"The entity type '{components.First()}' was not expected '{expectedType}'.", nameof(streamId));
    }

    RealmId? realmId = values.Length == 2 ? new(values.First()) : null;
    Guid entityId = new(Convert.FromBase64String(components.Last().FromUriSafeBase64()));
    return Tuple.Create(realmId, entityId);
  }
}
