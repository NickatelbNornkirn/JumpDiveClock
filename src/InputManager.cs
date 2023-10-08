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

namespace JumpDiveClock
{
    public class InputManager
    {
        public readonly IInputReader InputReader;

        public InputManager(Config config)
        {
            InputReader = ChooseInputReader(config);
        }

        /// <summary>
        /// Crashes if no available implementation is found.
        /// </summary>
        private IInputReader ChooseInputReader(Config config)
        {
            if (XReader.IsXInputAvailable())
            {
                return new XReader(config.KeyboardId);
            }
            else
            {
                throw new
                    NotSupportedException("Can't read global input. Your device isn't supported.");
            }
        }
    }
}