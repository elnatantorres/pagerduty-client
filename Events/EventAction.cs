namespace PagerDuty.Events
{
    /// <summary>
    /// The type of event.
    /// </summary>
    public enum EventAction
    {
        /// <summary>
        /// When PagerDuty receives a trigger event, it will either open a new alert, or add a new trigger 
        /// log entry to an existing alert, depending on the provided dedup_key.
        /// </summary>
        Trigger = 0,

        /// <summary>
        /// Acknowledge events cause the referenced incident to enter the acknowledged state.
        /// </summary>
        Acknowledge = 1,

        /// <summary>
        /// Resolve events cause the referenced incident to enter the resolved state.
        /// </summary>
        Resolve = 2,

    }
}
