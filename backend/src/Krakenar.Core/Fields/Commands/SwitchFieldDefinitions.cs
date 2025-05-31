using FluentValidation;
using Krakenar.Contracts.Fields;
using Krakenar.Core.Contents;
using Krakenar.Core.Fields.Validators;
using ContentTypeDto = Krakenar.Contracts.Contents.ContentType;

namespace Krakenar.Core.Fields.Commands;

public record SwitchFieldDefinitions(Guid ContentTypeId, SwitchFieldDefinitionsPayload Payload) : ICommand<ContentTypeDto?>;

/// <exception cref="ValidationException"></exception>
public class SwitchFieldDefinitionsHandler : ICommandHandler<SwitchFieldDefinitions, ContentTypeDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IContentTypeQuerier ContentTypeQuerier { get; }
  protected virtual IContentTypeRepository ContentTypeRepository { get; }

  public SwitchFieldDefinitionsHandler(IApplicationContext applicationContext, IContentTypeQuerier contentTypeQuerier, IContentTypeRepository contentTypeRepository)
  {
    ApplicationContext = applicationContext;
    ContentTypeQuerier = contentTypeQuerier;
    ContentTypeRepository = contentTypeRepository;
  }

  public virtual async Task<ContentTypeDto?> HandleAsync(SwitchFieldDefinitions command, CancellationToken cancellationToken)
  {
    SwitchFieldDefinitionsPayload payload = command.Payload;
    new SwitchFieldDefinitionsValidator().ValidateAndThrow(payload);

    ContentTypeId contentTypeId = new(command.ContentTypeId, ApplicationContext.RealmId);
    ContentType? contentType = await ContentTypeRepository.LoadAsync(contentTypeId, cancellationToken);
    if (contentType is null)
    {
      return null;
    }

    string[] fields = [.. payload.Fields.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim())];
    if (fields.Length != 2)
    {
      throw new InvalidOperationException($"There must be exactly 2 fields to switch, but {(fields.Length < 1 ? "none" : $"{fields.Length} fields")} were specified.");
    }
    FieldDefinition source = contentType.ResolveField(fields[0]) ?? throw new NotImplementedException(); // TODO(fpion): implement
    FieldDefinition destination = contentType.ResolveField(fields[1]) ?? throw new NotImplementedException(); // TODO(fpion): implement
    contentType.SwitchFields(source, destination, ApplicationContext.ActorId);

    await ContentTypeRepository.SaveAsync(contentType, cancellationToken);

    return await ContentTypeQuerier.ReadAsync(contentType, cancellationToken);
  }
}
