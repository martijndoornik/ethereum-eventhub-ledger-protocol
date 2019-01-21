using System;
using System.Collections.Generic;
using System.Text;

namespace AzureEventhubProtocol.Connect
{
    public class EventhubConnectionTemplate
    {
        public string EventHubName { get; set; }
        public string EventhubNamespace { get; set; }
        public string SharedAccessKey { get; set; }
        public string SharedAccessKeyName { get; set; }
        public string ConsumerGroupName { get; set; }
        public string ListenerName { get; set; }
        public string ListenerVersion { get; set; }

        public static EventhubConnectionTemplate FromArgumentList(
            string[] args,
            string listenerName,
            string listenerVersion)
        {
            Console.WriteLine("Setting up arguments for connection...");
            var template = new EventhubConnectionTemplate
            {
                ListenerName = listenerName,
                ListenerVersion = listenerVersion
            };
            for (var i = 0; i < args.Length; i++)
            {
                if (i + 1 >= args.Length) continue;
                switch (args[i].ToLower())
                {
                    case "-n":
                    case "--eventhubName":
                        template.EventHubName = args[++i];
                        Console.WriteLine("Eventhub Name retrieved.");
                        break;
                    case "-ns":
                    case "--eventhubNameSpace":
                        template.EventhubNamespace = args[++i];
                        Console.WriteLine("Eventhub Namespace retrieved.");
                        break;
                    case "-k":
                    case "--sharedAccessKey":
                        template.SharedAccessKey = args[++i];
                        Console.WriteLine("Shared Access Key retrieved.");
                        break;
                    case "-kn":
                    case "--sharedAccessKeyName":
                        template.SharedAccessKeyName = args[++i];
                        Console.WriteLine("Shared Access Keyname retrieved.");
                        break;
                    case "-cgn":
                    case "--consumerGroupName":
                        template.ConsumerGroupName = args[++i];
                        Console.WriteLine("Consumer Groupname retrieved.");
                        break;
                }
            }
            if (template.EventhubNamespace == null)
            {
                Console.WriteLine("Eventhub Namespace missing...");
            }
            if (template.EventHubName == null)
            {
                Console.WriteLine("Eventhub Name missing...");
            }
            if (template.SharedAccessKey == null) 
            {
                Console.WriteLine("Shared Access Key missing...");
            }
            if (template.SharedAccessKeyName == null)
            {
                Console.WriteLine("Shared Access Key missing...");
            }
            if (template.ConsumerGroupName == null)
            {
                Console.WriteLine("Consumer Groupname missing...");
            }
            Console.WriteLine($"Finished setting up template for connection as '{listenerName}' (version: {listenerVersion})");
            return template;
        }
    }
}
