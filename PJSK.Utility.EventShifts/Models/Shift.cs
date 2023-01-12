using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJSK.Utility.EventShifts.Models
{
    public class Shift : Data.EventShifts.DAL.Shift
    {
        #region Properties
        private static readonly string _defaultDataString = string.Join("|", Enumerable.Repeat("0", 24));

        public bool CanEncore
        {
            get { return base.CanEncoreInteger == 1; }
            set { base.CanEncoreInteger = value == true ? 1 : 0; }
        }

        public bool CanStandby
        {
            get { return base.CanStandbyInteger == 1; }
            set { base.CanStandbyInteger = value == true ? 1 : 0; }
        }

        public List<ShiftData> ShiftDataList
        {
            get
            {
                if (string.IsNullOrWhiteSpace(base.ActiveData)) { base.ActiveData = _defaultDataString; }
                if (string.IsNullOrWhiteSpace(base.EncoreData)) { base.EncoreData = _defaultDataString; }
                if (string.IsNullOrWhiteSpace(base.PositionData)) { base.PositionData = _defaultDataString; }

                var list = new List<ShiftData>();
                var activeDataSplitArray = base.ActiveData.Split("|");
                var encoreDataSplitArray = base.EncoreData.Split("|");
                var positionDataSplitArray = base.PositionData.Split("|");
                for (var i=0; i<24; i++)
                {
                    list.Add(new ShiftData(Constants.ShiftTimeMap.Forward[i], activeDataSplitArray[i] == "1", encoreDataSplitArray[i] == "1", int.Parse(positionDataSplitArray[i])));
                }
                return list;
            }
            set
            {
                var defaultData = string.Join("|", Enumerable.Repeat("0", 24));
                if (value == null)
                {
                    base.ActiveData = defaultData;
                    base.EncoreData = defaultData;
                    base.PositionData = defaultData;
                }
                else
                {
                    base.ActiveData = string.Join("|", value.ConvertAll(x => x.IsActive == true ? "1" : "0"));
                    base.EncoreData = string.Join("|", value.ConvertAll(x => x.IsEncore == true ? "1" : "0"));
                    base.PositionData = string.Join("|", value.ConvertAll(x => x.Position));
                }
            }
        }

        #endregion Properties

        #region Constructors

        public Shift()
        {
            base.Notes = string.Empty;
            base.ActiveData = _defaultDataString;
            base.EncoreData = _defaultDataString;
            base.PositionData = _defaultDataString;
        }

        #endregion Constructors

        #region Methods

        public Shift ShallowCopy()
        {
            return (Shift)this.MemberwiseClone();
        }

        public Shift DeepCopy()
        {
            var copy = ShallowCopy();
            copy.ShiftDataList = this.ShiftDataList.Select(x => x.ShallowCopy()).ToList();
            return copy;
        }

        public void AddNotesAbove(string notes)
        {
            if (!string.IsNullOrWhiteSpace(notes))
            {
                this.Notes = $"{notes}\n\n{this.Notes}";
            }
        }

        public void SetPosition(string shiftTime, int position)
        {
            var positionDataSplitArray = base.PositionData.Split("|");
            positionDataSplitArray[Constants.ShiftTimeMap.BackWard[shiftTime]] = position.ToString();
            base.PositionData = string.Join("|", positionDataSplitArray);
        }

        public void SetIsEncore(string shiftTime, bool isEncore)
        {
            var positionDataSplitArray = base.EncoreData.Split("|");
            positionDataSplitArray[Constants.ShiftTimeMap.BackWard[shiftTime]] = isEncore ? "1" : "0";
            base.EncoreData = string.Join("|", positionDataSplitArray);
        }

        #region Static Methods

        public static Shift ConvertDAL(Data.EventShifts.DAL.Shift other)
        {
            return Common.Converters.JsonConverter.ConvertType<Shift>(other);
        }
        public static Data.EventShifts.DAL.Shift ConvertDAL(Shift other)
        {
            return Common.Converters.JsonConverter.ConvertType<Data.EventShifts.DAL.Shift>(other);
        }

        #endregion Statuc Methods

        #endregion Methods
    }
}
