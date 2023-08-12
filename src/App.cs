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

        }

        private void SetupWindow()
        {
            const string Title = "Deinapar";
            Raylib.InitWindow(400, 800, Title);
            Raylib.SetWindowState(ConfigFlags.FLAG_WINDOW_RESIZABLE);
            Raylib.SetTargetFPS(_appConfig.MaximumFramerate);
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

        private void Update()
        {
            float deltaTime = Raylib.GetFrameTime();
            List<int> pressedKeys = GetPressedKeys();
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

        private void Draw()
        {
            Raylib.BeginDrawing();
            {
                Raylib.ClearBackground(Color.DARKGRAY);
                Raylib.DrawFPS(5, 5);
            }
            Raylib.EndDrawing();
        }

        private Color ToColor(string hexColor)
        {
            return new Color(
                Convert.ToInt32(hexColor.Substring(1, 2)),
                Convert.ToInt32(hexColor.Substring(3, 2)),
                Convert.ToInt32(hexColor.Substring(5, 2)),
                255
            );
        }
    }
}