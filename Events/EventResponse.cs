using Newtonsoft.Json;
using System.Collections.Generic;

namespace PagerDuty.Events
{
    /// <summary>
    /// Base class for an event response.
    /// </summary>
    public class EventResponse
    {
        /// <summary>
        /// The status of the HTTP response.
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        /// <summary>
        /// The status code of the HTTP response.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// The message of the response.
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        /// <summary>
        /// The dedup key of the event.
        /// </summary>
        [JsonProperty(PropertyName = "dedup_key")]
        public string DedupKey { get; set; }

        /// <summary>
        /// List of errors
        /// </summary>
        [JsonProperty(PropertyName = "errors")]
        public List<string> Errors { get; set; }

        /// <summary>
        /// Sets the status code field.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <returns>Self object reference.</returns>
        public EventResponse SetStatusCode(int value)
        {
            StatusCode = value;
            return this;
        }
    }
}
