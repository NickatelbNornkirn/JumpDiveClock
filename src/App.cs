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
        private StorageManager _storage = new StorageManager();
        private Timer _timer = null!;

        public void Exit()
        {
            Raylib.UnloadFont(_font);
        }

        public Result Init(string configPath = "config.yaml",
            string splitPath = "splits/example.yml")
        {
            var result = new Result();

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

            // TODO: custom fonts.
            _font = Raylib.LoadFont(_appConfig.FontPath);

            result.Success = true;
            return result;
        }

        public void Loop()
        {
            var running = true;

            while (running)
            {
                Update();
                Draw();

                if (Raylib.WindowShouldClose())
                {
                    running = false;
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
            Raylib.InitWindow(_appConfig.DefaultWidth, _appConfig.DefaultHeight, Title);
            Raylib.SetWindowState(ConfigFlags.FLAG_WINDOW_RESIZABLE);
            Raylib.SetTargetFPS(_appConfig.MaximumFramerate);
        }

        private void Update()
        {
            _timer.Update();
        }
    }
}