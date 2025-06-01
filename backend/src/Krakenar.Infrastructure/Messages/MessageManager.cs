using Krakenar.Contracts;
using Krakenar.Contracts.Senders;
using Krakenar.Core.Encryption;
using Krakenar.Core.Localization;
using Krakenar.Core.Messages;
using Krakenar.Core.Senders;
using Krakenar.Core.Senders.Settings;
using Krakenar.Core.Templates;
using Krakenar.Core.Users;
using Krakenar.Infrastructure.Messages.Providers;
using Logitar.EventSourcing;
using Logitar.Net.Mail;
using RazorEngine;
using RazorEngine.Templating;
using Sender = Krakenar.Core.Senders.Sender;

namespace Krakenar.Infrastructure.Messages;

public class MessageManager : IMessageManager
{
  protected virtual IEncryptionManager EncryptionManager { get; }
  protected virtual JsonSerializerOptions SerializerOptions { get; } = new();
  protected virtual Dictionary<SenderProvider, IProviderStrategy> Strategies { get; } = [];

  public MessageManager(IEncryptionManager encryptionManager, IEnumerable<IProviderStrategy> strategies)
  {
    EncryptionManager = encryptionManager;

    foreach (IProviderStrategy strategy in strategies)
    {
      Strategies[strategy.Provider] = strategy;
    }

    SerializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  public virtual Task<Content> CompileAsync(MessageId messageId, Template template, Dictionaries dictionaries, Locale? locale, User? user, Variables variables, CancellationToken cancellationToken)
  {
    Content content = template.Content;
    TemplateModel model = new(dictionaries, locale, user, variables);
    string text = Engine.Razor.RunCompile(content.Text, name: messageId.EntityId.ToString(), typeof(TemplateModel), model);
    return Task.FromResult(content.Create(text));
  }

  public virtual async Task SendAsync(Message message, Sender sender, ActorId? actorId, CancellationToken cancellationToken)
  {
    if (!Strategies.TryGetValue(sender.Provider, out IProviderStrategy? strategy))
    {
      throw new SenderProviderNotSupportedException(sender.Provider);
    }

    SenderSettings settings = EncryptionManager.DecryptSettings(sender);
    IMessageHandler messageHandler = strategy.Execute(settings);

    SendMailResult result;
    try
    {
      Content body = EncryptionManager.DecryptBody(message);
      result = await messageHandler.SendAsync(message, body, cancellationToken);
    }
    catch (Exception exception)
    {
      result = ToResult(exception);
    }

    IReadOnlyDictionary<string, string> resultData = SerializeData(result);
    if (result.Succeeded)
    {
      message.Succeed(resultData, actorId);
    }
    else
    {
      message.Fail(resultData, actorId);
    }
  }

  private Dictionary<string, string> SerializeData(SendMailResult result)
  {
    Dictionary<string, string> resultData = new(capacity: result.Data.Count);
    foreach (KeyValuePair<string, object?> data in result.Data)
    {
      if (data.Value is not null)
      {
        resultData[data.Key] = JsonSerializer.Serialize(data.Value, data.Value.GetType(), SerializerOptions).Trim('"');
      }
    }
    return resultData;
  }

  private static SendMailResult ToResult(Exception exception) => new(succeeded: false, new Dictionary<string, object?>
  {
    ["Error"] = new Error(exception)
  });
}
