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

using JumpDiveClock.Storage;

namespace JumpDiveClock.Timing
{
    public class AppConfig : UpgradableYml
    {
        private const double CurrentConfigVersion = 1.0;

        private double? _configVersion;
        private int? _maxBackups;
        private bool? _windowResizable;

        public double ConfigVersion
        {
            get => (double)_configVersion!;
            private set => _configVersion = value;
        }

        public int MaxBackups
        {
            get => (int)_maxBackups!;
            private set => _maxBackups = value;
        }

        public bool WindowResizable
        {
            get => (bool)_windowResizable!;
            private set => _windowResizable = value;
        }

        public override void UpgradeConfig()
        {
            SetDefaultValues();

            // If necessary, upgrade values here.
            
            if (_configVersion != CurrentConfigVersion)
            {
                _configVersion = CurrentConfigVersion;
                _upgradeResult = UpgradeResult.Upgraded;
            }
        }

        internal override void SetDefaultValues()
        {
            SetDefaultValue(ref _configVersion, CurrentConfigVersion, "config_version");
            SetDefaultValue(ref _maxBackups, 500, "max_backups");
            SetDefaultValue(ref _windowResizable, false, "window_resizable");
        }
    }
}
