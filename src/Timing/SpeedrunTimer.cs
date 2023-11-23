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
        public SpeedgameData SpeedgameData { get; private set; } = null!;

        public void AutoSave()
        {
            if (HasStarted())
            {
                SaveTimes();
            }
        }

        public SpeedrunTimer(AppConfig AppConfig, SpeedgameData layoutConfig, StorageManager storage)
        {
            _config = AppConfig;
            SpeedgameData = layoutConfig;
            _storage = storage;
            _globalInputManager = new GlobalInputManager(_config);
            _colors = new ColorManager(
                SpeedgameData.HexColors.Background, SpeedgameData.HexColors.TextBase,
                SpeedgameData.HexColors.PaceAheadGaining, SpeedgameData.HexColors.PaceAheadLosing,
                SpeedgameData.HexColors.PaceBehindGaining, SpeedgameData.HexColors.PaceBehindLosing,
                SpeedgameData.HexColors.PaceBest, SpeedgameData.HexColors.Separator
            );
            _stats = new Stats(this);

            Reset(true);
        }

        public void Draw(Font font)
        {
            int segmentsToDraw = Math.Min(SpeedgameData.Segments.Length,
                                          SpeedgameData.SegmentsPerScreen);

            int effectiveHeight = Raylib.GetScreenHeight()
                                    - SpeedgameData.SeparatorSize * (segmentsToDraw - 1);

            var headerHeight = (int)(effectiveHeight * (SpeedgameData.HeaderHeight / 100.0f));
            var timerHeight = (int)(effectiveHeight * (SpeedgameData.TimerSize / 100.0f));

            effectiveHeight -= headerHeight + timerHeight;

            var segmentHeight =
                (int)Math.Min(effectiveHeight / (segmentsToDraw + SpeedgameData.ExtraStats.Length),
                    SpeedgameData.MaxSegmentSize / 100 * effectiveHeight);

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

            if (_currentSegment >= 0 && _currentSegment < SpeedgameData.Segments.Length)
            {
                _currentTimeSecs += deltaTime;
            }

            HandleGlobalInput();
        }

        private void DrawExtraStats(Font font, float segmentHeight)
        {
            SpeedgameData.ExtraStats.ForeachI((stat, i) =>
            {
                string statName = _stats.GetStatName(stat);
                Vector2 statNameSize = Raylib.MeasureTextEx(
                    font, statName, SpeedgameData.SegmentFontSize, SpeedgameData.SegmentFontSpacing
                );
                var leftTextDrawPos = new Vector2(SpeedgameData.SegmentMargin,
                    Raylib.GetRenderHeight() -
                        ((i + 1) * segmentHeight + (i + 1) * SpeedgameData.SeparatorSize)
                        + (segmentHeight - statNameSize.Y) / 2.0f
                );

                string statTxt = _stats.GetStatText(stat);
                Vector2 statTimeSize = Raylib.MeasureTextEx(
                    font, statTxt, SpeedgameData.SegmentFontSize, SpeedgameData.SegmentFontSpacing
                );
                var statTxtPos = new Vector2(
                    Raylib.GetScreenWidth() - SpeedgameData.SegmentMargin - statTimeSize.X,
                    leftTextDrawPos.Y
                );

                Raylib.DrawTextEx(font, statName, leftTextDrawPos, SpeedgameData.SegmentFontSize,
                    SpeedgameData.SegmentFontSpacing, _colors.Base
                );
                Raylib.DrawTextEx(font, statTxt, statTxtPos, SpeedgameData.SegmentFontSize,
                    SpeedgameData.SegmentFontSpacing, _colors.Base
                );
            },
                true
            );
        }

        private void DrawHeader(Font font, float headerHeight)
        {
            Vector2 gameTitleSize = Raylib.MeasureTextEx(font, SpeedgameData.GameName,
                SpeedgameData.GameTitleFontSize, SpeedgameData.GameTitleFontSpacing
            );
            Vector2 categoryTitleSize = Raylib.MeasureTextEx(
                font, SpeedgameData.Category, SpeedgameData.CategoryTitleFontSize,
                SpeedgameData.CategoryTitleFontSpacing
            );
            Vector2 attemptCountSize = Raylib.MeasureTextEx(
                font, $"{SpeedgameData.AttemptCount}", SpeedgameData.AttemptCountFontSize,
                SpeedgameData.AttemptCountFontSpacing
            );

            float textLayoutHeight = gameTitleSize.Y + categoryTitleSize.Y
                                     + SpeedgameData.TitleCategoryTitlesGap;
            float textLayoutStartY = (headerHeight - textLayoutHeight) / 2.0f;

            var gameTitlePos = new Vector2((Raylib.GetScreenWidth() - gameTitleSize.X) / 2.0f,
                textLayoutStartY
            );
            var categoryTitlePos = new Vector2(
                (Raylib.GetScreenWidth() - categoryTitleSize.X) / 2.0f,
                textLayoutStartY + SpeedgameData.TitleCategoryTitlesGap + gameTitleSize.Y
            );
            var attemptCountPos = new Vector2(
                (Raylib.GetScreenWidth() * SpeedgameData.AttemptSizeTextPosX) - attemptCountSize.X,
                (headerHeight * SpeedgameData.AttemptSizeTextPosY) - attemptCountSize.Y
            );

            Raylib.DrawTextEx(font, SpeedgameData.GameName, gameTitlePos,
                SpeedgameData.GameTitleFontSize, SpeedgameData.GameTitleFontSpacing, _colors.Base
            );
            Raylib.DrawTextEx(
                font, SpeedgameData.Category, categoryTitlePos, SpeedgameData.CategoryTitleFontSize,
                SpeedgameData.CategoryTitleFontSpacing, _colors.Base
            );
            Raylib.DrawTextEx(
                font, $"{SpeedgameData.AttemptCount}", attemptCountPos,
                SpeedgameData.AttemptCountFontSize, SpeedgameData.AttemptCountFontSpacing,
                _colors.Base
            );
        }

        private void DrawSegments(Font font, float headerHeight, float segmentHeight,
            int segmentsToDraw)
        {
            int offset = Math.Min(
                Math.Max(_currentSegment - (segmentsToDraw - _config.MinSegmentsAheadToShow), 0),
                SpeedgameData.Segments.Length - segmentsToDraw
            );

            for (int i = offset; i < SpeedgameData.Segments.Length && i < offset + segmentsToDraw; i++)
            {
                SpeedgameData.Segments[i].Draw(headerHeight, i - offset, segmentHeight, font,
                    SpeedgameData.SegmentFontSpacing, _colors, SpeedgameData.SeparatorSize,
                    SpeedgameData.SegmentFontSize, SpeedgameData.SegmentMargin
                );
            }
        }

        private void DrawSeparators(
            int headerHeight, int segmentSize, int timerHeight, int segmentsToDraw)
        {
            Raylib.DrawRectangle(0, headerHeight, Raylib.GetRenderWidth(), SpeedgameData.SeparatorSize,
                _colors.Separator
            );

            for (int i = 1; i < segmentsToDraw + 1; i++)
            {
                Raylib.DrawRectangle(
                    0, headerHeight + i * segmentSize, Raylib.GetRenderWidth(),
                    SpeedgameData.SeparatorSize, _colors.Separator
                );
            }

            int timerOffset = SpeedgameData.ExtraStats.Length * segmentSize +
                (SpeedgameData.ExtraStats.Length - 1) * SpeedgameData.SeparatorSize;
            int timerY = Raylib.GetRenderHeight() - timerHeight - SpeedgameData.SeparatorSize
                         - timerOffset;

            Raylib.DrawRectangle(0, timerY + timerHeight, Raylib.GetRenderWidth(),
                SpeedgameData.SeparatorSize, _colors.Separator
            );

            for (int i = 1; i < SpeedgameData.ExtraStats.Length; i++)
            {
                Raylib.DrawRectangle(0,
                    timerY + timerHeight + i * segmentSize + SpeedgameData.SeparatorSize * (i - 1),
                    Raylib.GetRenderWidth(), SpeedgameData.SeparatorSize, _colors.Separator
                );
            }
        }

        private void DrawTimer(Font font, float timerHeight, float segmentHeight,
                float headerHeight)
        {
            string timerText = Formatter.SecondsToTime(_currentTimeSecs, true);
            Vector2 textSize = Raylib.MeasureTextEx(
                font, timerText, SpeedgameData.TimerFontSize, SpeedgameData.TimerFontSpacing
            );

            var textPos = new Vector2(
                (Raylib.GetRenderWidth() - textSize.X) / 2.0f,
                Raylib.GetRenderHeight() - segmentHeight * SpeedgameData.ExtraStats.Length
                    - SpeedgameData.SeparatorSize * (SpeedgameData.ExtraStats.Length - 1)
                    - (timerHeight + textSize.Y) / 2.0f
            );

            Color? drawColor = null;
            if (_currentTimeSecs == 0.0 || !RanAllSegmentsBefore())
            {
                drawColor = _colors.Base;
            }
            else
            {
                int segI = Math.Min(_currentSegment, SpeedgameData.Segments.Length - 1);
                bool ahead = _currentTimeSecs
                                < SpeedgameData.Segments[segI].GetAbsolutePbCompletionTime();

                if (!SpeedgameData.Segments.Last().IsAhead(_currentTimeSecs))
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
                        drawColor = _currentSegment >= SpeedgameData.Segments.Length - 1
                                    ? _colors.BehindLosing
                                    : _colors.BehindGaining;
                    }
                }
            }

            Raylib.DrawTextEx(
                font, timerText, textPos, SpeedgameData.TimerFontSize, SpeedgameData.TimerFontSpacing,
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
            for (int i = 0; i < SpeedgameData.Segments.Length; i++)
            {
                absPbTime = 0;
                for (int j = i; j >= 0; j--)
                {
                    if (absPbTime <= NoPbTime)
                    {
                        break;
                    }

                    if (!SpeedgameData.Segments[j].RanSegmentBefore())
                    {
                        absPbTime = NoPbTime;
                        break;
                    }

                    absPbTime += SpeedgameData.Segments[j].PbTimeRel;
                }

                SpeedgameData.Segments[i].Construct(absPbTime, SpeedgameData.CompletedRunBefore);
            }

            _pbTimeSecs = absPbTime;
        }

        private bool IsRunFinished()
            => _currentSegment >= SpeedgameData.Segments.Length;

        private bool RanAllSegmentsBefore() =>
            SpeedgameData.Segments.ToList().All(sgm => sgm.RanSegmentBefore());

        private void Redo()
        {
            if (!HasStarted() || _currentSegment >= SpeedgameData.Segments.Length)
            {
                return;
            }

            double originalTime = _history.RegisterRedo();

            SpeedgameData.Segments[_currentSegment].FinishSegment(originalTime);
            _currentSegment++;

            /*
                If we're redoing the last segment, the timer needs to be rolled back because the
                run already ended.
            */
            if (_currentSegment == SpeedgameData.Segments.Length)
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
                SpeedgameData.AttemptCount++;

                if (!IsRunFinished())
                {
                    SpeedgameData.Segments[_currentSegment].ResetCount++;
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
            SpeedgameData.Segments.ToList().ForEach(s =>
            {
                s.UpdateBestSegment();
            });

            if (IsRunFinished())
            {
                if (_currentTimeSecs < _pbTimeSecs || _pbTimeSecs == NoPbTime)
                {
                    SpeedgameData.Segments.ToList().ForEach(s =>
                    {
                        s.SetPersonalBest();
                    });
                }

                if (_currentTimeSecs < SpeedgameData.WorldRecordSeconds)
                {
                    SpeedgameData.WorldRecordSeconds = _currentTimeSecs;
                    SpeedgameData.WorldRecordOwner = "me";
                }

                SpeedgameData.CompletedRunBefore = true;
            }

            _storage.SaveTimerLayout(this, _config.SplitsStoragePath);
        }

        private void Split()
        {
            if (_currentSegment >= SpeedgameData.Segments.Length)
            {
                return;
            }

            if (HasStarted())
            {
                SpeedgameData.Segments[_currentSegment].FinishSegment(_currentTimeSecs);
            }

            _currentSegment++;

            if (_currentSegment < SpeedgameData.Segments.Length)
            {
                SpeedgameData.Segments[_currentSegment].BeginSegment(_currentTimeSecs);
            }

            _history.ClearHistory();
        }

        private void Undo()
        {
            if (_currentSegment > 0)
            {
                _currentSegment--;
                _history.RegisterUndo(
                    SpeedgameData.Segments[_currentSegment].GetAbsoluteCompletionTime());
                SpeedgameData.Segments[_currentSegment].UndoSplit();
            }
        }
    }
}