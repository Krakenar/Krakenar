using Krakenar.Contracts;
using Krakenar.Contracts.Search;
using Krakenar.Contracts.Templates;
using Krakenar.Core;
using Krakenar.Core.Templates;
using Logitar;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Content = Krakenar.Core.Templates.Content;
using ContentDto = Krakenar.Contracts.Templates.Content;
using Template = Krakenar.Core.Templates.Template;
using TemplateDto = Krakenar.Contracts.Templates.Template;

namespace Krakenar.Templates;

[Trait(Traits.Category, Categories.Integration)]
public class TemplateIntegrationTests : IntegrationTests
{
  private readonly ITemplateRepository _templateRepository;
  private readonly ITemplateService _templateService;

  private readonly Template _template;

  public TemplateIntegrationTests() : base()
  {
    _templateRepository = ServiceProvider.GetRequiredService<ITemplateRepository>();
    _templateService = ServiceProvider.GetRequiredService<ITemplateService>();

    Subject subject = new("PasswordRecovery_Subject");
    Content content = Content.Html("<div>Hello World!</div>");
    _template = new(new UniqueName(Realm.UniqueNameSettings, "PasswordRecovery"), subject, content, ActorId, TemplateId.NewId(Realm.Id));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _templateRepository.SaveAsync(_template);
  }

  [Fact(DisplayName = "It should create a new template.")]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created()
  {
    CreateOrReplaceTemplatePayload payload = new()
    {
      UniqueName = "AccountActivation",
      DisplayName = " Account Activation ",
      Description = "  This is the template for account activation.  ",
      Subject = " AccountActivation_Subject ",
      Content = ContentDto.PlainText("  Hello World!  ")
    };

    Guid id = Guid.NewGuid();
    CreateOrReplaceTemplateResult result = await _templateService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);

