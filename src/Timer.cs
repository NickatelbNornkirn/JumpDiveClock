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
using System.Data;
using System.Diagnostics;
using System.Numerics;

namespace JumpDiveClock
{
    public class Timer
    {
        private const string CurrentPace = "cp";
        private const string SumOfBest = "sob";
        private const string RunsThatReachHere = "rtrr";
        private readonly Dictionary<string, string> _statNames = new Dictionary<string, string>() {
            {CurrentPace, "Current pace:" },
            {SumOfBest, "Sum of best segments:" },
            {RunsThatReachHere, "Runs that reach here:" },
        };

        // TODO: check if all these values are really set by the configuration file.
        // TODO: REFACTOR: better declaration order for these variables.
        public string GameName = null!;
        public string Category = null!;

        public HexColors HexColors = null!;

        public int ResetCount;

        // TODO: error check.
        public Segment[] Segments = null!;

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

        public int TimerFontSize;
        public int TimerFontSpacing;

        public string[] ExtraStats = null!;

        private double _timeSecs;
        int _currentSegment;
        private bool _counting;

        private Color _backgroundColor;
        private Color _baseColor;
        private Color _aheadColor;
        private Color _behindColor;
        private Color _bestColor;
        private Color _separatorColor;

        public void ConvertHexColorsToRgb()
        {
            _backgroundColor = ToColor(HexColors.Background);
            _baseColor = ToColor(HexColors.TextBase);
            _aheadColor = ToColor(HexColors.PaceAhead);
            _behindColor = ToColor(HexColors.PaceBehind);
            _bestColor = ToColor(HexColors.PaceBest);
            _separatorColor = ToColor(HexColors.Separator);
        }

        public void Update(Config appConfig)
        {
            List<int> pressedKeys = GetPressedKeys(appConfig);
            float deltaTime = Raylib.GetFrameTime();

            if (_counting)
            {
                _timeSecs += deltaTime;
            }
        }

        public void Draw(Font font)
        {
            int effectiveHeight = Raylib.GetScreenHeight() - SeparatorSize * (Segments.Length - 1);

            var headerHeight = (int)(effectiveHeight * (HeaderSize / 100.0f));
            var timerHeight = (int)(effectiveHeight * (TimerSize / 100.0f));

            effectiveHeight -= headerHeight + timerHeight;

            var segmentHeight = (int)Math.Min(effectiveHeight / (Segments.Length + ExtraStats.Length),
                MaxSegmentSize / 100 * effectiveHeight
            );

            Raylib.ClearBackground(_backgroundColor);
            
            DrawSeparators(headerHeight, segmentHeight, timerHeight);
            DrawHeader(font, headerHeight);
            DrawSegments(font, headerHeight, segmentHeight);
            DrawTimer(font, timerHeight, segmentHeight, headerHeight);
            DrawExtraStats(font, segmentHeight);

        }

        private void Reset()
        {
            _timeSecs = 0;
            _currentSegment = 0;
            _counting = false;
        }

        private void DrawTimer(Font font, float timerHeight, float segmentHeight, float headerHeight)
        {
            string timerText = Formatter.SecondsToTime(_timeSecs);
            Vector2 textSize = Raylib.MeasureTextEx(
                font, timerText, TimerFontSize, TimerFontSpacing
            );

            var textPos = new Vector2(
                (Raylib.GetRenderWidth() - textSize.X) / 2.0f,
                Raylib.GetRenderHeight() - segmentHeight * ExtraStats.Length - SeparatorSize
                    * (ExtraStats.Length - 1) - (timerHeight + textSize.Y) / 2.0f
            );

            // TODO: color
            Raylib.DrawTextEx(
                font, timerText, textPos, TimerFontSize, TimerFontSpacing, _baseColor
            );
        }

        private void DrawExtraStats(Font font, float segmentHeight)
        {
            ExtraStats.ForeachI((statCode, i) => {
                    string statName = _statNames[statCode];
                    Vector2 statNameSize = Raylib.MeasureTextEx(
                        font, statName, SegmentFontSize, SegmentFontSpacing
                    );
                    var leftTextDrawPos = new Vector2(SegmentMargin,
                        Raylib.GetRenderHeight() -
                            ((i + 1) * segmentHeight + (i + 1) * SeparatorSize)
                            + (segmentHeight - statNameSize.Y) / 2.0f
                    );

                    Raylib.DrawTextEx(font, statName, leftTextDrawPos, SegmentFontSize,
                        SegmentFontSpacing, _baseColor
                    );
                },
                true
            );
        }

