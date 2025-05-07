using Krakenar.Contracts.Messages;
using Krakenar.Web.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.KrakenarAdmin)]
[Route("api/messages")]
public class MessageController : ControllerBase
{
  protected virtual IMessageService MessageService { get; }

  public MessageController(IMessageService messageService)
  {
    MessageService = messageService;
  }

  [HttpPost]
  public virtual async Task<ActionResult<SentMessages>> CreateAsync([FromBody] SendMessagePayload payload, CancellationToken cancellationToken)
  {
    SentMessages sentMessages = await MessageService.SendAsync(payload, cancellationToken);
    if (sentMessages.Ids.Count == 1)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/api/messages/{sentMessages.Ids.Single()}", UriKind.Absolute);
      return Created(location, sentMessages);
    }
    return Ok(sentMessages);
  }

  [HttpGet("{id}")]
  public virtual async Task<ActionResult<Message>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Message? message = await MessageService.ReadAsync(id, cancellationToken);
    return message is null ? NotFound() : Ok(message);
  }
}
