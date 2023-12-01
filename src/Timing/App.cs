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
using JumpDiveClock.Settings;
using Raylib_cs;

namespace JumpDiveClock.Timing
{
    public class App
    {
        private AppConfig _appConfig = null!;
        private Font _font;
        private StorageManager _storage = null!;
        private SpeedrunTimer _timer = null!;

        public void Exit()
        {
            Raylib.UnloadFont(_font);
        }

        public Result Init(string splitName, string? configFolder)
        {
            var result = new Result();

            string homeFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            configFolder ??= $"{homeFolder}/.config/jump-dive-clock";
            Directory.CreateDirectory(configFolder);

            string configPath = $"{configFolder}/config.yml";
            if (!File.Exists(configPath))
            {
                File.WriteAllText(configPath, File.ReadAllText("config.yml"));
            }

            string splitFolder = $"{configFolder}/splits";
            Directory.CreateDirectory(splitFolder);

            string splitsPath = $"{splitFolder}/{splitName}.yml";
            if (!File.Exists(splitsPath))
            {
                result.Error = $"{splitsPath} can't be found.";
                return result;
            }

            _storage = new StorageManager(configFolder, splitsPath);

            string fontDir = $"{configFolder}/fonts";
            Directory.CreateDirectory(fontDir);

            Console.WriteLine("Initializing app...");

            _appConfig = LoadFrom<AppConfig>(configPath, ref result);
            if (!result.Success)
            {
                return result;
            }

            _storage.MaxBackups = _appConfig.MaxBackups;

            Splits splits = LoadFrom<Splits>(splitsPath, ref result);
            if (!result.Success)
            {
                return result;
            }

            string stylesDir = $"{configFolder}/styles";
            string styleFile = $"{stylesDir}/{splits.StyleName}";
            Directory.CreateDirectory(stylesDir);
            if (!File.Exists(styleFile))
            {
                result.Error = $"{styleFile} can't be found.";
                result.Success = false;
                return result;
            }

            TimerStyle timerStyle = LoadFrom<TimerStyle>(styleFile, ref result);
            if (!result.Success)
            {
                result.Success = false;
                return result;
            }
            _timer = new SpeedrunTimer(_appConfig, splits, _storage, timerStyle);

            SetupWindow(timerStyle.DefaultWindowWidth, timerStyle.DefaultWindowHeight,
                splits.MaximumFramerate
            );

            string fontPath = $"{fontDir}/{timerStyle.FontFile}";
            if (timerStyle.FontFile != "default" && !File.Exists(fontPath))
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

        private T LoadFrom<T>(string path, ref Result result)
        {
            T? loadedObj = _storage.CreateObjectFromYml<T>(path, ref result);
            return loadedObj!;
        }

        private void SetupWindow(int defaultWidth, int defaultHeight, int maximumFramerate)
        {
            const string Title = "Jump Dive Clock";

            if (_appConfig.WindowResizable)
            {
                Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
            }

            Raylib.InitWindow(defaultWidth, defaultHeight, Title);
            Raylib.SetTargetFPS(maximumFramerate);
        }

        private void Update()
        {
            _timer.Update();
        }
    }
}