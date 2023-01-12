using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJSK.Utility.EventShifts.Models
{
    public class ShiftTable
    {
        #region Properties
        public string ShiftDate { get; set; }
        public Dictionary<string, ShiftTableRow> ShiftTableDict { get; set; }
        public List<ShiftTableRow> ActiveShiftTableRows
        {
            get
            {
                var list = new List<ShiftTableRow>();
                var firstActiveShiftTimeIndex = -1;
                var lastInactiveShiftTimeIndex = -1;
                var lastActiveShiftTimeIndex = -1;
                for (var i=0; i<24; i++)
                {
                    var shiftTime = Constants.ShiftTimeMap.Forward[i];
                    if (!ShiftTableDict[shiftTime].IsActive)
                    {
                        lastInactiveShiftTimeIndex = i;
                    }
                    else
                    {
                        // assign the first occurance of active row
                        if (firstActiveShiftTimeIndex == -1)
                        {
                            firstActiveShiftTimeIndex = i;
                        }

                        // if already active mode, last row is inactive, and now active, add all inactive rows in between
                        if (firstActiveShiftTimeIndex != -1 && lastActiveShiftTimeIndex != -1 && lastActiveShiftTimeIndex != i-1)
                        {
                            for (var j=lastActiveShiftTimeIndex+1; j<i; j++)
                            {
                                list.Add(ShiftTableDict[Constants.ShiftTimeMap.Forward[j]]);
                            }
                        }
                        // otherwise, might not enter active mode yet, only add last inactive row
                        else if (lastInactiveShiftTimeIndex != -1 && lastInactiveShiftTimeIndex == i-1)
                        {
                            list.Add(new ShiftTableRow(Constants.ShiftTimeMap.Forward[lastInactiveShiftTimeIndex]));
                        }
                        list.Add(ShiftTableDict[shiftTime]);
                        lastActiveShiftTimeIndex = i;
                    }
                }
                return list;
            }
        }
        #endregion Properties

        #region Constructors
        public ShiftTable(long eventId, string shiftDate)
        {
            Initialize(eventId, shiftDate);
        }
        #endregion Constructors

        #region Methods

        #region Initialization Methods

        private void Initialize(long eventId, string shiftDate)
        {
            ShiftDate = shiftDate;
            ShiftTableDict = new Dictionary<string, ShiftTableRow>();
            foreach (var shiftTime in Constants.ShiftTimeMap.BackWard.Keys)
            {
                ShiftTableDict.Add(shiftTime, new ShiftTableRow(shiftTime));
            }
            AssignShiftTableDict(eventId, shiftDate);
        }

        private void AssignShiftTableDict(long eventId, string shiftDate)
        {
            Common.Logging.Helper.ThrowException_FailedOrNotFound(Data.EventShifts.EventHelper.GetEventById(eventId, out var eventDAL, out var errorMsg),
                errorMsg, "GetEventById", new string[] { eventId.ToString() });
            Common.Logging.Helper.ThrowException_Failed(Data.EventShifts.ShiftHelper.GetShiftsByEventIdAndDate(eventId, shiftDate, out var shiftsDAL, out errorMsg),
                errorMsg, "GetShiftByEventIdAndDate", new string[] { eventId.ToString(), shiftDate });
            Common.Logging.Helper.ThrowException_Failed(Data.EventShifts.PlayerHelper.GetPlayers(eventId, out var playersDAL, out errorMsg),
                errorMsg, "GetPlayers", new string[] { eventId.ToString() });

            var ev = Event.ConvertDAL(eventDAL);
            var players = playersDAL.ConvertAll(x => Player.ConvertDAL(x)).ToList();

            foreach (var shift in shiftsDAL.ConvertAll(x => Shift.ConvertDAL(x)))
            {
                var player = Player.ConvertDAL(playersDAL.Where(x => x.PlayerId == shift.PlayerId).Single());
                foreach (var shiftData in shift.ShiftDataList)
                {
                    if (shiftData.IsActive)
                    {
                        ShiftTableDict[shiftData.ShiftTime].AllShifts.Add(new ShiftTableCell(ev, shift, shiftData, player));
                    }
                }
            }

            foreach (var kvp in ShiftTableDict)
            {
                kvp.Value.SortAllShiftsByRunnerThenPlayerData();
            }
            SetCustomColorUsage();
        }

        #endregion Initialization Methods

        public void AutoGenerateShiftTable()
        {
            foreach (var kvp in ShiftTableDict)
            {
                kvp.Value.AutoGenerateShiftTableRow();
            }
        }

        public List<Shift> GetShiftsFromShiftTableDict()
        {
            var shifts = new List<Shift>();

            foreach (var kvp in ShiftTableDict)
            {
                foreach (var shiftTableCell in kvp.Value.AllShifts)
                {
                    var shift = shifts.SingleOrDefault(x => x.ShiftId == shiftTableCell.ParentShift.ShiftId);
                    if (shift == null)
                    {
                        shift = shiftTableCell.ParentShift;
                        shifts.Add(shift);
                    }
                    shiftTableCell.UpdateParentShiftWithCurrentShiftData();
                }
            }

            return shifts;
        }

        public void SaveToDB()
        {
            var shifts = GetShiftsFromShiftTableDict();
            foreach (var shift in shifts)
            {
                Common.Logging.Helper.ThrowException_Failed(Data.EventShifts.ShiftHelper.UpsertShift(shift, out var errorMsg),
                    errorMsg, "UpsertShift", new string[] { });
            }
        }

        public void SetCustomColorUsage()
        {
            var dict = GetPlayerIdShiftCountDictionary(this);

            foreach (var kvp in ShiftTableDict.Values)
            {
                foreach (var shift in kvp.AssignedShifts)
                {
                    if (dict[shift.CurrentPlayer.PlayerId] > 1)
                    {
                        shift.CurrentPlayer.UseCustomBgColor = true;
                    }
                }
            }
        }

        #region Static Methods

        public static Dictionary<long, int> GetPlayerIdShiftCountDictionary(ShiftTable shiftTable)
        {
            var dict = new Dictionary<long, int>();
            foreach (var kvp in shiftTable.ShiftTableDict.Values)
            {
                foreach (var shift in kvp.AssignedShifts)
                {
                    if (!dict.ContainsKey(shift.CurrentPlayer.PlayerId))
                    {
                        dict.Add(shift.CurrentPlayer.PlayerId, 0);
                    }
                    dict[shift.CurrentPlayer.PlayerId]++;
                }
            }
            return dict;
        }

        #endregion Static Methods

        #endregion Methods
    }
}
