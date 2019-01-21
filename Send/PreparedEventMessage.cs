using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace AzureEventhubProtocol.Send
{
    public class PreparedEventMessage
    {
        public object Body { get; set; }
        public string EventName { get; set; }

        public PreparedEventMessage(string eventName)
        {
            EventName = eventName;
        }

        internal Message ToMessage(string listener, string listenerVersion)
        {
            Message message;
            if (Body != null)
            {
                var json = JsonConvert.SerializeObject(Body);
                message = new Message(Encoding.UTF8.GetBytes(json));
            } else
            {
                message = new Message();
            }
            message.UserProperties["eventName"] = EventName;
            message.UserProperties["listener"] = listener;
            message.UserProperties["listenerVersion"] = listenerVersion;
            
            return message;
        }
    }
}
