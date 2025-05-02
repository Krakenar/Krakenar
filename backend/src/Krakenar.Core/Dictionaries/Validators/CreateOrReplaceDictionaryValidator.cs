using FluentValidation;
using Krakenar.Contracts.Dictionaries;

namespace Krakenar.Core.Dictionaries.Validators;

public class CreateOrReplaceDictionaryValidator : AbstractValidator<CreateOrReplaceDictionaryPayload>
{
  public CreateOrReplaceDictionaryValidator()
  {
    RuleFor(x => x.Language).NotEmpty();

    RuleForEach(x => x.Entries).SetValidator(new DictionaryEntryValidator());
  }
}
