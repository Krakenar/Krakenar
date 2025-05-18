using FluentValidation.Results;
using Krakenar.Contracts.Fields.Settings;

namespace Krakenar.Core.Fields.Validators;

public class TagsValueValidator : IFieldValueValidator
{
  protected virtual ITagsSettings Settings { get; }

  public TagsValueValidator(ITagsSettings settings)
  {
    Settings = settings;
  }

  public Task<ValidationResult> ValidateAsync(FieldValue fieldValue, string propertyName, CancellationToken cancellationToken)
  {
    List<ValidationFailure> failures = new(capacity: 1);

    string value = fieldValue.Value;
    IReadOnlyCollection<string>? tags = null;
    try
    {
      tags = JsonSerializer.Deserialize<IReadOnlyCollection<string>>(value);
    }
    catch (Exception)
    {
    }
    if (tags is null || tags.Count < 1)
    {
      ValidationFailure failure = new(propertyName, "The value must be a JSON-serialized non-empty string array.", value)
      {
        ErrorCode = nameof(TagsValueValidator)
      };
      failures.Add(failure);
    }

    return Task.FromResult(new ValidationResult(failures));
  }
}
