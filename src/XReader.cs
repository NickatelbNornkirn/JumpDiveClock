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
    public class XReader : IGlobalInputReader
    {
        private const int LShiftKeyCode = 50;
        private const int RShiftKeyCode = 62;
        private Dictionary<int, DateTime> _lastKeyPressTimes = new Dictionary<int, DateTime>();
        private DateTime _lastResetPressTime = DateTime.MinValue;
        private List<int> _pressedKeys = new List<int>();
        private Process _xinput;

        public XReader(int keyboardId)
        {
            _xinput = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "xinput",
                    Arguments = $"query-state {keyboardId}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
        }

        public static bool IsBackendAvailable()
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

        public bool AskingForReset(Keybinding key)
        {
            const double MinPressInterval = 0.1;
            DateTime now = DateTime.Now;
            bool resetPressed = IsKeyPressed(key, MinPressInterval, now);

            bool result = resetPressed && now <= _lastResetPressTime.AddSeconds(0.5) &&
                now > _lastResetPressTime.AddSeconds(MinPressInterval);

            if (resetPressed)
            {
                _lastResetPressTime = now;
            }

            return result;
        }

        public bool IsKeyPressed(Keybinding key, double minimumDelay = 1.0, DateTime? now = null)
            => KeystrokeDown(key) && !JustPressedKey(key, minimumDelay, now ?? DateTime.Now);

        public bool JustPressedKey(Keybinding key, double secondsToCompare, DateTime now)
        {
            int keyCode = GetKeyCode(key);
            bool result = KeystrokeDown(key)
                            && _lastKeyPressTimes.ContainsKey(keyCode)
                            && now <= _lastKeyPressTimes[keyCode].AddSeconds(secondsToCompare);

            if (!result)
            {
                _lastKeyPressTimes[keyCode] = now;
            }

            return result;
        }

        public void UpdateKeyboardState()
        {
            _xinput.Start();
            string[] lines = _xinput.StandardOutput.ReadToEnd().Split('\n');

            _pressedKeys.Clear();

            /*
                The command outputs many lines like "key[xx]=up" or "key[yy]" = down.

                So we look for the ones that are down and grab their ID.
            */
            lines
                .Where(line => line.Contains("=down")).ToList()
                .ForEach(line => _pressedKeys.Add(
                    Int32.Parse(line.Split('[')[1].Split(']')[0])
            ));

            _xinput.Close();
        }

        private int GetKeyCode(Keybinding k) => Int32.Parse(k.KeyId);

        private bool KeystrokeDown(Keybinding key)
            => _pressedKeys.Contains(GetKeyCode(key)) && (!key.RequiresShift || ShiftPressed());

        private bool ShiftPressed()
            => _pressedKeys.Contains(RShiftKeyCode) || _pressedKeys.Contains(LShiftKeyCode);
    }
}
