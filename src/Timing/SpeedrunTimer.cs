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

using JumpDiveClock.Input;
using JumpDiveClock.Misc;
using JumpDiveClock.Settings;
using Raylib_cs;
using System.Numerics;

namespace JumpDiveClock.Timing
{
    public class SpeedrunTimer
    {
        private const int ClearTimerIndex = -1;
        private const double NoPbTime = -1;

        private ColorManager _colors = null!;
        private AppConfig _config = null!;
        private int _currentSegment;
        private double _currentTimeSecs;
        private GlobalInputManager _globalInputManager = null!;
        private HistoryManager _history = new HistoryManager();
        private double _pbTimeSecs;
        private Stats _stats = null!;
        private StorageManager _storage = null!;

        private bool _timerLocked = false;

        public SpeedrunTimer(AppConfig AppConfig, Splits splits, StorageManager storage)
        {
            _config = AppConfig;
            Splits = splits;
            _storage = storage;
            _globalInputManager = new GlobalInputManager(_config);
            _colors = new ColorManager(
                Splits.HexColors.Background, Splits.HexColors.TextBase,
                Splits.HexColors.PaceAheadGaining, Splits.HexColors.PaceAheadLosing,
                Splits.HexColors.PaceBehindGaining, Splits.HexColors.PaceBehindLosing,
                Splits.HexColors.PaceBest, Splits.HexColors.Separator
            );
            _stats = new Stats(this);

            Reset(true);
        }

        public Splits Splits { get; private set; } = null!;

        public void AutoSave()
        {
            if (HasStarted())
            {
                SaveTimes();
            }
        }

        public void Draw(Font font)
        {
            Raylib.ClearBackground(_colors.Background);

            int segmentsToDraw = Math.Min(Splits.Segments.Length,
                                          Splits.SegmentsPerScreen);

            int effectiveHeight = Raylib.GetScreenHeight()
                                    - Splits.SeparatorSize * (segmentsToDraw - 1);

            var headerHeight = (int)(effectiveHeight * (Splits.HeaderHeight / 100.0f));
            var timerHeight = (int)(effectiveHeight * (Splits.TimerSize / 100.0f));

            effectiveHeight -= headerHeight + timerHeight;

            var segmentHeight =
                (int)Math.Min(effectiveHeight / (segmentsToDraw + Splits.ExtraStats.Length),
                    Splits.MaxSegmentSize / 100 * effectiveHeight);

            DrawSeparators(headerHeight, segmentHeight, timerHeight, segmentsToDraw);
            DrawHeader(font, headerHeight);
            DrawSegments(font, headerHeight, segmentHeight, segmentsToDraw);
            DrawExtraStats(font, segmentHeight);
            DrawTimer(font, timerHeight, segmentHeight);
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

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_L) && _currentSegment < 0)
            {
                _timerLocked = !_timerLocked;
            }

            if (!_timerLocked)
            {
                HandleGlobalInput();
            }

            float deltaTime = Raylib.GetFrameTime();

