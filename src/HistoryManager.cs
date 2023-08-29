
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
    public class HistoryManager
    {
        private Stack<ActionType> _mainStack = new Stack<ActionType>();
        private Stack<ActionType> _undoStack = new Stack<ActionType>();
        private Stack<ActionType> _redoStack = new Stack<ActionType>();

        public void RegisterActionExecution(ActionType action)
        {
            _mainStack.Push(action);
            _redoStack.Clear();
        }

        // Returns null if there are no more actions to undo.
        public ActionType? RegisterActionUndo()
        {
            if (_mainStack.Count < 1 && _redoStack.Count < 1)
            {
                return null;
            }

            ActionType action = _mainStack.Pop();
            /* 
                PointerRedo means that the last action was a redo, so we get the action from the
                redo stack instead of the main one.
            */
            if (action == ActionType.PointerRedo)
            {
                action = _redoStack.Pop();
            }

            _undoStack.Push(action);

            return action;
        }

        // Returns null if there are no more actions to redo.
        public ActionType? RegisterActionRedo()
        {
            if (_undoStack.Count < 1)
            {
                return null;
            }

            ActionType action = _undoStack.Pop();
            _mainStack.Push(ActionType.PointerRedo);
            _redoStack.Push(action);

            return action;
        }
    }
}