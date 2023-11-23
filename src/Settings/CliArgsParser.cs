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

using JumpDiveClock.Misc;

namespace JumpDiveClock.Settings
{
    public static class CliArgsParser
    {
        public static (ParsedArgs, Result) ParseCliArgs(string[] args)
        {
            Result r = new Result() { Success = true };
            ParsedArgs pa = new ParsedArgs();
            foreach (string arg in args)
            {
                string? a;
                bool validKey = false;

                if ((a = ParseArg(arg, "config_folder")) is not null)
                {
                    pa.ConfigFolder = a;
                    validKey = true;
                }

                if ((a = ParseArg(arg, "split")) is not null)
                {
                    pa.SplitName = a;
                    validKey = true;
                }

                if (!validKey)
                {
                    r.Error = $"Can't parse '{arg}'.";
                    r.Success = false;
                    break;
                }
            }

            if (pa.SplitName is null)
            {
                r.Success = false;
                r.Error = "Split name not supplied.";
            }

            return (pa, r);
        }

        private static string? ParseArg(string arg, string keyName)
        {
            if (!arg.Contains(":"))
            {
                return null;
            }

            string[] tokens = arg.Split(":");

            if (tokens[0] != keyName)
            {
                return null;
            }

            if (tokens.Length > 2)
            {
                return null;
            }

            return tokens[1];
        }
    }
}
