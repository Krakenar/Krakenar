using Bogus;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Krakenar.Infrastructure.Passwords.Pbkdf2;

[Trait(Traits.Category, Categories.Unit)]
public class Pbkdf2PasswordTests
{
  private readonly Faker _faker = new();

  [Fact(DisplayName = "Decode: it should decode a PBKDF2 encoded password.")]
  public void Given_Valid_When_Decode_Then_Decoded()
  {
    string value = _faker.Internet.Password();

    KeyDerivationPrf algorithm = KeyDerivationPrf.HMACSHA256;
    int iterations = 600000;
    byte[] salt = RandomNumberGenerator.GetBytes(256 / 8);
    byte[] hash = KeyDerivation.Pbkdf2(value, salt, algorithm, iterations, salt.Length);
    string encoded = string.Join(':', "PBKDF2", algorithm, iterations, Convert.ToBase64String(salt), Convert.ToBase64String(hash));

    Pbkdf2Password password = Pbkdf2Password.Decode(encoded);

    Assert.Equal(algorithm, GetAlgorithm(password));
    Assert.Equal(iterations, GetIterations(password));
    Assert.Equal(salt, GetSalt(password));
    Assert.Equal(hash, GetHash(password));
  }

  [Theory(DisplayName = "Decode: it should throw ArgumentException when the value is not valid.")]
  [InlineData("invalid")]
  [InlineData("0:1:2:3:4")]
  public void Given_Invalid_When_Decode_Then_ArgumentException(string encoded)
  {
    var exception = Assert.Throws<ArgumentException>(() => Pbkdf2Password.Decode(encoded));
    Assert.StartsWith($"The value '{encoded}' is not a valid PBKDF2 password.", exception.Message);
    Assert.Equal("password", exception.ParamName);
  }

  [Fact(DisplayName = "Encode: it should return the correct encoded password.")]
  public void Given_Password_When_Encode_Then_Encoded()
  {
    string value = _faker.Internet.Password();
    KeyDerivationPrf algorithm = KeyDerivationPrf.HMACSHA256;
    int iterations = 600000;
    int saltLength = 256 / 8;
    Pbkdf2Password password = new(value, algorithm, iterations, saltLength);

    string encoded = password.Encode();

    string expected = string.Join(':', "PBKDF2", algorithm, iterations, Convert.ToBase64String(GetSalt(password)), Convert.ToBase64String(GetHash(password)));
    Assert.Equal(expected, encoded);
  }

  [Fact(DisplayName = "IsMatch: it should return false when the value is not correct.")]
  public void Given_Incorrect_When_IsMatch_Then_FalseReturned()
  {
    string value = _faker.Internet.Password();
    Pbkdf2Password password = new(value[..^1], KeyDerivationPrf.HMACSHA256, iterations: 600000, saltLength: 256 / 8);
    Assert.False(password.IsMatch(value));
  }

  [Fact(DisplayName = "IsMatch: it should return true when the value is correct.")]
  public void Given_Correct_When_IsMatch_Then_TrueReturned()
  {
    string value = _faker.Internet.Password();
    Pbkdf2Password password = new(value, KeyDerivationPrf.HMACSHA256, iterations: 600000, saltLength: 256 / 8);
    Assert.True(password.IsMatch(value));
  }

  [Theory(DisplayName = "It should construct the correct instance from arguments.")]
  [InlineData(false)]
  [InlineData(true)]
  public void Given_Arguments_When_ctor_Then_Constructed(bool withHashLength)
  {
    KeyDerivationPrf algorithm = withHashLength ? KeyDerivationPrf.HMACSHA512 : KeyDerivationPrf.HMACSHA256;
    int iterations = 600000;
    int saltLength = 256 / 8;
    int? hashLength = withHashLength ? (512 / 8) : null;

    string value = _faker.Internet.Password();
    Pbkdf2Password password = new(value, algorithm, iterations, saltLength, hashLength);

    Assert.Equal(algorithm, GetAlgorithm(password));
    Assert.Equal(iterations, GetIterations(password));
    Assert.Equal(saltLength, GetSalt(password).Length);
    Assert.Equal(hashLength ?? saltLength, GetHash(password).Length);
  }

  private static KeyDerivationPrf GetAlgorithm(Pbkdf2Password password)
  {
    FieldInfo? field = password.GetType().GetField("_algorithm", BindingFlags.NonPublic | BindingFlags.Instance);
    Assert.NotNull(field);

    KeyDerivationPrf? value = field.GetValue(password) as KeyDerivationPrf?;
    Assert.NotNull(value);
    return value.Value;
  }
  private static int GetIterations(Pbkdf2Password password)
  {
    FieldInfo? field = password.GetType().GetField("_iterations", BindingFlags.NonPublic | BindingFlags.Instance);
    Assert.NotNull(field);

    int? value = field.GetValue(password) as int?;
    Assert.NotNull(value);
    return value.Value;
  }
  private static byte[] GetSalt(Pbkdf2Password password)
  {
    FieldInfo? field = password.GetType().GetField("_salt", BindingFlags.NonPublic | BindingFlags.Instance);
    Assert.NotNull(field);

    byte[]? value = field.GetValue(password) as byte[];
    Assert.NotNull(value);
    return value;
  }
  private static byte[] GetHash(Pbkdf2Password password)
  {
    FieldInfo? field = password.GetType().GetField("_hash", BindingFlags.NonPublic | BindingFlags.Instance);
    Assert.NotNull(field);

    byte[]? value = field.GetValue(password) as byte[];
    Assert.NotNull(value);
    return value;
  }
}
