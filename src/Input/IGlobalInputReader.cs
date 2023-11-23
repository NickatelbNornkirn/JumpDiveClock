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

namespace JumpDiveClock.Input
{
    public interface IGlobalInputReader
    {
        public static abstract bool IsBackendAvailable();
        public bool AskingForReset(Keybinding key);
        public bool IsKeyPressed(Keybinding key, double minimumDelay = 1.0, DateTime? now = null);
        public bool JustPressedKey(Keybinding key, double secondsToCompare, DateTime now);
        public void UpdateKeyboardState(); // Call once per frame.
    }
}
