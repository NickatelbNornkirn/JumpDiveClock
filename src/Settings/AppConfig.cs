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

namespace JumpDiveClock.Settings
{
    public class AppConfig
    {
        private int? _defaultHeight;
        private int? _defaultWidth;
        private string? _fontFile;
        private Keybindings? _globalKeybindings;
        private int? _keyboardId;
        private int? _maxBackups;
        private int? _maximumFramerate;
        private int? _minSegmentsAheadToShow;
        private string? _timerLockingMessage;
        private bool? _windowResizable;

        public int DefaultHeight
        {
            get => (int)_defaultHeight!;
            private set => _defaultHeight = value;
        }

        public int DefaultWidth
        {
            get => (int)_defaultWidth!;
            private set => _defaultWidth = value;
        }

        public string FontFile
        {
            get => _fontFile!;
            private set => _fontFile = value;
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

        public int MaxBackups
        {
            get => (int)_maxBackups!;
            private set => _maxBackups = value;
        }

        public int MaximumFramerate
        {
            get => (int)_maximumFramerate!;
            private set => _maximumFramerate = value;
        }

        public int MinSegmentsAheadToShow
        {
            get => (int)_minSegmentsAheadToShow!;
            private set => _minSegmentsAheadToShow = value;
        }

        public string TimerLockingMessage
        {
            get => _timerLockingMessage!;
            private set => _timerLockingMessage = value;
        }

        public bool WindowResizable
        {
            get => (bool)_windowResizable!;
            private set => _windowResizable = value;
        }
    }
}