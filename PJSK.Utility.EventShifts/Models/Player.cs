using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJSK.Utility.EventShifts.Models
{
    public class Player : Data.EventShifts.DAL.Player
    {
        #region Properties

        public string PlayerNameEncore
        {
            get
            {
                return $"{base.PlayerName} (アンコ)";
            }
        }

        public List<string> PlayerOtherNameList
        {
            get
            {
                return base.PlayerOtherName.Split("|", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
            }
            set
            {
                base.PlayerOtherName = string.Join("|", value);
            }
        }

        public bool IsRunner
        {
            get { return base.IsRunnerInteger == 1; }
            set { base.IsRunnerInteger = value == true ? 1 : 0; }
        }

        public string PlayerData
        {
            get { return $"{base.PlayerLeaderSkill.ToString()}/{base.PlayerInternalValue.ToString()}/{base.PlayerTotalPower.ToString()}"; }
        }

        public string EncorePlayerData
        {
            get { return $"{base.EncorePlayerLeaderSkill.ToString()}/{base.EncorePlayerInternalValue.ToString()}/{base.EncorePlayerTotalPower.ToString()}"; }
        }

        public bool UseCustomBgColor { get; set; }

        public bool HasNgPlayer
        {
            get { return NgPlayerIds.Count > 0 && !NgPlayerIds.All(x => x == 0); }
        }

        public List<long> NgPlayerIds
        {
            get { return base.NgPlayerIdData == null ? new List<long>() 
                    : base.NgPlayerIdData.Split("|").ToList().Select(x => long.TryParse(x, out var y) ? y : 0).ToList(); }
        }

        public bool ToBeAssigned { get; set; }

        #endregion Properties

        #region Constructors

        public Player()
        {
            base.PlayerOtherName = string.Empty;
            base.Notes = string.Empty;
            base.BgColorHex = "#FFFFFF";
            base.NgPlayerIdData = string.Empty;
        }

        #endregion Constructors

        #region Methods

        public Player ShallowCopy()
        {
            return (Player)this.MemberwiseClone();
        }

        public void AddNotesAbove(string notes)
        {
            if (!string.IsNullOrWhiteSpace(notes))
            {
                this.Notes = $"{notes}\n\n{this.Notes}";
            }
        }

        public int GetTodayActualShiftCount(Shift shift)
        {
            return shift.ShiftDataList.Where(x => x.IsActive && x.Position > 0).Count();
        }

        public int GetTotalActualShiftCount(List<Shift> shifts)
        {
            var count = 0;
            foreach (var shift in shifts)
            {
                count += GetTodayActualShiftCount(shift);
            }
            return count;
        }

        #region Static Methods

        public static Player ConvertDAL(Data.EventShifts.DAL.Player other)
        {
            return Common.Converters.JsonConverter.ConvertType<Player>(other);
        }
        public static Data.EventShifts.DAL.Player ConvertDAL(Player other)
        {
            return Common.Converters.JsonConverter.ConvertType<Data.EventShifts.DAL.Player>(other);
        }

        #endregion Statuc Methods

        #endregion Methods
    }
}
