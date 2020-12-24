using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SchedulerOnDemandLoading
{
    /// <summary>   
    /// Represents custom appointment properties.   
    /// </summary> 
    public class Event
    {
        /// <summary>
        /// Gets or sets the subject of the appointment. 
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the id of the appointment. 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the start time of the appointment. 
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time of the appointment. 
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or sets teh value that indicates whether the appointment is all day. 
        /// </summary>
        public bool AllDay { get; set; }

        /// <summary>
        /// Gets or sets the recurrence rule of the appointment. 
        /// </summary>
        public string RecurrenceRule { get; set; }

        /// <summary>
        /// Gets or sets the background color of the appointment. 
        /// </summary>
        public Brush Color { get; set; }
    }
}
