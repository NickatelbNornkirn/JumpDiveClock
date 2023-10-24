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

using Raylib_cs;

namespace JumpDiveClock
{
    public class App
    {
        private Config _appConfig = null!;
        private Font _font;
        private StorageManager _storage = null!;
        private Timer _timer = null!;

        public void Exit()
        {
            Raylib.UnloadFont(_font);
        }

        public Result Init(string splitName, string? configFolder)
        {
            var result = new Result() { Success = false };

            string homeFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            configFolder ??= $"{homeFolder}/.config/jump-dive-clock";
            Directory.CreateDirectory(configFolder);

            _storage = new StorageManager(configFolder);

            string configPath = $"{configFolder}/config.yml";
            if (!File.Exists(configPath))
            {
                File.WriteAllText(configPath, File.ReadAllText("config.yml"));
            }

            string splitFolder = $"{configFolder}/splits";
            Directory.CreateDirectory(splitFolder);

            string splitPath = $"{splitFolder}/{splitName}.yml";
            if (!File.Exists(splitPath))
            {
                result.Error = $"{splitPath} can't be found.";
                return result;
            }

            string fontFolder = $"{configFolder}/fonts";
            Directory.CreateDirectory(fontFolder);

            Console.WriteLine("Initializing app...");

            Config? config = _storage.LoadConfig(configPath, splitPath, ref result);
            if (config is null)
            {
                return result;
            }
            else
            {
                _appConfig = config;
            }

            _storage.MaxBackups = _appConfig.MaxBackups;

            Timer? loadedTimer = _storage.LoadTimer(splitPath, _appConfig, ref result);
            if (loadedTimer is null)
            {
                return result;
            }
            else
            {
                _timer = loadedTimer;
            }

            SetupWindow();

            string fontPath = $"{fontFolder}/{_appConfig.FontFile}";
            if (_appConfig.FontFile != "default" && !File.Exists(fontPath))
            {
                result.Error = $"Could not find font '{fontPath}'.";
                result.Success = false;
                return result;
            }

            // If the font is not found, Raylib automatically loads the default one.
            _font = Raylib.LoadFont(fontPath);

            result.Success = true;
            return result;
        }

        public void MainLoop()
        {
            var running = true;

            while (running)
            {
                Update();
                Draw();

                if (Raylib.WindowShouldClose())
                {
                    running = false;
                    _timer.AutoSave();
                }
            }
        }

        private void Draw()
        {
            Raylib.BeginDrawing();
            _timer.Draw(_font);
            Raylib.EndDrawing();
        }

        private void SetupWindow()
        {
            const string Title = "Jump Dive Clock";

            if (_appConfig.WindowResizable)
            {
                Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
            }

            Raylib.InitWindow(_appConfig.DefaultWidth, _appConfig.DefaultHeight, Title);
            Raylib.SetTargetFPS(_appConfig.MaximumFramerate);
        }

        private void Update()
        {
            _timer.Update();
        }
    }
}