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

namespace JumpDiveClock.Timing
{
    public class TimerStyle
    {
        private int? _attemptCountFontSize;
        private int? _attemptCountFontSpacing;
        private float? _attemptSizeTextPosX;
        private float? _attemptSizeTextPosY;
        private int? _categoryTitleFontSize;
        private int? _categoryTitleFontSpacing;
        private int? _defaultWindowHeight;
        private int? _defaultWindowWidth;
        private StatType[]? _extraStats;
        private string? _fontFile;
        private int? _gameTitleFontSize;
        private int? _gameTitleFontSpacing;
        private float? _headerHeight;
        private HexColors? _hexColors;
        private float? _maxSegmentSize;
        private int? _minSegmentsAheadToShow;
        private int? _segmentFontSize;
        private int? _segmentFontSpacing;
        private int? _segmentMargin;
        private int? _segmentsPerScreen;
        private int? _separatorSize;
        private int? _timerFontSize;
        private int? _timerFontSpacing;
        private string? _timerLockingMessage;
        private float? _timerSize;
        private int? _titleCategoryTitlesGap;

        public int AttemptCountFontSize
        {
            get => (int)_attemptCountFontSize!;
            private set => _attemptCountFontSize = value;
        }

        public int AttemptCountFontSpacing
        {
            get => (int)_attemptCountFontSpacing!;
            private set => _attemptCountFontSpacing = value;
        }

        public float AttemptSizeTextPosX
        {
            get => (float)_attemptSizeTextPosX!;
            set => _attemptSizeTextPosX = value;
        }

        public float AttemptSizeTextPosY
        {
            get => (float)_attemptSizeTextPosY!;
            set => _attemptSizeTextPosY = value;
        }

        public int CategoryTitleFontSize
        {
            get => (int)_categoryTitleFontSize!;
            private set => _categoryTitleFontSize = value;
        }

        public int CategoryTitleFontSpacing
        {
            get => (int)_categoryTitleFontSpacing!;
            set => _categoryTitleFontSpacing = value;
        }

        public int DefaultWindowHeight
        {
            get => (int)_defaultWindowHeight!;
            private set => _defaultWindowHeight = value;
        }

        public int DefaultWindowWidth
        {
            get => (int)_defaultWindowWidth!;
            private set => _defaultWindowWidth = value;
        }

        public StatType[] ExtraStats
        {
            get => _extraStats!;
            private set => _extraStats = value;
        }

        public string FontFile
        {
            get => _fontFile!;
            private set => _fontFile = value;
        }

        public int GameTitleFontSize
        {
            get => (int)_gameTitleFontSize!;
            private set => _gameTitleFontSize = value;
        }

        public int GameTitleFontSpacing
        {
            get => (int)_gameTitleFontSpacing!;
            private set => _gameTitleFontSpacing = value;
        }

        public float HeaderHeight
        {
            get => (float)_headerHeight!;
            private set => _headerHeight = value;
        }

        public HexColors HexColors
        {
            get => _hexColors!;
            private set => _hexColors = value;
        }

        public float MaxSegmentSize
        {
            get => (float)_maxSegmentSize!;
            private set => _maxSegmentSize = value;
        }

        public int MinSegmentsAheadToShow
        {
            get => (int)_minSegmentsAheadToShow!;
            private set => _minSegmentsAheadToShow = value;
        }

        public int SegmentFontSize
        {
            get => (int)_segmentFontSize!;
            private set => _segmentFontSize = value;
        }

        public int SegmentFontSpacing
        {
            get => (int)_segmentFontSpacing!;
            private set => _segmentFontSpacing = value;
        }

        public int SegmentMargin
        {
            get => (int)_segmentMargin!;
            private set => _segmentMargin = value;
        }

        public int SegmentsPerScreen
        {
            get => (int)_segmentsPerScreen!;
            private set => _segmentsPerScreen = value;

        }

        public int SeparatorSize
        {
            get => (int)_separatorSize!;
            private set => _separatorSize = value;
        }

        public int TimerFontSize
        {
            get => (int)_timerFontSize!;
            private set => _timerFontSize = value;
        }

        public int TimerFontSpacing
        {
            get => (int)_timerFontSpacing!;
            private set => _timerFontSpacing = value;
        }

        public string TimerLockingMessage
        {
            get => _timerLockingMessage!;
            private set => _timerLockingMessage = value;
        }

        public float TimerSize
        {
            get => (float)_timerSize!;
            private set => _timerSize = value;
        }

        public int TitleCategoryTitlesGap
        {
            get => (int)_titleCategoryTitlesGap!;
            private set => _titleCategoryTitlesGap = value;
        }
    }
}
