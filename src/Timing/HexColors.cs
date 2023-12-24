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
        public string Background { get; set; } = null!;
        public string PaceAheadGaining { get; set; } = null!;
        public string PaceAheadLosing { get; set; } = null!;
        public string PaceBehindGaining { get; set; } = null!;
        public string PaceBehindLosing { get; set; } = null!;
        public string PaceBest { get; set; } = null!;
        public string Separator { get; set; } = null!;
        public string TextBase { get; set; } = null!;

        /*
            This is used instead of a construct because YAML deserialization requires
            a parameterless constructor.
        */
        public HexColors Construct(string background, string paceAheadGaining, string paceAheadLosing,
                string paceBehindGaining, string paceBehindLosing, string paceBest,
                string separator, string textBase)
        {
            Background = "#252525";
            PaceAheadGaining = "#1dbd48";
            PaceAheadLosing = "#6cbd82";
            PaceBehindGaining = "#da7c7c";
            PaceBehindLosing = "#da2121";
            PaceBest = "#fff63e";
            Separator = "#555555";
            TextBase = "#f2f2f2";

            return this;
        }
    }
}
