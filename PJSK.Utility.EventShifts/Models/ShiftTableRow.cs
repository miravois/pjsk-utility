using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJSK.Utility.EventShifts.Models
{
    public class ShiftTableRow
    {
        #region Properties

        public string ShiftTime { get; set; }

        public string ShiftTimeDisplay
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ShiftTime)) { return string.Empty; }
                var shiftTimeSplitArray = ShiftTime.Split('-');
                return $"{shiftTimeSplitArray[0]}:00-{shiftTimeSplitArray[1]}:00";
            }
        }

        public bool IsActive
        {
            get { return AllShifts != null && AllShifts.Exists(x => x.CurrentPlayer.IsRunner); }
        }

        public bool IsValid
        {
            get
            {
                var assignedShifts = AssignedShifts;

                // not exactly 5 in shift
                if (assignedShifts == null || (assignedShifts.Count != 5 && assignedShifts.Count != 6)) { return false; }

                // runner not in shift
                if (UnassignedShifts.Exists(x => x.CurrentPlayer.IsRunner && !x.IsAssigned)) { return false; }

                var isEncoreCount = 0;
                for (var i=0; i<assignedShifts.Count; i++)
                {
                    // not exactly one per position
                    if (assignedShifts[i].CurrentShiftData.Position != i + 1) { return false; }

                    // NG players together in shift
                    if (assignedShifts.Exists(x => assignedShifts[i].CurrentPlayer.NgPlayerIds.Contains(x.CurrentPlayer.PlayerId))) { return false; }

                    // more than one IsEncore
                    if (assignedShifts[i].CurrentShiftData.IsEncore) { isEncoreCount++; }

                    // no-standby player at standby position
                    if (!assignedShifts[i].ParentShift.CanStandby && assignedShifts[i].CurrentShiftData.Position == 6) { return false; }
                }
                if (isEncoreCount > 1) { return false; }
                return true;
            }
        }

        public List<ShiftTableCell> AssignedShifts
        {
            get
            {
                return AllShifts.Where(x => x.IsAssigned).OrderBy(x => x.CurrentShiftData.Position).ToList();
            }
        }

        public List<ShiftTableCell> AssignedPositionShifts
        {
            get
            {
                var assignedShifts = AssignedShifts;
                var assignedPositionShifts = new List<ShiftTableCell>();
                for (var i=0; i<Math.Max(5,AssignedShifts.Count); i++)
                {
                    var shiftAtPosition = assignedShifts.FirstOrDefault(x => x.CurrentShiftData.Position == (i + 1));
                    if (shiftAtPosition != null)
                    {
                        assignedPositionShifts.Add(shiftAtPosition);
                    }
                    else
                    {
                        assignedPositionShifts.Add(new ShiftTableCell(new ShiftData(ShiftTime, true, false, (i+1))));
                    }
                }
                return assignedPositionShifts;
            }
        }

        public List<ShiftTableCell> UnassignedShifts
        {
            get { return AllShifts.Where(x => !x.IsAssigned).ToList(); }
        }

        public List<ShiftTableCell> AllShifts { get; set; }
        #endregion Properties

        #region Constructors

        public ShiftTableRow()
        {
            this.AllShifts = new List<ShiftTableCell>();
        }

        public ShiftTableRow(string shiftTime)
        {
            this.ShiftTime = shiftTime;
            this.AllShifts = new List<ShiftTableCell>();
        }
        #endregion Constructors

        #region Methods

        public void AutoGenerateShiftTableRow()
        {
            if (!IsActive) { return; }

            AllShifts = ShiftTableCell.SetOptimalPosition(AllShifts);
        }

        public void SortAllShiftsByRunnerThenPlayerData()
        {
            AllShifts = ShiftTableCell.GetSortList_ByRunnerThenEncoreThenPlayerData(AllShifts);
        }

        #endregion Methods
    }
}
