namespace Krakenar.Infrastructure;

public class EventSerializer : Logitar.EventSourcing.Infrastructure.EventSerializer
{
  protected override void RegisterConverters()
  {
    base.RegisterConverters();

    // TODO(fpion): register converters
  }
}
