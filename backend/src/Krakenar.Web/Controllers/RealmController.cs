using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Realms.Commands;
using Krakenar.Core.Realms.Queries;
using Krakenar.Web.Constants;
using Krakenar.Web.Models.Realm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.KrakenarAdmin)]
[Route("api/realms")]
public class RealmController : ControllerBase
{
  private readonly ICommandHandler<CreateOrReplaceRealm, CreateOrReplaceRealmResult> _createOrReplaceRealm;
  private readonly IQueryHandler<ReadRealm, Realm?> _readRealm;
  private readonly IQueryHandler<SearchRealms, SearchResults<Realm>> _searchRealms;
  private readonly ICommandHandler<UpdateRealm, Realm?> _updateRealm;

  public RealmController(
    ICommandHandler<CreateOrReplaceRealm, CreateOrReplaceRealmResult> createOrReplaceRealm,
    IQueryHandler<ReadRealm, Realm?> readRealm,
    IQueryHandler<SearchRealms, SearchResults<Realm>> searchRealms,
    ICommandHandler<UpdateRealm, Realm?> updateRealm)
  {
    _createOrReplaceRealm = createOrReplaceRealm;
    _readRealm = readRealm;
    _searchRealms = searchRealms;
    _updateRealm = updateRealm;
  }

  [HttpPost]
  public async Task<ActionResult<Realm>> CreateAsync([FromBody] CreateOrReplaceRealmPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceRealm command = new(Id: null, payload, Version: null);
    CreateOrReplaceRealmResult result = await _createOrReplaceRealm.HandleAsync(command, cancellationToken);
    return ToActionResult(result);
  }

  // TODO(fpion): delete

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

  [HttpGet]
  public async Task<ActionResult<SearchResults<Realm>>> SearchAsync([FromQuery] SearchRealmsParameters parameters, CancellationToken cancellationToken)
  {
    SearchRealmsPayload payload = parameters.ToPayload();
    SearchRealms query = new(payload);
    SearchResults<Realm> realms = await _searchRealms.HandleAsync(query, cancellationToken);
    return Ok(realms);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<Realm>> UpdateAsync(Guid id, [FromBody] UpdateRealmPayload payload, CancellationToken cancellationToken)
  {
    UpdateRealm command = new(id, payload);
    Realm? realm = await _updateRealm.HandleAsync(command, cancellationToken);
    return realm is null ? NotFound() : Ok(realm);
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
