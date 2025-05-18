using FluentValidation.Results;
using Krakenar.Contracts.Fields.Settings;
using Krakenar.Core.Contents;

namespace Krakenar.Core.Fields.Validators;

public class RelatedContentValueValidator : IFieldValueValidator
{
  protected virtual IContentQuerier ContentQuerier { get; }
  protected virtual IRelatedContentSettings Settings { get; }

  public RelatedContentValueValidator(IContentQuerier contentQuerier, IRelatedContentSettings settings)
  {
    ContentQuerier = contentQuerier;
    Settings = settings;
  }

  public async Task<ValidationResult> ValidateAsync(FieldValue fieldValue, string propertyName, CancellationToken cancellationToken)
  {
    string value = fieldValue.Value;
    IReadOnlyCollection<Guid>? contentIds = null;
    try
    {
      contentIds = JsonSerializer.Deserialize<IReadOnlyCollection<Guid>>(value);
    }
    catch (Exception)
    {
    }
    if (contentIds is null || contentIds.Count < 1)
    {
      ValidationFailure failure = new(propertyName, "The value must be a JSON-serialized non-empty content ID array.", value)
      {
        ErrorCode = nameof(RelatedContentValueValidator)
      };
      return new ValidationResult([failure]);
    }
    else if (contentIds.Count > 1 && !Settings.IsMultiple)
    {
      ValidationFailure failure = new(propertyName, "Only one content may be selected.", value)
      {
        ErrorCode = "MultipleValidator"
      };
      return new ValidationResult([failure]);
    }

    List<ValidationFailure> failures = new(capacity: contentIds.Count);

    IReadOnlyDictionary<Guid, Guid> contentTypeIds = await ContentQuerier.FindContentTypeIdsAsync(contentIds, cancellationToken);
    Guid expectedContentTypeId = Settings.ContentTypeId;
    foreach (Guid contentId in contentIds)
    {
      if (!contentTypeIds.TryGetValue(contentId, out Guid contentTypeId))
      {
        ValidationFailure failure = new(propertyName, "The content could not be found.", contentId)
        {
          ErrorCode = "ContentValidator"
        };
        failures.Add(failure);
      }
      else if (contentTypeId != expectedContentTypeId)
      {
        string errorMessage = $"The content type 'Id={contentTypeId}' does not match the expected content type 'Id={expectedContentTypeId}'.";
        ValidationFailure failure = new(propertyName, errorMessage, contentId)
        {
          CustomState = new
          {
            ExpectedContentTypeId = expectedContentTypeId,
            ActualContentTypeId = contentTypeId
          },
          ErrorCode = "ContentTypeValidator"
        };
        failures.Add(failure);
      }
    }

    return new ValidationResult(failures);
  }
}
