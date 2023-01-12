using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJSK.Utility.EventShifts.Models
{
    public class Event : Data.EventShifts.DAL.Event
    {
        #region Properties

        public string EventIdName
        {
            get { return $"[{EventId.ToString().PadLeft(3, '0')}] - {EventName}"; }
        }

        public string EventDuration_Local
        {
            get { return $"{EventStartDate_LocalString} - {EventEndDate_LocalString}"; }
        }

        public DateTime? EventStartDate_UTCDateTime
        {
            get
            {
                if (DateTime.TryParse(base.EventStartDate, out var dtEventStartDate))
                {
                    return dtEventStartDate.Kind == DateTimeKind.Local ? dtEventStartDate.ToUniversalTime() : dtEventStartDate;
                }
                return null;
            }
        }

        public string EventStartDate_LocalString
        {
            get
            {
                return Common.Converters.DateTimeConverter.ConvertUTCToLocal_GetString(base.EventStartDate);
            }
        }

        public DateTime? EventStartDate_JapanDateTime
        {
            get
            {
                return Common.Converters.DateTimeConverter.ConvertUTCToJapan_GetDateTime(base.EventStartDate);
            }
            set
            {
                base.EventStartDate = Common.Converters.DateTimeConverter.ConvertJapanToUTC_GetString(value);
            }
        }
        public string EventStartDate_JapanString
        {
            get
            {
                return EventStartDate_JapanDateTime?.ToString(Common.Constants.DateTime.DateTimePatterns.DateTime);
            }
        }

        public DateTime? EventEndDate_UTCDateTime
        {
            get
            {
                if (DateTime.TryParse(base.EventEndDate, out var dtEventEndDate))
                {
                    return dtEventEndDate.Kind == DateTimeKind.Local ? dtEventEndDate.ToUniversalTime() : dtEventEndDate;
                }
                return null;
            }
        }
        public string EventEndDate_LocalString
        {
            get
            {
                return Common.Converters.DateTimeConverter.ConvertUTCToLocal_GetString(base.EventEndDate);
            }
        }

        public DateTime? EventEndDate_JapanDateTime
        {
            get
            {
                return Common.Converters.DateTimeConverter.ConvertUTCToJapan_GetDateTime(base.EventEndDate);
            }
            set
            {
                base.EventEndDate = Common.Converters.DateTimeConverter.ConvertJapanToUTC_GetString(value);
            }
        }
        public string EventEndDate_JapanString
        {
            get
            {
                return EventEndDate_JapanDateTime?.ToString(Common.Constants.DateTime.DateTimePatterns.DateTime);
            }
        }
        #endregion Properties

        #region Constructors

        public Event()
        {

        }

        #endregion Constructors

        #region Methods

        public Event ShallowCopy()
        {
            return (Event)this.MemberwiseClone();
        }

        #region Static Methods

        public static Event ConvertDAL(Data.EventShifts.DAL.Event other)
        {
            return Common.Converters.JsonConverter.ConvertType<Event>(other);
        }

        public static Data.EventShifts.DAL.Event ConvertDAL(Event other)
        {
            return Common.Converters.JsonConverter.ConvertType<Data.EventShifts.DAL.Event>(other);
        }

        #endregion Statuc Methods

        #endregion Methods
    }
}
