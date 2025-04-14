using Krakenar.Contracts.Search;
using Krakenar.Contracts.Users;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Users.Queries;

public record SearchUsers(SearchUsersPayload Payload) : IQuery<SearchResults<UserDto>>;

public class SearchUsersHandler : IQueryHandler<SearchUsers, SearchResults<UserDto>>
{
  protected virtual IUserQuerier UserQuerier { get; }

  public SearchUsersHandler(IUserQuerier userQuerier)
  {
    UserQuerier = userQuerier;
  }

  public virtual async Task<SearchResults<UserDto>> HandleAsync(SearchUsers query, CancellationToken cancellationToken)
  {
    return await UserQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
