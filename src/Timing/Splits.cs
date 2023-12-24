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
using JumpDiveClock.Storage;

namespace JumpDiveClock.Timing
{
    public class Splits : UpgradableYml
    {
        private const double CurrentSplitsVersion = 1.0;

        private int? _attemptCount;
        private string? _category;
        private bool? _completedRunBefore;
        private string? _gameName;
        private Keybindings? _globalKeybindings;
        private int? _keyboardId;
        private int? _maximumFramerate;
        private Segment[]? _segments;
        private double? _splitsVersion;
        private string? _styleName;
        private string? _worldRecordOwner;
        private double? _worldRecordSeconds;

        public int AttemptCount
        {
            get => (int)_attemptCount!;
            set => _attemptCount = value;
        }

        public string Category
        {
            get => _category!;
            private set => _category = value;
        }

        public bool CompletedRunBefore
        {
            get => (bool)_completedRunBefore!;
            set => _completedRunBefore = value;
        }

        public string GameName
        {
            get => _gameName!;
            private set => _gameName = value;
        }

        public Keybindings GlobalKeybindings
        {
            get => (Keybindings)_globalKeybindings!;
            private set => _globalKeybindings = value;
        }

        public int KeyboardId
        {
            get => (int)_keyboardId!;
            private set => _keyboardId = value;
        }

        public int MaximumFramerate
        {
            get => (int)_maximumFramerate!;
            private set => _maximumFramerate = value;
        }

        public Segment[] Segments
        {
            get => _segments!;
            private set => _segments = value;
        }

        public double SplitsVersion
        {
            get => (double)_splitsVersion!;
            private set => _splitsVersion = value;
        }

        public string StyleName
        {
            get => _styleName!;
            private set => _styleName = value;
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

        public override void UpgradeConfig()
        {
           SetDefaultValues();

            // Upgrade things here.

           if (_splitsVersion != CurrentSplitsVersion)
           {
               _splitsVersion = CurrentSplitsVersion;
               _upgradeResult = UpgradeResult.Upgraded;
           }
        }

        internal override void SetDefaultValues()
        {
            SetDefaultValue(ref _attemptCount, 0, "attempt_count");
            SetDefaultValue(ref _category, "Category name", "category");
            SetDefaultValue(ref _completedRunBefore, false, "completed_run_before");
            SetDefaultValue(ref _gameName, "Game name", "game_name");

            /*
                There are no possible sane defaults for keybindings and keyboard ID, since it's
                different for every person and device.
            */

            SetDefaultValue(ref _maximumFramerate, 30, "maximum_framerate");

            SetDefaultValue(ref _segments,
                    new Segment[]{ GenerateMockSegment(), GenerateMockSegment() },
                "segments"
            );

            // No reasonable default for style_name.
            
            SetDefaultValue(ref _worldRecordOwner, "John Doe", "world_record_owner");
            SetDefaultValue(ref _worldRecordSeconds, 8612.1298, "world_record_seconds");
        }

        private Segment GenerateMockSegment()
        {
            var s = new Segment();
            s.InitializeGenericValues();
            return s;
        }
    }
}
