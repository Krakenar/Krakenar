using Krakenar.Core.Messages;
using Logitar.Net.Mail;

namespace Krakenar.Infrastructure.Messages.Providers;

public interface IMessageHandler : IDisposable
{
  Task<SendMailResult> SendAsync(Message message, CancellationToken cancellationToken = default);
}
