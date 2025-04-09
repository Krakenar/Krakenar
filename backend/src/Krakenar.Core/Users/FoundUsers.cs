namespace Krakenar.Core.Users;

public record FoundUsers(User? ById, User? ByUniqueName, User? ByEmailAddress, User? ByCustomIdentifier);
