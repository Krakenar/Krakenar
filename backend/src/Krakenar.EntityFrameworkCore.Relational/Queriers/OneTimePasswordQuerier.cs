using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Realms;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.Passwords;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using OneTimePassword = Krakenar.Core.Passwords.OneTimePassword;
using OneTimePasswordDto = Krakenar.Contracts.Passwords.OneTimePassword;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class OneTimePasswordQuerier : IOneTimePasswordQuerier
{
  protected virtual IActorService ActorService { get; }
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual DbSet<Entities.OneTimePassword> OneTimePasswords { get; }
  protected virtual ISqlHelper SqlHelper { get; }

  public OneTimePasswordQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenarContext context, ISqlHelper sqlHelper)
  {
    ActorService = actorService;
    ApplicationContext = applicationContext;
    OneTimePasswords = context.OneTimePasswords;
    SqlHelper = sqlHelper;
  }

  public virtual async Task<OneTimePasswordDto> ReadAsync(OneTimePassword oneTimePassword, CancellationToken cancellationToken)
  {
    return await ReadAsync(oneTimePassword.Id, cancellationToken) ?? throw new InvalidOperationException($"The One-Time Password (OTP) entity 'StreamId={oneTimePassword.Id}' could not be found.");
  }
  public virtual async Task<OneTimePasswordDto?> ReadAsync(OneTimePasswordId id, CancellationToken cancellationToken)
  {
    if (id.RealmId != ApplicationContext.RealmId)
    {
      throw new NotSupportedException();
    }

    return await ReadAsync(id.EntityId, cancellationToken);
  }
  public virtual async Task<OneTimePasswordDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Entities.OneTimePassword? oneTimePassword = await OneTimePasswords.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .Include(x => x.User)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return oneTimePassword is null ? null : await MapAsync(oneTimePassword, cancellationToken);
  }

  protected virtual async Task<OneTimePasswordDto> MapAsync(Entities.OneTimePassword oneTimePassword, CancellationToken cancellationToken)
  {
    return (await MapAsync([oneTimePassword], cancellationToken)).Single();
  }
  protected virtual async Task<IReadOnlyCollection<OneTimePasswordDto>> MapAsync(IEnumerable<Entities.OneTimePassword> oneTimePasswords, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = oneTimePasswords.SelectMany(oneTimePassword => oneTimePassword.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await ActorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    Realm? realm = ApplicationContext.Realm;
    return oneTimePasswords.Select(oneTimePassword => mapper.ToOneTimePassword(oneTimePassword, realm)).ToList().AsReadOnly();
  }
}
