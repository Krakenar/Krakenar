using Krakenar.Core.Fields.Settings;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Krakenar.Core.Fields.Validators;

[Trait(Traits.Category, Categories.Unit)]
public class BooleanValueValidatorTests
{
  private const string PropertyName = "IsFeatured";

  private readonly CancellationToken _cancellationToken = default;

  private readonly BooleanSettings _settings = new();
  private readonly BooleanValueValidator _validator;

  public BooleanValueValidatorTests()
  {
    _validator = new(_settings);
  }

  [Fact(DisplayName = "Validation should fail when the value could not be parsed.")]
  public async Task Given_NotParsed_When_ValidateAsync_Then_FailureResult()
  {
    FieldValue fieldValue = new("invalid");
    ValidationResult result = await _validator.ValidateAsync(fieldValue, PropertyName, _cancellationToken);
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.ErrorCode == "BooleanValueValidator" && e.ErrorMessage == "The value is not a valid boolean."
      && e.AttemptedValue.Equals(fieldValue.Value) && e.PropertyName == PropertyName);
  }

  [Theory(DisplayName = "Validation should succeed when the value is valid.")]
  [InlineData("false")]
  [InlineData("true")]
  [InlineData(" FALSE ")]
  [InlineData(" TRue  ")]
  public async Task Given_ValidValue_When_ValidateAsync_Then_SuccessResult(string value)
  {
    FieldValue fieldValue = new(value);
    ValidationResult result = await _validator.ValidateAsync(fieldValue, PropertyName, _cancellationToken);
    Assert.True(result.IsValid);
  }
}
