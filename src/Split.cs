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
using System.Diagnostics;
using System.Numerics;

namespace JumpDiveClock
{
    public class Split
    {
        // TODO: check if all these values are really set by the configuration file.
        // TODO: REFACTOR: better declaration order for these variables.
        public string GameName = null!;
        public string Category = null!;

        public string BackgroundColorHex = null!;
        public string BaseColorHex = null!;
        public string AheadColorHex = null!;
        public string BehindColorHex = null!;
        public string BestColorHex = null!;
        public string SeparatorColorHex = null!;

        public int ResetCount;

        public List<Segment> Segments = null!;

        public float HeaderSize;
        public float TimerSize;
        public float MaxSegmentSize;

        public int SeparatorSize;

        public int GameTitleFontSize;
        public int GameTitleFontSpacing;
        public int CategoryTitleFontSize;
        public int CategoryTitleFontSpacing;
        public int TitleCategoryTitlesGap;

        public int SegmentMargin;
        public int SegmentFontSize;
        public int SegmentFontSpacing;

        private float _timeSecs;
        private bool _paused;

        private Color _backgroundColor;
        private Color _baseColor;
        private Color _aheadColor;
        private Color _behindColor;
        private Color _bestColor;
        private Color _separatorColor;

        public void ConvertHexColorsToRgb()
        {
            _backgroundColor = ToColor(BackgroundColorHex);
            _baseColor = ToColor(BaseColorHex);
            _aheadColor = ToColor(AheadColorHex);
            _behindColor = ToColor(BehindColorHex);
            _bestColor = ToColor(BestColorHex);
            _separatorColor = ToColor(SeparatorColorHex);
        }

        public void Update(Config appConfig)
        {
            List<int> pressedKeys = GetPressedKeys(appConfig);
            float deltaTime = Raylib.GetFrameTime();

            if (!_paused)
            {
                _timeSecs += deltaTime;
            }
        }

        public void Draw(Font font)
        {
            int effectiveHeight = Raylib.GetScreenHeight() - SeparatorSize * (Segments.Count - 1);

            var headerHeight = (int)(effectiveHeight * (HeaderSize / 100.0f));
            var timerHeight = (int)(effectiveHeight * (TimerSize / 100.0f));

            effectiveHeight -= headerHeight + timerHeight;

            var segmentHeight = (int)Math.Min(
                effectiveHeight / Segments.Count, MaxSegmentSize / 100 * effectiveHeight
            );

            Raylib.ClearBackground(_backgroundColor);

            DrawSeparators(headerHeight, segmentHeight);
            DrawHeader(font, headerHeight);
            DrawSegments(font, headerHeight, segmentHeight);
        }

        private void DrawSeparators(int headerHeight, int segmentSize)
        {
            Raylib.DrawRectangle(0, headerHeight, Raylib.GetRenderWidth(), SeparatorSize, _separatorColor);
            for (int i = 1; i < Segments.Count + 1; i++)
            {
                Raylib.DrawRectangle(
                    0, headerHeight + i * segmentSize, Raylib.GetRenderWidth(), SeparatorSize, _separatorColor
                );
            }
        }

        private void DrawHeader(Font font, float headerHeight)
        {
            Vector2 gameTitleSize = Raylib.MeasureTextEx(font, GameName, GameTitleFontSize, GameTitleFontSpacing);
            Vector2 categoryTitleSize = Raylib.MeasureTextEx(
                font, Category, CategoryTitleFontSize, CategoryTitleFontSpacing
            );
            float textLayoutHeight = gameTitleSize.Y + categoryTitleSize.Y + TitleCategoryTitlesGap;
            float textLayoutStartY = (headerHeight - textLayoutHeight) / 2.0f;
            var gameTitlePos = new Vector2((Raylib.GetScreenWidth() - gameTitleSize.X) / 2.0f, textLayoutStartY);
            var categoryTitlePos = new Vector2(
                (Raylib.GetScreenWidth() - categoryTitleSize.X) / 2.0f,
                textLayoutStartY + TitleCategoryTitlesGap + gameTitleSize.Y
            );
            Raylib.DrawTextEx(font, GameName, gameTitlePos, GameTitleFontSize, GameTitleFontSpacing, _baseColor);
            Raylib.DrawTextEx(
                font, Category, categoryTitlePos, CategoryTitleFontSize, CategoryTitleFontSpacing, _baseColor
            );
        }

        private void DrawSegments(Font font, float headerHeight, float segmentHeight)
        {
            for (int i = 0; i < Segments.Count; i++)
            {
                Segment segment = Segments[i];
                string pbTimeText = SecondsToTime(segment.PbTime);
                float segmentStartY = headerHeight + segmentHeight * i + SeparatorSize * (i + 1);
                Vector2 segmentNameSize = Raylib.MeasureTextEx(font, segment.Name, SegmentFontSize, SegmentFontSpacing);
                // TODO: include time loss/gain.
                Vector2 pbTimeSize = Raylib.MeasureTextEx(font, pbTimeText, SegmentFontSize, SegmentFontSpacing);

                var segmentNamePos = new Vector2(
                    SegmentMargin, segmentStartY + ((segmentHeight - segmentNameSize.Y) / 2.0f)
                );

                var pbTimePos = new Vector2(
                    Raylib.GetScreenWidth() - pbTimeSize.X - SegmentMargin,
                    segmentStartY + ((segmentHeight - pbTimeSize.Y) / 2.0f)
                );

                Raylib.DrawTextEx(font, segment.Name, segmentNamePos, SegmentFontSize, SegmentFontSpacing, _baseColor);
                Raylib.DrawTextEx(font, pbTimeText, pbTimePos, SegmentFontSize, SegmentFontSpacing, _baseColor);
            }
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

        private List<int> GetPressedKeys(Config appConfig)
        {
            var p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "xinput",
                    Arguments = $"query-state {appConfig.KeyboardId}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            p.Start();
            string[] lines = p.StandardOutput.ReadToEnd().Split('\n');

            var kbStates = new List<int>();
            lines
                .Where(line => line.Contains("=down")).ToList()
                .ForEach(line => kbStates.Add(Int32.Parse(line.Split('[')[1].Split(']')[0])));

            return kbStates;
        }

        // TODO: test throughly.
        private string SecondsToTime(float seconds)
        {
            const int MinuteInSecs = 60;
            const int HourInSecs = MinuteInSecs * 60;
            const char Separator = ':';

            var ss = (int)(seconds < MinuteInSecs ? seconds : seconds % MinuteInSecs);
            var mm = (int)(seconds < HourInSecs ? seconds / MinuteInSecs : seconds % HourInSecs);
            var hh = (int)(seconds >= HourInSecs ? seconds / HourInSecs : 0);

            string result = "";

            if (hh > 0)
            {
                result += hh + Separator;
            }

            result += $"{mm.ToString("D2")}{Separator}{ss.ToString("D2")}";

            return result;
        }
    }
}