using Bogus;
using Krakenar.Contracts.Realms;

namespace Krakenar.Core.Tokens;

[Trait(Traits.Category, Categories.Unit)]
public class TokenHelperTests
{
  private readonly Faker _faker = new();

  private readonly string _baseUrl;

  public TokenHelperTests()
  {
    _baseUrl = $"https://www.{_faker.Internet.DomainName()}";
  }

  [Fact(DisplayName = "ResolveAudience: it should return the formatted input value when the realm is not null.")]
  public void Given_AudienceAndRealm_When_ResolveAudience_Then_FormattedInput()
  {
    string audience = "{Id}|{UniqueSlug}|{Url}";

    Realm realm = new()
    {
      Id = Guid.NewGuid(),
      UniqueSlug = "new-world",
      Url = $"https://www.{_faker.Internet.DomainName()}"
    };
    string expected = string.Join('|', realm.Id, realm.UniqueSlug, realm.Url);

    Assert.Equal(expected, TokenHelper.ResolveAudience(audience, realm, _baseUrl));
  }

  [Fact(DisplayName = "ResolveAudience: it should return the formatted input value when the realm is null.")]
  public void Given_Audience_When_ResolveAudience_Then_FormattedInput()
  {
    string expected = string.Join(':', "aud", _baseUrl);
    Assert.Equal(expected, TokenHelper.ResolveAudience("aud:{BaseUrl}", realm: null, _baseUrl));
  }

  [Fact(DisplayName = "ResolveAudience: it should return the base URL when there is no audience, nor a realm.")]
  public void Given_NoAudienceNorRealm_When_ResolveAudience_Then_BaseUrl()
  {
    Assert.Equal(_baseUrl, TokenHelper.ResolveAudience(audience: null, realm: null, _baseUrl));
  }

  [Theory(DisplayName = "ResolveAudience: it should return the realm URL or slug.")]
  [InlineData(false)]
  [InlineData(true)]
  public void Given_Realm_When_ResolveAudience_Then_UrlOrSlug(bool hasUrl)
  {
    Realm realm = new()
    {
      UniqueSlug = "new-world"
    };
    string audience = realm.UniqueSlug;
    if (hasUrl)
    {
      realm.Url = _baseUrl;
      audience = _baseUrl;
    }
    Assert.Equal(audience, TokenHelper.ResolveAudience(audience: null, realm, "baseUrl"));
  }

  [Fact(DisplayName = "ResolveIssuer: it should return the formatted input value when the realm is not null.")]
  public void Given_AudienceAndRealm_When_ResolveIssuer_Then_FormattedInput()
  {
    string audience = "{Id}|{UniqueSlug}|{Url}";

    Realm realm = new()
    {
      Id = Guid.NewGuid(),
      UniqueSlug = "new-world",
      Url = $"https://www.{_faker.Internet.DomainName()}"
    };
    string expected = string.Join('|', realm.Id, realm.UniqueSlug, realm.Url);

    Assert.Equal(expected, TokenHelper.ResolveIssuer(audience, realm, _baseUrl));
  }

  [Fact(DisplayName = "ResolveIssuer: it should return the formatted input value when the realm is null.")]
  public void Given_Issuer_When_ResolveIssuer_Then_FormattedInput()
  {
    string expected = string.Join(':', "iss", _baseUrl);
    Assert.Equal(expected, TokenHelper.ResolveIssuer("iss:{BaseUrl}", realm: null, _baseUrl));
  }

  [Fact(DisplayName = "ResolveIssuer: it should return the base URL when there is no audience, nor a realm.")]
  public void Given_NoIssuerNorRealm_When_ResolveIssuer_Then_BaseUrl()
  {
    Assert.Equal(_baseUrl, TokenHelper.ResolveIssuer(issuer: null, realm: null, _baseUrl));
  }

  [Fact(DisplayName = "ResolveIssuer: it should return the realm formatted slug.")]
  public void Given_Realm_When_ResolveIssuer_Then_UrlOrSlug()
  {
    Realm realm = new()
    {
      UniqueSlug = "new-world",
      Url = _baseUrl
    };
    string issuer = $"{_baseUrl}/app/realms/{realm.UniqueSlug}";
    Assert.Equal(issuer, TokenHelper.ResolveIssuer(issuer: null, realm, _baseUrl));
  }
}
