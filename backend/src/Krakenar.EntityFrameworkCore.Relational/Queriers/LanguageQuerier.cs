using Krakenar.Core.Localization;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class LanguageQuerier : ILanguageQuerier // TODO(fpion): implement
{
  public virtual Task<LanguageId?> FindIdAsync(Locale locale, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
