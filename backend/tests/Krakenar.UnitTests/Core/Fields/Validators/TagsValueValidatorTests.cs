using Krakenar.Core.Fields.Settings;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Krakenar.Core.Fields.Validators;

[Trait(Traits.Category, Categories.Unit)]
public class TagsValueValidatorTests
{
  private const string PropertyName = "Keywords";

  private readonly CancellationToken _cancellationToken = default;

  private readonly TagsSettings _settings = new();
  private readonly TagsValueValidator _validator;

  public TagsValueValidatorTests()
  {
    _validator = new(_settings);
  }

  [Theory(DisplayName = "Validation should fail when the value could not be parsed.")]
  [InlineData(" [] ")]
  [InlineData("invalid")]
  public async Task Given_NotParsed_When_ValidateAsync_Then_FailureResult(string value)
  {
    FieldValue fieldValue = new(value);
    ValidationResult result = await _validator.ValidateAsync(fieldValue, PropertyName, _cancellationToken);
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.ErrorCode == "TagsValueValidator" && e.ErrorMessage == "The value must be a JSON-serialized non-empty string array."
      && e.AttemptedValue.Equals(value.Trim()) && e.PropertyName == PropertyName);
  }

  [Fact(DisplayName = "Validation should succeed when the value is valid.")]
  public async Task Given_ValidValue_When_ValidateAsync_Then_SuccessResult()
  {
    FieldValue fieldValue = new(@"[""hello"", ""word""]");
    ValidationResult result = await _validator.ValidateAsync(fieldValue, PropertyName, _cancellationToken);
    Assert.True(result.IsValid);
  }
}
