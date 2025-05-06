using Krakenar.Core.Realms;
using Krakenar.Core.Settings;
using Krakenar.Core.Templates.Events;
using Logitar.EventSourcing;

namespace Krakenar.Core.Templates;

[Trait(Traits.Category, Categories.Unit)]
public class TemplateTests
{
  private readonly UniqueNameSettings _uniqueNameSettings = new();
  private readonly Template _template;

  public TemplateTests()
  {
    _template = new(new UniqueName(_uniqueNameSettings, "PasswordRecovery"), new Subject("PasswordRecovery_Subject"), Content.PlainText("Hello World!"));
  }

  [Fact(DisplayName = "Delete: it should delete the template.")]
  public void Given_Template_When_Delete_Then_Deleted()
  {
    Assert.False(_template.IsDeleted);

    _template.Delete();
    Assert.True(_template.IsDeleted);
    Assert.Contains(_template.Changes, change => change is TemplateDeleted);

    _template.ClearChanges();
    _template.Delete();
    Assert.False(_template.HasChanges);
    Assert.Empty(_template.Changes);
  }

  [Theory(DisplayName = "It should construct a new template from arguments.")]
  [InlineData(null, null, null)]
  [InlineData("244c4394-53a5-45cc-b918-650e67ea230c", "9c8a8b2f-e9f4-4e1e-a266-6656ee53bc29", "de9db727-c742-4f2f-9ebd-46f1c153450b")]
  public void Given_Arguments_When_ctor_Then_Template(string? actorIdValue, string? realmIdValue, string? templateIdValue)
  {
    ActorId? actorId = actorIdValue is null ? null : new(Guid.Parse(actorIdValue));
    RealmId? realmId = realmIdValue is null ? null : new(Guid.Parse(realmIdValue));
    TemplateId? templateId = templateIdValue is null ? null : new(Guid.Parse(templateIdValue), realmId);

    Template template = new(_template.UniqueName, _template.Subject, _template.Content, actorId, templateId);

    Assert.Equal(_template.UniqueName, template.UniqueName);
    Assert.Equal(_template.Subject, template.Subject);
    Assert.Equal(_template.Content, template.Content);
    Assert.Equal(actorId, template.CreatedBy);
    Assert.Equal(actorId, template.UpdatedBy);

    Assert.Equal(realmId, template.RealmId);
    if (templateId.HasValue)
    {
      Assert.Equal(templateId, template.Id);
    }
    else
    {
      Assert.NotEqual(Guid.Empty, template.EntityId);
    }
  }

  [Fact(DisplayName = "It should handle Content change correctly.")]
  public void Given_Changes_When_Content_Then_Changed()
  {
    Content content = Content.Html("<div>Hello World!</div>");
    _template.Content = content;
    _template.Update(_template.CreatedBy);
    Assert.Equal(content, _template.Content);
    Assert.Contains(_template.Changes, change => change is TemplateUpdated updated && updated.Content is not null && updated.Content.Equals(content));

    _template.ClearChanges();

    _template.Content = content;
    _template.Update();
    Assert.False(_template.HasChanges);
  }

  [Fact(DisplayName = "It should handle Description change correctly.")]
  public void Given_Changes_When_Description_Then_Changed()
  {
    Description description = new("  This is the password recovery template.  ");
    _template.Description = description;
    _template.Update(_template.CreatedBy);
    Assert.Equal(description, _template.Description);
    Assert.Contains(_template.Changes, change => change is TemplateUpdated updated && updated.Description?.Value is not null && updated.Description.Value.Equals(description));

    _template.ClearChanges();

    _template.Description = description;
    _template.Update();
    Assert.False(_template.HasChanges);
  }

  [Fact(DisplayName = "It should handle DisplayName change correctly.")]
  public void Given_Changes_When_DisplayName_Then_Changed()
  {
    DisplayName displayName = new(" Password Recovery ");
    _template.DisplayName = displayName;
    _template.Update(_template.CreatedBy);
    Assert.Equal(displayName, _template.DisplayName);
    Assert.Contains(_template.Changes, change => change is TemplateUpdated updated && updated.DisplayName?.Value is not null && updated.DisplayName.Value.Equals(displayName));

    _template.ClearChanges();

    _template.DisplayName = displayName;
    _template.Update();
    Assert.False(_template.HasChanges);
  }

  [Fact(DisplayName = "It should handle Subject change correctly.")]
  public void Given_Changes_When_Subject_Then_Changed()
  {
    Subject subject = new(" ResetPassword_Subject ");
    _template.Subject = subject;
    _template.Update(_template.CreatedBy);
    Assert.Equal(subject, _template.Subject);
    Assert.Contains(_template.Changes, change => change is TemplateUpdated updated && updated.Subject is not null && updated.Subject.Equals(subject));

    _template.ClearChanges();

    _template.Subject = subject;
    _template.Update();
    Assert.False(_template.HasChanges);
  }

  [Fact(DisplayName = "SetUniqueName: it should handle changes correctly.")]
  public void Given_Changes_When_SetUniqueName_Then_Changed()
  {
    _template.ClearChanges();
    _template.SetUniqueName(_template.UniqueName);
    Assert.False(_template.HasChanges);
    Assert.Empty(_template.Changes);

    UniqueName uniqueName = new(_uniqueNameSettings, "AccountActivation");
    _template.SetUniqueName(uniqueName);
    Assert.Equal(uniqueName, _template.UniqueName);
    Assert.Contains(_template.Changes, change => change is TemplateUniqueNameChanged changed && changed.UniqueName.Equals(uniqueName));
  }

  [Fact(DisplayName = "ToString: it should return the correct string representation.")]
  public void Given_Template_When_ToString_Then_CorrectString()
  {
    Assert.StartsWith(_template.UniqueName.Value, _template.ToString());

    _template.DisplayName = new DisplayName("Password Recovery");
    Assert.StartsWith(_template.DisplayName.Value, _template.ToString());
  }
}
