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

using JumpDiveClock.Settings;
using JumpDiveClock.Input;
using JumpDiveClock.Misc;
using Raylib_cs;
using System.Numerics;

namespace JumpDiveClock.Timing
{
    public class SpeedrunTimer
    {
        public float AttemptSizeTextPosX;
        public float AttemptSizeTextPosY;
        private const int ClearTimerIndex = -1;
        private const double NoPbTime = -1;

        private ColorManager _colors = null!;
        private Config _config = null!;
        private int _currentSegment;

        private double _currentTimeSecs;

        private GlobalInputManager _globalInputManager = null!;
        private HistoryManager _history = new HistoryManager();
        private double _pbTimeSecs;
        private Stats _stats = null!;
        private StorageManager _storage = null!;

        public int AttemptCount { get; private set; }
        public int AttemptCountFontSize { get; private set; }
        public int AttemptCountFontSpacing { get; private set; }
        public string Category { get; private set; } = null!;
        public int CategoryTitleFontSize { get; private set; }
        public int CategoryTitleFontSpacing { get; private set; }
        public bool CompletedRunBefore { get; private set; }
        public StatType[] ExtraStats { get; private set; } = null!;

        public string GameName { get; private set; } = null!;

        public int GameTitleFontSize { get; private set; }
        public int GameTitleFontSpacing { get; private set; }

        public float HeaderHeight { get; private set; }

        public HexColors HexColors { get; private set; } = null!;
        public float MaxSegmentSize { get; private set; }
        public int SegmentFontSize { get; private set; }
        public int SegmentFontSpacing { get; private set; }

        public int SegmentMargin { get; private set; }

        public Segment[] Segments { get; private set; } = null!;
        public int SegmentsPerScreen { get; private set; }

        public int SeparatorSize { get; private set; }

        public int TimerFontSize { get; private set; }
        public int TimerFontSpacing { get; private set; }
        public float TimerSize { get; private set; }
        public int TitleCategoryTitlesGap { get; private set; }
        public string WorldRecordOwner { get; private set; } = null!;
        public double WorldRecordSeconds { get; private set; }

        public void AutoSave()
        {
            if (HasStarted())
            {
                SaveTimes();
            }
        }

        /*
            This is being used instead of a normal constructor as a workaround. That is because
            YamlDotNet's deserializing function requires this class to have a parameterless
            constructor.
        */
        public void Construct(Config config, StorageManager storage)
        {
            _config = config;
            _storage = storage;
            _globalInputManager = new GlobalInputManager(_config);
            _colors = new ColorManager(
                HexColors.Background, HexColors.TextBase, HexColors.PaceAheadGaining,
                HexColors.PaceAheadLosing, HexColors.PaceBehindGaining, HexColors.PaceBehindLosing,
                HexColors.PaceBest, HexColors.Separator
            );
            _stats = new Stats(this);

            Reset(true);
        }

        public void Draw(Font font)
        {
            int segmentsToDraw = Math.Min(Segments.Length, SegmentsPerScreen);

            int effectiveHeight = Raylib.GetScreenHeight() - SeparatorSize * (segmentsToDraw - 1);

            var headerHeight = (int)(effectiveHeight * (HeaderHeight / 100.0f));
            var timerHeight = (int)(effectiveHeight * (TimerSize / 100.0f));

            effectiveHeight -= headerHeight + timerHeight;

            var segmentHeight =
                (int)Math.Min(effectiveHeight / (segmentsToDraw + ExtraStats.Length),
                    MaxSegmentSize / 100 * effectiveHeight);

            Raylib.ClearBackground(_colors.Background);
            DrawSeparators(headerHeight, segmentHeight, timerHeight, segmentsToDraw);
            DrawHeader(font, headerHeight);
            DrawSegments(font, headerHeight, segmentHeight, segmentsToDraw);
            DrawTimer(font, timerHeight, segmentHeight, headerHeight);
            DrawExtraStats(font, segmentHeight);
        }

        public int GetCurrentSegment() => _currentSegment;

        public double GetCurrentTimeSecs() => _currentTimeSecs;

        public double GetPbTime()
            => _pbTimeSecs;

        public bool HasPb() => _pbTimeSecs != NoPbTime;

        public bool HasStarted() => _currentSegment > -1;

        public void Update()
        {
            _globalInputManager.InputReader.UpdateKeyboardState();

            float deltaTime = Raylib.GetFrameTime();

            if (_currentSegment >= 0 && _currentSegment < Segments.Length)
            {
                _currentTimeSecs += deltaTime;
            }

            HandleGlobalInput();
        }

