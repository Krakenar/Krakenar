using FluentValidation;
using Krakenar.Contracts.Dictionaries;

namespace Krakenar.Core.Dictionaries.Validators;

public class UpdateDictionaryValidator : AbstractValidator<UpdateDictionaryPayload>
{
  public UpdateDictionaryValidator()
  {
    RuleForEach(x => x.Entries).SetValidator(new DictionaryEntryValidator());
  }
}
