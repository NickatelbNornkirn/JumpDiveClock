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

using System.Reflection;

// TODO: add null checks to all sub-configurations (e.g. hex_colors.background).
namespace JumpDiveClock.Timing
{
    // This class contains everything in the split.yml file, nothing more (except for a few methods)
    // and nothing less
    public class Splits
    {
        private int? _attemptCount;
        private int? _attemptCountFontSize;
        private int? _attemptCountFontSpacing;
        private float? _attemptSizeTextPosX;
        private float? _attemptSizeTextPosY;
        private string? _category;
        private int? _categoryTitleFontSize;
        private int? _categoryTitleFontSpacing;
        private bool? _completedRunBefore;
        private StatType[]? _extraStats;
        private string? _gameName;
        private int? _gameTitleFontSize;
        private int? _gameTitleFontSpacing;
        private float? _headerHeight;
        private HexColors? _hexColors;
        private float? _maxSegmentSize;
        private int? _segmentFontSize;
        private int? _segmentFontSpacing;
        private int? _segmentMargin;
        private Segment[]? _segments;
        private int? _segmentsPerScreen;
        private int? _separatorSize;
        private int? _timerFontSize;
        private int? _timerFontSpacing;
        private float? _timerSize;
        private int? _titleCategoryTitlesGap;
        private string? _worldRecordOwner;
        private double? _worldRecordSeconds;

        public int AttemptCount
        {
            get => (int)_attemptCount!;
            set => _attemptCount = value;
        }

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

        public string Category
        {
            get => _category!;
            private set => _category = value;
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

        public bool CompletedRunBefore
        {
            get => (bool)_completedRunBefore!;
            set => _completedRunBefore = value;
        }

        public StatType[] ExtraStats
        {
            get => _extraStats!;
            private set => _extraStats = value;
        }

        public string GameName
        {
            get => _gameName!;
            private set => _gameName = value;
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

        public Segment[] Segments
        {
            get => _segments!;
            private set => _segments = value;
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

        public string WorldRecordOwner
        {
            get => _worldRecordOwner!;
            set => _worldRecordOwner = value;
        }

        public double WorldRecordSeconds
        {
            get => (double)_worldRecordSeconds!;
            set => _worldRecordSeconds = value;
        }

        public List<String> GetUninitializedFields()
        {
            FieldInfo[] fil = GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                              .ToArray();
            var unitializedFieldNames = new List<String>();
            foreach (FieldInfo fi in fil)
            {
                if (fi.GetValue(this) is null)
                {
                    unitializedFieldNames.Add(fi.Name);
                }
            }

            // YAML file uses snake case
            unitializedFieldNames = unitializedFieldNames.Select(s => s = ToSnakeCase(s)).ToList();

            return unitializedFieldNames;
        }

        private bool IsUpper(string s) => s.ToUpper() == s;

        private string ToSnakeCase(string s)
        {
            string result = "";
            // We start from 1 so we don't include the '_'.
            foreach (char c in s[1..])
            {
                result += IsUpper(c.ToString()) ? "_" + c.ToString().ToLower() : c;
            }

            return result;
        }
    }
}
