using Krakenar.Core.Localization;
using LanguageDto = Krakenar.Contracts.Localization.Language;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class LanguageQuerier : ILanguageQuerier // TODO(fpion): implement
{
  public virtual Task<Locale> FindDefaultPlatformLocaleAsync(CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public virtual Task<LanguageId?> FindIdAsync(Locale locale, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public virtual Task<LanguageDto> ReadAsync(Language language, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public virtual Task<LanguageDto?> ReadAsync(LanguageId id, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public virtual Task<LanguageDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public virtual Task<LanguageDto?> ReadAsync(string locale, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public virtual Task<LanguageDto> ReadDefaultAsync(CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
