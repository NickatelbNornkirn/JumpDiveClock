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

using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace JumpDiveClock
{
    public class StorageManager
    {
        private IDeserializer _deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();
        private ISerializer _serializer = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        public Config? LoadConfig(string configPath, string splitsPath, ref Result result)
        {
            if (!File.Exists(configPath))
            {
                result.Error = $"Config file \"{configPath}\" could not be found.";
                result.Success = false;
                return null;
            }

            if (LoadText(configPath) is string configText)
            {
                try
                {
                    Config config = _deserializer.Deserialize<Config>(configText);
                    config.SplitsStoragePath = splitsPath;
                    result.Success = true;
                    return config;
                }
                catch (YamlException ex)
                {
                    result.Error = "Failed to deserialize config.\n" + ex.Message;
                    result.Success = false;
                    return null;
                }
            }
            else
            {
                result.Error = "Failed to read config.";
                result.Success = false;
                return null;
            }
        }

        public Timer? LoadTimer(string path, Config config, ref Result result)
        {
            if (LoadText(path) is string splitsYml)
            {
                try
                {
                    Timer timer = _deserializer.Deserialize<Timer>(splitsYml);
                    timer.Construct(config, this);

                    result.Success = true;
                    return timer;
                }
                catch (YamlException ex)
                {
                    result.Error = "Failed to deserialize splits.\n" + ex.Message;
                    result.Success = false;
                    return null;
                }
            }
            else
            {
                result.Error = "Failed to read splits.";
                result.Success = false;
                return null;
            }
        }

        public void SaveTimer(Timer timer, string storagePath)
        {
            string yamlText = _serializer.Serialize(timer);
            File.WriteAllText(storagePath, yamlText);
            Console.WriteLine("saved");
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
    }
}