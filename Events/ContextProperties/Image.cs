namespace PagerDuty.Events.ContextProperties
{
    /// <summary>
    /// This property is used to attach images to the incident.
    /// </summary>
    public class Image
    {
        /// <summary>
        /// The source (URL) of the image being attached to the incident. This image must be served via HTTPS.
        /// </summary>
        public string SourceUrl { get; set; }

        /// <summary>
        /// Optional URL; makes the image a clickable link.
        /// </summary>
        public string HypertextReference { get; set; }

        /// <summary>
        /// Optional alternative text for the image.
        /// </summary>
        public string AlternativeText { get; set; }
    }
}
