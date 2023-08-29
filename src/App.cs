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
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace JumpDiveClock
{
    public class App
    {
        private Config _appConfig = null!;
        private Timer _timer = null!;
        private Font _font;
        private IDeserializer _deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

        public Result Init(string configPath = "config.yaml",
            string splitPath = "splits/example.yml")
        {
            var result = new Result() { Success = true };

            Console.WriteLine("Initializing app...");

            if (LoadConfig(configPath) is Config loadedConfig)
            {
                _appConfig = loadedConfig;
            }
            else
            {
                result.Success = false;
                result.Error = "Failed to read string.";
                return result;
            }

            if (LoadTimer(splitPath, _appConfig) is Timer loadedTimer)
            {
                _timer = loadedTimer;
            }
            else
            {
                result.Success = false;
                result.Error = "Failed to read split.";
                return result;
            }

            SetupWindow();

            // TODO: custom fonts.
            _font = Raylib.LoadFont("fonts/PublicPixel-z84yD.ttf");

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

        public void Exit()
        {
            Raylib.UnloadFont(_font);
        }

        private void SetupWindow()
        {
            const string Title = "Deinapar";
            Raylib.InitWindow(_appConfig.DefaultWidth, _appConfig.DefaultHeight, Title);
            Raylib.SetWindowState(ConfigFlags.FLAG_WINDOW_RESIZABLE);
            Raylib.SetTargetFPS(_appConfig.MaximumFramerate);
        }

        private void Update()
        {
            _timer.Update();
        }

        private void Draw()
        {
            Raylib.BeginDrawing();
            _timer.Draw(_font);
            Raylib.EndDrawing();
        }

        private Config? LoadConfig(string configPath)
        {
            if (!File.Exists(configPath))
            {
                Console.WriteLine($"Config file \"{configPath}\" could not be found.");
                return null;
            }

            if (LoadText(configPath) is string configText)
            {
                try
                {
                    return _deserializer.Deserialize<Config>(configText);
                }
                catch (YamlException)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private string? LoadText(string path)
        {
            string? text = null;
            try
            {
                text = File.ReadAllText(path);
            }
            catch (IOException)
            {
                Console.WriteLine($"Config file \"{path}\" could not be read.");
            }

            return text;
        }

        private Timer? LoadTimer(string path, Config config)
        {
            if (LoadText(path) is string splitsYml)
            {
                try
                {
                    // TODO: better errors.
                    return _deserializer.Deserialize<Timer>(splitsYml).Construct(config);
                }
                catch (YamlException)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}