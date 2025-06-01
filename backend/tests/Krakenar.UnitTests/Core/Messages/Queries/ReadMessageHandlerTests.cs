using Krakenar.Contracts.Messages;
using Krakenar.Contracts.Templates;
using Krakenar.Core.Encryption;
using Moq;
using MessageDto = Krakenar.Contracts.Messages.Message;

namespace Krakenar.Core.Messages.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadMessageHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IEncryptionManager> _encryptionManager = new();
  private readonly Mock<IMessageQuerier> _messageQuerier = new();

  private readonly ReadMessageHandler _handler;

  private readonly MessageDto _message = new()
  {
    Id = Guid.NewGuid()
  };

  public ReadMessageHandlerTests()
  {
    _handler = new(_encryptionManager.Object, _messageQuerier.Object);

    _messageQuerier.Setup(x => x.ReadAsync(_message.Id, _cancellationToken)).ReturnsAsync(_message);
  }

  [Fact(DisplayName = "It should return null when the message was not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    ReadMessage query = new(Guid.Empty);
    Assert.Null(await _handler.HandleAsync(query, _cancellationToken));
  }

  [Fact(DisplayName = "It should return the message found by ID.")]
  public async Task Given_FoundById_When_HandleAsync_Then_MessageReturned()
  {
    string bodyDecrypted = "Hello World!";
    string bodyEncrypted = Convert.ToBase64String(Encoding.ASCII.GetBytes(bodyDecrypted));
    _message.Body = Content.PlainText(bodyEncrypted);

    string codeDecrypted = "17946";
    string codeEncrypted = Convert.ToBase64String(Encoding.ASCII.GetBytes(codeDecrypted));
    _message.Variables.Add(new Variable("Code", codeEncrypted));

    _encryptionManager.Setup(x => x.Decrypt(It.Is<EncryptedString>(s => s.Value == bodyEncrypted), null)).Returns(bodyDecrypted);
    _encryptionManager.Setup(x => x.Decrypt(It.Is<EncryptedString>(s => s.Value == codeEncrypted), null)).Returns(codeDecrypted);

    ReadMessage query = new(_message.Id);
    MessageDto? message = await _handler.HandleAsync(query, _cancellationToken);

    Assert.NotNull(message);
    Assert.Same(_message, message);

    Assert.Equal(bodyDecrypted, message.Body.Text);

    Variable variable = Assert.Single(message.Variables);
    Assert.Equal("Code", variable.Key);
    Assert.Equal(codeDecrypted, variable.Value);
  }
}
