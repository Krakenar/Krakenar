using Krakenar.Core.Messages;
using Krakenar.Core.Templates;
using Logitar.Net.Mail;

namespace Krakenar.Infrastructure.Messages.Providers;

public interface IMessageHandler : IDisposable
{
  Task<SendMailResult> SendAsync(Message message, Content body, CancellationToken cancellationToken = default);
}
