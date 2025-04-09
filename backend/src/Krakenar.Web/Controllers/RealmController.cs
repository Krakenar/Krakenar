using Krakenar.Contracts.Realms;
using Krakenar.Core;
using Krakenar.Core.Realms.Commands;
using Krakenar.Core.Realms.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/realms")]
public class RealmController : ControllerBase
{
  private readonly ICommandHandler<CreateOrReplaceRealm, CreateOrReplaceRealmResult> _createOrReplaceRealm;
  private readonly IQueryHandler<ReadRealm, Realm?> _readRealm;

  public RealmController(
    ICommandHandler<CreateOrReplaceRealm, CreateOrReplaceRealmResult> createOrReplaceRealm,
    IQueryHandler<ReadRealm, Realm?> readRealm)
  {
    _createOrReplaceRealm = createOrReplaceRealm;
    _readRealm = readRealm;
  }

  [HttpPost]
  public async Task<ActionResult<Realm>> CreateAsync([FromBody] CreateOrReplaceRealmPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceRealm command = new(Id: null, payload, Version: null);
    CreateOrReplaceRealmResult result = await _createOrReplaceRealm.HandleAsync(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Realm>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadRealm query = new(id, UniqueSlug: null);
    Realm? realm = await _readRealm.HandleAsync(query, cancellationToken);
    return realm is null ? NotFound() : Ok(realm);
  }

  [HttpGet("slug:{uniqueSlug}")]
  public async Task<ActionResult<Realm>> ReadAsync(string uniqueSlug, CancellationToken cancellationToken)
  {
    ReadRealm query = new(Id: null, uniqueSlug);
    Realm? realm = await _readRealm.HandleAsync(query, cancellationToken);
    return realm is null ? NotFound() : Ok(realm);
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
