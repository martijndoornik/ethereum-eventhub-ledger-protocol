using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using AzureEventhubProtocol.Exceptions;

namespace AzureEventhubProtocol.Receive
{
    public class EventMessage
    {
        private DateTime _dateTime { get; set; }
        private string _eventName { get; set; }
        private long _id { get; set; }
        private IDictionary<string, object> _json { get; set; }
        private string _listener { get; set; }
        private string _listenerVersion { get; set; }
        private string _rawBody { get; set; }

        public DateTime DateTime => _dateTime;
        public string EventName => _eventName;
        public long Id => _id;
        public IDictionary<string, object> JSON => _json;
        public bool HasBody => _rawBody != null;
        public bool HasJson => _json != null;
        public string Listener => _listener;
        public string ListenerVersion => _listenerVersion;
        public string RawBody => _rawBody;

        private EventMessage()
        {

        }

        internal EventMessage(string EventName, object content)
        {

        }

        internal static EventMessage Convert(Message original)
        {
            if (original.UserProperties.ContainsKey("eventName"))
            {
                var dateTime = original.SystemProperties.EnqueuedTimeUtc;
                var eventName = original.UserProperties["eventName"].ToString();
                var id = original.SystemProperties.SequenceNumber;
                var listener = original.UserProperties["listener"]?.ToString();
                var listenerVersion = original.UserProperties["listenerVersion"]?.ToString();
                string rawBody = null;
                if (original.Body != null)
                {
                    rawBody = Encoding.UTF8.GetString(original.Body);
                }
                var ret = new EventMessage
                {
                    _dateTime = dateTime,
                    _eventName = eventName,
                    _id = id,
                    _rawBody = rawBody,
                    _listener = listener,
                    _listenerVersion = listenerVersion
                };
                if (ret.HasBody)
                {
                    try
                    {
                        ret._json = JsonConvert.DeserializeObject<IDictionary<String, Object>>(ret._rawBody);
                    }
                    catch (JsonReaderException e)
                    {
                        Console.WriteLine($"Event {id} (`{eventName}`) contains incorrect JSON at {dateTime}.");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                return ret;
            }
            else
            {
                throw new InvalidEventConversion("Event name is not set in event");
            }
        }
    }
}
