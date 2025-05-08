using Krakenar.Contracts;
using Krakenar.Core.Realms;
using Logitar;

namespace Krakenar.Core.Templates;

public class TemplateNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified template could not be found.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public string Template
  {
    get => (string)Data[nameof(Template)]!;
    private set => Data[nameof(Template)] = value;
  }
  public string PropertyName
  {
    get => (string?)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(RealmId)] = RealmId;
      error.Data[nameof(Template)] = Template;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public TemplateNotFoundException(RealmId? realmId, string template, string propertyName) : base(BuildMessage(realmId, template, propertyName))
  {
    RealmId = realmId?.ToGuid();
    Template = template;
    PropertyName = propertyName;
  }

  private static string BuildMessage(RealmId? realmId, string template, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), realmId?.ToGuid(), "<null>")
    .AddData(nameof(Template), template)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
