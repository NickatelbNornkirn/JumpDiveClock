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

/*
    TODO:

    Condition for saving golds

    Advanced to the next split, 5 seconds after end of run or reset.
*/

namespace JumpDiveClock
{
    // TODO: max number of segments per line.
    // TODO: don't stop counting after a reset, so you can undo a reset.
    public class Timer
    {
        private const string CurrentPace = "cp";
        private const string SumOfBest = "sob";
        private const string RunsThatReachHere = "rtrr";
        private const string BestPossibleTime = "bpt";
        private readonly Dictionary<string, string> _statNames = new Dictionary<string, string>() {
            { CurrentPace, "Current pace:" },
            { SumOfBest, "Sum of best segments:" },
            { RunsThatReachHere, "Runs that reach here:" },
            { BestPossibleTime, "Best possible time:" },
        };

        // TODO: check if all these values are really set by the configuration file.
        // TODO: REFACTOR: better declaration order for these variables.
        public string GameName { get; set; } = null!;
        public string Category { get; set; } = null!;

        public HexColors HexColors { get; set; } = null!;

        public int ResetCount { get; set; }

        // TODO: error check.
        public Segment[] Segments { get; set; } = null!;

        public float HeaderHeight { get; set; }
        public float TimerSize { get; set; }
        public float MaxSegmentSize { get; set; }

        public int SeparatorSize { get; set; }

        public int GameTitleFontSize { get; set; }
        public int GameTitleFontSpacing { get; set; }
        public int CategoryTitleFontSize { get; set; }
        public int CategoryTitleFontSpacing { get; set; }
        public int TitleCategoryTitlesGap { get; set; }

        public int SegmentMargin { get; set; }
        public int SegmentFontSize { get; set; }
        public int SegmentFontSpacing { get; set; }

        public int TimerFontSize { get; set; }
        public int TimerFontSpacing { get; set; }

        public string[] ExtraStats { get; set; } = null!;

        private double _currentTimeSecs;
        private double _displayedTimeSecs;
        private int _currentSegment;
        private int _displayedSegment;

        private InputManager _inputManager = null!;
        private HistoryManager _historyManager = new HistoryManager();
        private Config _config = null!;

        private Color _backgroundColor;
        private Color _baseColor;
        private Color _aheadGainingColor;
        private Color _aheadLosingColor;
        private Color _behindGainingColor;
        private Color _behindLosingColor;
        private Color _bestColor;
        private Color _separatorColor;

        /*
            This is being used instead of a normal constructor as a workaround. That is because
            YamlDotNet's deserializing function requires this class to have a parameterless
            constructor.
        */
        public Timer Construct(Config config)
        {
            _config = config;
            _inputManager = new InputManager(_config);
            ConvertHexColorsToRgb();
            Reset();

            return this;
        }

        public void ConvertHexColorsToRgb()
        {
            _backgroundColor = ToColor(HexColors.Background);
            _baseColor = ToColor(HexColors.TextBase);
            _aheadGainingColor = ToColor(HexColors.PaceAheadGaining);
            _aheadLosingColor = ToColor(HexColors.PaceAheadLosing);
            _behindGainingColor = ToColor(HexColors.PaceBehindGaining);
            _behindLosingColor = ToColor(HexColors.PaceBehindLosing);
            _bestColor = ToColor(HexColors.PaceBest);
            _separatorColor = ToColor(HexColors.Separator);
        }

        public void Update()
        {
            _inputManager.UpdateKeyboardState(_config);

            float deltaTime = Raylib.GetFrameTime();

            if (_currentSegment >= 0)
            {
                _currentTimeSecs += deltaTime;
            }

            if (_inputManager.IsKeyPressed(_config.Keybindings.Split))
            {
                Split();
                _historyManager.RegisterActionExecution(ActionType.Split);
            }
            
            if (_inputManager.IsKeyPressed(_config.Keybindings.Reset))
            {
                Reset();
                _historyManager.RegisterActionExecution(ActionType.Reset);
            }

        }

        public void Draw(Font font)
        {
            int effectiveHeight = Raylib.GetScreenHeight() - SeparatorSize * (Segments.Length - 1);

            var headerHeight = (int)(effectiveHeight * (HeaderHeight / 100.0f));
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

        private void Split(bool reverse = false)
        {
            _currentSegment += reverse ? -1 : 1;
        }

        private void Reset(bool reverse = false)
        {
            _currentTimeSecs = reverse ? _displayedTimeSecs : 0;
            _currentSegment = reverse ? _displayedSegment : -1;
        }

        // TODO: replace real variables with display variables
        private void DrawTimer(Font font, float timerHeight, float segmentHeight, float headerHeight)
        {
            string timerText = Formatter.SecondsToTime(_displayedTimeSecs);
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
            ExtraStats.ForeachI((statCode, i) =>
            {
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
            Raylib.DrawRectangle(0, timerY, Raylib.GetRenderWidth(), SeparatorSize, _separatorColor);
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
            for (int i = 0; i < Segments.Length; i++)
            {
                Segments[i].Draw(headerHeight, i, segmentHeight, font, SegmentFontSpacing,
                    _baseColor, SeparatorSize, SegmentFontSize, SegmentMargin
                );
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

    }
}