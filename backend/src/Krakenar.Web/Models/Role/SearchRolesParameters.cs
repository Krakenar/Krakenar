using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Search;
using Krakenar.Web.Models.Search;

namespace Krakenar.Web.Models.Role;

public record SearchRolesParameters : SearchParameters
{
  public virtual SearchRolesPayload ToPayload()
  {
    SearchRolesPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out RoleSort field))
      {
        payload.Sort.Add(new RoleSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
