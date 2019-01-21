
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using AzureEventhubProtocol.Exceptions;
using AzureEventhubProtocol.Receive;
using AzureEventhubProtocol.Send;
using AzureEventhubProtocol.Constants.EventNames;

namespace AzureEventhubProtocol.Connect
{
    public class EventhubConnection : IDisposable
    {
        private const string EventhubUriTemplate = "sb://{0}.servicebus.windows.net";

        private static EventhubConnection _instance;
        private readonly MessageSender eventhubSender;
        public readonly string ListenerName;
        public readonly string ListenerVersion;

        private EventhubConnection(
            string eventhubNamespace,
            string eventHubName,
            string sharedAccessKeyName,
            string sharedAccessKey,
            string consumerGroupName,
            string listenerName,
            string listenerVersion,
            Func<EventMessage, Task> onMessage,
            Func<Exception, Task> onError = null)
        {
            ListenerName = listenerName;
            ListenerVersion = listenerVersion;
            var endpoint = string.Format(EventhubUriTemplate, eventhubNamespace);
            var receiverConnectionStringBuilder = new ServiceBusConnectionStringBuilder(endpoint, $"{eventHubName}/ConsumerGroups/{consumerGroupName}/Partitions/1", sharedAccessKeyName, sharedAccessKey);
            var receiver = new MessageReceiver(receiverConnectionStringBuilder, ReceiveMode.ReceiveAndDelete);
            receiver.RegisterMessageHandler(async (message, cancellationToken) =>
            {
                try
                {
                    var eventMessage = EventMessage.Convert(message);
                    switch(eventMessage.EventName)
                    {
                        case EventNames.STATUS_REQUEST:
                            await SendStatusMessage();
                            break;
                    }
                    await onMessage(eventMessage);
                }
                catch (InvalidEventConversion e)
                {
                    if (onError != null)
                    {
                        await onError(e);
                    }
                }
            }, 
                new MessageHandlerOptions((exceptions) =>
            {
                Console.WriteLine($"Error received: {exceptions.Exception.Message}");
                return Task.FromResult<object>(null);
            }));
            var senderConnectionStringBuilder = new ServiceBusConnectionStringBuilder(endpoint, $"{eventHubName}/Partitions/1", sharedAccessKeyName, sharedAccessKey);
            eventhubSender = new MessageSender(senderConnectionStringBuilder);
            SendOnlineMessage().GetAwaiter().GetResult();
        }

        public static EventhubConnection Connect(
            string eventhubNamespace,
            string eventHubName,
            string sharedAccessKeyName,
            string sharedAccessKey,
            string consumerGroupName,
            string listenerName,
            string listenerVersion,
            Func<EventMessage, Task> onMessage,
            Func<Exception, Task> onError = null)
        {
            if (_instance == null)
            {
                Console.WriteLine("Trying to make a connection to Azure Eventhubs...");
                _instance = new EventhubConnection(
                    eventhubNamespace,
                    eventHubName,
                    sharedAccessKeyName,
                    sharedAccessKey,
                    consumerGroupName,
                    listenerName,
                    listenerVersion,
                    onMessage);
                Console.WriteLine("Connection made! Eventhubs now available.");
            }
            else
            {
                Console.Error.WriteLine("Attempted to connect while already having an active connection.");
            }
            return _instance;
        }

        public static EventhubConnection Connect(
            EventhubConnectionTemplate template,
            Func<EventMessage, Task> onMessage,
            Func<Exception, Task> onError = null)
        {
            return Connect(
                template.EventhubNamespace,
                template.EventHubName,
                template.SharedAccessKeyName,
                template.SharedAccessKey,
                template.ConsumerGroupName,
                template.ListenerName,
                template.ListenerVersion,
                onMessage);
        }

        public async void Dispose()
        {
            await SendOfflineMessage();
        }

        public static bool HasConnection()
        {
            return _instance != null;
        }

        public async Task SendMessage(
            PreparedEventMessage message)
        {
            await eventhubSender.SendAsync(message.ToMessage(ListenerName, ListenerVersion));
        }

        private async Task SendOfflineMessage()
        {
            var message = new PreparedEventMessage(EventNames.SIGN_OUT);
            await SendMessage(message);
        }

        private async Task SendOnlineMessage()
        {
            var message = new PreparedEventMessage(EventNames.REGISTER);
            await SendMessage(message);
        }

        public async Task SendStatusMessage()
        {
            var newMessage = new PreparedEventMessage(EventNames.STATUS_RESPONSE)
            {
                Body = new
                {
                    status = "OK",
                    message = "Nothing wrong here"
                }
            };
            await SendMessage(newMessage);
        }
    }
}
