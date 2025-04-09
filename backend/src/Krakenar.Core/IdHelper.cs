using Krakenar.Core.Realms;
using Logitar;
using Logitar.EventSourcing;

namespace Krakenar.Core;

public static class IdHelper
{
  private const char EntitySeparator = ':';
  private const char Separator = '|';

  public static StreamId Construct(string entityType, Guid entityId, RealmId? realmId = null)
  {
    string entity = string.Join(EntitySeparator, entityType, Convert.ToBase64String(entityId.ToByteArray()).ToUriSafeBase64());
    return new StreamId(realmId.HasValue ? string.Join(Separator, realmId.Value, entity) : entity);
  }

  public static Tuple<Guid, RealmId?> Deconstruct(StreamId streamId, string expectedType)
  {
    string[] values = streamId.Value.Split(Separator);
    if (values.Length < 1 || values.Length > 2)
    {
      throw new ArgumentException($"The value '{streamId}' is not a valid aggregate identifier.", nameof(streamId));
    }

    string[] entity = values.Last().Split(EntitySeparator);
    if (entity.Length != 2)
    {
      throw new ArgumentException($"The value '{values.Last()}' is not a valid entity.", nameof(streamId));
    }

    if (entity.First() != expectedType)
    {
      throw new ArgumentException($"The entity type '{entity.First()}' was not expected ({expectedType}).", nameof(streamId));
    }
    Guid entityId = new(Convert.FromBase64String(entity.Last().FromUriSafeBase64()));

    RealmId? realmId = values.Length == 2 ? new(values.First()) : null;

    return Tuple.Create(entityId, realmId);
  }
}
