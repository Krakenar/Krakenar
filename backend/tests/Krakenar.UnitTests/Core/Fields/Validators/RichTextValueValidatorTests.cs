using Krakenar.Core.Fields.Settings;
using Logitar.Security.Cryptography;
using MediaTypeNames = System.Net.Mime.MediaTypeNames;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Krakenar.Core.Fields.Validators;

[Trait(Traits.Category, Categories.Unit)]
public class RichTextValueValidatorTests
{
  private const string PropertyName = "Contents";

  private readonly CancellationToken _cancellationToken = default;

  private readonly RichTextSettings _settings = new(MediaTypeNames.Text.Plain, minimumLength: 10, maximumLength: 1000);
  private readonly RichTextValueValidator _validator;

  public RichTextValueValidatorTests()
  {
    _validator = new(_settings);
  }

  [Fact(DisplayName = "Validation should fail when the value is too long.")]
  public async Task Given_TooLong_When_ValidateAsync_Then_FailureResult()
  {
    string value = RandomStringGenerator.GetString(9999);
    FieldValue fieldValue = new(value);
    ValidationResult result = await _validator.ValidateAsync(fieldValue, PropertyName, _cancellationToken);
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.ErrorMessage == "The length of the value may not exceed 1000 characters."
      && e.AttemptedValue.Equals(value) && e.PropertyName == PropertyName && HasProperty(e.CustomState, "MaximumLength", _settings.MaximumLength));
  }

  [Fact(DisplayName = "Validation should fail when the value is too short.")]
  public async Task Given_TooShort_When_ValidateAsync_Then_FailureResult()
  {
    string value = RandomStringGenerator.GetString(9);
    FieldValue fieldValue = new(value);
    ValidationResult result = await _validator.ValidateAsync(fieldValue, PropertyName, _cancellationToken);
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.ErrorCode == "MinimumLengthValidator" && e.ErrorMessage == "The length of the value must be at least 10 characters."
      && e.AttemptedValue.Equals(value) && e.PropertyName == PropertyName && HasProperty(e.CustomState, "MinimumLength", _settings.MinimumLength));
  }

  [Fact(DisplayName = "Validation should succeed when the value is valid.")]
  public async Task Given_ValidValue_When_ValidateAsync_Then_SuccessResult()
  {
    string value = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer ac ex ac lacus hendrerit lacinia. Pellentesque velit urna, suscipit a mollis vitae, auctor vitae diam. Mauris scelerisque tellus vitae tellus laoreet vehicula. Quisque placerat sed diam sed blandit. Duis bibendum, mauris quis cursus venenatis, turpis felis ullamcorper arcu, sit amet iaculis orci felis eget nunc. Nullam vel sapien ut felis tincidunt cursus id ac orci. Integer in ligula vel nibh interdum malesuada. Curabitur vitae massa neque.";
    FieldValue fieldValue = new(value);
    ValidationResult result = await _validator.ValidateAsync(fieldValue, PropertyName, _cancellationToken);
    Assert.True(result.IsValid);
  }

  private static bool HasProperty(object instance, string propertyName, object? propertyValue)
  {
    PropertyInfo? property = instance.GetType().GetProperty(propertyName);
    Assert.NotNull(property);

    object? value = property.GetValue(instance);
    return propertyValue == null ? value == null : propertyValue.Equals(value);
  }
}
