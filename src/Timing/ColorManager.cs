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

using Raylib_cs;

namespace JumpDiveClock.Timing
{
    public class ColorManager
    {

        /*
            The constructors receives #rrggbb or #rrggbbaa colors.
        */
        public ColorManager(string backgroundColor, string baseColor, string aheadGainingColor,
                            string aheadLosingColor, string behindGainingColor,
                            string behindLosingColor, string bestColor, string separatorColor,
                            string detailedTimer)
        {
            Background = ToColor(backgroundColor);
            Base = ToColor(baseColor);
            AheadGaining = ToColor(aheadGainingColor);
            AheadLosing = ToColor(aheadLosingColor);
            BehindGaining = ToColor(behindGainingColor);
            BehindLosing = ToColor(behindLosingColor);
            Best = ToColor(bestColor);
            Separator = ToColor(separatorColor);
            DetailedTimer = ToColor(detailedTimer);
        }

        public Color AheadGaining { get; }
        public Color AheadLosing { get; }
        public Color Background { get; }
        public Color Base { get; }
        public Color BehindGaining { get; }
        public Color BehindLosing { get; }
        public Color Best { get; }
        public Color Separator { get; }
        public Color DetailedTimer { get; }

        private Color ToColor(string hexColor)
        {
            Console.WriteLine($"Parsing color '{hexColor}'");

            if (hexColor.Length != 7 && hexColor.Length != 9)
            {
                Console.WriteLine($"Failed to parse color '{hexColor}'. Not in the correct format");
                Console.WriteLine("Valid formats are '#rrggbb' and '#rrggbbaa'");
                Console.WriteLine("Returning default color (transparent yellow)");
                // Arbitrary color as default.
                return new Color(0xFF, 0xFF, 0x00, 0x7f);
            }

            // Length 9 means #rrggbbaa format
            int alpha = hexColor.Length == 9 ? Convert.ToInt32(hexColor.Substring(7, 2), 16) : 0xff;

            // Colors are in the #rrggbbaa format.
            return new Color(
                Convert.ToInt32(hexColor.Substring(1, 2), 16),
                Convert.ToInt32(hexColor.Substring(3, 2), 16),
                Convert.ToInt32(hexColor.Substring(5, 2), 16),
                alpha
            );
        }
    }
}
