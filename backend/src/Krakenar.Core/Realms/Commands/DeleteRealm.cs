using RealmDto = Krakenar.Contracts.Realms.Realm;

namespace Krakenar.Core.Realms.Commands;

public record DeleteRealm(Guid Id) : ICommand<RealmDto?>;
