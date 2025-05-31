using Bogus;
using Krakenar.Contracts.Logging;
using Krakenar.Core.Users.Queries;
using Moq;

namespace Krakenar.Core.Logging;

public class LoggingServiceTests
{
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<ILogRepository> _logRepository = new();

  private readonly LoggingServiceMock _service;

  public LoggingServiceTests()
  {
    _service = new LoggingServiceMock(_applicationContext.Object, [_logRepository.Object]);
  }

  [Fact(DisplayName = "ShouldSaveLog: it should return false when logging extent is ActivityOnly.")]
  public void Given_ActivityOnly_When_ShouldSaveLog_Then_False()
  {
    LoggingSettings settings = new(LoggingExtent.ActivityOnly, onlyErrors: false);
    _applicationContext.SetupGet(x => x.LoggingSettings).Returns(settings);

    _service.Open(Guid.NewGuid().ToString(), "GET", "/api/users/abc123", _faker.Internet.Ip(), $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}");
    Assert.False(_service.ShouldSaveLog());
  }

  [Fact(DisplayName = "ShouldSaveLog: it should return false when logging extent is ActivityOnly without errors.")]
  public void Given_ActivityOnlyWithoutError_When_ShouldSaveLog_Then_False()
  {
    LoggingSettings settings = new(LoggingExtent.ActivityOnly, onlyErrors: true);
    _applicationContext.SetupGet(x => x.LoggingSettings).Returns(settings);

    _service.Open(Guid.NewGuid().ToString(), "GET", "/api/users/abc123", _faker.Internet.Ip(), $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}");
    _service.SetActivity(new ReadUser(null, null, null));
    Assert.False(_service.ShouldSaveLog());
  }

  [Fact(DisplayName = "ShouldSaveLog: it should return false when logging extent is Full without errors.")]
  public void Given_FullWithoutError_When_ShouldSaveLog_Then_False()
  {
    LoggingSettings settings = new(LoggingExtent.Full, onlyErrors: true);
    _applicationContext.SetupGet(x => x.LoggingSettings).Returns(settings);

    _service.Open(Guid.NewGuid().ToString(), "GET", "/api/users/abc123", _faker.Internet.Ip(), $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}");
    Assert.False(_service.ShouldSaveLog());
  }

  [Fact(DisplayName = "ShouldSaveLog: it should return false when logging extent is None.")]
  public void Given_None_When_ShouldSaveLog_Then_False()
  {
    LoggingSettings settings = new(LoggingExtent.None, onlyErrors: false);
    _applicationContext.SetupGet(x => x.LoggingSettings).Returns(settings);

    Assert.False(_service.ShouldSaveLog());
  }

  [Fact(DisplayName = "ShouldSaveLog: it should return true when logging extent is ActivityOnly.")]
  public void Given_ActivityOnly_When_ShouldSaveLog_Then_True()
  {
    LoggingSettings settings = new(LoggingExtent.ActivityOnly, onlyErrors: false);
    _applicationContext.SetupGet(x => x.LoggingSettings).Returns(settings);

    _service.Open(Guid.NewGuid().ToString(), "GET", "/api/users/abc123", _faker.Internet.Ip(), $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}");
    _service.SetActivity(new ReadUser(null, null, null));
    Assert.True(_service.ShouldSaveLog());
  }

  [Fact(DisplayName = "ShouldSaveLog: it should return true when logging extent is ActivityOnly with errors.")]
  public void Given_ActivityOnlyWithError_When_ShouldSaveLog_Then_True()
  {
    LoggingSettings settings = new(LoggingExtent.ActivityOnly, onlyErrors: true);
    _applicationContext.SetupGet(x => x.LoggingSettings).Returns(settings);

    _service.Open(Guid.NewGuid().ToString(), "GET", "/api/users/abc123", _faker.Internet.Ip(), $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}");
    _service.SetActivity(new ReadUser(null, null, null));
    _service.Report(new Exception());
    Assert.True(_service.ShouldSaveLog());
  }

  [Fact(DisplayName = "ShouldSaveLog: it should return true when logging extent is Full.")]
  public void Given_Full_When_ShouldSaveLog_Then_True()
  {
    LoggingSettings settings = new(LoggingExtent.Full, onlyErrors: false);
    _applicationContext.SetupGet(x => x.LoggingSettings).Returns(settings);

    _service.Open(Guid.NewGuid().ToString(), "GET", "/api/users/abc123", _faker.Internet.Ip(), $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}");
    Assert.True(_service.ShouldSaveLog());
  }

  [Fact(DisplayName = "ShouldSaveLog: it should return true when logging extent is Full with errors.")]
  public void Given_Errors_When_ShouldSaveLog_Then_True()
  {
    LoggingSettings settings = new(LoggingExtent.Full, onlyErrors: true);
    _applicationContext.SetupGet(x => x.LoggingSettings).Returns(settings);

    _service.Open(Guid.NewGuid().ToString(), "GET", "/api/users/abc123", _faker.Internet.Ip(), $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}");
    _service.Report(new Exception());
    Assert.True(_service.ShouldSaveLog());
  }
}
