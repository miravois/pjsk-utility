using System;
using System.Collections.Generic;

namespace PJSK.Utility.Data.EventShifts.DAL
{
    public partial class Shift
    {
        public long ShiftId { get; set; }
        public long EventId { get; set; }
        public long PlayerId { get; set; }
        public string ShiftDate { get; set; } = null!;
        public long CanEncoreInteger { get; set; }
        public long CanStandbyInteger { get; set; }
        public string ActiveData { get; set; } = null!;
        public string EncoreData { get; set; } = null!;
        public string PositionData { get; set; } = null!;
        public string Notes { get; set; } = null!;

        public virtual Event Event { get; set; } = null!;
        public virtual Player Player { get; set; } = null!;
    }
}
