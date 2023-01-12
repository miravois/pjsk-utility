using PJSK.Utility.Data.EventShifts.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJSK.Utility.Data.EventShifts
{
    public static class Helper
    {
    }

    public static class EventHelper
    {
        public static Common.Enums.QueryStatusTypes UpsertEvent(Event newDAL, out string errorMsg)
        {
            errorMsg = string.Empty;

            var status = Common.Enums.QueryStatusTypes.None;
            try
            {
                using (var context = new EventShiftsContext())
                {
                    if (newDAL.EventId == 0)
                    {
                        context.Events.Add(newDAL);
                    }
                    else
                    {
                        var existingDAL = context.Events.SingleOrDefault(x => x.EventId == newDAL.EventId);
                        if (existingDAL == null)
                        {
                            context.Events.Add(newDAL);
                        }
                        else
                        {
                            context.Entry(existingDAL).CurrentValues.SetValues(newDAL);
                        }
                    }
                    context.SaveChanges();
                }
                status = Common.Enums.QueryStatusTypes.UpsertSuccess;
            }
            catch (Exception ex)
            {
                errorMsg = ex.ToString();
                status = Common.Enums.QueryStatusTypes.Failed;
            }
            return status;
        }

        public static Common.Enums.QueryStatusTypes GetEventById(long eventId, out Event resultDAL, out string errorMsg)
        {
            resultDAL = null;
            errorMsg = string.Empty;

            var status = Common.Enums.QueryStatusTypes.None;
            try
            {
                using (var context = new EventShiftsContext())
                {
                    resultDAL = context.Events.SingleOrDefault(x => x.EventId == eventId);
                }
                status = resultDAL != null ? Common.Enums.QueryStatusTypes.DataFound : Common.Enums.QueryStatusTypes.DataNotFound;
            }
            catch (Exception ex)
            {
                errorMsg = ex.ToString();
                status = Common.Enums.QueryStatusTypes.Failed;
            }
            return status;
        }

        public static Common.Enums.QueryStatusTypes GetEvents(out List<Event> resultDALs, out string errorMsg)
        {
            resultDALs = null;
            errorMsg = string.Empty;

            var status = Common.Enums.QueryStatusTypes.None;
            try
            {
                using (var context = new EventShiftsContext())
                {
                    resultDALs = context.Events.ToList();
                }
                status = resultDALs != null && resultDALs.Count > 0 ? Common.Enums.QueryStatusTypes.DataFound : Common.Enums.QueryStatusTypes.DataNotFound;
            }
            catch (Exception ex)
            {
                errorMsg = ex.ToString();
                status = Common.Enums.QueryStatusTypes.Failed;
            }
            return status;
        }
    }

    public class PlayerHelper
    {
        public static Common.Enums.QueryStatusTypes UpsertPlayer(Player newDAL, out string errorMsg)
        {
            errorMsg = string.Empty;

            var status = Common.Enums.QueryStatusTypes.None;
            try
            {
                using (var context = new EventShiftsContext())
                {
                    if (newDAL.PlayerId == 0)
                    {
                        context.Players.Add(newDAL);
                    }
                    else
                    {
                        var existingDAL = context.Players.SingleOrDefault(x => x.PlayerId == newDAL.PlayerId);
                        if (existingDAL == null)
                        {
                            context.Players.Add(newDAL);
                        }
                        else
                        {
                            context.Entry(existingDAL).CurrentValues.SetValues(newDAL);
                        }
                    }
                    context.SaveChanges();
                }
                status = Common.Enums.QueryStatusTypes.UpsertSuccess;
            }
            catch (Exception ex)
            {
                errorMsg = ex.ToString();
                status = Common.Enums.QueryStatusTypes.Failed;
            }
            return status;
        }


        public static Common.Enums.QueryStatusTypes GetPlayerById(long playerId, out Player resultDAL, out string errorMsg)
        {
            resultDAL = null;
            errorMsg = string.Empty;

            var status = Common.Enums.QueryStatusTypes.None;
            try
            {
                using (var context = new EventShiftsContext())
                {
                    resultDAL = context.Players.SingleOrDefault(x => x.PlayerId == playerId);
                }
                status = resultDAL != null ? Common.Enums.QueryStatusTypes.DataFound : Common.Enums.QueryStatusTypes.DataNotFound;
            }
            catch (Exception ex)
            {
                errorMsg = ex.ToString();
                status = Common.Enums.QueryStatusTypes.Failed;
            }
            return status;
        }
        public static Common.Enums.QueryStatusTypes GetPlayers(long eventId, out List<Player> resultDALs, out string errorMsg)
        {
            resultDALs = new List<Player>();
            errorMsg = string.Empty;

            var status = Common.Enums.QueryStatusTypes.None;
            try
            {
                using (var context = new EventShiftsContext())
                {
                    resultDALs = context.Players
                        .Where(x => x.EventId == eventId)
                        .OrderByDescending(x => x.IsRunnerInteger)
                        .ThenByDescending(x => x.PlayerLeaderSkill)
                        .ThenByDescending(x => x.PlayerInternalValue)
                        .ThenByDescending(x => x.PlayerTotalPower)
                        .ToList();
                }
                status = resultDALs != null && resultDALs.Count > 0 ? Common.Enums.QueryStatusTypes.DataFound : Common.Enums.QueryStatusTypes.DataNotFound;
            }
            catch (Exception ex)
            {
                errorMsg = ex.ToString();
                status = Common.Enums.QueryStatusTypes.Failed;
            }
            return status;
        }

        public static Common.Enums.QueryStatusTypes GetPlayersByName(long eventId, string name, out List<Player> resultDALs, out string errorMsg)
        {
            resultDALs = new List<Player>();
            errorMsg = string.Empty;

            var status = Common.Enums.QueryStatusTypes.None;
            try
            {
                using (var context = new EventShiftsContext())
                {
                    long.TryParse(name, out var nameAsId);
                    resultDALs = context.Players
                        .Where(x => x.EventId == eventId
                            && (x.PlayerName.Contains(name) || name.Contains(x.PlayerName)
                                || (!string.IsNullOrWhiteSpace(x.PlayerOtherName) && x.PlayerOtherName.Contains(name))
                                || (!string.IsNullOrWhiteSpace(x.PlayerOtherName) && name.Contains(x.PlayerOtherName)))
                                || (x.PlayerId == nameAsId))
                        .OrderByDescending(x => x.IsRunnerInteger)
                        .ThenByDescending(x => x.PlayerLeaderSkill)
                        .ThenByDescending(x => x.PlayerInternalValue)
                        .ThenByDescending(x => x.PlayerTotalPower)
                        .ToList();
                }
                status = resultDALs != null && resultDALs.Count > 0 ? Common.Enums.QueryStatusTypes.DataFound : Common.Enums.QueryStatusTypes.DataNotFound;
            }
            catch (Exception ex)
            {
                errorMsg = ex.ToString();
                status = Common.Enums.QueryStatusTypes.Failed;
            }
            return status;
        }
    }

    public class ShiftHelper
    {
        public static Common.Enums.QueryStatusTypes UpsertShift(Shift newDAL, out string errorMsg)
        {
            errorMsg = string.Empty;

            var status = Common.Enums.QueryStatusTypes.None;
            try
            {
                using (var context = new EventShiftsContext())
                {
                    if (newDAL.ShiftId == 0)
                    {
                        context.Shifts.Add(newDAL);
                    }
                    else
                    {
                        var existingDAL = context.Shifts.SingleOrDefault(x => x.ShiftId == newDAL.ShiftId);
                        if (existingDAL == null)
                        {
                            context.Shifts.Add(newDAL);
                        }
                        else
                        {
                            context.Entry(existingDAL).CurrentValues.SetValues(newDAL);
                        }
                    }
                    context.SaveChanges();
                }
                status = Common.Enums.QueryStatusTypes.UpsertSuccess;
            }
            catch (Exception ex)
            {
                errorMsg = ex.ToString();
                status = Common.Enums.QueryStatusTypes.Failed;
            }
            return status;
        }

        public static Common.Enums.QueryStatusTypes GetShiftById(long shiftId, out Shift resultDAL, out string errorMsg)
        {
            resultDAL = null;
            errorMsg = string.Empty;

            var status = Common.Enums.QueryStatusTypes.None;
            try
            {
                using (var context = new EventShiftsContext())
                {
                    resultDAL = context.Shifts.SingleOrDefault(x => x.ShiftId == shiftId);
                }
                status = resultDAL != null ? Common.Enums.QueryStatusTypes.DataFound : Common.Enums.QueryStatusTypes.DataNotFound;
            }
            catch (Exception ex)
            {
                errorMsg = ex.ToString();
                status = Common.Enums.QueryStatusTypes.Failed;
            }
            return status;
        }

        public static Common.Enums.QueryStatusTypes GetShiftByPlayerIdAndDate(long playerId, string shiftDate, out Shift resultDAL, out string errorMsg)
        {
            resultDAL = null;
            errorMsg = string.Empty;

            var status = Common.Enums.QueryStatusTypes.None;
            try
            {
                using (var context = new EventShiftsContext())
                {
                    resultDAL = context.Shifts.SingleOrDefault(x => x.PlayerId == playerId && x.ShiftDate == shiftDate);
                }
                status = resultDAL != null ? Common.Enums.QueryStatusTypes.DataFound : Common.Enums.QueryStatusTypes.DataNotFound;
            }
            catch (Exception ex)
            {
                errorMsg = ex.ToString();
                status = Common.Enums.QueryStatusTypes.Failed;
            }
            return status;
        }

        public static Common.Enums.QueryStatusTypes GetShiftsByEventIdAndDate(long eventId, string shiftDate, out List<Shift> resultDALs, out string errorMsg)
        {
            resultDALs = new List<Shift>();
            errorMsg = string.Empty;

            var status = Common.Enums.QueryStatusTypes.None;
            try
            {
                using (var context = new EventShiftsContext())
                {
                    resultDALs = context.Shifts.Where(x => x.EventId == eventId && x.ShiftDate == shiftDate).ToList();
                }
                status = resultDALs != null && resultDALs.Count > 0 ? Common.Enums.QueryStatusTypes.DataFound : Common.Enums.QueryStatusTypes.DataNotFound;
            }
            catch (Exception ex)
            {
                errorMsg = ex.ToString();
                status = Common.Enums.QueryStatusTypes.Failed;
            }
            return status;
        }

        public static Common.Enums.QueryStatusTypes GetShiftsByPlayerId(long playerId, out List<Shift> resultDALs, out string errorMsg)
        {
            resultDALs = new List<Shift>();
            errorMsg = string.Empty;

            var status = Common.Enums.QueryStatusTypes.None;
            try
            {
                using (var context = new EventShiftsContext())
                {
                    resultDALs = context.Shifts.Where(x => x.PlayerId == playerId).ToList();
                }
                status = resultDALs != null && resultDALs.Count > 0 ? Common.Enums.QueryStatusTypes.DataFound : Common.Enums.QueryStatusTypes.DataNotFound;
            }
            catch (Exception ex)
            {
                errorMsg = ex.ToString();
                status = Common.Enums.QueryStatusTypes.Failed;
            }
            return status;
        }

    }

}
