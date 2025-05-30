﻿using Krakenar.Contracts.Messages;
using Krakenar.Contracts.Search;
using MessageDto = Krakenar.Contracts.Messages.Message;

namespace Krakenar.Core.Messages;

public interface IMessageQuerier
{
  Task<MessageDto> ReadAsync(Message message, CancellationToken cancellationToken = default);
  Task<MessageDto?> ReadAsync(MessageId id, CancellationToken cancellationToken = default);
  Task<MessageDto?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<MessageDto>> SearchAsync(SearchMessagesPayload payload, CancellationToken cancellationToken = default);
}
