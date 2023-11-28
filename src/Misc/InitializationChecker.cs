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

using System.Reflection;

namespace JumpDiveClock.Misc
{
    public static class InitializationChecker
    {
        // Ignore deprecated fields.
        private static string[] _deprecatedFields = { "_configVersion"};

        public static List<String> GetUninitializedPrivateFields(Object classInstance)
        {
            FieldInfo[] fil = classInstance.GetType()
                              .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                              .Where(f => !_deprecatedFields.Contains(f.Name)).ToArray();
            var unitializedFieldNames = new List<String>();
            foreach (FieldInfo fi in fil)
            {
                if (fi.GetValue(classInstance) is null)
                {
                    unitializedFieldNames.Add(fi.Name);
                }
            }

            // YAML file uses snake case
            unitializedFieldNames = unitializedFieldNames.Select(s => s = ToSnakeCase(s)).ToList();

            return unitializedFieldNames;
        }

        private static bool IsUpper(string s) => s.ToUpper() == s;

        // Don't change the _privateField naming convention.
        private static string ToSnakeCase(string s)
        {
            string result = "";
            // We start from 1 so we don't include the '_'.
            foreach (char c in s[1..])
            {
                result += IsUpper(c.ToString()) ? "_" + c.ToString().ToLower() : c;
            }

            return result;
        }
    }
}
