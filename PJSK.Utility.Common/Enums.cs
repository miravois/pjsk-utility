using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJSK.Utility.Common
{
    public static class Enums
    {
        public enum StatusTypes
        {
            None = 0,
            Success = 1,
            Failed = 2,
        }

        public enum QueryStatusTypes
        {
            None = 0,
            Failed = 1,
            UpsertSuccess = 2,
            DataFound = 3,
            DataNotFound = 4,
        }

        public enum LogLevelTypes
        {
            Trace = 0,
            Debug = 1,
            Info = 2,
            Warn = 3,
            Error = 4,
            Fatal = 5,
        }
    }
}
