using Bogus;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Krakenar.Core.Passwords;
using Krakenar.Infrastructure.Passwords.Pbkdf2;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Moq;

namespace Krakenar.Infrastructure.Passwords;

public class PasswordServiceTests
{
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();

  private readonly PasswordService _manager;

  private readonly PasswordSettings _passwordSettings = new();

  public PasswordServiceTests()
  {
    IPasswordStrategy[] strategies =
    [
      new Base64Stragegy(),
      new Pbkdf2Strategy(new Pbkdf2Settings())
    ];
    _manager = new(_applicationContext.Object, strategies);

    _applicationContext.SetupGet(x => x.PasswordSettings).Returns(_passwordSettings);
  }

  [Fact(DisplayName = "Decode: it should decode passwords from various strategies.")]
  public void Given_Password_When_Decode_Then_Decoded()
  {
    string password = _faker.Internet.Password();

    Pbkdf2Password pbkdf2 = new(password, KeyDerivationPrf.HMACSHA256, iterations: 600000, saltLength: 256 / 8);
    Password decoded = _manager.Decode(pbkdf2.Encode());
    pbkdf2 = Assert.IsType<Pbkdf2Password>(decoded);
    Assert.True(pbkdf2.IsMatch(password));

    Base64Password base64 = new(password);
    decoded = _manager.Decode(base64.Encode());
    base64 = Assert.IsType<Base64Password>(decoded);
    Assert.True(base64.IsMatch(password));
  }

  [Fact(DisplayName = "Decode: it should throw PasswordStrategyNotSupportedException when the strategy is not supported.")]
  public void Given_NoStrategy_When_Decode_Then_PasswordStrategyNotSupportedException()
  {
    string encoded = "invalid:strategy";
    var exception = Assert.Throws<PasswordStrategyNotSupportedException>(() => _manager.Decode(encoded));
    Assert.Equal("invalid", exception.Strategy);
  }

  [Theory(DisplayName = "Generate: it should generate a password from the specified characters of the specified length.")]
  [InlineData(20)]
  public void Given_LengthAndCharacters_When_Generate_Then_Generated(int length)
  {
    string characters = "0123456789";

    Password password = _manager.Generate(characters, length, out string passwordString);
    Assert.IsType<Pbkdf2Password>(password);
    Assert.True(passwordString.All(characters.Contains));
    Assert.Equal(length, passwordString.Length);
    Assert.True(password.IsMatch(passwordString));
  }

  [Theory(DisplayName = "Generate: it should generate a password of the specified length.")]
  [InlineData(6)]
  public void Given_Length_When_Generate_Then_Generated(int length)
  {
    Password password = _manager.Generate(length, out string passwordString);
    Assert.IsType<Pbkdf2Password>(password);
    Assert.Equal(length, passwordString.Length);
    Assert.True(password.IsMatch(passwordString));
  }

  [Theory(DisplayName = "GenerateBase64: it should generate a base64 password of the specified length.")]
  [InlineData(32)]
  public void Given_Length_When_GenerateBase64_Then_Generated(int length)
  {
    Password password = _manager.GenerateBase64(length, out string passwordString);
    Assert.IsType<Pbkdf2Password>(password);
    Assert.True(password.IsMatch(passwordString));

    byte[] bytes = Convert.FromBase64String(passwordString);
    Assert.Equal(length, bytes.Length);
  }

  [Fact(DisplayName = "Hash: it should hash the password.")]
  public void Given_Password_When_Hash_Then_Hashed()
  {
    string passwordString = _faker.Internet.Password();

    Password password = _manager.Hash(passwordString);
    Assert.IsType<Pbkdf2Password>(password);
    Assert.True(password.IsMatch(passwordString));
  }

  [Fact(DisplayName = "Hash: it should throw PasswordStrategyNotSupportedException when the strategy is not supported.")]
  public void Given_NoStrategy_When_Hash_Then_PasswordStrategyNotSupportedException()
  {
    _passwordSettings.HashingStrategy = "invalid";

    var exception = Assert.Throws<PasswordStrategyNotSupportedException>(() => _manager.Hash(_faker.Internet.Password()));
    Assert.Equal(_passwordSettings.HashingStrategy, exception.Strategy);
  }

