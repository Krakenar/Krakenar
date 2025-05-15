using Krakenar.Contracts.Templates;
using Krakenar.Core.Realms;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using Logitar.Security.Cryptography;
using Moq;
using ContentDto = Krakenar.Contracts.Templates.Content;
using TemplateDto = Krakenar.Contracts.Templates.Template;

namespace Krakenar.Core.Templates.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateTemplateHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<ITemplateManager> _templateManager = new();
  private readonly Mock<ITemplateQuerier> _templateQuerier = new();
  private readonly Mock<ITemplateRepository> _templateRepository = new();

  private readonly UpdateTemplateHandler _handler;

  private readonly UniqueNameSettings _uniqueNameSettings = new();

  public UpdateTemplateHandlerTests()
  {
    _handler = new(_applicationContext.Object, _templateManager.Object, _templateQuerier.Object, _templateRepository.Object);

    _applicationContext.SetupGet(x => x.UniqueNameSettings).Returns(_uniqueNameSettings);
  }

  [Fact(DisplayName = "It should return null when the template was not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    UpdateTemplatePayload payload = new();
    UpdateTemplate command = new(Guid.Empty, payload);
    Assert.Null(await _handler.HandleAsync(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    UpdateTemplatePayload payload = new()
    {
      UniqueName = "admin!",
      DisplayName = new Contracts.Change<string>(RandomStringGenerator.GetString(999)),
      Subject = RandomStringGenerator.GetString(999),
      Content = new ContentDto("application/json", string.Empty)
    };

    UpdateTemplate command = new(Guid.Empty, payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(5, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "AllowedCharactersValidator" && e.PropertyName == "UniqueName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "DisplayName.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Subject");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MediaTypeValidator" && e.PropertyName == "Content.Type");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Content.Text");
  }

  [Fact(DisplayName = "It should update the template.")]
  public async Task Given_Template_When_HandleAsync_Then_Updated()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    Subject subject = new("PasswordRecovery_Subject");
    Content content = Content.PlainText("Hello World!");
    Template template = new(new UniqueName(_uniqueNameSettings, "PasswordRecovery_"), subject, content, actorId, TemplateId.NewId(realmId));
    _templateRepository.Setup(x => x.LoadAsync(template.Id, _cancellationToken)).ReturnsAsync(template);

    UpdateTemplatePayload payload = new()
    {
      UniqueName = "PasswordRecovery",
      DisplayName = new Contracts.Change<string>(" Password Recovery "),
      Description = new Contracts.Change<string>("  This is the password recovery template.  "),
      Content = ContentDto.Html("<div>Hello World!</div>")
    };

    TemplateDto dto = new();
    _templateQuerier.Setup(x => x.ReadAsync(template, _cancellationToken)).ReturnsAsync(dto);

    UpdateTemplate command = new(template.EntityId, payload);
    TemplateDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    _templateManager.Verify(x => x.SaveAsync(template, _cancellationToken), Times.Once);

    Assert.Equal(payload.UniqueName, template.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Value?.Trim(), template.DisplayName?.Value);
    Assert.Equal(payload.Description.Value?.Trim(), template.Description?.Value);
    Assert.Equal(subject.Value, template.Subject.Value);
    Assert.Equal(payload.Content.Type, template.Content.Type);
    Assert.Equal(payload.Content.Text, template.Content.Text);
  }
}
