using Krakenar.Client.Templates;
using Krakenar.Contracts;
using Krakenar.Contracts.Search;
using Krakenar.Contracts.Templates;

namespace Krakenar.Clients;

[Trait(Traits.Category, Categories.EndToEnd)]
public class TemplateClientE2ETests : E2ETests
{
  private readonly CancellationToken _cancellationToken = default;

  public TemplateClientE2ETests() : base()
  {
  }

  [Fact(DisplayName = "Templates should be managed correctly through the API client.")]
  public async Task Given_Realm_When_Client_Then_Success()
  {
    await InitializeRealmAsync(_cancellationToken);

    using HttpClient httpClient = new();
    KrakenarSettings.Realm = Realm.UniqueSlug;
    TemplateClient templates = new(httpClient, KrakenarSettings);

    Guid id = Guid.Parse("a9b3d6a5-773c-4694-90e3-75c46ef46ddd");
    Template? template = await templates.ReadAsync(id, uniqueName: null, _cancellationToken);

    CreateOrReplaceTemplatePayload createOrReplaceTemplate = new("PasswordRecovery", "PasswordRecovery_Subject", Content.PlainText("Hello World!"));
    CreateOrReplaceTemplateResult templateResult = await templates.CreateOrReplaceAsync(createOrReplaceTemplate, id, version: null, _cancellationToken);
    Assert.Equal(templateResult.Created, template is null);
    template = templateResult.Template;
    Assert.NotNull(template);
    Assert.Equal(id, template.Id);
    Assert.Equal(createOrReplaceTemplate.UniqueName, template.UniqueName);
    Assert.Equal(createOrReplaceTemplate.Subject, template.Subject);
    Assert.Equal(createOrReplaceTemplate.Content, template.Content);

    UpdateTemplatePayload updateTemplate = new()
    {
      DisplayName = new Change<string>(" Password Recovery "),
      Description = new Change<string>("  This is the password recovery template.  ")
    };
    template = await templates.UpdateAsync(id, updateTemplate, _cancellationToken);
    Assert.NotNull(template);
    Assert.Equal(createOrReplaceTemplate.UniqueName, template.UniqueName);
    Assert.Equal(updateTemplate.DisplayName.Value?.Trim(), template.DisplayName);
    Assert.Equal(updateTemplate.Description.Value?.Trim(), template.Description);

    template = await templates.ReadAsync(id: null, template.UniqueName, _cancellationToken);
    Assert.NotNull(template);
    Assert.Equal(id, template.Id);

    SearchTemplatesPayload searchTemplates = new()
    {
      Ids = [template.Id],
      Search = new TextSearch([new SearchTerm("%recover_")])
    };
    SearchResults<Template> results = await templates.SearchAsync(searchTemplates, _cancellationToken);
    Assert.Equal(1, results.Total);
    template = Assert.Single(results.Items);
    Assert.Equal(id, template.Id);

    createOrReplaceTemplate.UniqueName = "AccountActivation";
    createOrReplaceTemplate.Subject = "AccountActivation_Subject";
    templateResult = await templates.CreateOrReplaceAsync(createOrReplaceTemplate, id: null, version: null, _cancellationToken);
    template = templateResult.Template;
    Assert.NotNull(template);

    template = await templates.DeleteAsync(template.Id, _cancellationToken);
    Assert.NotNull(template);
    Assert.Equal(createOrReplaceTemplate.UniqueName, template.UniqueName);
  }
}
