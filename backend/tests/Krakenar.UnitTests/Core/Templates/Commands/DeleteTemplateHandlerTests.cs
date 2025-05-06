using Krakenar.Core.Realms;
using Krakenar.Core.Settings;
using Krakenar.Core.Templates.Events;
using Logitar.EventSourcing;
using Moq;
using TemplateDto = Krakenar.Contracts.Templates.Template;

namespace Krakenar.Core.Templates.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteTemplateHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly UniqueNameSettings _uniqueNameSettings = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<ITemplateQuerier> _templateQuerier = new();
  private readonly Mock<ITemplateRepository> _templateRepository = new();

  private readonly DeleteTemplateHandler _handler;

  public DeleteTemplateHandlerTests()
  {
    _handler = new(_applicationContext.Object, _templateQuerier.Object, _templateRepository.Object);
  }

  [Fact(DisplayName = "It should delete the template.")]
  public async Task Given_Template_When_HandleAsync_Then_Deleted()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    Subject subject = new("PasswordRecovery_Subject");
    Content content = Content.PlainText("Hello World!");
    Template template = new(new UniqueName(_uniqueNameSettings, "PasswordRecovery"), subject, content, actorId, TemplateId.NewId(realmId));
    _templateRepository.Setup(x => x.LoadAsync(template.Id, _cancellationToken)).ReturnsAsync(template);

    TemplateDto dto = new();
    _templateQuerier.Setup(x => x.ReadAsync(template, _cancellationToken)).ReturnsAsync(dto);

    DeleteTemplate command = new(template.EntityId);
    TemplateDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    Assert.True(template.IsDeleted);
    Assert.Contains(template.Changes, change => change is TemplateDeleted deleted && deleted.ActorId == actorId);

    _templateRepository.Verify(x => x.SaveAsync(template, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the template was not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    DeleteTemplate command = new(Guid.Empty);
    Assert.Null(await _handler.HandleAsync(command, _cancellationToken));
  }
}
