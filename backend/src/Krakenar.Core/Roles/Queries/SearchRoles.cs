using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Search;
using Logitar.CQRS;
using RoleDto = Krakenar.Contracts.Roles.Role;

namespace Krakenar.Core.Roles.Queries;

public record SearchRoles(SearchRolesPayload Payload) : IQuery<SearchResults<RoleDto>>;

public class SearchRolesHandler : IQueryHandler<SearchRoles, SearchResults<RoleDto>>
{
  protected virtual IRoleQuerier RoleQuerier { get; }

  public SearchRolesHandler(IRoleQuerier roleQuerier)
  {
    RoleQuerier = roleQuerier;
  }

  public virtual async Task<SearchResults<RoleDto>> HandleAsync(SearchRoles query, CancellationToken cancellationToken)
  {
    return await RoleQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
