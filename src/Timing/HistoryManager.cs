
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

namespace JumpDiveClock.Timing
{
    public class HistoryManager
    {
        private Stack<double> _undoSplitTimes = new Stack<double>();

        public bool CanRedo()
        {
            return _undoSplitTimes.Count > 0;
        }

        public void ClearHistory()
        {
            _undoSplitTimes.Clear();
        }

        public double RegisterRedo()
        {
            double result = -1;

            if (_undoSplitTimes.Count > 0)
            {
                result = _undoSplitTimes.Pop();
            }

            return result;
        }

        public void RegisterUndo(double actionTime)
        {
            _undoSplitTimes.Push(actionTime);
        }

    }
}