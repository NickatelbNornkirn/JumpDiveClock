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

namespace JumpDiveClock.Timing
{
    public class Splits
    {
        private int? _attemptCount;
        private string? _category;
        private bool? _completedRunBefore;
        private string? _gameName;
        private Keybindings? _globalKeybindings;
        private int? _keyboardId;
        private int? _maximumFramerate;
        private Segment[]? _segments;
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
    }
}
