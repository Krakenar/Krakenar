using Krakenar.Core.Realms;
using Krakenar.Core.Realms.Commands;
using Logitar.CQRS;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using RealmDto = Krakenar.Contracts.Realms.Realm;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers.Realms;

public class DeleteRealmHandler : ICommandHandler<DeleteRealm, RealmDto?>
{
  protected virtual EventContext Event { get; }
  protected virtual KrakenarContext Krakenar { get; }
  protected virtual IRealmQuerier RealmQuerier { get; }

  public DeleteRealmHandler(EventContext eventContext, KrakenarContext krakenarContext, IRealmQuerier realmQuerier)
  {
    Event = eventContext;
    Krakenar = krakenarContext;
    RealmQuerier = realmQuerier;
  }

  public virtual async Task<RealmDto?> HandleAsync(DeleteRealm command, CancellationToken cancellationToken)
  {
    RealmDto? realm = await RealmQuerier.ReadAsync(command.Id, cancellationToken);
    if (realm is null)
    {
      return null;
    }
    string streamId = new RealmId(realm.Id).Value;

    // Logging
    await Krakenar.Logs.Where(x => x.RealmId == streamId).ExecuteDeleteAsync(cancellationToken);

    // Contents
    await Krakenar.FieldTypes
      .Where(x => x.RelatedContentTypeId.HasValue)
      .ExecuteUpdateAsync(setters => setters
        .SetProperty(x => x.RelatedContentTypeId, x => null)
        .SetProperty(x => x.RelatedContentTypeUid, x => null), cancellationToken);
    await Krakenar.ContentLocales.Include(x => x.Content).Where(x => x.Content!.RealmUid == realm.Id).ExecuteDeleteAsync(cancellationToken);
    await Krakenar.Contents.Where(x => x.RealmUid == realm.Id).ExecuteDeleteAsync(cancellationToken);
    await Krakenar.ContentTypes.Where(x => x.RealmUid == realm.Id).ExecuteDeleteAsync(cancellationToken);
    await Krakenar.FieldTypes.Where(x => x.RealmUid == realm.Id).ExecuteDeleteAsync(cancellationToken);

    // Messaging
    await Krakenar.Messages.Where(x => x.RealmUid == realm.Id).ExecuteDeleteAsync(cancellationToken);
    await Krakenar.Templates.Where(x => x.RealmUid == realm.Id).ExecuteDeleteAsync(cancellationToken);
    await Krakenar.Senders.Where(x => x.RealmUid == realm.Id).ExecuteDeleteAsync(cancellationToken);

    // Localization
    await Krakenar.Dictionaries.Where(x => x.RealmUid == realm.Id).ExecuteDeleteAsync(cancellationToken);
    await Krakenar.Languages.Where(x => x.RealmUid == realm.Id).ExecuteDeleteAsync(cancellationToken);

    // Identity
    await Krakenar.Sessions.Where(x => x.RealmUid == realm.Id).ExecuteDeleteAsync(cancellationToken);
    await Krakenar.OneTimePasswords.Where(x => x.RealmUid == realm.Id).ExecuteDeleteAsync(cancellationToken);
    await Krakenar.Users.Where(x => x.RealmUid == realm.Id).ExecuteDeleteAsync(cancellationToken);
    await Krakenar.ApiKeys.Where(x => x.RealmUid == realm.Id).ExecuteDeleteAsync(cancellationToken);
    await Krakenar.Roles.Where(x => x.RealmUid == realm.Id).ExecuteDeleteAsync(cancellationToken);
    await Krakenar.CustomAttributes.Where(x => x.Entity.StartsWith(streamId)).ExecuteDeleteAsync(cancellationToken);
    await Krakenar.Actors.Where(x => x.RealmUid == realm.Id).ExecuteDeleteAsync(cancellationToken);

    // EventSourcing
    await Event.Streams.Where(x => x.Id.StartsWith(streamId)).ExecuteDeleteAsync(cancellationToken);

    await Krakenar.Realms.Where(x => x.Id == realm.Id).ExecuteDeleteAsync(cancellationToken);

    return realm;
  }
}
