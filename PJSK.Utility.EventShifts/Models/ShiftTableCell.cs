using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJSK.Utility.EventShifts.Models
{
    public class ShiftTableCell
    {
        #region Properties

        public bool IsAssigned
        {
            get { return CurrentShiftData.Position > 0; }
        }
        public Event CurrentEvent { get; set; }
        public Player CurrentPlayer { get; set; }
        public Shift ParentShift { get; set; }
        public ShiftData CurrentShiftData { get; set; }
        public string DetailDataString
        {
            get
            {
                return string.Format("{3}{0}{4}{0}{5}", Environment.NewLine,
                    CurrentShiftData.Position, CurrentPlayer.PlayerName, 
                    CurrentPlayer.PlayerLeaderSkill, CurrentPlayer.PlayerInternalValue, ((decimal)CurrentPlayer.PlayerTotalPower/10000).ToString("0.0"));
            }
        }

        #endregion Properties

        #region Constructors

        public ShiftTableCell(ShiftData shiftData)
        {
            CurrentEvent = new Event();
            CurrentPlayer = new Player();
            ParentShift = new Shift();
            CurrentShiftData = shiftData;
        }

        public ShiftTableCell(Event ev, Shift shift, ShiftData shiftData, Player player)
        {
            CurrentEvent = ev;
            CurrentPlayer = player;
            ParentShift = shift;
            CurrentShiftData = shiftData;
        }

        #endregion Constructors

        #region Methods

        public ShiftTableCell ShallowCopy()
        {
            return (ShiftTableCell)this.MemberwiseClone();
        }

        public ShiftTableCell DeepCopy()
        {
            var copy = ShallowCopy();
            copy.CurrentEvent = CurrentEvent.ShallowCopy();
            copy.CurrentPlayer = CurrentPlayer.ShallowCopy();
            copy.ParentShift = ParentShift.ShallowCopy();
            copy.CurrentShiftData = CurrentShiftData.ShallowCopy();
            return copy;
        }

        public void ResetShiftData()
        {
            CurrentShiftData.Position = CurrentShiftData.Position == -1 ? -1 : 0;
            //CurrentShiftData.IsEncore = false;
        }

        public void UpdateParentShiftWithCurrentShiftData()
        {
            ParentShift.SetPosition(CurrentShiftData.ShiftTime, CurrentShiftData.Position);
            ParentShift.SetIsEncore(CurrentShiftData.ShiftTime, CurrentShiftData.IsEncore);
        }

        #region Static Methods

        public static List<ShiftTableCell> GetSortList_ByRunnerThenEncoreThenPlayerData(List<ShiftTableCell> list)
        {
            return list
                .OrderByDescending(x => x.CurrentPlayer.IsRunner)
                .ThenByDescending(x => x.CurrentShiftData.IsEncore)
                .ThenByDescending(x => x.CurrentPlayer.PlayerLeaderSkill)
                .ThenByDescending(x => x.CurrentPlayer.PlayerInternalValue)
                .ThenByDescending(x => x.CurrentPlayer.PlayerTotalPower).ToList();
        }

        public static List<ShiftTableCell> GetSortList_ByPlayerData(List<ShiftTableCell> list)
        {
            return list
                .OrderByDescending(x => x.CurrentPlayer.PlayerLeaderSkill)
                .ThenByDescending(x => x.CurrentPlayer.PlayerInternalValue)
                .ThenByDescending(x => x.CurrentPlayer.PlayerTotalPower).ToList();
        }

        public static List<ShiftTableCell> SetOptimalPosition(List<ShiftTableCell> list)
        {
            list = GetSortList_ByRunnerThenEncoreThenPlayerData(list);
            list.ForEach(x => x.ResetShiftData());

            var availableShifts = list.Where(x => x.CurrentShiftData.Position != -1).ToList();
            var assignedShifts = new List<ShiftTableCell>();

            var shiftTime = availableShifts[0].CurrentShiftData.ShiftTime;
            var runnerShift = availableShifts.First(x => x.CurrentPlayer.IsRunner);

            // assign CanEncore shift to be IsEncore is applicable
            var canEncoreShift = availableShifts.FirstOrDefault(x => x.ParentShift.CanEncore && x.CurrentPlayer.EncorePlayerTotalPower + Constants.P1TotalPowerBonus >= runnerShift.CurrentPlayer.PlayerTotalPower);
            if (canEncoreShift != null && !availableShifts.Exists(x => x.CurrentShiftData.IsEncore))
            {
                canEncoreShift.CurrentShiftData.IsEncore = true;
            }

            if (availableShifts.Exists(x => x.CurrentShiftData.IsEncore))
            {
                var encoreShift = availableShifts.First(x => x.CurrentShiftData.IsEncore);

                if (encoreShift.CurrentPlayer.EncorePlayerTotalPower >= runnerShift.CurrentPlayer.PlayerTotalPower + Constants.P1TotalPowerBonus)
                {
                    // if encoreShift can do encore at any position, put runnerShift at P1
                    assignedShifts = availableShifts.Where(x => !x.CurrentPlayer.IsRunner).Take(4).ToList();
                    assignedShifts = GetSortList_ByPlayerData(assignedShifts);

                    if (assignedShifts.Count == 0)
                    {
                        assignedShifts.Add(new ShiftTableCell(new ShiftData(shiftTime, true, false, 5)));
                    }
                    if (assignedShifts.Count == 1)
                    {
                        assignedShifts.Add(new ShiftTableCell(new ShiftData(shiftTime, true, false, 4)));
                    }
                    assignedShifts.Insert(2, runnerShift); // 1
                }
                else if (encoreShift.CurrentPlayer.EncorePlayerTotalPower > runnerShift.CurrentPlayer.PlayerTotalPower + 2000)
                {
                    // if encoreShift can do encore only if runnerShift not at P1, put runnerShift at P2
                    assignedShifts = availableShifts.Where(x => !x.CurrentPlayer.IsRunner).Take(4).ToList();
                    assignedShifts = GetSortList_ByPlayerData(assignedShifts);

                    if (assignedShifts.Count == 0)
                    {
                        assignedShifts.Add(new ShiftTableCell(new ShiftData(shiftTime, true, false, 5)));
                    }
                    if (assignedShifts.Count == 1)
                    {
                        assignedShifts.Add(new ShiftTableCell(new ShiftData(shiftTime, true, false, 4)));
                    }
                    if (assignedShifts.Count == 2)
                    {
                        assignedShifts.Add(new ShiftTableCell(new ShiftData(shiftTime, true, false, 1)));
                    }
                    if (assignedShifts.Count == 3)
                    {
                        assignedShifts.Add(new ShiftTableCell(new ShiftData(shiftTime, true, false, 3)));
                    }
                    assignedShifts.Add(runnerShift); // 2
                }
                else
                {
                    // encoreShift at P1, runnerShift at p2
                    assignedShifts = availableShifts.Where(x => !x.CurrentPlayer.IsRunner && !x.CurrentShiftData.IsEncore).Take(3).ToList();
                    assignedShifts = GetSortList_ByPlayerData(assignedShifts);

                    if (assignedShifts.Count == 0)
                    {
                        assignedShifts.Add(new ShiftTableCell(new ShiftData(shiftTime, true, false, 5)));
                    }
                    if (assignedShifts.Count == 1)
                    {
                        assignedShifts.Add(new ShiftTableCell(new ShiftData(shiftTime, true, false, 4)));
                    }
                    assignedShifts.Insert(2, encoreShift); // 1
                    if (assignedShifts.Count == 3)
                    {
                        assignedShifts.Add(new ShiftTableCell(new ShiftData(shiftTime, true, false, 3)));
                    }
                    assignedShifts.Add(runnerShift); // 2
                }
            }

            // if no encoreShift, Runner will be P=1
            else
            {
                assignedShifts = availableShifts.Where(x => !x.CurrentPlayer.IsRunner).Take(4).ToList();
                assignedShifts = GetSortList_ByPlayerData(assignedShifts);
                if (assignedShifts.Count == 0)
                {
                    assignedShifts.Add(new ShiftTableCell(new ShiftData(shiftTime, true, false, 0)));
                }
                if (assignedShifts.Count == 1)
                {
                    assignedShifts.Add(new ShiftTableCell(new ShiftData(shiftTime, true, false, 1)));
                }
                assignedShifts.Insert(2, runnerShift);
            }

            if (assignedShifts.Count == 1)
            {
                assignedShifts[0].CurrentShiftData.Position = 1;
            }
            else
            {
                for (var i = 0; i < assignedShifts.Count; i++)
                {
                    assignedShifts[i].CurrentShiftData.Position = Constants.SolitaryEnvySkillStrenthOrder[i];
                }
            }

            var standby = availableShifts
                    .Where(x => !assignedShifts.Contains(x) && x.ParentShift.CanStandby)
                    .FirstOrDefault();
            if (standby != null)
            {
                standby.CurrentShiftData.Position = 6;
            }

            return list;
        }

        #endregion Static Methods

        #endregion Methods
    }
}
