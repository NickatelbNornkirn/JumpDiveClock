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
using JumpDiveClock.Storage;
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
        private double _finishTime;
        private GlobalInputManager _globalInputManager = null!;
        private HistoryManager _history = new HistoryManager();
        private double _pbTimeSecs;
        private Stats _stats = null!;
        private StorageManager _storage = null!;
        private TimerStyle _style;
        private bool _timerLocked = false;

        public Splits Splits { get; private set; } = null!;

        public SpeedrunTimer(AppConfig AppConfig, Splits splits, StorageManager storage,
                            TimerStyle style)
        {
            _style = style;
            _config = AppConfig;
            Splits = splits;
            _storage = storage;
            _globalInputManager = new GlobalInputManager(splits.KeyboardId);
            _colors = new ColorManager(
                _style.HexColors.Background, _style.HexColors.TextBase,
                _style.HexColors.PaceAheadGaining, _style.HexColors.PaceAheadLosing,
                _style.HexColors.PaceBehindGaining, _style.HexColors.PaceBehindLosing,
                _style.HexColors.PaceBest, _style.HexColors.Separator,
                _style.HexColors.DetailedTimer
            );
            _stats = new Stats(this);

            Reset(true);
        }

        public void AutoSave()
        {
            SaveTimes();
            Console.WriteLine("Autosaved splits");
        }

        public void Draw(Font font)
        {
            Raylib.ClearBackground(_colors.Background);

            int segmentsToDraw = Math.Min(Splits.Segments.Length, _style.SegmentsPerScreen);

            int effectiveHeight = Raylib.GetScreenHeight()
                                    - _style.SeparatorSize * (segmentsToDraw - 1);

            var headerHeight = (int)(effectiveHeight * (_style.HeaderHeight / 100.0f));
            var timerHeight = (int)(effectiveHeight * (_style.TimerSize / 100.0f));

            effectiveHeight -= headerHeight + timerHeight;

            var segmentHeight =
                (int)Math.Min(effectiveHeight / (segmentsToDraw + _style.ExtraStats.Length),
                        _style.MaxSegmentSize / 100 * effectiveHeight);

            DrawSeparators(headerHeight, segmentHeight, timerHeight, segmentsToDraw);
            DrawHeader(font, headerHeight);
            DrawSegments(font, headerHeight, segmentHeight, segmentsToDraw);
            DrawExtraStats(font, segmentHeight);
            DrawTimer(font, timerHeight, segmentHeight);
        }

        public int GetCurrentSegment() => _currentSegment;

        public double GetCurrentTimeSecs() => _currentTimeSecs;

        public double GetPbTime() => _pbTimeSecs;

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

            if (_currentSegment >= 0)
            {
                _currentTimeSecs += deltaTime;
            }
        }

        private void DrawExtraStats(Font font, float segmentHeight)
        {
            _style.ExtraStats.ForeachI((stat, i) =>
            {
                string statName = _stats.GetStatName(stat);
                Vector2 statNameSize = Raylib.MeasureTextEx(
                    font, statName, _style.SegmentFontSize, _style.SegmentFontSpacing
                );
                var leftTextDrawPos = new Vector2(_style.SegmentMargin,
                    Raylib.GetRenderHeight() -
                        ((i + 1) * segmentHeight + (i + 1) * _style.SeparatorSize)
                        + (segmentHeight - statNameSize.Y) / 2.0f
                );

                string statTxt = _stats.GetStatText(stat);
                Vector2 statTimeSize = Raylib.MeasureTextEx(
                    font, statTxt, _style.SegmentFontSize, _style.SegmentFontSpacing
                );
                var statTxtPos = new Vector2(
                    Raylib.GetScreenWidth() - _style.SegmentMargin - statTimeSize.X,
                    leftTextDrawPos.Y
                );

                Raylib.DrawTextEx(font, statName, leftTextDrawPos, _style.SegmentFontSize,
                    _style.SegmentFontSpacing, _colors.Base
                );
                Raylib.DrawTextEx(font, statTxt, statTxtPos, _style.SegmentFontSize,
                    _style.SegmentFontSpacing, _colors.Base
                );
            },
                ForEachDir.Descending
            );
        }

        private void DrawHeader(Font font, float headerHeight)
        {
            Vector2 gameTitleSize = Raylib.MeasureTextEx(font, Splits.GameName,
                _style.GameTitleFontSize, _style.GameTitleFontSpacing
            );
            Vector2 categoryTitleSize = Raylib.MeasureTextEx(
                font, Splits.Category, _style.CategoryTitleFontSize,
                _style.CategoryTitleFontSpacing
            );
            Vector2 attemptCountSize = Raylib.MeasureTextEx(
                font, $"{Splits.AttemptCount}", _style.AttemptCountFontSize,
                _style.AttemptCountFontSpacing
            );

            float textLayoutHeight = gameTitleSize.Y + categoryTitleSize.Y
                                     + _style.TitleCategoryTitlesGap;
            float textLayoutStartY = (headerHeight - textLayoutHeight) / 2.0f;

            var gameTitlePos = new Vector2((Raylib.GetScreenWidth() - gameTitleSize.X) / 2.0f,
                textLayoutStartY
            );
            var categoryTitlePos = new Vector2(
                (Raylib.GetScreenWidth() - categoryTitleSize.X) / 2.0f,
                textLayoutStartY + _style.TitleCategoryTitlesGap + gameTitleSize.Y
            );
            var attemptCountPos = new Vector2(
                (Raylib.GetScreenWidth() * _style.AttemptSizeTextPosX) - attemptCountSize.X,
                (headerHeight * _style.AttemptSizeTextPosY) - attemptCountSize.Y
            );

            Raylib.DrawTextEx(font, Splits.GameName, gameTitlePos,
                _style.GameTitleFontSize, _style.GameTitleFontSpacing, _colors.Base
            );
            Raylib.DrawTextEx(
                font, Splits.Category, categoryTitlePos, _style.CategoryTitleFontSize,
                _style.CategoryTitleFontSpacing, _colors.Base
            );
            Raylib.DrawTextEx(
                font, $"{Splits.AttemptCount}", attemptCountPos,
                _style.AttemptCountFontSize, _style.AttemptCountFontSpacing,
                _colors.Base
            );
        }

        private void DrawSegments(Font font, float headerHeight, float segmentHeight,
            int segmentsToDraw)
        {
            int offset = Math.Min(
                Math.Max(_currentSegment - (segmentsToDraw - _style.MinSegmentsAheadToShow), 0),
                Splits.Segments.Length - segmentsToDraw
            );

            for (int i = offset; i < Splits.Segments.Length && i < offset + segmentsToDraw; i++)
            {
                Splits.Segments[i].Draw(headerHeight, i - offset, segmentHeight, font,
                    _style.SegmentFontSpacing, _colors, _style.SeparatorSize,
                    _style.SegmentFontSize, _style.SegmentMargin
                );
            }
        }

        private void DrawSeparators(
            int headerHeight, int segmentSize, int timerHeight, int segmentsToDraw)
        {
            Raylib.DrawRectangle(0, headerHeight, Raylib.GetRenderWidth(), _style.SeparatorSize,
                _colors.Separator
            );

            for (int i = 1; i < segmentsToDraw + 1; i++)
            {
                Raylib.DrawRectangle(
                    0, headerHeight + i * segmentSize, Raylib.GetRenderWidth(),
                    _style.SeparatorSize, _colors.Separator
                );
            }

            int timerOffset = _style.ExtraStats.Length * segmentSize +
                (_style.ExtraStats.Length - 1) * _style.SeparatorSize;
            int timerY = Raylib.GetRenderHeight() - timerHeight - _style.SeparatorSize
                         - timerOffset;

            if (_style.DrawTimerContainerTopSeparator)
            {
                Raylib.DrawRectangle(0, timerY, Raylib.GetRenderWidth(),
                    _style.SeparatorSize, _colors.Separator
                );

            }

            Raylib.DrawRectangle(0, timerY + timerHeight, Raylib.GetRenderWidth(),
                _style.SeparatorSize, _colors.Separator
            );

            for (int i = 1; i < _style.ExtraStats.Length; i++)
            {
                Raylib.DrawRectangle(0,
                    timerY + timerHeight + i * segmentSize + _style.SeparatorSize * (i - 1),
                    Raylib.GetRenderWidth(), _style.SeparatorSize, _colors.Separator
                );
            }
        }

        private void DrawTimer(Font font, float timerHeight, float segmentHeight)
        {
            double t = IsRunFinished() ? _finishTime : _currentTimeSecs;

            string timerText = _timerLocked
                                ? _style.TimerLockingMessage : Formatter.SecondsToTime(t, true);
            Vector2 textSize = Raylib.MeasureTextEx(
                font, timerText, _style.TimerFontSize, _style.TimerFontSpacing
            );
            float bottomHeight = segmentHeight * _style.ExtraStats.Length
                                + _style.SeparatorSize * (_style.ExtraStats.Length - 1);

            var timerTextPos = new Vector2(
                (Raylib.GetRenderWidth() - textSize.X) * 0.5f,
                Raylib.GetRenderHeight() - bottomHeight
                    - (timerHeight + textSize.Y) * 0.5f
            );

            Color drawColor = GetTimerDrawColor(t);

            Raylib.DrawTextEx(
                font, timerText, timerTextPos, _style.TimerFontSize, _style.TimerFontSpacing,
                (Color)drawColor
            );

            if (_style.DetailedTimer)
            {
                DrawDetailedTimer(font, timerHeight, bottomHeight);
            }
        }

        private Color GetTimerDrawColor(double time)
        {
            Color? drawColor = null;
            if (time == 0.0 || !RanAllSegmentsBefore())
            {
                drawColor = _colors.Base;
            }
            else
            {
                int segI = Math.Min(_currentSegment, Splits.Segments.Length - 1);
                bool ahead = time < Splits.Segments[segI].GetAbsolutePbCompletionTime();

                if (!Splits.Segments.Last().IsAhead(time))
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

            return (Color)drawColor;
        }

        private void DrawDetailedTimer(Font font, float timerHeight,float bottomHeight)
        {
            double detailedTime = GetDetailedTime();
            int detailedTimerFontSize = (int)Math.Round(
                                            _style.TimerFontSize * _style.DetailedTimerSize);
            int detailedTimerFontSpacing = (int)Math.Max(1, Math.Round(
                                            _style.TimerFontSpacing * _style.DetailedTimerSize));
            string detailedTimeText = _timerLocked
                ? _style.TimerLockingMessage
                : Formatter.SecondsToTime(detailedTime, true);
            Vector2 detailedTimerTextSize = Raylib.MeasureTextEx(
                font, detailedTimeText, detailedTimerFontSize, detailedTimerFontSpacing
            );
            var detailedTimerPos = new Vector2(
                (Raylib.GetRenderWidth() - detailedTimerTextSize.X)
                    - (Raylib.GetRenderWidth()) * (1 - _style.DetailedTimerMarginX) + detailedTimerTextSize.X / 2,
                Raylib.GetRenderHeight() - bottomHeight
                    - (timerHeight) / (1.0f / (1 - _style.DetailedTimerMarginY)) - detailedTimerTextSize.Y / 2
            );
            Raylib.DrawTextEx(
                font, detailedTimeText, detailedTimerPos, detailedTimerFontSize,
                detailedTimerFontSpacing, _colors.DetailedTimer
            );
        }

        private double GetDetailedTime()
        {
            double st = _currentTimeSecs;
            for (int i = _currentSegment - 1; i >= 0; i--)
            {
                st -= Splits.Segments[i].GetRelTime();
            }
            return st;
        }

        private void HandleGlobalInput()
        {
            if (_globalInputManager.InputReader.IsKeyPressed(Splits.GlobalKeybindings.Split))
            {
                Split();
            }

            if (_globalInputManager.InputReader.AskingForReset(Splits.GlobalKeybindings.Reset))
            {
                Reset();
            }

            if (_globalInputManager.InputReader.IsKeyPressed(Splits.GlobalKeybindings.Undo))
            {
                Undo();
            }

            if (_globalInputManager.InputReader.IsKeyPressed(Splits.GlobalKeybindings.Redo))
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

            _storage.SaveTimerSplits(Splits);
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

            if (IsRunFinished())
            {
                _finishTime = _currentTimeSecs;
            }
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
