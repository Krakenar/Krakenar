using Krakenar.Contracts.Search;
using Krakenar.Contracts.Senders;
using Krakenar.Web.Constants;
using Krakenar.Web.Models.Sender;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.KrakenarAdmin)]
[Route("api/senders")]
public class SenderController : ControllerBase
{
  protected virtual ISenderService SenderService { get; }

  public SenderController(ISenderService senderService)
  {
    SenderService = senderService;
  }

  [HttpPost]
  public virtual async Task<ActionResult<Sender>> CreateAsync([FromBody] CreateOrReplaceSenderPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceSenderResult result = await SenderService.CreateOrReplaceAsync(payload, id: null, version: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  public virtual async Task<ActionResult<Sender>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    Sender? sender = await SenderService.DeleteAsync(id, cancellationToken);
    return sender is null ? NotFound() : Ok(sender);
  }

  [HttpGet("{id}")]
  public virtual async Task<ActionResult<Sender>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Sender? sender = await SenderService.ReadAsync(id, kind: null, cancellationToken);
    return sender is null ? NotFound() : Ok(sender);
  }

  [HttpGet("default/{kind}")]
  public virtual async Task<ActionResult<Sender>> ReadAsync(SenderKind kind, CancellationToken cancellationToken)
  {
    Sender? sender = await SenderService.ReadAsync(id: null, kind, cancellationToken);
    return sender is null ? NotFound() : Ok(sender);
  }

  [HttpPut("{id}")]
  public virtual async Task<ActionResult<Sender>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceSenderPayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceSenderResult result = await SenderService.CreateOrReplaceAsync(payload, id, version, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public virtual async Task<ActionResult<SearchResults<Sender>>> SearchAsync([FromQuery] SearchSendersParameters parameters, CancellationToken cancellationToken)
  {
    SearchSendersPayload payload = parameters.ToPayload();
    SearchResults<Sender> senders = await SenderService.SearchAsync(payload, cancellationToken);
    return Ok(senders);
  }

  [HttpPatch("{id}/default")]
  public virtual async Task<ActionResult<Sender>> SetDefaultAsync(Guid id, CancellationToken cancellationToken)
  {
    Sender? sender = await SenderService.SetDefaultAsync(id, cancellationToken);
    return sender is null ? NotFound() : Ok(sender);
  }

  [HttpPatch("{id}")]
  public virtual async Task<ActionResult<Sender>> UpdateAsync(Guid id, [FromBody] UpdateSenderPayload payload, CancellationToken cancellationToken)
  {
    Sender? sender = await SenderService.UpdateAsync(id, payload, cancellationToken);
    return sender is null ? NotFound() : Ok(sender);
  }

  protected virtual ActionResult<Sender> ToActionResult(CreateOrReplaceSenderResult result)
  {
    if (result.Sender is null)
    {
      return NotFound();
    }
    else if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/api/senders/{result.Sender.Id}", UriKind.Absolute);
      return Created(location, result.Sender);
    }

    return Ok(result.Sender);
  }
}
