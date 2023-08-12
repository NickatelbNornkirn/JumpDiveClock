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
    public class Entrypoint
    {
        public static void Main(string[] args)
        {
            var app = new App();

            Result r = app.Init();

            if (!r.Success)
            {
                Console.WriteLine("Failed to read initialize app.");
                Console.WriteLine(r.Error);
            }
            app.Loop();
            app.Exit();
        }
    }
}