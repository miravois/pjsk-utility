using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJSK.Utility.EventShifts.Models
{
    public class ShiftData
    {
        #region Properties
        public string ShiftTime { get; set; }
        public int ShiftTimeStartHour { get; set; }
        public bool IsActive { get; set; }
        public bool IsEncore { get; set; }
        public int Position { get; set; }
        #endregion Properties

        #region Constructors

        public ShiftData(string shiftTime, bool isActive, bool isEncore, int position)
        {
            this.ShiftTime = shiftTime;
            this.ShiftTimeStartHour = Constants.ShiftTimeMap.BackWard[shiftTime];
            this.IsActive = isActive;
            this.IsEncore = isEncore;
            this.Position = position;
        }

        #endregion Constructors

        #region Methods

        public ShiftData ShallowCopy()
        {
            return (ShiftData)this.MemberwiseClone();
        }

        #endregion Methods
    }
}
