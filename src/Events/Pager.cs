using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PagerDuty.Events
{
    /// <summary>
    /// Provides access to PagerDuty's Events API
    /// </summary>
    public static class Pager
    {
        private const string endpoint = "https://events.pagerduty.com/v2/enqueue";


        /// <summary>
        /// Triggers an event with severity Error. 
        /// </summary>
        /// <param name="summary">A high-level, text summary message of the event. Will be used to construct an alert's description.</param>
        /// <param name="component">The part or component of the affected system that is broken.</param>
        /// <param name="dedupKey">Deduplication key for correlating triggers and resolves. If the event must be added to an existing alert,
        /// the provided dedup key must be equal to all the other events of the alert. If it is a new alert, the dedup key must be null.</param>
        /// <returns>The event response.</returns>
        public static EventResponse Trigger(string summary, string component, Guid? dedupKey = null)
        {
            return TriggerEvent.New().Trigger(summary, component, dedupKey);
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
        public static EventResponse Trigger(string summary, string component, EventSeverity eventSeverity, Guid? dedupKey = null)
        {
            return TriggerEvent.New().Trigger(summary, component, eventSeverity, dedupKey);
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
        public static EventResponse Trigger(string summary, string component, EventSeverity eventSeverity, string group, string eventClass, Guid? dedupKey = null)
        {
            return TriggerEvent.New().Trigger(summary, component, eventSeverity, group, eventClass, dedupKey);
        }

        /// <summary>
        /// Enqueues an event.
        /// </summary>
        /// <param name="eventToBeEnqueued">The event to be enqueued.</param>
        /// <returns>The event response returned by PagerDuty.</returns>
        public static EventResponse EnqueueEvent(Event eventToBeEnqueued)
        {
            ValidateEvent(eventToBeEnqueued);

            HttpResponseMessage httpResponseMessage = SendServiceRequest(eventToBeEnqueued);

            EventResponse response = JsonConvert
                     .DeserializeObject<EventResponse>(httpResponseMessage.Content.ReadAsStringAsync().Result);

            response = response.SetStatusCode((int)httpResponseMessage.StatusCode);

            return response;
        }

        /// <summary>
        /// Sends a request to PagerDuty' Events API
        /// </summary>
        /// <param name="body">The body of the request.</param>
        /// <returns>An HTTP response message.</returns>
        private static HttpResponseMessage SendServiceRequest(object body)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    Uri uri = new Uri(endpoint);

                    var json = JsonConvert.SerializeObject(body);

                    HttpContent httpContent = new StringContent(json);
                    httpContent.Headers.ContentType.MediaType = "application/json";

                    Task<HttpResponseMessage> httpResponseMessage;

                    using (var requestMessage = new HttpRequestMessage())
                    {
                        requestMessage.Content = httpContent;
                        requestMessage.Method = HttpMethod.Post;
                        requestMessage.RequestUri = uri;
                        requestMessage.Version = HttpVersion.Version11;

                        httpResponseMessage = httpClient.SendAsync(requestMessage);

                        return httpResponseMessage.Result;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Validates an event
        /// </summary>
        /// <param name="pagerEvent">The event to be validated.</param>
        private static void ValidateEvent(Event pagerEvent)
        {
            Type eventType = pagerEvent.GetType();

            if(eventType == typeof(TriggerEvent))
            {
                TriggerEvent triggerEvent = (TriggerEvent)pagerEvent;

                if(string.IsNullOrEmpty(triggerEvent.RoutingKey))
                    throw new ArgumentException($"'{nameof(triggerEvent.RoutingKey)}' must be defined.");

                if (triggerEvent.DedupKey != null)
                    if(triggerEvent.DedupKey.Length > 255)
                    throw new ArgumentException($" The length of'{nameof(triggerEvent.DedupKey)}' cannot be greater than 255.");

                if (string.IsNullOrEmpty(triggerEvent.Payload.Summary))
                    throw new ArgumentException($"'{nameof(triggerEvent.Payload.Summary)}' must be defined.");

                if (string.IsNullOrEmpty(triggerEvent.Payload.Source))
                    throw new ArgumentException($"'{nameof(triggerEvent.Payload.Source)}' must be defined.");

                if (string.IsNullOrEmpty(triggerEvent.Payload.Severity.ToString()))
                    throw new ArgumentException($"'{nameof(triggerEvent.Payload.Severity)}' must be defined.");

                if(triggerEvent.Payload.Timestamp != null)
                    if (!(DateTime.TryParse(triggerEvent.Payload.Timestamp, out DateTime tempDate)))
                        throw new ArgumentException($"'{nameof(triggerEvent.Payload.Timestamp)}' is not a valid DateTime.");
            }
        }
    }
}
