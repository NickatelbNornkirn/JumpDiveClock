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

            (ParsedArgs pa, Result paR) = CliArgsParser.ParseCliArgs(args);
            if (!paR.Success)
            {
                Console.WriteLine("Failed to parse command line arguments.");
                Console.WriteLine(paR.Error);
                Console.WriteLine(
                    @"Usage: JumpDiveClock [config_folder:<folder>] split:<split name>
                    Check DOCS.md for more details.");
                return;
            }

            Result r;
            try
            {
                r = app.Init(pa.SplitName, pa.ConfigFolder);
            }
            catch (NotSupportedException e)
            {
                r = new Result() { Success = false, Error = e.Message };
            }

            if (!r.Success)
            {
                Console.WriteLine("Failed to read initialize app.");
                Console.WriteLine(r.Error);
                return;
            }

            app.MainLoop();
            app.Exit();
        }
    }
}