        private void DrawSeparators(int headerHeight, int segmentSize, int timerHeight)
        {
            Raylib.DrawRectangle(0, headerHeight, Raylib.GetRenderWidth(), SeparatorSize,
                _separatorColor
            );
            for (int i = 1; i < Segments.Length + 1; i++)
            {
                Raylib.DrawRectangle(
                    0, headerHeight + i * segmentSize, Raylib.GetRenderWidth(), SeparatorSize,
                    _separatorColor
                );
            }

            int timerOffset = ExtraStats.Length * segmentSize +
                (ExtraStats.Length - 1) * SeparatorSize;
            int timerY = Raylib.GetRenderHeight() - timerHeight - SeparatorSize - timerOffset;
            Raylib.DrawRectangle(0, timerY, Raylib.GetRenderWidth(),SeparatorSize, _separatorColor);
            Raylib.DrawRectangle(0, timerY + timerHeight, Raylib.GetRenderWidth(), SeparatorSize,
                _separatorColor
            );

            for (int i = 1; i < ExtraStats.Length; i++)
            {
                Raylib.DrawRectangle(0,
                    timerY + timerHeight + i * segmentSize + SeparatorSize * (i - 1),
                    Raylib.GetRenderWidth(), SeparatorSize, _separatorColor
                );
            }
        }

        private void DrawHeader(Font font, float headerHeight)
        {
            Vector2 gameTitleSize = Raylib.MeasureTextEx(font, GameName, GameTitleFontSize,
                GameTitleFontSpacing
            );
            Vector2 categoryTitleSize = Raylib.MeasureTextEx(
                font, Category, CategoryTitleFontSize, CategoryTitleFontSpacing
            );
            float textLayoutHeight = gameTitleSize.Y + categoryTitleSize.Y + TitleCategoryTitlesGap;
            float textLayoutStartY = (headerHeight - textLayoutHeight) / 2.0f;
            var gameTitlePos = new Vector2((Raylib.GetScreenWidth() - gameTitleSize.X) / 2.0f,
                textLayoutStartY
            );
            var categoryTitlePos = new Vector2(
                (Raylib.GetScreenWidth() - categoryTitleSize.X) / 2.0f,
                textLayoutStartY + TitleCategoryTitlesGap + gameTitleSize.Y
            );
            Raylib.DrawTextEx(font, GameName, gameTitlePos, GameTitleFontSize, GameTitleFontSpacing,
                _baseColor
            );
            Raylib.DrawTextEx(
                font, Category, categoryTitlePos, CategoryTitleFontSize, CategoryTitleFontSpacing,
                _baseColor
            );
        }

        private void DrawSegments(Font font, float headerHeight, float segmentHeight)
        {
            Segments.ForeachI((segment, i) => {
                string pbTimeText = Formatter.SecondsToTime(segment.PbTime);
                float segmentStartY = headerHeight + segmentHeight * i + SeparatorSize * (i + 1);
                Vector2 segmentNameSize = Raylib.MeasureTextEx(font, segment.Name, SegmentFontSize,
                    SegmentFontSpacing
                );
                // TODO: include time loss/gain.
                Vector2 pbTimeSize = Raylib.MeasureTextEx(font, pbTimeText, SegmentFontSize,
                    SegmentFontSpacing
                );

                var segmentNamePos = new Vector2(
                    SegmentMargin, segmentStartY + ((segmentHeight - segmentNameSize.Y) / 2.0f)
                );

                var pbTimePos = new Vector2(
                    Raylib.GetScreenWidth() - pbTimeSize.X - SegmentMargin,
                    segmentStartY + ((segmentHeight - pbTimeSize.Y) / 2.0f)
                );

                Raylib.DrawTextEx(font, segment.Name, segmentNamePos, SegmentFontSize,
                    SegmentFontSpacing, _baseColor
                );
                Raylib.DrawTextEx(font, pbTimeText, pbTimePos, SegmentFontSize, SegmentFontSpacing,
                    _baseColor
                );
            });
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
    }
}