using PJSK.Utility.EventShifts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PJSK.Utility.EventShifts
{
    public class DiscordMessageParser
    {
        #region Properties

        public bool IsInUse
        {
            get { return MessageLines != null && MessageLines.Count > 0; }
        }
        public string OriginalMessage { get; }
        public string NameLine { get; set; }
        public List<string> MessageLines { get; set; }

        #endregion Properties

        #region Constructors

        public DiscordMessageParser(string message)
        {
            OriginalMessage = message ?? string.Empty;

            var lines = OriginalMessage.Split(Environment.NewLine).ToList();
            NameLine = lines[0];
            lines.RemoveAt(0);
            MessageLines = lines;
        }

        #endregion Constructors

        #region Methods

        public string GetPlayerName()
        {
            // テン — 04/29/2022
            var match = new Regex(@"(.*)\s—").Match(NameLine);
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        public void GetPlayerData(out int leaderSkill, out int internalValue, out long totalPower,
            out int encoreLeaderSkill, out int encoreIntervalValue, out long encoreTotalPower)
        {
            leaderSkill = 0;
            internalValue = 0;
            totalPower = 0;
            encoreLeaderSkill = 0;
            encoreIntervalValue = 0;
            encoreTotalPower = 0;

            foreach (var line in MessageLines.Where(x => x.Contains('/') && x.Any(char.IsDigit)))
            {
                var splitRegex = new Regex(@"(.+)/(.+)/(.+)");
                var splitMatch = splitRegex.Match(line);
                if (splitMatch.Success && splitMatch.Groups.Count == 4)
                {
                    var decimalRegex = new Regex(@"[\d\.]+");
                    var decimal1Matches = decimalRegex.Matches(splitMatch.Groups[1].Value);
                    var decimal2Matches = decimalRegex.Matches(splitMatch.Groups[2].Value);
                    var decimal3Matches = decimalRegex.Matches(splitMatch.Groups[3].Value);

                    var dataList = new List<decimal>
                    {
                        decimal1Matches.Count > 0 ? decimal.Parse(decimal1Matches[0].Value) : 0,
                        decimal2Matches.Count > 0 ? decimal.Parse(decimal2Matches[0].Value) : 0,
                        decimal3Matches.Count > 0 ? decimal.Parse(decimal3Matches[0].Value) : 0,
                    };

                    // LeaderSkill in range [80,150]
                    var thisLeaderSkill = dataList.SingleOrDefault(x => x >= 80 && x <= 150);

                    // InternalValue in range [550,]
                    var thisInternalValue = dataList.SingleOrDefault(x => x >= 550);

                    // TotalPower in range [15,80]
                    var thisTotalPower = dataList.SingleOrDefault(x => x >= 15 && x < 50);

                    // return the one with highest leaderSkill and internalValue
                    if (thisLeaderSkill >= leaderSkill && thisInternalValue >= internalValue)
                    {
                        leaderSkill = decimal.ToInt32(thisLeaderSkill);
                        internalValue = decimal.ToInt32(thisInternalValue);
                        totalPower = decimal.ToInt64(thisTotalPower * 10000);
                    }

                    if (line.Contains("あんこ") || line.Contains("アンコ"))
                    {
                        encoreLeaderSkill = decimal.ToInt32(thisLeaderSkill);
                        encoreIntervalValue = decimal.ToInt32(thisInternalValue);
                        encoreTotalPower = decimal.ToInt64(thisTotalPower * 10000);
                    }
                }
            }

            if (encoreLeaderSkill == 0)
            {
                encoreLeaderSkill = leaderSkill;
                encoreIntervalValue = internalValue;
                encoreTotalPower = totalPower;
            }
        }

        public List<ShiftData> GetShiftDataList(Shift currentShift)
        {
            var newShiftDataList = currentShift.ShiftDataList.Select(x => x.ShallowCopy()).ToList();
            foreach (var line in MessageLines)
            {
                var lineSplitArray = line.Contains(",") ? line.Replace(" ", "").Split(",") : line.Split(" ");
                foreach (var timeBlock in lineSplitArray)
                {
                    var activeHours = new List<int>();
                    var matches = new Regex(@"\d?\d").Matches(timeBlock);
                    if (matches.Count == 1)
                    {
                        activeHours.Add(int.Parse(matches[0].Value));
                    }
                    else if (matches.Count == 2)
                    {
                        var startHour = int.Parse(matches[0].Value);
                        var endHour = int.Parse(matches[1].Value);
                        for (var i = startHour; i < endHour; i++)
                        {
                            activeHours.Add(i);
                        }
                    }
                    newShiftDataList.Where(x => activeHours.Contains(x.ShiftTimeStartHour)).ToList()
                        .ForEach(x => x.IsActive = true);
                }
            }
            return newShiftDataList;
        }

        #endregion Methods
    }
}
