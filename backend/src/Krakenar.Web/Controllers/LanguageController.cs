using Krakenar.Contracts.Localization;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Localization.Commands;
using Krakenar.Core.Localization.Queries;
using Krakenar.Web.Models.Language;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/languages")]
public class LanguageController : ControllerBase
{
  private readonly ICommandHandler<CreateOrReplaceLanguage, CreateOrReplaceLanguageResult> _createOrReplaceLanguage;
  private readonly ICommandHandler<DeleteLanguage, Language?> _deleteLanguage;
  private readonly IQueryHandler<ReadLanguage, Language?> _readLanguage;
  private readonly IQueryHandler<SearchLanguages, SearchResults<Language>> _searchLanguages;
  private readonly ICommandHandler<SetDefaultLanguage, Language?> _setDefaultLanguage;
  private readonly ICommandHandler<UpdateLanguage, Language?> _updateLanguage;

  public LanguageController(
    ICommandHandler<CreateOrReplaceLanguage, CreateOrReplaceLanguageResult> createOrReplaceLanguage,
    ICommandHandler<DeleteLanguage, Language?> deleteLanguage,
    IQueryHandler<ReadLanguage, Language?> readLanguage,
    IQueryHandler<SearchLanguages, SearchResults<Language>> searchLanguages,
    ICommandHandler<SetDefaultLanguage, Language?> setDefaultLanguage,
    ICommandHandler<UpdateLanguage, Language?> updateLanguage)
  {
    _createOrReplaceLanguage = createOrReplaceLanguage;
    _deleteLanguage = deleteLanguage;
    _readLanguage = readLanguage;
    _searchLanguages = searchLanguages;
    _setDefaultLanguage = setDefaultLanguage;
    _updateLanguage = updateLanguage;
  }

  [HttpPost]
  public async Task<ActionResult<Language>> CreateAsync([FromBody] CreateOrReplaceLanguagePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceLanguage command = new(Id: null, payload, Version: null);
    CreateOrReplaceLanguageResult result = await _createOrReplaceLanguage.HandleAsync(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<Language>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteLanguage command = new(id);
    Language? language = await _deleteLanguage.HandleAsync(command, cancellationToken);
    return language is null ? NotFound() : Ok(language);
  }

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

  [HttpPut("{id}")]
  public async Task<ActionResult<Language>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceLanguagePayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceLanguage command = new(id, payload, version);
    CreateOrReplaceLanguageResult result = await _createOrReplaceLanguage.HandleAsync(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<Language>>> SearchAsync([FromQuery] SearchLanguagesParameters parameters, CancellationToken cancellationToken)
  {
    SearchLanguagesPayload payload = parameters.ToPayload();
    SearchLanguages query = new(payload);
    SearchResults<Language> languages = await _searchLanguages.HandleAsync(query, cancellationToken);
    return Ok(languages);
  }

  [HttpPatch("{id}/default")]
  public async Task<ActionResult<Language>> SetDefaultAsync(Guid id, CancellationToken cancellationToken)
  {
    SetDefaultLanguage command = new(id);
    Language? language = await _setDefaultLanguage.HandleAsync(command, cancellationToken);
    return language is null ? NotFound() : Ok(language);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<Language>> UpdateAsync(Guid id, [FromBody] UpdateLanguagePayload payload, CancellationToken cancellationToken)
  {
    UpdateLanguage command = new(id, payload);
    Language? language = await _updateLanguage.HandleAsync(command, cancellationToken);
    return language is null ? NotFound() : Ok(language);
  }

  private ActionResult<Language> ToActionResult(CreateOrReplaceLanguageResult result)
  {
    if (result.Language is null)
    {
      return NotFound();
    }
    else if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/api/languages/{result.Language.Id}", UriKind.Absolute);
      return Created(location, result.Language);
    }

    return Ok(result.Language);
  }
}
