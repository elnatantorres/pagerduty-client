using Newtonsoft.Json;

namespace PagerDuty.Events.ContextProperties
{
    /// <summary>
    /// This property is used to attach text links to the incident.
    /// </summary>
    public class Link
    {
        /// <summary>
        /// URL of the link to be attached.
        /// </summary>
        [PropertyName("href")]
        public string HypertextReference { get; set; }

        /// <summary>
        /// Plain text that describes the purpose of the link, and can be used as the link's text.
        /// </summary>
        public string Text { get; set; }
    }
}
