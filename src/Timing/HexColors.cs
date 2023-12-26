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
    public class HexColors
    {
        public string Background = null!;
        public string DetailedTimer = null!;
        public string PaceAheadGaining = null!;
        public string PaceAheadLosing = null!;
        public string PaceBehindGaining = null!;
        public string PaceBehindLosing = null!;
        public string PaceBest = null!;
        public string Separator = null!;
        public string TextBase = null!;

        /*
            This is used instead of a construct because YAML deserialization requires
            a parameterless constructor.
        */
        public HexColors Construct(string background, string paceAheadGaining, string paceAheadLosing,
                string paceBehindGaining, string paceBehindLosing, string paceBest,
                string separator, string textBase, string detailedTimer)
        {
            Background = background;
            PaceAheadGaining = paceAheadGaining;
            PaceAheadLosing = paceAheadLosing;
            PaceBehindGaining = paceBehindGaining;
            PaceBehindLosing = paceBehindLosing;
            PaceBest = paceBest;
            Separator = separator;
            TextBase = textBase;
            DetailedTimer = detailedTimer;

            return this;
        }
    }
}
