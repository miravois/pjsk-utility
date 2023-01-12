using System;
using System.Collections.Generic;

namespace PJSK.Utility.Data.EventShifts.DAL
{
    public partial class Event
    {
        public Event()
        {
            Players = new HashSet<Player>();
            Shifts = new HashSet<Shift>();
        }

        public long EventId { get; set; }
        public string EventName { get; set; } = null!;
        public long EventType { get; set; }
        public string EventStartDate { get; set; } = null!;
        public string EventEndDate { get; set; } = null!;

        public virtual ICollection<Player> Players { get; set; }
        public virtual ICollection<Shift> Shifts { get; set; }
    }
}
