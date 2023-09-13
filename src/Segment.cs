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

using System.Numerics;
using Raylib_cs;

namespace JumpDiveClock
{
    public class Segment
    {

        private double _completedTimeAbs;
        private double _pbCompletedTimeAbs;
        private double _startedSegmentTimeAbs;
        public double BestSegmentTimeRel { get; private set; }
        public string Name { get; private set; } = null!;
        public double PbTimeRel { get; private set; }
        public int ResetCount { get; set; }

        public void BeginSegment(double time)
        {
            _startedSegmentTimeAbs = time;
        }

        /*
            This is being used instead of a normal constructor as a workaround. That is because
            YamlDotNet's deserializing function requires this class to have a parameterless
            constructor.
        */
        public void Construct(double pbCompletedTimeAbs)
        {
            _pbCompletedTimeAbs = pbCompletedTimeAbs;
            Reset();
        }

        /// <summary>
        /// Draw segment.
        /// </summary>
        /// <param name="headerHeight">The height of the timer's header.</param>
        /// <param name="drawIndex">The index of this segment in the list of segments.</param>
        /// <param name="separatorHeight">The height of the timer's separator.</param>
        public void Draw(float headerHeight, int drawIndex, float segmentHeight, Font font,
            int fontSpacing, ColorManager colorManager, int separatorHeight, int fontSize,
            int marginSize)
        {
            string pbTimeText = Formatter.SecondsToTime(_pbCompletedTimeAbs, false);
            float segmentStartY = headerHeight + segmentHeight * drawIndex + separatorHeight *
                (drawIndex + 1);
            Vector2 segmentNameSize = Raylib.MeasureTextEx(font, Name, fontSize,
                fontSpacing
            );

            Vector2 pbTimeSize = Raylib.MeasureTextEx(font, pbTimeText, fontSize,
                fontSpacing
            );

            var segmentNamePos = new Vector2(
                marginSize,
                segmentStartY + ((segmentHeight - segmentNameSize.Y) / 2.0f)
                    - separatorHeight * (drawIndex + 1)

            );

            var pbTimePos = new Vector2(
                Raylib.GetScreenWidth() - pbTimeSize.X - marginSize,
                segmentStartY + ((segmentHeight - pbTimeSize.Y) / 2.0f)
                    - separatorHeight * (drawIndex + 1)
            );

            if (_completedTimeAbs != 0)
            {
                string completedTimeTxt = (IsAhead(_completedTimeAbs) ? "-" : "+") +
                    Formatter.SecondsToTime(Math.Abs(_completedTimeAbs - _pbCompletedTimeAbs), true);

                Vector2 completedTimeSize = Raylib.MeasureTextEx(
                    font, completedTimeTxt, fontSize, marginSize
                );
                var completedTimePos = new Vector2(
                    Raylib.GetRenderWidth() - pbTimeSize.X - completedTimeSize.X - marginSize * 2,
                    pbTimePos.Y
                );

                Raylib.DrawTextEx(
                    font, completedTimeTxt, completedTimePos, fontSize, fontSpacing,
                    PickColor(colorManager)
                );
            }

            Raylib.DrawTextEx(font, Name, segmentNamePos, fontSize,
                fontSpacing, colorManager.Base
            );
            Raylib.DrawTextEx(font, pbTimeText, pbTimePos, fontSize, fontSpacing,
                colorManager.Base
            );
        }

        public void FinishSegment(double time)
        {
            _completedTimeAbs = time;
        }

        public double GetAbsoluteCompletionTime() => _completedTimeAbs;

        public double GetRelTime() => _completedTimeAbs - _startedSegmentTimeAbs;

        public bool IsAhead(double timeAbs) => timeAbs < _pbCompletedTimeAbs;

        public bool IsBest() => GetRelTime() < BestSegmentTimeRel;

        public bool IsCompleted() => _completedTimeAbs > 0;

        public void Reset()
        {
            _completedTimeAbs = 0;
            _startedSegmentTimeAbs = 0;
        }

        public void SetPersonalBest()
        {
            PbTimeRel = GetRelTime();
        }

        public void UndoSplit()
        {
            _completedTimeAbs = 0;
        }

        public void UpdateBestSegment()
        {
            if (_completedTimeAbs != 0 && IsBest())
            {
                BestSegmentTimeRel = GetRelTime();
            }
        }

        public bool WasAheadOnFinish() => _completedTimeAbs < _pbCompletedTimeAbs;

        /// <summary>
        /// Picks a color based on how long the segment took to be completed.
        /// </summary>
        private Color PickColor(ColorManager cm)
            => IsBest() ? cm.Best :
                (_completedTimeAbs < _pbCompletedTimeAbs
                    ? (GetRelTime() < PbTimeRel ? cm.AheadGaining : cm.AheadLosing)
                    : (GetRelTime() < PbTimeRel ? cm.BehindGaining : cm.BehindLosing));
    }
}