using Krakenar.Contracts;
using Krakenar.Contracts.Senders;
using Krakenar.Contracts.Senders.Settings;
using Krakenar.Contracts.Users;
using Moq;
using SenderDto = Krakenar.Contracts.Senders.Sender;

namespace Krakenar.Core.Senders.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadSenderHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ISenderQuerier> _senderQuerier = new();

  private readonly ReadSenderHandler _handler;

  private readonly SenderDto _sendGrid;
  private readonly SenderDto _twilio;

  public ReadSenderHandlerTests()
  {
    _handler = new(_senderQuerier.Object);

    _sendGrid = new SenderDto
    {
      Id = Guid.NewGuid(),
      Kind = SenderKind.Email,
      IsDefault = true,
      Email = new Email(""),
      SendGrid = new SendGridSettings(SenderHelper.GenerateSendGridSettings())
    };
    _twilio = new SenderDto
    {
      Id = Guid.NewGuid(),
      Kind = SenderKind.Phone,
      IsDefault = true,
      Phone = new Phone(countryCode: "CA", number: "1 (514) 845-4636", extension: null, e164Formatted: "+15148454636"),
      Twilio = new TwilioSettings(SenderHelper.GenerateTwilioSettings())
    };

    _senderQuerier.Setup(x => x.ReadAsync(_sendGrid.Id, _cancellationToken)).ReturnsAsync(_sendGrid);
    _senderQuerier.Setup(x => x.ReadAsync(_twilio.Id, _cancellationToken)).ReturnsAsync(_twilio);
    _senderQuerier.Setup(x => x.ReadDefaultAsync(_sendGrid.Kind, _cancellationToken)).ReturnsAsync(_sendGrid);
    _senderQuerier.Setup(x => x.ReadDefaultAsync(_twilio.Kind, _cancellationToken)).ReturnsAsync(_twilio);
  }

  [Fact(DisplayName = "It should return null when no sender was found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    ReadSender query = new(Guid.Empty, (SenderKind)(-1));
    Assert.Null(await _handler.HandleAsync(query, _cancellationToken));
  }

  [Fact(DisplayName = "It should return the sender found by ID.")]
  public async Task Given_FoundById_When_HandleAsync_Then_SenderReturned()
  {
    ReadSender query = new(_sendGrid.Id, (SenderKind)(-1));
    SenderDto? sender = await _handler.HandleAsync(query, _cancellationToken);

    Assert.NotNull(sender);
    Assert.Same(_sendGrid, sender);

    Assert.NotNull(query.Kind);
    _senderQuerier.Verify(x => x.ReadDefaultAsync(query.Kind.Value, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return the sender found by unique name.")]
  public async Task Given_FoundByUniqueName_When_HandleAsync_Then_SenderReturned()
  {
    ReadSender query = new(Guid.Empty, _sendGrid.Kind);
    SenderDto? sender = await _handler.HandleAsync(query, _cancellationToken);

    Assert.NotNull(sender);
    Assert.Same(_sendGrid, sender);

    Assert.True(query.Id.HasValue);
    _senderQuerier.Verify(x => x.ReadAsync(query.Id.Value, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when multiple senders were found.")]
  public async Task Given_MultipleFound_When_HandleAsync_Then_TooManyResultsException()
  {
    ReadSender query = new(_sendGrid.Id, _twilio.Kind);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<SenderDto>>(async () => await _handler.HandleAsync(query, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
