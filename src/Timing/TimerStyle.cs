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

using JumpDiveClock.Storage;

namespace JumpDiveClock.Timing
{
    public class TimerStyle : UpgradableYml
    {
        private const double CurrentStyleVersion = 1.0;

        private int? _attemptCountFontSize;
        private int? _attemptCountFontSpacing;
        private float? _attemptSizeTextPosX;
        private float? _attemptSizeTextPosY;
        private int? _categoryTitleFontSize;
        private int? _categoryTitleFontSpacing;
        private int? _defaultWindowHeight;
        private int? _defaultWindowWidth;
        private bool? _detailedTimer;
        private float? _detailedTimerSize;
        private float? _detailedTimerMarginX;
        private float? _detailedTimerMarginY;
        private bool? _drawTimerContainerTopSeparator;
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
        private double? _styleVersion;
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

        public bool DetailedTimer
        {
            get => (bool)_detailedTimer!;
            private set => _detailedTimer = value;
        }

        public float DetailedTimerSize
        {
            get => (float)_detailedTimerSize!;
            private set => _detailedTimerSize = value;
        }

        public float DetailedTimerMarginX
        {
            get => (float)_detailedTimerMarginX!;
            private set => _detailedTimerMarginX = value;
        }

        public float DetailedTimerMarginY
        {
            get => (float)_detailedTimerMarginY!;
            private set => _detailedTimerMarginY = value;
        }

        public bool DrawTimerContainerTopSeparator
        {
            get => (bool)_drawTimerContainerTopSeparator!;
            private set => _drawTimerContainerTopSeparator = value;
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

        public double StyleVersion
        {
            get => (double)_styleVersion!;
            private set => _styleVersion = value;
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

        public override void UpgradeConfig()
        {
            SetDefaultValues();

            // Upgrade stuff here.
            
            if (_styleVersion != CurrentStyleVersion)
            {
                _styleVersion = CurrentStyleVersion;
                _upgradeResult = UpgradeResult.Upgraded;
            }
        }

        internal override void SetDefaultValues()
        {
            SetDefaultValue(ref _styleVersion, CurrentStyleVersion, "style_version");
            SetDefaultValue(ref _attemptSizeTextPosX, 0.98f, "attempt_size_text_pos_x");
            SetDefaultValue(ref _attemptSizeTextPosY, 0.9f, "attempt_size_text_pos_y");
            SetDefaultValue(ref _attemptCountFontSize, 16, "attempt_count_font_size");
            SetDefaultValue(ref _attemptCountFontSpacing, 1, "attempt_count_font_spacing");
            SetDefaultValue(ref _categoryTitleFontSize, 20, "category_title_font_size");
            SetDefaultValue(ref _categoryTitleFontSpacing, 2, "category_title_font_spacing");
            SetDefaultValue(ref _gameTitleFontSize, 38, "game_title_font_size");
            SetDefaultValue(ref _gameTitleFontSpacing, 4, "game_title_font_spacing");
            SetDefaultValue(ref _segmentFontSize, 24, "segment_font_size");
            SetDefaultValue(ref _segmentFontSpacing, 2, "segment_font_spacing");
            SetDefaultValue(ref _headerHeight, 10, "header_height");
            SetDefaultValue(ref _maxSegmentSize, 8, "max_segment_size");
            SetDefaultValue(ref _segmentMargin, 3, "segment_margin");
            SetDefaultValue(ref _separatorSize, 2, "separator_size");
            SetDefaultValue(ref _timerFontSize, 48, "timer_font_size");
            SetDefaultValue(ref _timerFontSpacing, 2, "timer_font_spacing");
            SetDefaultValue(ref _timerSize, 15, "timer_size");
            SetDefaultValue(ref _titleCategoryTitlesGap, 3, "title_category_titles_gap");
            SetDefaultValue(ref _segmentsPerScreen, 10, "segments_per_screen");
            SetDefaultValue(ref _extraStats,
                new StatType[] {
                    StatType.CurrentPace, StatType.PersonalBest, StatType.BestPossibleTime,
                    StatType.SumOfBest, StatType.WorldRecord, StatType.RunsThatReachHere},
                "extra_stats"
            );

            const string DefBackground = "#252525ff";
            const string DefPaceAheadGaining = "#1dbd48ff";
            const string DefPaceAheadLosing = "#6cbd82ff";
            const string DefPaceBehindGaining = "#da7c7cff";
            const string DefPaceBehindLosing = "#da2121ff";
            const string DefPaceBest = "#fff663ff";
            const string DefSeparator = "#555555ff";
            const string DefTextBase = "#f2f2f2ff";
            const string DefDetailedTimer = "#f2f2f27f";
            SetDefaultValue(ref _hexColors,
                 new HexColors().Construct(DefBackground, DefPaceAheadGaining,
                    DefPaceAheadLosing, DefPaceBehindGaining, DefPaceBehindLosing,
                    DefPaceBest, DefSeparator, DefTextBase, DefDetailedTimer),
                "hex_colors"
            );
            SetDefaultValue(ref _hexColors!.Background, DefBackground, "hex_colors.background");
            SetDefaultValue(ref _hexColors!.DetailedTimer, DefDetailedTimer, "hex_colors.detailed_timer");
            SetDefaultValue(ref _hexColors!.PaceAheadGaining, DefPaceAheadGaining, "hex_colors.pace_ahead_gaining");
            SetDefaultValue(ref _hexColors!.PaceAheadLosing, DefPaceAheadLosing, "hex_colors.pace_ahead_losing");
            SetDefaultValue(ref _hexColors!.PaceBehindGaining, DefPaceBehindGaining, "hex_colors.pace_behind_gaining");
            SetDefaultValue(ref _hexColors!.PaceBehindLosing, DefPaceBehindLosing, "hex_colors.pace_behind_losing");
            SetDefaultValue(ref _hexColors!.PaceBest, DefPaceBest, "hex_colors.pace_best");
            SetDefaultValue(ref _hexColors!.Separator, DefSeparator, "hex_colors.separator");
            SetDefaultValue(ref _hexColors!.TextBase, DefTextBase, "hex_colors.text_base");

            SetDefaultValue(ref _fontFile, "default", "font_file");
            SetDefaultValue(ref _minSegmentsAheadToShow, 2, "min_segments_ahead_to_show");
            SetDefaultValue(ref _defaultWindowWidth, 400, "default_window_width");
            SetDefaultValue(ref _defaultWindowHeight, 800, "default_window_height");
            SetDefaultValue(ref _timerLockingMessage, "Locked", "timer_locking_message");
            SetDefaultValue(ref _detailedTimerSize, 0.75f, "detailed_timer_size");
            SetDefaultValue(ref _detailedTimerMarginX, 0.85f, "detailed_timer_margin_x");
            SetDefaultValue(ref _detailedTimerMarginY, 0.8f, "detailed_timer_margin_y");
            SetDefaultValue(ref _detailedTimer, true, "detailed_timer");

            SetDefaultValue(ref _drawTimerContainerTopSeparator, false, "draw_timer_container_top_separator");
        }
    }
}
