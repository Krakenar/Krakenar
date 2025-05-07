using Krakenar.Core.Localization;
using Krakenar.Core.Messages;
using Krakenar.Core.Senders;
using Krakenar.Core.Templates;
using Krakenar.Core.Users;
using Logitar.EventSourcing;
using RazorEngine;
using RazorEngine.Templating;

namespace Krakenar.Infrastructure.Messages;

public class MessageManager : IMessageManager
{
  public virtual Task<Content> CompileAsync(MessageId messageId, Template template, Dictionaries dictionaries, Locale? locale, User? user, Variables variables, CancellationToken cancellationToken)
  {
    Content content = template.Content;
    TemplateModel model = new(dictionaries, locale, user, variables);
    string text = Engine.Razor.RunCompile(content.Text, name: messageId.EntityId.ToString(), typeof(TemplateModel), model);
    return Task.FromResult(content.Create(text));
  }

  public virtual Task SendAsync(Message message, Sender sender, ActorId? actorId, CancellationToken cancellationToken)
  {
    return Task.CompletedTask; // TODO(fpion): implement
  }
}
