using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PagerDuty.Events.ContextProperties;
using System;
using System.Collections.Generic;
using System.Threading;

namespace PagerDuty.Events
{
    /// <summary>
    /// When PagerDuty receives a trigger event, it will either open a new alert, or add a new trigger log entry 
    /// to an existing alert, depending on the provided dedup_key.
    /// </summary>
    public class TriggerEvent : Event, IDisposable
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public TriggerEvent()
        {
            Payload = new EventPayload();
            Action = EventAction.Trigger.ToString().ToLower();
        }

        /// <summary>
        /// The trigger event payload.
        /// </summary>
        public class EventPayload
        {
            /// <summary>
            /// Default constructor.
            /// </summary>
            public EventPayload()
            {
                rawTimestamp = DateTime.UtcNow;
            }

            /// <summary>
            /// A high-level, text summary message of the event. Will be used to construct an alert's description.
            /// </summary>
            [JsonProperty(PropertyName = "summary")]
            public string Summary { get; set; }

            /// <summary>
            /// When the upstream system detected/created the event. This is useful if a system batches or holds events before sending them to PagerDuty.
            /// </summary>
            [JsonProperty(PropertyName = "timestamp")]
            public string Timestamp
            {
                get
                {
                    // Format the value if needed.
                    if (rawTimestamp.HasValue && timestamp == null)
                    {
                        timestamp = rawTimestamp.Value.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
                    }

                    return timestamp;
                }
                set
                {
                    // Remove any raw value.
                    rawTimestamp = null;

                    // Set new value.
                    timestamp = value;
                }
            }
            private string timestamp = null;
            private DateTime? rawTimestamp = null;

            /// <summary>
            /// Specific human-readable unique identifier, such as a hostname, for the system having the problem.
            /// </summary>
            [JsonProperty(PropertyName = "source")]
            public string Source { get { return CachedMachineName.Value; } }

            /// <summary>
            /// How impacted the affected system is. Displayed to users in lists and influences the priority of any created incidents.
            /// </summary>
            [JsonProperty(PropertyName = "severity")]
            [JsonConverter(typeof(StringEnumConverter))]
            public EventSeverity Severity { get; set; }

            /// <summary>
            /// The part or component of the affected system that is broken.
            /// </summary>
            [JsonProperty(PropertyName = "component")]
            public string Component { get; set; }

            /// <summary>
            /// A cluster or grouping of sources.
            /// </summary>
            [JsonProperty(PropertyName = "group")]
            public string Group { get; set; }

            /// <summary>
            /// The class/type of the event.
            /// </summary>
            [JsonProperty(PropertyName = "class")]
            public string Class { get; set; }

            /// <summary>
            /// Free-form details from the event.
            /// </summary>
            [JsonProperty(PropertyName = "custom_details")]
            public IDictionary<string, object> CustomDetails
            {
                get
                {
                    // Create field if needed.
                    if (additionalData == null)
                    {
                        additionalData = new SortedDictionary<string, object>();
                    }

                    return additionalData;
                }
            }
            private IDictionary<string, object> additionalData = null;
        }

        [JsonProperty(PropertyName = "payload")]
        public EventPayload Payload { get; set; }

        /// <summary>
        /// List of images to include.
        /// </summary>
        [JsonProperty(PropertyName = "images")]
        public List<Image> Images { get; set; }

        /// <summary>
        /// List of links to include.
        /// </summary>
        [JsonProperty(PropertyName = "links")]
        public List<Link> Links { get; set; }

        /// <summary>
        /// Free-form details from the event.
        /// </summary>
        [JsonIgnore]
        public IDictionary<string, object> CustomDetails
        {
            get
            {
                // Create field if needed.
                if (customDetails == null)
                {
                    customDetails = new SortedDictionary<string, object>();
                }

                return customDetails;
            }
        }
        private IDictionary<string, object> customDetails = null;

