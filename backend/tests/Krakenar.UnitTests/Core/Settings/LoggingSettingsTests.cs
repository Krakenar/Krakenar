using FluentValidation;
using Krakenar.Contracts.Logging;
using LoggingSettingsDto = Krakenar.Contracts.Logging.LoggingSettings;

namespace Krakenar.Core.Settings;

[Trait(Traits.Category, Categories.Unit)]
public class LoggingSettingsTests
{
  [Fact(DisplayName = "It should construct the correct instance from another instance.")]
  public void Given_Instance_When_ctor_Then_Constructed()
  {
    LoggingSettingsDto instance = new()
    {
      Extent = LoggingExtent.ActivityOnly,
      OnlyErrors = true
    };

    LoggingSettings settings = new(instance);

    Assert.Equal(instance.Extent, settings.Extent);
    Assert.Equal(instance.OnlyErrors, settings.OnlyErrors);
  }

  [Fact(DisplayName = "It should construct the correct instance from arguments.")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    LoggingSettings settings = new();
    Assert.Equal(LoggingExtent.ActivityOnly, settings.Extent);
    Assert.False(settings.OnlyErrors);

    LoggingExtent extent = LoggingExtent.Full;
    bool onlyError = true;
    settings = new(extent, onlyError);
    Assert.Equal(extent, settings.Extent);
    Assert.Equal(onlyError, settings.OnlyErrors);
  }

  [Fact(DisplayName = "It should throw ValidationException when the arguments are not valid.")]
  public void Given_Invalid_When_ctor_Then_ValidationException()
  {
    var exception = Assert.Throws<ValidationException>(() => new LoggingSettings(LoggingExtent.None, onlyErrors: true));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.ErrorCode == "EqualValidator" && e.PropertyName == "OnlyErrors");
  }
}
