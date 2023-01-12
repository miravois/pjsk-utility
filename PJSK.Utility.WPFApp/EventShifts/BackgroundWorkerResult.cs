using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PJSK.Utility.EventShifts.Models;

namespace PJSK.Utility.WPFApp.EventShifts
{
    internal class BackgroundWorkerResult
    {
        #region Properties
        internal Common.Enums.StatusTypes Status { get; set; }
        internal Common.Enums.LogLevelTypes LogLevel { get; set; }
        internal string Message { get; set; }
        internal bool IsSuccess { get { return Status == Common.Enums.StatusTypes.Success; } }
        internal bool IsFailed { get { return Status == Common.Enums.StatusTypes.Failed; } }
        internal bool TreatDataNotFoundAsFailed { get; set; }
        public string TagString { get; set; }

        internal List<Event> Events { get; set; }
        internal Event CurrentEvent { get; set; }
        internal List<Player> Players { get; set; }
        internal Shift CurrentShift { get; set; }
        public List<Shift> Shifts { get; set; }
        #endregion Properties

        #region Constructors

        public BackgroundWorkerResult()
        {

        }

        #endregion Constructors

        #region Methods

        public bool HandleQueryResult(Common.Enums.QueryStatusTypes status, string message)
        {
            if (status == Common.Enums.QueryStatusTypes.UpsertSuccess
                || status == Common.Enums.QueryStatusTypes.DataFound
                || (!TreatDataNotFoundAsFailed && status == Common.Enums.QueryStatusTypes.DataNotFound))
            {
                Status = Common.Enums.StatusTypes.Success;
                return true;
            }

            Status = Common.Enums.StatusTypes.Failed;
            LogLevel = Common.Enums.LogLevelTypes.Error;
            Message = message;
            return false;
        }

        #endregion Methods
    }
}
