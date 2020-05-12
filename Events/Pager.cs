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
