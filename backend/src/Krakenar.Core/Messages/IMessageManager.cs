using Krakenar.Core.Localization;
using Krakenar.Core.Senders;
using Krakenar.Core.Templates;
using Krakenar.Core.Users;
using Logitar.EventSourcing;

namespace Krakenar.Core.Messages;

public interface IMessageManager
{
  Task<Content> CompileAsync(MessageId messageId, Template template, Dictionaries dictionaries, Locale? locale, User? user, Variables variables, CancellationToken cancellationToken = default);
  Task SendAsync(Message message, Sender sender, ActorId? actorId = null, CancellationToken cancellationToken = default);
}