  [Fact(DisplayName = "Validate: it should throw ValidationException when the password is not valid.")]
  public void Given_Invalid_When_Validate_Then_ValidationException()
  {
    var exception = Assert.Throws<FluentValidation.ValidationException>(() => _manager.Validate("secret"));
    Assert.Equal(5, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordTooShort"
      && e.ErrorMessage == "Passwords must be at least 8 characters."
      && e.PropertyName == "Password");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresUniqueChars"
      && e.ErrorMessage == "Passwords must use at least 8 different characters."
      && e.PropertyName == "Password");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresNonAlphanumeric"
      && e.ErrorMessage == "Passwords must have at least one non alphanumeric character."
      && e.PropertyName == "Password");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresUpper"
      && e.ErrorMessage == "Passwords must have at least one uppercase ('A'-'Z')."
      && e.PropertyName == "Password");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresDigit"
      && e.ErrorMessage == "Passwords must have at least one digit ('0'-'9')."
      && e.PropertyName == "Password");
  }

  [Fact(DisplayName = "Validate: it should validate the password.")]
  public void Given_Valid_When_Validate_Then_Validated()
  {
    _passwordSettings.RequiredLength = 6;
    _passwordSettings.RequiredUniqueChars = 1;
    _passwordSettings.RequireNonAlphanumeric = false;
    _passwordSettings.RequireLowercase = false;
    _passwordSettings.RequireUppercase = false;
    _passwordSettings.RequireDigit = false;
    _manager.Validate("secret");
  }

  [Fact(DisplayName = "ValidateAndHash: it should throw PasswordStrategyNotSupportedException when the strategy is not supported.")]
  public void Given_NoStrategy_When_ValidateAndHash_Then_PasswordStrategyNotSupportedException()
  {
    _passwordSettings.RequiredLength = 0;
    _passwordSettings.RequiredUniqueChars = 0;
    _passwordSettings.RequireNonAlphanumeric = false;
    _passwordSettings.RequireLowercase = false;
    _passwordSettings.RequireUppercase = false;
    _passwordSettings.RequireDigit = false;
    _passwordSettings.HashingStrategy = "invalid";

    var exception = Assert.Throws<PasswordStrategyNotSupportedException>(() => _manager.ValidateAndHash(_faker.Internet.Password(), settings: null));
    Assert.Equal(_passwordSettings.HashingStrategy, exception.Strategy);
  }

  [Fact(DisplayName = "ValidateAndHash: it should throw ValidationException when the password is not valid.")]
  public void Given_Invalid_When_ValidateAndHash_Then_ValidationException()
  {
    var exception = Assert.Throws<FluentValidation.ValidationException>(() => _manager.ValidateAndHash("secret", settings: null));
    Assert.Equal(5, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordTooShort"
      && e.ErrorMessage == "Passwords must be at least 8 characters."
      && e.PropertyName == "Password");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresUniqueChars"
      && e.ErrorMessage == "Passwords must use at least 8 different characters."
      && e.PropertyName == "Password");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresNonAlphanumeric"
      && e.ErrorMessage == "Passwords must have at least one non alphanumeric character."
      && e.PropertyName == "Password");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresUpper"
      && e.ErrorMessage == "Passwords must have at least one uppercase ('A'-'Z')."
      && e.PropertyName == "Password");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresDigit"
      && e.ErrorMessage == "Passwords must have at least one digit ('0'-'9')."
      && e.PropertyName == "Password");
  }

  [Fact(DisplayName = "ValidateAndHash: it should validate and hash the password.")]
  public void Given_Valid_When_ValidateAndHash_Then_ValidatedAndHashed()
  {
    string passwordString = "Test123!";
    Password password = _manager.ValidateAndHash(passwordString, settings: null);
    Assert.IsType<Pbkdf2Password>(password);
    Assert.True(password.IsMatch(passwordString));

    PasswordSettings settings = new(requiredLength: 6, requiredUniqueChars: 1, requireNonAlphanumeric: false, requireLowercase: false, requireUppercase: false, requireDigit: false, Base64Password.Key);
    passwordString = "secret";
    password = _manager.ValidateAndHash(passwordString, settings);
    Assert.IsType<Base64Password>(password);
    Assert.True(password.IsMatch(passwordString));
  }
}
