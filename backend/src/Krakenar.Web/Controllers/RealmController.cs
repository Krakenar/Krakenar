using Krakenar.Contracts.Realms;
using Krakenar.Core;
using Krakenar.Core.Realms.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/realms")]
public class RealmController : ControllerBase
{
  private readonly ICommandHandler<CreateOrReplaceRealm, CreateOrReplaceRealmResult> _createOrReplaceRealm;

  public RealmController(ICommandHandler<CreateOrReplaceRealm, CreateOrReplaceRealmResult> createOrReplaceRealm)
  {
    _createOrReplaceRealm = createOrReplaceRealm;
  }

  [HttpPost]
  public async Task<ActionResult<Realm>> CreateAsync([FromBody] CreateOrReplaceRealmPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceRealm command = new(Id: null, payload, Version: null);
    CreateOrReplaceRealmResult result = await _createOrReplaceRealm.HandleAsync(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Realm>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceRealmPayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceRealm command = new(id, payload, version);
    CreateOrReplaceRealmResult result = await _createOrReplaceRealm.HandleAsync(command, cancellationToken);
    return ToActionResult(result);
  }

  private ActionResult<Realm> ToActionResult(CreateOrReplaceRealmResult result)
  {
    if (result.Realm is null)
    {
      return NotFound();
    }
    else if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/api/realms/{result.Realm.Id}", UriKind.Absolute);
      return Created(location, result.Realm);
    }

    return Ok(result.Realm);
  }
}
