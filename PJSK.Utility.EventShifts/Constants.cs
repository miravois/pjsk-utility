using PJSK.Utility.Common.DataStructures;
namespace PJSK.Utility.EventShifts
{
    public static class Constants
    {
        private static Map<int, string> _shiftTimeMap = null;
        /// <summary>
        /// (0-based-index, time-shift-string)
        /// </summary>
        public static Map<int, string> ShiftTimeMap
        {
            get
            {
                if (_shiftTimeMap == null)
                {
                    _shiftTimeMap = new Map<int, string>();
                    for (var i=0; i<24; i++)
                    {
                        _shiftTimeMap.Add(i, $"{i.ToString().PadLeft(2,'0')}-{(i+1).ToString().PadLeft(2,'0')}");
                    }
                    _shiftTimeMap.Add(-1, "");
                }
                return _shiftTimeMap;
            }
        }

        public static int[] SolitaryEnvySkillStrenthOrder { get { return new int[] { 5, 4, 1, 3, 2 }; } }

        public static int P1TotalPowerBonus
        {
            get
            {
                return int.Parse(System.Configuration.ConfigurationManager.AppSettings["EventShifts_P1TotalPowerBonus"]);
            }
        }
    }
}