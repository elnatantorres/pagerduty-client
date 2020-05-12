using System.Runtime.Serialization;

namespace PagerDuty.Events
{
    /// <summary>
    /// Event severity
    /// </summary>
    public enum EventSeverity
    {
        /// <summary>
        /// System is unusable. Triggered when system is not responding or is completely non functional. Action must be taken immediately.
        /// </summary>
        [EnumMember(Value = "critical")]
        Critical = 0,

        /// <summary>
        /// Error conditions. Triggered by an unexpected situation that needs to be analysed quickly. System still up and running.
        /// </summary>
        [EnumMember(Value = "error")]
        Error = 1,

        /// <summary>
        /// Warning conditions. Triggered by an expected situation that needs to be analysed. System still up and running.
        /// </summary>
        [EnumMember(Value = "warning")]
        Warning = 2,

        /// <summary>
        /// Informational messages. System is up and running normally.
        /// </summary>
        [EnumMember(Value = "info")]
        Info = 3
    }
}
