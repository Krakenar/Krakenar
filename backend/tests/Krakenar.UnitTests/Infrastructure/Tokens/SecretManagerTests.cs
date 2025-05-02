using Krakenar.Core;
using Krakenar.Core.Realms;
using Krakenar.Core.Tokens;
using Krakenar.Infrastructure.Settings;
using Logitar.Security.Cryptography;
using Moq;

namespace Krakenar.Infrastructure.Tokens;

[Trait(Traits.Category, Categories.Unit)]
public class SecretManagerTests
{
  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly EncryptionSettings _settings = new()
  {
    Key = "E8Qh3xq7vSas9KdPpL2XFfeRt6mATMCc"
  };

  private readonly SecretManager _helper;

  public SecretManagerTests()
  {
    _helper = new(_applicationContext.Object, _settings);
  }

  [Theory(DisplayName = "Decrypt: it should decrypt a token secret.")]
  [InlineData(null)]
  [InlineData("00b999e6-9f62-40d4-9804-b90813a81a73")]
  public void Given_Encrypted_When_Decrypt_Then_Decrypted(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue == null ? null : new(Guid.Parse(realmIdValue));
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    string secret = RandomStringGenerator.GetString(Secret.MinimumLength);
    Secret encrypted = Encrypt(secret, realmId);

    string decrypted = _helper.Decrypt(encrypted, realmId);

    Assert.Equal(secret, decrypted);
  }

  [Theory(DisplayName = "Encrypt: it should encrypt a token secret.")]
  [InlineData(null)]
  [InlineData("fcc15961-2b98-4385-b9ed-381b24850455")]
  public void Given_Secret_When_Encrypt_Then_Encrypted(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue == null ? null : new(Guid.Parse(realmIdValue));

    string secret = RandomStringGenerator.GetString(Secret.MinimumLength);
    Secret encrypted = _helper.Encrypt(secret, realmId);

    byte[] iv = GetIV(encrypted);
    Secret expected = Encrypt(secret, realmId, iv);
    Assert.Equal(expected, encrypted);
  }

  [Theory(DisplayName = "Generate: it should generate a random token secret.")]
  [InlineData(null)]
  [InlineData("a5e97f1f-620f-46bb-ad86-1415232d0d97")]
  public void Given_RealmId_When_Generate_Then_Generated(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue == null ? null : new(Guid.Parse(realmIdValue));

    Secret encrypted = _helper.Generate(realmId);

    string decrypted = Decrypt(encrypted, realmId);
    Assert.Equal(Secret.MinimumLength, decrypted.Length);
  }

  [Theory(DisplayName = "It should generate, encrypt, then decrypt a secret correctly.")]
  [InlineData(null)]
  [InlineData("fba7555a-f0dc-44b9-8bf6-b8396bfaf9b8")]
  public void Given_Secret_When_Helper_Then_CorrectResult(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue == null ? null : new(Guid.Parse(realmIdValue));
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    Secret encrypted = _helper.Generate(realmId);
    string decrypted = _helper.Decrypt(encrypted, realmId);
    Assert.Equal(Secret.MinimumLength, decrypted.Length);
  }

  [Theory(DisplayName = "Resolve: it should return the decrypted configuration secret when the input is empty.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_Empty_When_Resolve_Then_RealmDecrypted(string? value)
  {
    string secret = "f6gMmrwcEHy3QtsbGWFDJLCuT2ZYU5qS";
    Secret encrypted = Encrypt(secret, realmId: null);
    _applicationContext.SetupGet(x => x.Secret).Returns(encrypted);

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
    Secret encrypted = Encrypt(secret, realmId);
    _applicationContext.SetupGet(x => x.Secret).Returns(encrypted);

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

  private string Decrypt(Secret secret, RealmId? realmId)
  {
    byte[] bytes = Convert.FromBase64String(secret.Value);
    byte length = bytes.First();
    byte[] iv = bytes.Skip(1).Take(length).ToArray();
    byte[] encryptedBytes = bytes.Skip(1 + length).ToArray();

    using Aes aes = Aes.Create();
    using ICryptoTransform decryptor = aes.CreateDecryptor(GetEncryptionKey(realmId), iv);
    using MemoryStream encryptedStream = new(encryptedBytes);
    using CryptoStream cryptoStream = new(encryptedStream, decryptor, CryptoStreamMode.Read);

    using MemoryStream decryptedStream = new();
    cryptoStream.CopyTo(decryptedStream);
    return Encoding.UTF8.GetString(decryptedStream.ToArray());
  }
  private Secret Encrypt(string secret, RealmId? realmId, byte[]? iv = null)
  {
    using Aes aes = Aes.Create();
    iv ??= aes.IV;
    using ICryptoTransform encryptor = aes.CreateEncryptor(GetEncryptionKey(realmId), iv);
    using MemoryStream encryptedStream = new();
    using CryptoStream cryptoStream = new(encryptedStream, encryptor, CryptoStreamMode.Write);
    byte[] data = Encoding.UTF8.GetBytes(secret);
    cryptoStream.Write(data, 0, data.Length);
    cryptoStream.FlushFinalBlock();
    byte[] encryptedBytes = encryptedStream.ToArray();

    byte length = (byte)iv.Length;
    string encrypted = Convert.ToBase64String(new byte[] { length }.Concat(iv).Concat(encryptedBytes).ToArray());
    return new Secret(encrypted);
  }
  private byte[] GetEncryptionKey(RealmId? realmId)
  {
    byte[] key = HKDF.Extract(HashAlgorithmName.SHA256, Encoding.UTF8.GetBytes(_settings.Key));
    if (realmId.HasValue)
    {
      key = HKDF.Expand(HashAlgorithmName.SHA256, key, key.Length, realmId.Value.ToGuid().ToByteArray());
    }
    return key;
  }
  private static byte[] GetIV(Secret secret)
  {
    byte[] bytes = Convert.FromBase64String(secret.Value);
    byte length = bytes.First();
    return bytes.Skip(1).Take(length).ToArray();
  }
}
