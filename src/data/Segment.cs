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
    public class Segment
    {
        public string Name = null!;
        public double PbTime;
        public double BestSegment;
        public int ResetCount;

        public bool CompletedSegment = false;
        public double CompletedTime = 0;

        public string GetSegmentText(double runTime)
        {
            double time = CompletedSegment ? CompletedTime : runTime;

            string timeText;
            if (time > PbTime)
            {
                timeText = "+";
            }
            else if (time < PbTime)
            {
                timeText = "-";
            }
            else
            {
                timeText = " ";
            }

            timeText += Formatter.SecondsToTime(time);

            return timeText;
        }

        /// <summary>
        /// Draw segment.
        /// </summary>
        /// <param name="headerHeight">The height of the timer's header.</param>
        /// <param name="drawIndex">The index of this segment in the list of segments.</param>
        /// <param name="separatorHeight">The height of the timer's separator.</param>
        public void Draw(float headerHeight, int drawIndex, float segmentHeight, Font font,
            int fontSpacing, Color textColor, int separatorHeight, int fontSize, int marginSize)
        {
            string pbTimeText = Formatter.SecondsToTime(PbTime);
            float segmentStartY = headerHeight + segmentHeight * drawIndex + separatorHeight *
                (drawIndex + 1);
            Vector2 segmentNameSize = Raylib.MeasureTextEx(font, Name, fontSize,
                fontSpacing
            );
            // TODO: include time loss/gain.
            Vector2 pbTimeSize = Raylib.MeasureTextEx(font, pbTimeText, fontSize,
                fontSpacing
            );

            var segmentNamePos = new Vector2(
                marginSize, segmentStartY + ((segmentHeight - segmentNameSize.Y) / 2.0f)
            );

            var pbTimePos = new Vector2(
                Raylib.GetScreenWidth() - pbTimeSize.X - marginSize,
                segmentStartY + ((segmentHeight - pbTimeSize.Y) / 2.0f)
            );

            Raylib.DrawTextEx(font, Name, segmentNamePos, fontSize,
                fontSpacing, textColor
            );
            Raylib.DrawTextEx(font, pbTimeText, pbTimePos, fontSize, fontSpacing,
                textColor
            );
        }
    }
}