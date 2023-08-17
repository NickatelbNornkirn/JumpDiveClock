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

namespace JumpDiveClock
{
    public class Segment
    {
        public string Name = null!;
        public float PbTime;
        public float BestSegment;
        public int ResetCount;

        public bool CompletedSegment = false;
        public float CompletedTime = 0;

        public string GetSegmentText(float runTime)
        {
            float time = CompletedSegment ? CompletedTime : runTime;

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
    }
}