    TemplateDto? template = result.Template;
    Assert.NotNull(template);
    Assert.Equal(id, template.Id);
    Assert.Equal(2, template.Version);
    Assert.Equal(Actor, template.CreatedBy);
    Assert.Equal(DateTime.UtcNow, template.CreatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, template.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, template.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(RealmDto, template.Realm);
    Assert.Equal(payload.UniqueName, template.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), template.DisplayName);
    Assert.Equal(payload.Description.Trim(), template.Description);
    Assert.Equal(payload.Subject.Trim(), template.Subject);
    Assert.Equal(payload.Content.Type, template.Content.Type);
    Assert.Equal(payload.Content.Text.Trim(), template.Content.Text);
  }

  [Fact(DisplayName = "It should delete the template.")]
  public async Task Given_Template_When_Delete_Then_Deleted()
  {
    TemplateDto? template = await _templateService.DeleteAsync(_template.EntityId);
    Assert.NotNull(template);
    Assert.Equal(_template.EntityId, template.Id);

    Assert.Empty(await KrakenarContext.Templates.AsNoTracking().Where(x => x.StreamId == _template.Id.Value).ToArrayAsync());
  }

  [Fact(DisplayName = "It should read the template by ID.")]
  public async Task Given_Id_When_Read_Then_Found()
  {
    TemplateDto? template = await _templateService.ReadAsync(_template.EntityId);
    Assert.NotNull(template);
    Assert.Equal(_template.EntityId, template.Id);
  }

  [Fact(DisplayName = "It should read the template by unique name.")]
  public async Task Given_UniqueName_When_Read_Then_Found()
  {
    TemplateDto? template = await _templateService.ReadAsync(id: null, _template.UniqueName.Value);
    Assert.NotNull(template);
    Assert.Equal(_template.EntityId, template.Id);
  }

  [Fact(DisplayName = "It should replace an existing template.")]
  public async Task Given_NoVersion_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceTemplatePayload payload = new()
    {
      UniqueName = "PasswordRecovery",
      DisplayName = " Password Recovery ",
      Description = "  This is the password recovery template.  ",
      Subject = " PasswordRecovery_Subject ",
      Content = ContentDto.PlainText(" Hello World! ")
    };

    CreateOrReplaceTemplateResult result = await _templateService.CreateOrReplaceAsync(payload, _template.EntityId);
    Assert.False(result.Created);

    TemplateDto? template = result.Template;
    Assert.NotNull(template);
    Assert.Equal(_template.EntityId, template.Id);
    Assert.Equal(_template.Version + 1, template.Version);
    Assert.Equal(Actor, template.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, template.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(payload.UniqueName, template.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), template.DisplayName);
    Assert.Equal(payload.Description.Trim(), template.Description);
    Assert.Equal(payload.Subject.Trim(), template.Subject);
    Assert.Equal(payload.Content.Type, template.Content.Type);
    Assert.Equal(payload.Content.Text.Trim(), template.Content.Text);
  }

  [Fact(DisplayName = "It should replace an existing template from reference.")]
  public async Task Given_Version_When_CreateOrReplace_Then_Replaced()
  {
    long version = _template.Version;

    Description description = new("  This is the password recovery template.  ");
    _template.Description = description;
    Content content = Content.PlainText("Hello World!");
    _template.Content = content;
    _template.Update(ActorId);
    await _templateRepository.SaveAsync(_template);

    CreateOrReplaceTemplatePayload payload = new()
    {
      UniqueName = "PasswordRecovery",
      DisplayName = " Password Recovery ",
      Subject = " PasswordRecovery_Subject ",
      Content = new ContentDto(_template.Content)
    };

    CreateOrReplaceTemplateResult result = await _templateService.CreateOrReplaceAsync(payload, _template.EntityId, version);
    Assert.False(result.Created);

    TemplateDto? template = result.Template;
    Assert.NotNull(template);
    Assert.Equal(_template.EntityId, template.Id);
    Assert.Equal(_template.Version + 1, template.Version);
    Assert.Equal(Actor, template.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, template.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(payload.UniqueName, template.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), template.DisplayName);
    Assert.Equal(description.Value, template.Description);
    Assert.Equal(payload.Subject.Trim(), template.Subject);
    Assert.Equal(content.Type, template.Content.Type);
    Assert.Equal(content.Text, template.Content.Text);
  }

  [Fact(DisplayName = "It should return null when the template cannot be found.")]
  public async Task Given_NotFound_When_Read_Then_NullReturned()
  {
    Assert.Null(await _templateService.ReadAsync(Guid.Empty, "not-found"));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Templates_When_Search_Then_CorrectResults()
  {
    Template accountActivation = new(new UniqueName(Realm.UniqueNameSettings, "AccountActivation"), new Subject("AccountActivation_Subject"), Content.PlainText("Hello World!"), ActorId, TemplateId.NewId(Realm.Id));
    Template mfaEmail = new(new UniqueName(Realm.UniqueNameSettings, "MFA_Email"), new Subject("MFA_Email_Subject"), Content.PlainText("Hello World!"), ActorId, TemplateId.NewId(Realm.Id));
    Template mfaPhone = new(new UniqueName(Realm.UniqueNameSettings, "MFA_Phone"), new Subject("MFA_Phone_Subject"), Content.PlainText("Hello World!"), ActorId, TemplateId.NewId(Realm.Id));
    await _templateRepository.SaveAsync([accountActivation, mfaEmail, mfaPhone]);

    SearchTemplatesPayload payload = new()
    {
      Ids = [_template.EntityId, accountActivation.EntityId, mfaEmail.EntityId, Guid.Empty],
      Search = new TextSearch([new SearchTerm("%password%"), new SearchTerm("mfa%")], SearchOperator.Or),
      Sort = [new TemplateSortOption(TemplateSort.Subject, isDescending: false)],
      Skip = 1,
      Limit = 1
    };
    SearchResults<TemplateDto> results = await _templateService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    TemplateDto template = Assert.Single(results.Items);
    Assert.Equal(_template.EntityId, template.Id);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when multiple templates were read.")]
  public async Task Given_MultipleResults_When_Read_Then_TooManyResultsException()
  {
    Subject subject = new("AccountActivation_Subject");
    Content content = Content.Html("<div>Hello World!</div>");
    Template template = new(new UniqueName(Realm.UniqueNameSettings, "AccountActivation"), subject, content, ActorId, TemplateId.NewId(Realm.Id));
    await _templateRepository.SaveAsync(template);

    var exception = await Assert.ThrowsAsync<TooManyResultsException<TemplateDto>>(async () => await _templateService.ReadAsync(_template.EntityId, template.UniqueName.Value));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsedException when a unique name conflict occurs.")]
  public async Task Given_UniqueNameConflict_When_CreateOrReplace_Then_UniqueNameAlreadyUsedException()
  {
    CreateOrReplaceTemplatePayload payload = new()
    {
      UniqueName = _template.UniqueName.Value,
      Subject = _template.Subject.Value,
      Content = new ContentDto(_template.Content)
    };
    Guid id = Guid.NewGuid();
    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException>(async () => await _templateService.CreateOrReplaceAsync(payload, id));

    Assert.Equal(_template.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal("Template", exception.EntityType);
    Assert.Equal(id, exception.EntityId);
    Assert.Equal(_template.EntityId, exception.ConflictId);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
    Assert.Equal("UniqueName", exception.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing template.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    UpdateTemplatePayload payload = new()
    {
      DisplayName = new Contracts.Change<string>(" Password Recovery "),
      Description = new Contracts.Change<string>("  This is the password recovery template.  ")
    };
    TemplateDto? template = await _templateService.UpdateAsync(_template.EntityId, payload);
    Assert.NotNull(template);

    Assert.Equal(_template.EntityId, template.Id);
    Assert.Equal(_template.Version + 1, template.Version);
    Assert.Equal(Actor, template.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, template.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(_template.UniqueName.Value, template.UniqueName);
    Assert.Equal(payload.DisplayName.Value?.Trim(), template.DisplayName);
    Assert.Equal(payload.Description.Value?.Trim(), template.Description);
    Assert.Equal(_template.Subject.Value, template.Subject);
    Assert.Equal(_template.Content.Type, template.Content.Type);
    Assert.Equal(_template.Content.Text, template.Content.Text);
  }
}
