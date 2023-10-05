/*
    JumpDiveClock -  Simple-ish speedrun timer for X11.
    Copyright (C) 2023  Nickatelb Nornkirn

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

namespace JumpDiveClock
{
    public class Stats
    {
        public readonly Dictionary<StatType, string> _statNames =
            new Dictionary<StatType, string>()
        {
            { StatType.CurrentPace, "Current pace:" },
            { StatType.SumOfBest, "Sum of best:" },
            { StatType.RunsThatReachHere, "Runs that reach here:" },
            { StatType.BestPossibleTime, "Best possible time:" },
            { StatType.PersonalBest, "Personal best: "},
            { StatType.WorldRecord, "World record: "}
        };
        private readonly Timer _timer;

        public Stats(Timer timer)
        {
            _timer = timer;
        }

        public string GetStatName(StatType stat) => _statNames[stat];

        public string GetStatText(StatType stat)
        {
            string result = "";

            switch (stat)
            {
                case StatType.BestPossibleTime:
                    result = GetBestPossibleTime();
                    break;
                case StatType.CurrentPace:
                    result = GetCurrentPace();
                    break;
                case StatType.RunsThatReachHere:
                    result = GetRunsThatReachHere();
                    break;
                case StatType.PersonalBest:
                    result = GetPersonalBest();
                    break;
                case StatType.WorldRecord:
                    result = GetWorldRecord();
                    break;
                case StatType.SumOfBest:
                    result = GetSumOfBest();
                    break;
            }

            return result;
        }

        private string GetBestPossibleTime()
        {
            double tt = 0;
            foreach (Segment sgm in _timer.Segments)
            {
                if (!sgm.RanSegmentBefore())
                {
                    return "**:**";
                }

                tt += sgm.IsCompleted() ? sgm.GetRelTime() : sgm.BestSegmentTimeRel;
            }

            return Formatter.SecondsToTime(tt, false);
        }

        private string GetCurrentPace()
        {
            double tm = 0;
            for (int i = 0; i < _timer.Segments.Length; i++)
            {
                Segment sgm = _timer.Segments[i];

                if (!sgm.RanSegmentBefore())
                {
                    return "**:**";
                }

                if (sgm.IsCompleted())
                {
                    tm += sgm.GetRelTime() - sgm.PbTimeRel;
                }
            }

            double resultT = _timer.GetPbTime() + tm;

            return Formatter.SecondsToTime(resultT, false);
        }

        private string GetPersonalBest()
        {
            if (!_timer.HasPb())
            {
                return "**:**";
            }

            return Formatter.SecondsToTime(_timer.GetPbTime(), false);
        }

        private string GetRunsThatReachHere()
        {
            string result;
            if (_timer.HasStarted() && _timer.GetCurrentSegment() > 0)
            {
                double rc = 0;
                for (int i = 0; i < _timer.GetCurrentSegment(); i++)
                {
                    rc += _timer.Segments[i].ResetCount;
                }
                double p = (_timer.AttemptCount - rc) / Math.Max(_timer.AttemptCount, 1) * 100.0;
                result = p.ToString("00.00") + "%";
            }
            else
            {
                result = "***";
            }

            return result;
        }

        private string GetSumOfBest()
        {
            double r = 0;
            foreach (Segment sgm in _timer.Segments)
            {
                if (!sgm.RanSegmentBefore())
                {
                    return "**:**";
                }

                r += sgm.BestSegmentTimeRel;
            }

            return Formatter.SecondsToTime(r, false);
        }

        private string GetWorldRecord()
            => Formatter.SecondsToTime(_timer.WorldRecordSeconds, false)
                + $" by {_timer.WorldRecordOwner}";
    }
}