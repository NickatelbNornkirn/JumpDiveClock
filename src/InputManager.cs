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

using System.ComponentModel;
using System.Data;
using System.Diagnostics;

namespace JumpDiveClock
{
    // REFACTOR: better implementation?
    public class InputManager
    {
        private Keybindings _keybindings;
        private int _keyboardId;
        private Dictionary<int, DateTime> _lastKeyPressTimes = new Dictionary<int, DateTime>();
        private List<int> _pressedKeys = new List<int>();

        public InputManager(Config config)
        {
            _keybindings = config.Keybindings;
            _keyboardId = config.KeyboardId;
        }

        public static bool IsXInputAvailable()
        {
            try
            {
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "xinput",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                p.Start();
                p.Close();

                return true;
            }
            catch (Win32Exception)
            {
                return false;
            }
        }

        public bool IsKeyPressed(int keyCode)
        {
            if (_pressedKeys.Count > 0)
            {
                Console.WriteLine(_pressedKeys);
            }

            bool keyDown = _pressedKeys.Contains(keyCode);
            bool result = keyDown && !JustPressedKey(keyCode, keyDown);

            return result;
        }

        // Call once per frame.
        public void UpdateKeyboardState(Config appConfig)
        {

            var p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "xinput",
                    Arguments = $"query-state {appConfig.KeyboardId}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            p.Start();
            string[] lines = p.StandardOutput.ReadToEnd().Split('\n');

            _pressedKeys.Clear();
            lines
                .Where(line => line.Contains("=down")).ToList()
                .ForEach(line => _pressedKeys.Add(Int32.Parse(line.Split('[')[1].Split(']')[0])));

            p.Close();
        }

        private bool JustPressedKey(int keyCode, bool keyDown)
        {
            DateTime now = DateTime.Now;
            bool result = _lastKeyPressTimes.ContainsKey(keyCode)
                            ? now <= _lastKeyPressTimes[keyCode].AddSeconds(1) : false;

            if (!result)
            {
                _lastKeyPressTimes[keyCode] = now;
            }

            return result;
        }

    }
}