using Krakenar.Core.Settings;
using Moq;

namespace Krakenar.Core.Templates;

[Trait(Traits.Category, Categories.Unit)]
public class TemplateManagerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly UniqueNameSettings _uniqueNameSettings = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<ITemplateQuerier> _templateQuerier = new();
  private readonly Mock<ITemplateRepository> _templateRepository = new();

  private readonly TemplateManager _manager;

  public TemplateManagerTests()
  {
    _manager = new(_applicationContext.Object, _templateQuerier.Object, _templateRepository.Object);
  }

  [Fact(DisplayName = "SaveAsync: it should save the template when the unique name has not changed.")]
  public async Task Given_UniqueNameNotChanged_When_SaveAsync_Then_Saved()
  {
    Template template = new(new UniqueName(_uniqueNameSettings, "PasswordRecovery"), new Subject("PasswordRecovery_Subject"), Content.PlainText("Hello World!"));
    template.ClearChanges();

    template.Delete();
    await _manager.SaveAsync(template, _cancellationToken);

    _templateQuerier.Verify(x => x.FindIdAsync(It.IsAny<UniqueName>(), It.IsAny<CancellationToken>()), Times.Never);
    _templateRepository.Verify(x => x.SaveAsync(template, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "SaveAsync: it should save the template when there is no unique name conflict.")]
  public async Task Given_NoUniqueNameConflict_When_SaveAsync_Then_Saved()
  {
    Template template = new(new UniqueName(_uniqueNameSettings, "PasswordRecovery"), new Subject("PasswordRecovery_Subject"), Content.PlainText("Hello World!"));
    _templateQuerier.Setup(x => x.FindIdAsync(template.UniqueName, _cancellationToken)).ReturnsAsync(template.Id);

    await _manager.SaveAsync(template, _cancellationToken);

    _templateQuerier.Verify(x => x.FindIdAsync(template.UniqueName, _cancellationToken), Times.Once);
    _templateRepository.Verify(x => x.SaveAsync(template, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "SaveAsync: it should throw UniqueNameAlreadyUsedException when there is a unique name conflict.")]
  public async Task Given_UniqueNameConflict_When_SaveAsync_Then_UniqueNameAlreadyUsedException()
  {
    Template conflict = new(new UniqueName(_uniqueNameSettings, "PasswordRecovery"), new Subject("PasswordRecovery_Subject"), Content.PlainText("Hello World!"));
    _templateQuerier.Setup(x => x.FindIdAsync(conflict.UniqueName, _cancellationToken)).ReturnsAsync(conflict.Id);

    Template template = new(new UniqueName(_uniqueNameSettings, "AccountActivation"), new Subject("AccountActivation_Subject"), Content.PlainText("Hello World!"));
    template.ClearChanges();
    template.SetUniqueName(conflict.UniqueName);

    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException>(async () => await _manager.SaveAsync(template, _cancellationToken));
    Assert.Equal(template.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal("Template", exception.EntityType);
    Assert.Equal(template.EntityId, exception.EntityId);
    Assert.Equal(conflict.EntityId, exception.ConflictId);
    Assert.Equal(template.UniqueName.Value, exception.UniqueName);
    Assert.Equal("UniqueName", exception.PropertyName);
  }
}
