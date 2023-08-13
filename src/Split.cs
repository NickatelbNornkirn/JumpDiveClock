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
using System.Numerics;

namespace JumpDiveClock
{
    public class Split
    {
        public string GameName;
        public string Category;

        public string BackgroundColor;
        public string BaseColor;
        public string AheadColor;
        public string BehindColor;
        public string BestColor;
        public string SeparatorColor;

        public int ResetCount;

        public List<Segment> Segments;

        public float HeaderSize;
        public float TimerSize;

        public int SeparatorSize;

        private float _timeSecs;
        private bool paused;

        public void Update(float deltaTime)
        {
            if (!paused)
            {
                _timeSecs += deltaTime;
            }
        }

        public void Draw(Font font)
        {
            Color backgroundColor = ToColor(BackgroundColor);
            Color baseColor = ToColor(BaseColor);

            int effectiveHeight = Raylib.GetScreenHeight() - SeparatorSize * (Segments.Count - 1);

            Raylib.ClearBackground(backgroundColor);

            Raylib.DrawRectangle(0, 0, 2000, (int)(effectiveHeight * 0.10f), Color.RED);
            Raylib.DrawTextEx(font, "title", new Vector2(10, 10), 12, 2, baseColor);
        }

        private Color ToColor(string hexColor)
        {
            // Colors are in the #rrggbb format.
            return new Color(
                Convert.ToInt32(hexColor.Substring(1, 2), 16),
                Convert.ToInt32(hexColor.Substring(3, 2), 16),
                Convert.ToInt32(hexColor.Substring(5, 2), 16),
                255
            );
        }
    }
}