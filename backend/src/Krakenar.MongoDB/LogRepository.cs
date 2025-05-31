using Krakenar.Core.Logging;
using MongoDB.Driver;
using CoreLog = Krakenar.Core.Logging.Log;

namespace Krakenar.MongoDB;

public class LogRepository : ILogRepository
{
  protected virtual IMongoCollection<Log> Logs { get; }
  protected virtual JsonSerializerOptions SerializerOptions { get; } = new();

  public LogRepository(IMongoDatabase database)
  {
    Logs = database.GetCollection<Log>("logs");

    SerializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  public virtual async Task SaveAsync(CoreLog log, CancellationToken cancellationToken)
  {
    Log entity = new(log, SerializerOptions);

    await Logs.InsertOneAsync(entity, new InsertOneOptions(), cancellationToken);
  }
}
