using FluentValidation;
using Krakenar.Contracts.Dictionaries;

namespace Krakenar.Core.Dictionaries.Validators;

public class DictionaryEntryValidator : AbstractValidator<DictionaryEntry>
{
  public DictionaryEntryValidator()
  {
    RuleFor(x => x.Key).Identifier();
  }
}
