using Krakenar.Contracts.Localization;
using Krakenar.Core;
using Krakenar.Core.Localization.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/languages")]
public class LanguageController : ControllerBase
{
  private readonly IQueryHandler<ReadLanguage, Language?> _readLanguage;

  public LanguageController(IQueryHandler<ReadLanguage, Language?> readLanguage)
  {
    _readLanguage = readLanguage;
  }

  // TODO(fpion): create

  // TODO(fpion): delete

  [HttpGet("{id}")]
  public async Task<ActionResult<Language>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadLanguage query = new(id, Locale: null, IsDefault: false);
    Language? language = await _readLanguage.HandleAsync(query, cancellationToken);
    return language is null ? NotFound() : Ok(language);
  }

  [HttpGet("locale:{locale}")]
  public async Task<ActionResult<Language>> ReadAsync(string locale, CancellationToken cancellationToken)
  {
    ReadLanguage query = new(Id: null, locale, IsDefault: false);
    Language? language = await _readLanguage.HandleAsync(query, cancellationToken);
    return language is null ? NotFound() : Ok(language);
  }

  [HttpGet("default")]
  public async Task<ActionResult<Language>> ReadDefaultAsync(CancellationToken cancellationToken)
  {
    ReadLanguage query = new(Id: null, Locale: null, IsDefault: true);
    Language? language = await _readLanguage.HandleAsync(query, cancellationToken);
    return language is null ? NotFound() : Ok(language);
  }

  // TODO(fpion): replace

  // TODO(fpion): search

  // TODO(fpion): set default

  // TODO(fpion): update
}
