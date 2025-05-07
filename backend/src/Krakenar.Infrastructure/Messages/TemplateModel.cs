using Krakenar.Core.Localization;
using Krakenar.Core.Messages;
using Krakenar.Core.Users;

namespace Krakenar.Infrastructure.Messages;

public class TemplateModel
{
  private readonly Dictionaries _dictionaries;
  private readonly Variables _variables;

  public TemplateModel(Dictionaries? dictionaries = null, Locale? locale = null, User? user = null, Variables? variables = null)
  {
    _dictionaries = dictionaries ?? new();
    _variables = variables ?? new();

    Locale = locale;
    User = user;
  }

  public Locale? Locale { get; }
  public User? User { get; }

  public string Resource(string key) => _dictionaries.Translate(key);
  public string Variable(string key) => _variables.Resolve(key);
}
