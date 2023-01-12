using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PJSK.Utility.EventShifts.Models;

namespace PJSK.Utility.WPFApp.EventShifts
{
    internal class BackgroundWorkerArgument
    {
        public string TagString { get; set; }

        public long EventId { get; set; }
        public string PlayerName { get; set; }
        public string ShiftDate { get; set; }
        public Event CurrentEvent { get; set; }
        public Player CurrentPlayer { get; set; }
        public Shift CurrentShift { get; set; }
    }
}