        private void DrawExtraStats(Font font, float segmentHeight)
        {
            ExtraStats.ForeachI((stat, i) =>
            {
                string statName = _stats.GetStatName(stat);
                Vector2 statNameSize = Raylib.MeasureTextEx(
                    font, statName, SegmentFontSize, SegmentFontSpacing
                );
                var leftTextDrawPos = new Vector2(SegmentMargin,
                    Raylib.GetRenderHeight() -
                        ((i + 1) * segmentHeight + (i + 1) * SeparatorSize)
                        + (segmentHeight - statNameSize.Y) / 2.0f
                );

                string statTxt = _stats.GetStatText(stat);
                Vector2 statTimeSize = Raylib.MeasureTextEx(
                    font, statTxt, SegmentFontSize, SegmentFontSpacing
                );
                var statTxtPos = new Vector2(
                    Raylib.GetScreenWidth() - SegmentMargin - statTimeSize.X,
                    leftTextDrawPos.Y
                );

                Raylib.DrawTextEx(font, statName, leftTextDrawPos, SegmentFontSize,
                    SegmentFontSpacing, _colors.Base
                );
                Raylib.DrawTextEx(font, statTxt, statTxtPos, SegmentFontSize, SegmentFontSpacing,
                    _colors.Base
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
            Vector2 attemptCountSize = Raylib.MeasureTextEx(
                font, $"{AttemptCount}", AttemptCountFontSize, AttemptCountFontSpacing
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
            var attemptCountPos = new Vector2(
                (Raylib.GetScreenWidth() * AttemptSizeTextPosX) - attemptCountSize.X,
                (headerHeight * AttemptSizeTextPosY) - attemptCountSize.Y
            );

            Raylib.DrawTextEx(font, GameName, gameTitlePos, GameTitleFontSize, GameTitleFontSpacing,
                _colors.Base
            );
            Raylib.DrawTextEx(
                font, Category, categoryTitlePos, CategoryTitleFontSize, CategoryTitleFontSpacing,
                _colors.Base
            );
            Raylib.DrawTextEx(
                font, $"{AttemptCount}", attemptCountPos, AttemptCountFontSize,
                AttemptCountFontSpacing, _colors.Base
            );
        }

        private void DrawSegments(Font font, float headerHeight, float segmentHeight,
            int segmentsToDraw)
        {
            int offset = Math.Min(
                Math.Max(_currentSegment - (segmentsToDraw - _config.MinSegmentsAheadToShow), 0),
                Segments.Length - segmentsToDraw
            );

            for (int i = offset; i < Segments.Length && i < offset + segmentsToDraw; i++)
            {
                Segments[i].Draw(headerHeight, i - offset, segmentHeight, font, SegmentFontSpacing,
                    _colors, SeparatorSize, SegmentFontSize, SegmentMargin
                );
            }
        }

        private void DrawSeparators(
            int headerHeight, int segmentSize, int timerHeight, int segmentsToDraw)
        {
            Raylib.DrawRectangle(0, headerHeight, Raylib.GetRenderWidth(), SeparatorSize,
                _colors.Separator
            );

            for (int i = 1; i < segmentsToDraw + 1; i++)
            {
                Raylib.DrawRectangle(
                    0, headerHeight + i * segmentSize, Raylib.GetRenderWidth(), SeparatorSize,
                    _colors.Separator
                );
            }

            int timerOffset = ExtraStats.Length * segmentSize +
                (ExtraStats.Length - 1) * SeparatorSize;
            int timerY = Raylib.GetRenderHeight() - timerHeight - SeparatorSize - timerOffset;

            Raylib.DrawRectangle(0, timerY + timerHeight, Raylib.GetRenderWidth(), SeparatorSize,
                _colors.Separator
            );

            for (int i = 1; i < ExtraStats.Length; i++)
            {
                Raylib.DrawRectangle(0,
                    timerY + timerHeight + i * segmentSize + SeparatorSize * (i - 1),
                    Raylib.GetRenderWidth(), SeparatorSize, _colors.Separator
                );
            }
        }

        private void DrawTimer(Font font, float timerHeight, float segmentHeight,
                float headerHeight)
        {
            string timerText = Formatter.SecondsToTime(_currentTimeSecs, true);
            Vector2 textSize = Raylib.MeasureTextEx(
                font, timerText, TimerFontSize, TimerFontSpacing
            );

            var textPos = new Vector2(
                (Raylib.GetRenderWidth() - textSize.X) / 2.0f,
                Raylib.GetRenderHeight() - segmentHeight * ExtraStats.Length - SeparatorSize
                    * (ExtraStats.Length - 1) - (timerHeight + textSize.Y) / 2.0f
            );

            Color? drawColor = null;
            if (_currentTimeSecs == 0.0 || !RanAllSegmentsBefore())
            {
                drawColor = _colors.Base;
            }
            else
            {
                int segI = Math.Min(_currentSegment, Segments.Length - 1);
                bool ahead = _currentTimeSecs < Segments[segI].GetAbsolutePbCompletionTime();

                if (!Segments.Last().IsAhead(_currentTimeSecs))
                {
                    drawColor = _colors.BehindLosing;
                }

                if (drawColor is null)
                {
                    if (ahead)
                    {
                        drawColor = _colors.AheadGaining;
                    }
                    else
                    {
                        drawColor = _currentSegment >= Segments.Length - 1
                                    ? _colors.BehindLosing
                                    : _colors.BehindGaining;
                    }
                }
            }

            Raylib.DrawTextEx(
                font, timerText, textPos, TimerFontSize, TimerFontSpacing, (Color)drawColor
            );
        }

        private void HandleGlobalInput()
        {
            if (_globalInputManager.InputReader.IsKeyPressed(_config.GlobalKeybindings.Split))
            {
                Split();
            }

            if (_globalInputManager.InputReader.AskingForReset(_config.GlobalKeybindings.Reset))
            {
                Reset();
            }

            if (_globalInputManager.InputReader.IsKeyPressed(_config.GlobalKeybindings.Undo))
            {
                Undo();
            }

            if (_globalInputManager.InputReader.IsKeyPressed(_config.GlobalKeybindings.Redo))
            {
                Redo();
            }
        }

        private void InitializeSegments()
        {
            double absPbTime = 0;
            for (int i = 0; i < Segments.Length; i++)
            {
                absPbTime = 0;
                for (int j = i; j >= 0; j--)
                {
                    if (absPbTime <= NoPbTime)
                    {
                        break;
                    }

                    if (!Segments[j].RanSegmentBefore())
                    {
                        absPbTime = NoPbTime;
                        break;
                    }

                    absPbTime += Segments[j].PbTimeRel;
                }

                Segments[i].Construct(absPbTime, CompletedRunBefore);
            }

            _pbTimeSecs = absPbTime;
        }

        private bool IsRunFinished()
            => _currentSegment >= Segments.Length;

        private bool RanAllSegmentsBefore() => Segments.ToList().All(sgm => sgm.RanSegmentBefore());

        private void Redo()
        {
            if (!HasStarted() || _currentSegment >= Segments.Length)
            {
                return;
            }

            double originalTime = _history.RegisterRedo();

            Segments[_currentSegment].FinishSegment(originalTime);
            _currentSegment++;

            /*
                If we're redoing the last segment, the timer needs to be rolled back because the
                run already ended.
            */
            if (_currentSegment == Segments.Length)
            {
                _currentTimeSecs = originalTime;
            }
        }

        private void Reset(bool initialReset = false)
        {
            if (!HasStarted())
            {
                return;
            }

            if (!initialReset)
            {
                AttemptCount++;

                if (!IsRunFinished())
                {
                    Segments[_currentSegment].ResetCount++;
                }

                SaveTimes();
            }

            _pbTimeSecs = _currentTimeSecs;
            _currentTimeSecs = 0;
            _currentSegment = ClearTimerIndex;
            _history.ClearHistory();
            InitializeSegments();
        }

        private void SaveTimes()
        {
            Segments.ToList().ForEach(s =>
            {
                s.UpdateBestSegment();
            });

            if (IsRunFinished())
            {
                if (_currentTimeSecs < _pbTimeSecs || _pbTimeSecs == NoPbTime)
                {
                    Segments.ToList().ForEach(s =>
                    {
                        s.SetPersonalBest();
                    });
                }

                if (_currentTimeSecs < WorldRecordSeconds)
                {
                    WorldRecordSeconds = _currentTimeSecs;
                    WorldRecordOwner = "me";
                }

                CompletedRunBefore = true;
            }

            _storage.SaveTimer(this, _config.SplitsStoragePath);
        }

        private void Split()
        {
            if (_currentSegment >= Segments.Length)
            {
                return;
            }

            if (HasStarted())
            {
                Segments[_currentSegment].FinishSegment(_currentTimeSecs);
            }

            _currentSegment++;

            if (_currentSegment < Segments.Length)
            {
                Segments[_currentSegment].BeginSegment(_currentTimeSecs);
            }

            _history.ClearHistory();
        }

        private void Undo()
        {
            if (_currentSegment > 0)
            {
                _currentSegment--;
                _history.RegisterUndo(Segments[_currentSegment].GetAbsoluteCompletionTime());
                Segments[_currentSegment].UndoSplit();
            }
        }
    }
}