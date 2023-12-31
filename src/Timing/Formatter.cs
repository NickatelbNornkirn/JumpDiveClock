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

namespace JumpDiveClock.Timing
{
    public static class Formatter
    {
        public static string SecondsToTime(double seconds, bool detailed)
        {
            const int MinuteInSecs = 60;
            const int HourInSecs = MinuteInSecs * 60;
            const char Separator = ':';

            var ss = seconds < MinuteInSecs ? seconds : seconds % MinuteInSecs;
            var hh = (int)(seconds >= HourInSecs ? seconds / HourInSecs : 0);
            var mm = (int)(seconds < HourInSecs
                            ? seconds / MinuteInSecs
                            : (seconds - hh * HourInSecs) / MinuteInSecs);

            string result = "";

            if (hh > 0)
            {
                result += $"{hh}{Separator}";
            }

            result += $"{mm:D2}{Separator}";
            result += $"{(detailed ? ss : (int)ss).ToString("00" + (detailed ? ".0" : ""))}";

            return result;
        }
    }
}