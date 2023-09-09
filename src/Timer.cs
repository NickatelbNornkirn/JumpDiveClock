using System.Numerics;
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

/*
    TODO:

    Condition for saving golds

    Advanced to the next split, 5 seconds after end of run or reset.

    Make reset jump to final segment and then reset for real
*/

namespace JumpDiveClock
{
    // TODO: max number of segments per line.
    // TODO: don't stop counting after a reset, so you can undo a reset.
    public class Timer
    {
        private const string BestPossibleTime = "bpt";
        private const int ClearTimerIndex = -1;
        private const string CurrentPace = "cp";
        private const bool ReverseAction = true;
        private const string RunsThatReachHere = "rtrr";
        private const string SumOfBest = "sob";
        private readonly Dictionary<string, string> _statNames = new Dictionary<string, string>() {
            { CurrentPace, "Current pace:" },
            { SumOfBest, "Sum of best segments:" },
            { RunsThatReachHere, "Runs that reach here:" },
            { BestPossibleTime, "Best possible time:" },
        };
        private ColorManager _colorManager = null!;
        private Config _config = null!;
        private int _currentSegment;

        private double _currentTimeSecs;
        private int _displayedSegment;
        private double _displayedTimeSecs;
        private HistoryManager _historyManager = new HistoryManager();

        private InputManager _inputManager = null!;
        public string Category { get; private set; } = null!;
        public int CategoryTitleFontSize { get; private set; }
        public int CategoryTitleFontSpacing { get; private set; }

        public string[] ExtraStats { get; private set; } = null!;

        // TODO: check if all these values are really set by the configuration file.
        // TODO: REFACTOR: better declaration order for these variables.
        public string GameName { get; private set; } = null!;

        public int GameTitleFontSize { get; private set; }
        public int GameTitleFontSpacing { get; private set; }

        public float HeaderHeight { get; private set; }

        public HexColors HexColors { get; private set; } = null!;
        public float MaxSegmentSize { get; private set; }

        public int ResetCount { get; private set; }
        public int SegmentFontSize { get; private set; }
        public int SegmentFontSpacing { get; private set; }

        public int SegmentMargin { get; private set; }

        // TODO: error check.
        public Segment[] Segments { get; private set; } = null!;

        public int SeparatorSize { get; private set; }

        public int TimerFontSize { get; private set; }
        public int TimerFontSpacing { get; private set; }
        public float TimerSize { get; private set; }
        public int TitleCategoryTitlesGap { get; private set; }

        /*
            This is being used instead of a normal constructor as a workaround. That is because
            YamlDotNet's deserializing function requires this class to have a parameterless
            constructor.
        */
        public void Construct(Config config)
        {
            _config = config;
            _inputManager = new InputManager(_config);
            _colorManager = new ColorManager(
                HexColors.Background, HexColors.TextBase, HexColors.PaceAheadGaining,
                HexColors.PaceAheadLosing, HexColors.PaceBehindGaining, HexColors.PaceBehindLosing,
                HexColors.PaceBest, HexColors.Separator
            );
            Reset();
            InitializeSegments();
        }

        public void Draw(Font font)
        {
            int effectiveHeight = Raylib.GetScreenHeight() - SeparatorSize * (Segments.Length - 1);

            var headerHeight = (int)(effectiveHeight * (HeaderHeight / 100.0f));
            var timerHeight = (int)(effectiveHeight * (TimerSize / 100.0f));

            effectiveHeight -= headerHeight + timerHeight;

            var segmentHeight =
                (int)Math.Min(effectiveHeight / (Segments.Length + ExtraStats.Length),
                    MaxSegmentSize / 100 * effectiveHeight);

            Raylib.ClearBackground(_colorManager.Background);

            DrawSeparators(headerHeight, segmentHeight, timerHeight);
            DrawHeader(font, headerHeight);
            DrawSegments(font, headerHeight, segmentHeight);
            DrawTimer(font, timerHeight, segmentHeight, headerHeight);
            DrawExtraStats(font, segmentHeight);
        }

