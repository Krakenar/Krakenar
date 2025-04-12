using Bogus;
using Krakenar.Core.Passwords;

namespace Krakenar.Infrastructure.Passwords.Pbkdf2;

[Trait(Traits.Category, Categories.Unit)]
public class Pbkdf2StrategyTests
{
  private readonly Faker _faker = new();
  private readonly Pbkdf2Settings _settings = new();

  private readonly Pbkdf2Strategy _strategy;

  public Pbkdf2StrategyTests()
  {
    _strategy = new(_settings);
  }

  [Fact(DisplayName = "Decode: it should decode a PBKDF2 encoded password.")]
  public void Given_Encoded_When_Decode_Then_Decoded()
  {
    string passwordString = _faker.Internet.Password();
    string encoded = new Pbkdf2Password(passwordString, _settings.Algorithm, _settings.Iterations, _settings.SaltLength, _settings.HashLength).Encode();

    Password decoded = _strategy.Decode(encoded);

    Pbkdf2Password pbkdf2 = Assert.IsType<Pbkdf2Password>(decoded);
    Assert.True(pbkdf2.IsMatch(passwordString));
  }

  [Fact(DisplayName = "Hash: it should hash a password.")]
  public void Given_Password_When_Hash_Then_Hashed()
  {
    string passwordString = _faker.Internet.Password();

    Password password = _strategy.Hash(passwordString);

    Pbkdf2Password pbkdf2 = Assert.IsType<Pbkdf2Password>(password);
    Assert.True(pbkdf2.IsMatch(passwordString));

    Assert.Equal(_settings.Algorithm, pbkdf2.GetType().GetField("_algorithm", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(pbkdf2));
    Assert.Equal(_settings.Iterations, pbkdf2.GetType().GetField("_iterations", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(pbkdf2));
    Assert.Equal(_settings.SaltLength, (pbkdf2.GetType().GetField("_salt", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(pbkdf2) as byte[])?.Length);
    Assert.Equal(_settings.HashLength ?? _settings.SaltLength, (pbkdf2.GetType().GetField("_hash", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(pbkdf2) as byte[])?.Length);
  }
}
