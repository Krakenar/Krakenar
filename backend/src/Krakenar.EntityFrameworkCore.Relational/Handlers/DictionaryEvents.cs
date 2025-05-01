using Krakenar.Core;
using Krakenar.Core.Dictionaries.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DictionaryEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Dictionary;
using DictionaryEntry = Krakenar.EntityFrameworkCore.Relational.Entities.DictionaryEntry;
using LanguageEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Language;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers;

public class DictionaryEvents : IEventHandler<DictionaryCreated>, IEventHandler<DictionaryDeleted>, IEventHandler<DictionaryLanguageChanged>, IEventHandler<DictionaryUpdated>
{
  protected virtual KrakenarContext Context { get; }
  protected virtual ILogger<DictionaryEvents> Logger { get; }

  public DictionaryEvents(KrakenarContext context, ILogger<DictionaryEvents> logger)
  {
    Context = context;
    Logger = logger;
  }

  public virtual async Task HandleAsync(DictionaryCreated @event, CancellationToken cancellationToken)
  {
    DictionaryEntity? dictionary = await Context.Dictionaries.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (dictionary is null)
    {
      LanguageEntity language = await Context.Languages
        .SingleOrDefaultAsync(x => x.StreamId == @event.LanguageId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The language entity 'StreamId={@event.LanguageId}' could not be found.");

      dictionary = new DictionaryEntity(language, @event);

      Context.Dictionaries.Add(dictionary);

      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
    else
    {
      Logger.LogUnexpectedVersion(@event, dictionary);
    }
  }

  public virtual async Task HandleAsync(DictionaryDeleted @event, CancellationToken cancellationToken)
  {
    DictionaryEntity? dictionary = await Context.Dictionaries.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (dictionary is null)
    {
      Logger.LogUnexpectedVersion(@event);
    }
    else
    {
      Context.Dictionaries.Remove(dictionary);

      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
  }

  public virtual async Task HandleAsync(DictionaryLanguageChanged @event, CancellationToken cancellationToken)
  {
    DictionaryEntity? dictionary = await Context.Dictionaries.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (dictionary is null || dictionary.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event);
      return;
    }

    LanguageEntity language = await Context.Languages
      .SingleOrDefaultAsync(x => x.StreamId == @event.LanguageId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The language entity 'StreamId={@event.LanguageId}' could not be found.");

    dictionary.SetLanguage(language, @event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(DictionaryUpdated @event, CancellationToken cancellationToken)
  {
    DictionaryEntity? dictionary = await Context.Dictionaries
      .Include(x => x.Entries)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (dictionary is null || dictionary.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event);
      return;
    }

    Dictionary<string, DictionaryEntry> entries = dictionary.Entries.ToDictionary(x => x.Key, x => x);
    foreach (KeyValuePair<Identifier, string?> entry in @event.Entries)
    {
      if (entries.TryGetValue(entry.Key.Value, out DictionaryEntry? entity))
      {
        if (entry.Value is null)
        {
          Context.DictionaryEntries.Remove(entity);
        }
        else
        {
          entity.Value = entry.Value;
        }
      }
      else if (entry.Value is not null)
      {
        entity = new DictionaryEntry(dictionary, entry.Key, entry.Value);
        dictionary.Entries.Add(entity);
      }
    }

    dictionary.Update(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }
}