        public void Update()
        {
            _inputManager.UpdateKeyboardState(_config);

            float deltaTime = Raylib.GetFrameTime();

            if (_currentSegment >= 0 && _currentSegment < Segments.Length)
            {
                _currentTimeSecs += deltaTime;
            }

            if (_currentSegment != ClearTimerIndex)
            {
                _displayedSegment = _currentSegment;
                _displayedTimeSecs = _currentTimeSecs;
            }

            if (_inputManager.IsKeyPressed(_config.Keybindings.Split))
            {
                Split();
            }

            if (_inputManager.IsKeyPressed(_config.Keybindings.Reset))
            {
                Reset();
            }

            if (_inputManager.IsKeyPressed(_config.Keybindings.Undo))
            {
                Undo();
            }

            if (_inputManager.IsKeyPressed(_config.Keybindings.Redo))
            {
                Redo();
            }

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
                    SegmentFontSpacing, _colorManager.Base
                );
            },
                true
            );
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
                _colorManager.Base
            );
            Raylib.DrawTextEx(
                font, Category, categoryTitlePos, CategoryTitleFontSize, CategoryTitleFontSpacing,
                _colorManager.Base
            );
        }

        private void DrawSegments(Font font, float headerHeight, float segmentHeight)
        {
            for (int i = 0; i < Segments.Length; i++)
            {
                Segments[i].Draw(headerHeight, i, segmentHeight, font, SegmentFontSpacing,
                    _colorManager, SeparatorSize, SegmentFontSize, SegmentMargin
                );
            }
        }

        private void DrawSeparators(int headerHeight, int segmentSize, int timerHeight)
        {
            Raylib.DrawRectangle(0, headerHeight, Raylib.GetRenderWidth(), SeparatorSize,
                _colorManager.Separator
            );

            for (int i = 1; i < Segments.Length + 1; i++)
            {
                Raylib.DrawRectangle(
                    0, headerHeight + i * segmentSize, Raylib.GetRenderWidth(), SeparatorSize,
                    _colorManager.Separator
                );
            }

            int timerOffset = ExtraStats.Length * segmentSize +
                (ExtraStats.Length - 1) * SeparatorSize;
            int timerY = Raylib.GetRenderHeight() - timerHeight - SeparatorSize - timerOffset;
            Raylib.DrawRectangle(
                0, timerY, Raylib.GetRenderWidth(), SeparatorSize, _colorManager.Separator
            );
            Raylib.DrawRectangle(0, timerY + timerHeight, Raylib.GetRenderWidth(), SeparatorSize,
                _colorManager.Separator
            );

            for (int i = 1; i < ExtraStats.Length; i++)
            {
                Raylib.DrawRectangle(0,
                    timerY + timerHeight + i * segmentSize + SeparatorSize * (i - 1),
                    Raylib.GetRenderWidth(), SeparatorSize, _colorManager.Separator
                );
            }
        }

        private void DrawTimer(Font font, float timerHeight, float segmentHeight,
                float headerHeight)
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
                font, timerText, textPos, TimerFontSize, TimerFontSpacing, _colorManager.Base
            );
        }

        private void InitializeSegments()
        {
            for (int i = 0; i < Segments.Length; i++)
            {
                double absPbTime = 0;
                for (int j = i; j >= 0; j--)
                {
                    absPbTime += Segments[j].PbTimeRel;
                }

                Segments[i].Construct(absPbTime);
            }
        }

        private void Pause()
        {

        }

        private void Redo()
        {

        }

        private void Reset()
        {
            _currentTimeSecs = 0;
            _currentSegment = ClearTimerIndex;
            _historyManager.ClearHistory();
        }

        private void Split()
        {
            if (_currentSegment <= ClearTimerIndex)
            {
                _currentSegment = 0;
                return;
            }

            Segments[_currentSegment].FinishSegment(_currentTimeSecs);
            _currentSegment++;
        }

        private void Undo()
        {
            if (_currentSegment > 0)
            {
                Console.WriteLine("A");
                _currentSegment--;
                _historyManager.RegisterUndo(_currentTimeSecs);
            }
        }
    }
}