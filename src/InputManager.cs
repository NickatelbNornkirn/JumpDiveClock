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
    // TODO: Wayland.
    // TODO: less scuffed implementation?
    public class InputManager
    {
        private Keybindings _keybindings;
        private int _keyboardId;
        private Dictionary<int, DateTime> _lastKeyPressTimes = new Dictionary<int, DateTime>();
        private DateTime _lastResetPressTime = DateTime.MinValue;
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

        public bool AskingForReset(int keyCode)
        {
            const double MinPressInterval = 0.1;
            DateTime now = DateTime.Now;
            bool resetPressed = IsKeyPressed(keyCode, MinPressInterval, now);
            
            bool result = resetPressed && now <= _lastResetPressTime.AddSeconds(0.5) &&
                now > _lastResetPressTime.AddSeconds(MinPressInterval);

            if (resetPressed)
            {
                _lastResetPressTime = now;
            }

            return result;
        }

        public bool IsKeyPressed(int keyCode, double minimumDelay = 1.0, DateTime? now = null)
        {
            bool keyDown = _pressedKeys.Contains(keyCode);
            bool result = keyDown && !JustPressedKey(keyCode, minimumDelay, now ?? DateTime.Now);

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

        private bool JustPressedKey(int keyCode, double secondsToCompare, DateTime now)
        {
            bool result = _lastKeyPressTimes.ContainsKey(keyCode)
                            && now <= _lastKeyPressTimes[keyCode].AddSeconds(secondsToCompare);
            
            if (!result)
            {
                _lastKeyPressTimes[keyCode] = now;
            }

            return result;
        }

    }
}