        /// <summary>
        /// Cached machine name.
        /// </summary>
        private static readonly Lazy<string> CachedMachineName = new Lazy<string>(() => Environment.MachineName, LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Creates a new event object.
        /// </summary>
        /// <returns>New event object.</returns>
        public static TriggerEvent New()
        {
            return new TriggerEvent();
        }

        /// <summary>
        /// Sets the summary data field.
        /// </summary>
        /// <param name="value">Value to be set.</param>
        /// <returns>Self object reference.</returns>
        public TriggerEvent SetSummary(string value)
        {
            Payload.Summary = value;
            return this;
        }

        /// <summary>
        /// Sets the timestamp data field.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <returns>Self object reference.</returns>
        public TriggerEvent SetTimestamp(string value)
        {
            Payload.Timestamp = value;
            return this;
        }

        /// <summary>
        /// Sets the severity data field. The default value is Error.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <returns>Self object reference.</returns>
        public TriggerEvent SetSeverity(EventSeverity value)
        {
            Payload.Severity = value;
            return this;
        }

        /// <summary>
        /// Sets the componnent data field.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <returns>Self object reference.</returns>
        public TriggerEvent SetComponent(string value)
        {
            Payload.Component = value;
            return this;
        }

        /// <summary>
        /// Sets the group data field.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <returns>Self object reference.</returns>
        public TriggerEvent SetGroup(string value)
        {
            Payload.Group = value;
            return this;
        }

        /// <summary>
        /// Sets the class data field.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <returns>Self object reference.</returns>
        public TriggerEvent SetClass(string value)
        {
            Payload.Class = value;
            return this;
        }

        /// <summary>
        /// Sets new values for custom details data field.
        /// </summary>
        /// <param name="values">New values to set.</param>
        /// <returns>Self object reference.</returns>
        public TriggerEvent SetCustomDetails(IDictionary<string, object> values)
        {
            if (values == null) return this;

            foreach (var item in values)
            {
                Payload.CustomDetails[item.Key] = item.Value;
            }

            return this;
        }

        /// <summary>
        /// Sets the object for a given custom detail data name.
        /// </summary>
        /// <param name="name">Name of the object.</param>
        /// <param name="value">Value to set.</param>
        /// <returns>Self object reference.</returns>
        public TriggerEvent SetCustomDetails(string name, object value)
        {
            Payload.CustomDetails[name] = value;
            return this;
        }

        /// <summary>
        /// Add a new image to image data field
        /// </summary>
        /// <param name="image">Image to be added.</param>
        /// <returns>Self object reference.</returns>
        public TriggerEvent SetImage(Image image)
        {
            if (Images == null)
                Images = new List<Image>();

            Images.Add(image);
            return this;
        }

        /// <summary>
        /// Add a new link to link data field
        /// </summary>
        /// <param name="link">Link to be added.</param>
        /// <returns>Self object reference.</returns>
        public TriggerEvent SetLink(Link link)
        {
            if (Links == null)
                Links = new List<Link>();

            Links.Add(link);
            return this;
        }

        /// <summary>
        /// Sets the dedup key field.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <returns>Self object reference.</returns>
        public TriggerEvent SetDedupKey(Guid? value)
        {
            DedupKey = value?.ToString();
            return this;
        }

        /// <summary>
        /// Triggers an event with severity Error. 
        /// </summary>
        /// <param name="summary">A high-level, text summary message of the event. Will be used to construct an alert's description.</param>
        /// <param name="component">The part or component of the affected system that is broken.</param>
        /// <param name="dedupKey">Deduplication key for correlating triggers and resolves. If the event must be added to an existing alert,
        /// the provided dedup key must be equal to all the other events of the alert. If it is a new alert, the dedup key must be null.</param>
        /// <returns>The event response.</returns>
        public EventResponse Trigger(string summary, string component, Guid? dedupKey = null)
        {
            TriggerEvent triggerEvent = New()
               .SetSummary(summary)
               .SetComponent(component)
               .SetSeverity(EventSeverity.Error)
               .SetCustomDetails(CustomDetails)
               .SetDedupKey(dedupKey);

            return Pager.EnqueueEvent(triggerEvent);
        }

        /// <summary>
        /// Triggers an event with a given severity.
        /// </summary>
        /// <param name="summary">A high-level, text summary message of the event. Will be used to construct an alert's description.</param>
        /// <param name="component">The part or component of the affected system that is broken.</param>
        /// <param name="eventSeverity">How impacted the affected system is. Displayed to users in lists and influences the priority of any created incidents.</param>
        /// <param name="dedupKey">Deduplication key for correlating triggers and resolves. If the event must be added to an existing alert,
        /// the provided dedup key must be equal to all the other events of the alert. If it is a new alert, the dedup key must be null.</param>
        /// <returns>The event response.</returns>
        public EventResponse Trigger(string summary, string component, EventSeverity eventSeverity, Guid? dedupKey = null)
        {
            TriggerEvent triggerEvent = New()
                .SetSummary(summary)
                .SetComponent(component)
                .SetSeverity(eventSeverity)
                .SetCustomDetails(CustomDetails)
                .SetDedupKey(dedupKey);

            return Pager.EnqueueEvent(triggerEvent);
        }

        /// <summary>
        /// Triggers an event with a given severity and more details about the occurrence.
        /// </summary>
        /// <param name="summary">A high-level, text summary message of the event. Will be used to construct an alert's description.</param>
        /// <param name="component">The part or component of the affected system that is broken.</param>
        /// <param name="eventSeverity">How impacted the affected system is. Displayed to users in lists and influences the priority of any created incidents.</param>
        /// <param name="group">A cluster or grouping of sources.</param>
        /// <param name="eventClass">The class/type of the event.</param>
        /// <param name="dedupKey">Deduplication key for correlating triggers and resolves. If the event must be added to an existing alert,
        /// the provided dedup key must be equal to all the other events of the alert. If it is a new alert, the dedup key must be null.</param>
        /// <returns>The event response.</returns>
        public EventResponse Trigger(string summary, string component, EventSeverity eventSeverity, string group, string eventClass, Guid? dedupKey = null)
        {
            TriggerEvent triggerEvent = New() 
                .SetSummary(summary)
                .SetComponent(component)
                .SetSeverity(eventSeverity)
                .SetGroup(group).SetClass(eventClass)
                .SetCustomDetails(CustomDetails)
                .SetDedupKey(dedupKey);

            return Pager.EnqueueEvent(triggerEvent);
        }

        /// <summary>
        /// The dispose method.
        /// </summary>
        public void Dispose()
        {
            return;
        }
    }
}
