using Logitar.EventSourcing;

namespace Krakenar.Core.Localization;

public interface ILanguageRepository
{
  Task<Language?> LoadAsync(LanguageId id, CancellationToken cancellationToken = default);
  Task<Language?> LoadAsync(LanguageId id, long? version, CancellationToken cancellationToken = default);

  Task SaveAsync(Language language, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Language> languages, CancellationToken cancellationToken = default);
}

public class LanguageRepository : Repository, ILanguageRepository
{
  public LanguageRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public virtual async Task<Language?> LoadAsync(LanguageId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public virtual async Task<Language?> LoadAsync(LanguageId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Language>(id.StreamId, version, cancellationToken);
  }

  public virtual async Task SaveAsync(Language language, CancellationToken cancellationToken)
  {
    await base.SaveAsync(language, cancellationToken);
  }
  public virtual async Task SaveAsync(IEnumerable<Language> languages, CancellationToken cancellationToken)
  {
    await base.SaveAsync(languages, cancellationToken);
  }
}
