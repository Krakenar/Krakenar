using FluentValidation;
using Krakenar.Contracts;
using Logitar.CQRS;
using CustomIdentifierDto = Krakenar.Contracts.CustomIdentifier;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Users.Queries;

public record ReadUser(Guid? Id, string? UniqueName, CustomIdentifierDto? CustomIdentifier) : IQuery<UserDto?>;

/// <exception cref="TooManyResultsException{T}"></exception>
public class ReadUserHandler : IQueryHandler<ReadUser, UserDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IUserQuerier UserQuerier { get; }

  public ReadUserHandler(IApplicationContext applicationContext, IUserQuerier userQuerier)
  {
    ApplicationContext = applicationContext;
    UserQuerier = userQuerier;
  }

  public virtual async Task<UserDto?> HandleAsync(ReadUser query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, UserDto> users = new(capacity: 3);

    if (query.Id.HasValue)
    {
      UserDto? user = await UserQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (user is not null)
      {
        users[user.Id] = user;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      UserDto? user = await UserQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (user is not null)
      {
        users[user.Id] = user;
      }
      else if (ApplicationContext.RequireUniqueEmail)
      {
        try
        {
          Email email = new(query.UniqueName);
          IReadOnlyCollection<UserDto> usersByEmail = await UserQuerier.ReadAsync(email, cancellationToken);
          if (usersByEmail.Count == 1)
          {
            user = usersByEmail.Single();
            users[user.Id] = user;
          }
        }
        catch (ValidationException)
        {
        }
      }
    }

    if (query.CustomIdentifier is not null)
    {
      UserDto? user = await UserQuerier.ReadAsync(query.CustomIdentifier, cancellationToken);
      if (user is not null)
      {
        users[user.Id] = user;
      }
    }

    if (users.Count > 1)
    {
      throw TooManyResultsException<UserDto>.ExpectedSingle(users.Count);
    }

    return users.SingleOrDefault().Value;
  }
}
