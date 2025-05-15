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
public class CreateOrReplaceTemplateHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly UniqueNameSettings _uniqueNameSettings = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<ITemplateManager> _templateManager = new();
  private readonly Mock<ITemplateQuerier> _templateQuerier = new();
  private readonly Mock<ITemplateRepository> _templateRepository = new();

  private readonly CreateOrReplaceTemplateHandler _handler;

  public CreateOrReplaceTemplateHandlerTests()
  {
    _handler = new(_applicationContext.Object, _templateManager.Object, _templateQuerier.Object, _templateRepository.Object);

    _applicationContext.SetupGet(x => x.UniqueNameSettings).Returns(_uniqueNameSettings);
  }

  [Theory(DisplayName = "It should create a new template.")]
  [InlineData(null)]
  [InlineData("694fde75-fd7d-4b07-8719-34da9f7df1de")]
  public async Task Given_NotExists_When_HandleAsync_Then_Created(string? idValue)
  {
    Guid? id = idValue is null ? null : Guid.Parse(idValue);

    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    CreateOrReplaceTemplatePayload payload = new()
    {
      UniqueName = "PasswordRecovery",
      DisplayName = " Password Recovery ",
      Description = "  This is the password recovery template.  ",
      Subject = " PasswordRecovery_Subject ",
      Content = ContentDto.PlainText("Hello World!")
    };

    TemplateDto dto = new();
    _templateQuerier.Setup(x => x.ReadAsync(It.IsAny<Template>(), _cancellationToken)).ReturnsAsync(dto);

    Template? template = null;
    _templateManager.Setup(x => x.SaveAsync(It.IsAny<Template>(), _cancellationToken)).Callback<Template, CancellationToken>((r, _) => template = r);

    CreateOrReplaceTemplate command = new(id, payload, Version: null);
    CreateOrReplaceTemplateResult result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.True(result.Created);
    Assert.NotNull(result.Template);
    Assert.Same(dto, result.Template);

    Assert.NotNull(template);
    Assert.Equal(actorId, template.CreatedBy);
    Assert.Equal(actorId, template.UpdatedBy);
    Assert.Equal(realmId, template.RealmId);
    Assert.Equal(payload.UniqueName, template.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Trim(), template.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), template.Description?.Value);
    Assert.Equal(payload.Subject.Trim(), template.Subject.Value);
    Assert.Equal(payload.Content.Type, template.Content.Type);
    Assert.Equal(payload.Content.Text, template.Content.Text);

    if (id.HasValue)
    {
      Assert.Equal(id.Value, template.EntityId);

      _templateRepository.Verify(x => x.LoadAsync(It.Is<TemplateId>(i => i.RealmId == realmId && i.EntityId == id.Value), _cancellationToken), Times.Once);
    }
    else
    {
      Assert.NotEqual(Guid.Empty, template.EntityId);
    }
  }

  [Fact(DisplayName = "It should replace an existing template.")]
  public async Task Given_Found_When_HandleAsync_Then_Replaced()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    Template template = new(new UniqueName(_uniqueNameSettings, "passwordrecovery"), new Subject("passwordrecovery"), Content.PlainText("test"), actorId);
    _templateRepository.Setup(x => x.LoadAsync(template.Id, _cancellationToken)).ReturnsAsync(template);

    CreateOrReplaceTemplatePayload payload = new()
    {
      UniqueName = "PasswordRecovery",
      DisplayName = " Password Recovery ",
      Description = "  This is the password recovery template.  ",
      Subject = "PasswordRecovery_Subject",
      Content = ContentDto.Html("  <div>Hello World!</div>  ")
    };

    TemplateDto dto = new();
    _templateQuerier.Setup(x => x.ReadAsync(template, _cancellationToken)).ReturnsAsync(dto);

    CreateOrReplaceTemplate command = new(template.EntityId, payload, Version: null);
    CreateOrReplaceTemplateResult result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.Template);
    Assert.Same(dto, result.Template);

    Assert.Equal(actorId, template.UpdatedBy);
    Assert.Equal(payload.UniqueName, template.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Trim(), template.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), template.Description?.Value);
    Assert.Equal(payload.Subject.Trim(), template.Subject.Value);
    Assert.Equal(payload.Content.Type, template.Content.Type);
    Assert.Equal(payload.Content.Text.Trim(), template.Content.Text);

    _templateRepository.Verify(x => x.LoadAsync(It.IsAny<TemplateId>(), It.IsAny<long?>(), It.IsAny<CancellationToken>()), Times.Never);
    _templateManager.Verify(x => x.SaveAsync(template, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the template does not exist.")]
  public async Task Given_NotFound_Then_HandleAsync_Then_NullReturned()
  {
    CreateOrReplaceTemplatePayload payload = new()
    {
      UniqueName = "not_found",
      Subject = "NotFound",
      Content = ContentDto.PlainText("404 Not Found")
    };
    CreateOrReplaceTemplate command = new(Guid.Empty, payload, Version: -1);
    CreateOrReplaceTemplateResult result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.Null(result.Template);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    CreateOrReplaceTemplatePayload payload = new()
    {
      UniqueName = "invalid!",
      DisplayName = RandomStringGenerator.GetString(999),
      Subject = RandomStringGenerator.GetString(999),
      Content = new ContentDto("application/json", string.Empty)
    };

    CreateOrReplaceTemplate command = new(Id: null, payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(5, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "AllowedCharactersValidator" && e.PropertyName == "UniqueName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "DisplayName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Subject");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MediaTypeValidator" && e.PropertyName == "Content.Type");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Content.Text");
  }

  [Fact(DisplayName = "It should update an existing template.")]
  public async Task Given_Found_When_HandleAsync_Then_Updated()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    Template template = new(new UniqueName(_uniqueNameSettings, "PasswordRecovery"), new Subject("PasswordRcovery"), Content.PlainText("Hello World!"), actorId);
    _templateRepository.Setup(x => x.LoadAsync(template.Id, _cancellationToken)).ReturnsAsync(template);

    Template reference = new(template.UniqueName, template.Subject, template.Content, actorId, template.Id);
    _templateRepository.Setup(x => x.LoadAsync(reference.Id, reference.Version, _cancellationToken)).ReturnsAsync(reference);

    Description description = new("  This is the password recovery template.  ");
    template.Description = description;
    Subject subject = new(" PasswordRecovery_Subject ");
    template.Subject = subject;
    template.Update(actorId);

    CreateOrReplaceTemplatePayload payload = new()
    {
      UniqueName = template.UniqueName.Value,
      DisplayName = " Password Recovery ",
      Subject = " PasswordRcovery ",
      Content = ContentDto.Html("  <div>Hello World!</div>  ")
    };

    TemplateDto dto = new();
    _templateQuerier.Setup(x => x.ReadAsync(template, _cancellationToken)).ReturnsAsync(dto);

    CreateOrReplaceTemplate command = new(template.EntityId, payload, reference.Version);
    CreateOrReplaceTemplateResult result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.Template);
    Assert.Same(dto, result.Template);

    Assert.Equal(actorId, template.UpdatedBy);
    Assert.Equal(payload.UniqueName, template.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Trim(), template.DisplayName?.Value);
    Assert.Equal(description, template.Description);
    Assert.Equal(subject, template.Subject);
    Assert.Equal(payload.Content.Type, template.Content.Type);
    Assert.Equal(payload.Content.Text.Trim(), template.Content.Text);

    _templateRepository.Verify(x => x.LoadAsync(reference.Id, reference.Version, _cancellationToken), Times.Once);
    _templateManager.Verify(x => x.SaveAsync(template, _cancellationToken), Times.Once);
  }
}