            if (_currentSegment >= 0 && _currentSegment < Splits.Segments.Length)
            {
                _currentTimeSecs += deltaTime;
            }

        }

        private void DrawExtraStats(Font font, float segmentHeight)
        {
            Splits.ExtraStats.ForeachI((stat, i) =>
            {
                string statName = _stats.GetStatName(stat);
                Vector2 statNameSize = Raylib.MeasureTextEx(
                    font, statName, Splits.SegmentFontSize, Splits.SegmentFontSpacing
                );
                var leftTextDrawPos = new Vector2(Splits.SegmentMargin,
                    Raylib.GetRenderHeight() -
                        ((i + 1) * segmentHeight + (i + 1) * Splits.SeparatorSize)
                        + (segmentHeight - statNameSize.Y) / 2.0f
                );

                string statTxt = _stats.GetStatText(stat);
                Vector2 statTimeSize = Raylib.MeasureTextEx(
                    font, statTxt, Splits.SegmentFontSize, Splits.SegmentFontSpacing
                );
                var statTxtPos = new Vector2(
                    Raylib.GetScreenWidth() - Splits.SegmentMargin - statTimeSize.X,
                    leftTextDrawPos.Y
                );

                Raylib.DrawTextEx(font, statName, leftTextDrawPos, Splits.SegmentFontSize,
                    Splits.SegmentFontSpacing, _colors.Base
                );
                Raylib.DrawTextEx(font, statTxt, statTxtPos, Splits.SegmentFontSize,
                    Splits.SegmentFontSpacing, _colors.Base
                );
            },
                true
            );
        }

        private void DrawHeader(Font font, float headerHeight)
        {
            Vector2 gameTitleSize = Raylib.MeasureTextEx(font, Splits.GameName,
                Splits.GameTitleFontSize, Splits.GameTitleFontSpacing
            );
            Vector2 categoryTitleSize = Raylib.MeasureTextEx(
                font, Splits.Category, Splits.CategoryTitleFontSize,
                Splits.CategoryTitleFontSpacing
            );
            Vector2 attemptCountSize = Raylib.MeasureTextEx(
                font, $"{Splits.AttemptCount}", Splits.AttemptCountFontSize,
                Splits.AttemptCountFontSpacing
            );

            float textLayoutHeight = gameTitleSize.Y + categoryTitleSize.Y
                                     + Splits.TitleCategoryTitlesGap;
            float textLayoutStartY = (headerHeight - textLayoutHeight) / 2.0f;

            var gameTitlePos = new Vector2((Raylib.GetScreenWidth() - gameTitleSize.X) / 2.0f,
                textLayoutStartY
            );
            var categoryTitlePos = new Vector2(
                (Raylib.GetScreenWidth() - categoryTitleSize.X) / 2.0f,
                textLayoutStartY + Splits.TitleCategoryTitlesGap + gameTitleSize.Y
            );
            var attemptCountPos = new Vector2(
                (Raylib.GetScreenWidth() * Splits.AttemptSizeTextPosX) - attemptCountSize.X,
                (headerHeight * Splits.AttemptSizeTextPosY) - attemptCountSize.Y
            );

            Raylib.DrawTextEx(font, Splits.GameName, gameTitlePos,
                Splits.GameTitleFontSize, Splits.GameTitleFontSpacing, _colors.Base
            );
            Raylib.DrawTextEx(
                font, Splits.Category, categoryTitlePos, Splits.CategoryTitleFontSize,
                Splits.CategoryTitleFontSpacing, _colors.Base
            );
            Raylib.DrawTextEx(
                font, $"{Splits.AttemptCount}", attemptCountPos,
                Splits.AttemptCountFontSize, Splits.AttemptCountFontSpacing,
                _colors.Base
            );
        }

        private void DrawSegments(Font font, float headerHeight, float segmentHeight,
            int segmentsToDraw)
        {
            int offset = Math.Min(
                Math.Max(_currentSegment - (segmentsToDraw - _config.MinSegmentsAheadToShow), 0),
                Splits.Segments.Length - segmentsToDraw
            );

            for (int i = offset; i < Splits.Segments.Length && i < offset + segmentsToDraw; i++)
            {
                Splits.Segments[i].Draw(headerHeight, i - offset, segmentHeight, font,
                    Splits.SegmentFontSpacing, _colors, Splits.SeparatorSize,
                    Splits.SegmentFontSize, Splits.SegmentMargin
                );
            }
        }

        private void DrawSeparators(
            int headerHeight, int segmentSize, int timerHeight, int segmentsToDraw)
        {
            Raylib.DrawRectangle(0, headerHeight, Raylib.GetRenderWidth(), Splits.SeparatorSize,
                _colors.Separator
            );

            for (int i = 1; i < segmentsToDraw + 1; i++)
            {
                Raylib.DrawRectangle(
                    0, headerHeight + i * segmentSize, Raylib.GetRenderWidth(),
                    Splits.SeparatorSize, _colors.Separator
                );
            }

            int timerOffset = Splits.ExtraStats.Length * segmentSize +
                (Splits.ExtraStats.Length - 1) * Splits.SeparatorSize;
            int timerY = Raylib.GetRenderHeight() - timerHeight - Splits.SeparatorSize
                         - timerOffset;

            Raylib.DrawRectangle(0, timerY + timerHeight, Raylib.GetRenderWidth(),
                Splits.SeparatorSize, _colors.Separator
            );

            for (int i = 1; i < Splits.ExtraStats.Length; i++)
            {
                Raylib.DrawRectangle(0,
                    timerY + timerHeight + i * segmentSize + Splits.SeparatorSize * (i - 1),
                    Raylib.GetRenderWidth(), Splits.SeparatorSize, _colors.Separator
                );
            }
        }

        private void DrawTimer(Font font, float timerHeight, float segmentHeight)
        {
            string timerText = _timerLocked
                                ? _config.TimerLockingMessage
                                : Formatter.SecondsToTime(_currentTimeSecs, true);
            Vector2 textSize = Raylib.MeasureTextEx(
                font, timerText, Splits.TimerFontSize, Splits.TimerFontSpacing
            );

            var textPos = new Vector2(
                (Raylib.GetRenderWidth() - textSize.X) / 2.0f,
                Raylib.GetRenderHeight() - segmentHeight * Splits.ExtraStats.Length
                    - Splits.SeparatorSize * (Splits.ExtraStats.Length - 1)
                    - (timerHeight + textSize.Y) / 2.0f
            );

            Color? drawColor = null;
            if (_currentTimeSecs == 0.0 || !RanAllSegmentsBefore())
            {
                drawColor = _colors.Base;
            }
            else
            {
                int segI = Math.Min(_currentSegment, Splits.Segments.Length - 1);
                bool ahead = _currentTimeSecs
                                < Splits.Segments[segI].GetAbsolutePbCompletionTime();

                if (!Splits.Segments.Last().IsAhead(_currentTimeSecs))
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
                        drawColor = _currentSegment >= Splits.Segments.Length - 1
                                    ? _colors.BehindLosing
                                    : _colors.BehindGaining;
                    }
                }
            }

            Raylib.DrawTextEx(
                font, timerText, textPos, Splits.TimerFontSize, Splits.TimerFontSpacing,
                (Color)drawColor
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
            for (int i = 0; i < Splits.Segments.Length; i++)
            {
                absPbTime = 0;
                for (int j = i; j >= 0; j--)
                {
                    if (absPbTime <= NoPbTime)
                    {
                        break;
                    }

                    if (!Splits.Segments[j].RanSegmentBefore())
                    {
                        absPbTime = NoPbTime;
                        break;
                    }

                    absPbTime += Splits.Segments[j].PbTimeRel;
                }

                Splits.Segments[i].Construct(absPbTime, Splits.CompletedRunBefore);
            }

            _pbTimeSecs = absPbTime;
        }

        private bool IsRunFinished()
            => _currentSegment >= Splits.Segments.Length;

        private bool RanAllSegmentsBefore() =>
            Splits.Segments.ToList().All(sgm => sgm.RanSegmentBefore());

        private void Redo()
        {
            if (!HasStarted() || _currentSegment >= Splits.Segments.Length)
            {
                return;
            }

            double originalTime = _history.RegisterRedo();

            Splits.Segments[_currentSegment].FinishSegment(originalTime);
            _currentSegment++;

            /*
                If we're redoing the last segment, the timer needs to be rolled back because the
                run already ended.
            */
            if (_currentSegment == Splits.Segments.Length)
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
                Splits.AttemptCount++;

                if (!IsRunFinished())
                {
                    Splits.Segments[_currentSegment].ResetCount++;
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
            Splits.Segments.ToList().ForEach(s =>
            {
                s.UpdateBestSegment();
            });

            if (IsRunFinished())
            {
                if (_currentTimeSecs < _pbTimeSecs || _pbTimeSecs == NoPbTime)
                {
                    Splits.Segments.ToList().ForEach(s =>
                    {
                        s.SetPersonalBest();
                    });
                }

                if (_currentTimeSecs < Splits.WorldRecordSeconds)
                {
                    Splits.WorldRecordSeconds = _currentTimeSecs;
                    Splits.WorldRecordOwner = "me";
                }

                Splits.CompletedRunBefore = true;
            }

            _storage.SaveTimerSplits(this);
        }

        private void Split()
        {
            if (_currentSegment >= Splits.Segments.Length)
            {
                return;
            }

            if (HasStarted())
            {
                Splits.Segments[_currentSegment].FinishSegment(_currentTimeSecs);
            }

            _currentSegment++;

            if (_currentSegment < Splits.Segments.Length)
            {
                Splits.Segments[_currentSegment].BeginSegment(_currentTimeSecs);
            }

            _history.ClearHistory();
        }

        private void Undo()
        {
            if (_currentSegment > 0)
            {
                _currentSegment--;
                _history.RegisterUndo(
                    Splits.Segments[_currentSegment].GetAbsoluteCompletionTime());
                Splits.Segments[_currentSegment].UndoSplit();
            }
        }
    }
}