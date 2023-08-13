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
using YamlDotNet.Serialization;
using YamlDotNet.Core;
using YamlDotNet.Serialization.NamingConventions;
using System.Diagnostics;

namespace JumpDiveClock
{
    public class App
    {
        private Config _appConfig;
        private Split _split;
        private Font _font;
        private IDeserializer _deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

        public Result Init()
        {
            var result = new Result() { Success = true };

            Console.WriteLine("Initializing app...");

            if (LoadConfig("config.yaml") is Config loadedConfig)
            {
                _appConfig = loadedConfig;
            }
            else
            {
                result.Success = false;
                result.Error = "Failed to read string.";
                return result;
            }


            if (LoadSplit("splits/example.yml") is Split loadedSplit)
            {
                _split = loadedSplit;
            }
            else
            {
                result.Success = false;
                result.Error = "Failed to read split.";
                return result;
            }


            SetupWindow();

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
            List<int> pressedKeys = GetPressedKeys();
            float deltaTime = Raylib.GetFrameTime();
            _split.Update(deltaTime);
        }

        private void Draw()
        {

            Raylib.BeginDrawing();
            _split.Draw(_font);
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

        private Split? LoadSplit(string path)
        {
            if (LoadText(path) is string splitsYml)
            {
                try
                {
                    return _deserializer.Deserialize<Split>(splitsYml);
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

        private List<int> GetPressedKeys()
        {
            var p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "xinput",
                    Arguments = $"query-state {_appConfig.KeyboardId}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            p.Start();
            string[] lines = p.StandardOutput.ReadToEnd().Split('\n');

            var kbStates = new List<int>();
            lines
                .Where(line => line.Contains("=down")).ToList()
                .ForEach(line => kbStates.Add(Int32.Parse(line.Split('[')[1].Split(']')[0])));

            return kbStates;
        }

    }
}