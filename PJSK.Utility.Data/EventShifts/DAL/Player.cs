using System;
using System.Collections.Generic;

namespace PJSK.Utility.Data.EventShifts.DAL
{
    public partial class Player
    {
        public Player()
        {
            Shifts = new HashSet<Shift>();
        }

        public long PlayerId { get; set; }
        public long EventId { get; set; }
        public string PlayerName { get; set; } = null!;
        public string PlayerOtherName { get; set; } = null!;
        public long PlayerLeaderSkill { get; set; }
        public long PlayerInternalValue { get; set; }
        public long PlayerTotalPower { get; set; }
        public long EncorePlayerLeaderSkill { get; set; }
        public long EncorePlayerInternalValue { get; set; }
        public long EncorePlayerTotalPower { get; set; }
        public long IsRunnerInteger { get; set; }
        public string BgColorHex { get; set; } = null!;
        public string NgPlayerIdData { get; set; } = null!;
        public string Notes { get; set; } = null!;

        public virtual Event Event { get; set; } = null!;
        public virtual ICollection<Shift> Shifts { get; set; }
    }
}
