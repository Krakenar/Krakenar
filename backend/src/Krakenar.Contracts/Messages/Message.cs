using Krakenar.Contracts.Realms;

namespace Krakenar.Contracts.Messages;

public class Message : Aggregate
{
  public Realm? Realm { get; set; }

  // TODO(fpion): implement
}
