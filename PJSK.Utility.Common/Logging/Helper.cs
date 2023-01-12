using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJSK.Utility.Common.Logging
{
    public static class Helper
    {
        public static void ThrowException_Failed(Enums.QueryStatusTypes status, string errorMessage, string queryMethodName, string[] parameters)
        {
            if (status == Enums.QueryStatusTypes.UpsertSuccess || status == Enums.QueryStatusTypes.DataFound || status == Enums.QueryStatusTypes.DataNotFound) { return; }
            errorMessage = !string.IsNullOrEmpty(errorMessage) ? ": " + errorMessage : errorMessage;
            throw new Exception($"{queryMethodName}({string.Join(",", parameters)}) {status.ToString()}{errorMessage}");
        }

        public static void ThrowException_FailedOrNotFound(Enums.QueryStatusTypes status, string errorMessage, string queryMethodName, string[] parameters)
        {
            if (status == Enums.QueryStatusTypes.UpsertSuccess || status == Enums.QueryStatusTypes.DataFound) { return; }
            errorMessage = !string.IsNullOrEmpty(errorMessage) ? ": " + errorMessage : errorMessage;
            throw new Exception($"{queryMethodName}({string.Join(",", parameters)}) {status.ToString()}{errorMessage}");
        }
    }
}
