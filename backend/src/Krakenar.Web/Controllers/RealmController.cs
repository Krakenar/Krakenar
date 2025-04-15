using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Search;
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
  protected virtual IRealmService RealmService { get; }

  public RealmController(IRealmService realmService)
  {
    RealmService = realmService;
  }

  [HttpPost]
  public virtual async Task<ActionResult<Realm>> CreateAsync([FromBody] CreateOrReplaceRealmPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceRealmResult result = await RealmService.CreateOrReplaceAsync(payload, id: null, version: null, cancellationToken);
    return ToActionResult(result);
  }

  // TODO(fpion): delete

  [HttpGet("{id}")]
  public virtual async Task<ActionResult<Realm>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Realm? realm = await RealmService.ReadAsync(id, uniqueSlug: null, cancellationToken);
    return realm is null ? NotFound() : Ok(realm);
  }

  [HttpGet("slug:{uniqueSlug}")]
  public virtual async Task<ActionResult<Realm>> ReadAsync(string uniqueSlug, CancellationToken cancellationToken)
  {
    Realm? realm = await RealmService.ReadAsync(id: null, uniqueSlug, cancellationToken);
    return realm is null ? NotFound() : Ok(realm);
  }

  [HttpPut("{id}")]
  public virtual async Task<ActionResult<Realm>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceRealmPayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceRealmResult result = await RealmService.CreateOrReplaceAsync(payload, id, version, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public virtual async Task<ActionResult<SearchResults<Realm>>> SearchAsync([FromQuery] SearchRealmsParameters parameters, CancellationToken cancellationToken)
  {
    SearchRealmsPayload payload = parameters.ToPayload();
    SearchResults<Realm> realms = await RealmService.SearchAsync(payload, cancellationToken);
    return Ok(realms);
  }

  [HttpPatch("{id}")]
  public virtual async Task<ActionResult<Realm>> UpdateAsync(Guid id, [FromBody] UpdateRealmPayload payload, CancellationToken cancellationToken)
  {
    Realm? realm = await RealmService.UpdateAsync(id, payload, cancellationToken);
    return realm is null ? NotFound() : Ok(realm);
  }

  protected virtual ActionResult<Realm> ToActionResult(CreateOrReplaceRealmResult result)
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
