using Newtonsoft.Json;
using System;
using System.Threading;

namespace PagerDuty.Events
{
    /// <summary>
    /// Base class for an event.
    /// </summary>
    public abstract class Event
    {
        /// <summary>
        /// This is the 32 character Integration Key for an integration on a service or on a global ruleset.
        /// </summary>
        [JsonProperty(PropertyName = "routing_key")]
        public string RoutingKey { get { return cachedRoutingKey.Value; } }

        /// <summary>
        /// Deduplication key for correlating triggers and resolves. The maximum permitted length of this property is 255 characters.
        /// </summary>
        [JsonProperty(PropertyName = "dedup_key")]
        public string DedupKey { get; set; }

        /// <summary>
        /// The type of event. Can be trigger, acknowledge or resolve.
        /// </summary>
        [JsonProperty(PropertyName = "event_action")]
        protected string Action { get; set; }

        /// <summary>
        /// Cached routing key.
        /// </summary>
        private static readonly Lazy<string> cachedRoutingKey = new Lazy<string>(() => Environment.GetEnvironmentVariable("ROUTING_KEY")?.Trim(), LazyThreadSafetyMode.PublicationOnly);
    }
}
    
