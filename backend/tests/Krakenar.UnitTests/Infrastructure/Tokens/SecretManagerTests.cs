using Krakenar.Core;
using Krakenar.Core.Encryption;
using Krakenar.Core.Realms;
using Krakenar.Core.Tokens;
using Logitar.Security.Cryptography;
using Moq;

namespace Krakenar.Infrastructure.Tokens;

[Trait(Traits.Category, Categories.Unit)]
public class SecretManagerTests
{
  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IEncryptionManager> _encryptionManager = new();

  private readonly SecretManager _helper;

  public SecretManagerTests()
  {
    _helper = new(_applicationContext.Object, _encryptionManager.Object);
  }

  [Theory(DisplayName = "Decrypt: it should decrypt a token secret.")]
  [InlineData(null)]
  [InlineData("00b999e6-9f62-40d4-9804-b90813a81a73")]
  public void Given_Encrypted_When_Decrypt_Then_Decrypted(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue is null ? null : new(Guid.Parse(realmIdValue));
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    string secret = RandomStringGenerator.GetString(Secret.MinimumLength);
    Secret encrypted = new(Convert.ToBase64String(Encoding.ASCII.GetBytes(secret)));

    _encryptionManager.Setup(x => x.Decrypt(It.Is<EncryptedString>(s => s.Value == encrypted.Value), realmId)).Returns(secret);

    string decrypted = _helper.Decrypt(encrypted, realmId);
    Assert.Equal(secret, decrypted);
  }

  [Theory(DisplayName = "Encrypt: it should encrypt a token secret.")]
  [InlineData(null)]
  [InlineData("fcc15961-2b98-4385-b9ed-381b24850455")]
  public void Given_Secret_When_Encrypt_Then_Encrypted(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue is null ? null : new(Guid.Parse(realmIdValue));

    string secret = RandomStringGenerator.GetString(Secret.MinimumLength);
    Secret expected = new(Convert.ToBase64String(Encoding.ASCII.GetBytes(secret)));
    _encryptionManager.Setup(x => x.Encrypt(secret, realmId)).Returns(new EncryptedString(expected.Value));

    Secret encrypted = _helper.Encrypt(secret, realmId);

    Assert.Equal(expected, encrypted);
  }

  [Theory(DisplayName = "Generate: it should generate a random token secret.")]
  [InlineData(null)]
  [InlineData("a5e97f1f-620f-46bb-ad86-1415232d0d97")]
  public void Given_RealmId_When_Generate_Then_Generated(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue is null ? null : new(Guid.Parse(realmIdValue));

    string secret = string.Empty;
    EncryptedString expected = new(RandomStringGenerator.GetString(Secret.MinimumLength));
    _encryptionManager.Setup(x => x.Encrypt(It.IsAny<string>(), realmId))
      .Callback<string, RealmId?>((s, _) => secret = s)
      .Returns(expected);

    Secret encrypted = _helper.Generate(realmId);

    Assert.Equal(expected.Value, encrypted.Value);
    Assert.Equal(Secret.MinimumLength, secret.Length);
  }

  [Theory(DisplayName = "It should generate, encrypt, then decrypt a secret correctly.")]
  [InlineData(null)]
  [InlineData("fba7555a-f0dc-44b9-8bf6-b8396bfaf9b8")]
  public void Given_Secret_When_Helper_Then_CorrectResult(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue is null ? null : new(Guid.Parse(realmIdValue));
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    string secretString = string.Empty;
    EncryptedString encrypted = new(RandomStringGenerator.GetString(Secret.MinimumLength));
    _encryptionManager.Setup(x => x.Encrypt(It.IsAny<string>(), realmId))
      .Callback<string, RealmId?>((s, _) => secretString = s)
      .Returns(encrypted);

    Secret secret = _helper.Generate(realmId);

    _encryptionManager.Setup(x => x.Decrypt(It.Is<EncryptedString>(s => s.Value == encrypted.Value), realmId)).Returns(secretString);
    string decrypted = _helper.Decrypt(secret, realmId);

    Assert.Equal(secretString, decrypted);
    Assert.Equal(Secret.MinimumLength, decrypted.Length);
  }

  [Theory(DisplayName = "Resolve: it should return the decrypted configuration secret when the input is empty.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_Empty_When_Resolve_Then_RealmDecrypted(string? value)
  {
    string secret = "f6gMmrwcEHy3QtsbGWFDJLCuT2ZYU5qS";
    Secret encrypted = new(Convert.ToBase64String(Encoding.ASCII.GetBytes(secret)));
    _applicationContext.SetupGet(x => x.Secret).Returns(encrypted);

    _encryptionManager.Setup(x => x.Decrypt(It.Is<EncryptedString>(s => s.Value == encrypted.Value), null)).Returns(secret);

    string resolved = _helper.Resolve(value);
    Assert.Equal(secret, resolved);
  }

  [Theory(DisplayName = "Resolve: it should return the decrypted realm secret when the input is empty.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_EmptyAndRealm_When_Resolve_Then_RealmDecrypted(string? value)
  {
    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    string secret = "f6gMmrwcEHy3QtsbGWFDJLCuT2ZYU5qS";
    Secret encrypted = new(Convert.ToBase64String(Encoding.ASCII.GetBytes(secret)));
    _applicationContext.SetupGet(x => x.Secret).Returns(encrypted);

    _encryptionManager.Setup(x => x.Decrypt(It.Is<EncryptedString>(s => s.Value == encrypted.Value), realmId)).Returns(secret);

    string resolved = _helper.Resolve(value);
    Assert.Equal(secret, resolved);
  }

  [Theory(DisplayName = "Resolve: it should return the input secret when it is not empty.")]
  [InlineData("y7Umw4zqQvnFP6CTWHxGSMpDt8cE2hV3")]
  [InlineData("  FtgWYvby=~E*naqR_96kpQUP2s{$xczr>j8+B3S`:@%#  ")]
  public void Given_NotEmpty_When_Resolve_Then_Trimmed(string value)
  {
    string resolved = _helper.Resolve(value);
    Assert.Equal(value.Trim(), resolved);
  }
}
