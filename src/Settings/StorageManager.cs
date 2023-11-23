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
using JumpDiveClock.Timing;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace JumpDiveClock.Settings
{
    public class StorageManager
    {
        private string _configFolder;
        private IDeserializer _deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();
        private ISerializer _serializer = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        public StorageManager(string configFolder)
        {
            _configFolder = configFolder;
        }
        public int MaxBackups { get; set; }

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

        public SpeedrunTimer? LoadTimer(string path, Config config, ref Result result)
        {
            if (LoadText(path) is string splitsYml)
            {
                try
                {
                    SpeedrunTimer timer = _deserializer.Deserialize<SpeedrunTimer>(splitsYml);
                    timer.Construct(config, this);
                    SaveBackup(splitsYml, GetFileName(path));

                    result.Success = true;
                    return timer;
                }
                catch (YamlException ex)
                {
                    Console.WriteLine(ex.InnerException);
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

        public void SaveTimer(SpeedrunTimer timer, string storagePath)
        {
            string yamlText = _serializer.Serialize(timer);
            File.WriteAllText(storagePath, yamlText);
            SaveBackup(yamlText, GetFileName(storagePath));
        }

        private ulong GetDiff(ulong x, ulong y)
            => x > y ? x - y : y - x;

        private string GetFileName(string filePath)
        {
            string file = filePath.Split("/").Last();

            // We need to remove the extension.
            List<string> dotTokens = file.Split(".").ToList();
            string fileName = String.Join('.', dotTokens.Take(dotTokens.Count - 1));

            return fileName;
        }

        private ulong? GetLatestBackupNumber(string backupFolder)
        {
            string[] fileList = Directory.GetFiles(backupFolder)
                                .Where(f => f.EndsWith(".yml"))
                                .ToArray();
            ulong? fileN = null;
            foreach (string file in fileList)
            {
                try
                {
                    ulong n = ParseFileNumber(GetFileName(file));
                    if (fileN is null || n > fileN)
                    {
                        fileN = n;
                    }
                }
                catch (FormatException)
                {
                }
            }

            return fileN;
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

        private ulong ParseFileNumber(string fileName)
            => UInt64.Parse(fileName.Split("_").Last());

        private void PurgeOldBackups(ulong newestBackupN, string backupFolder)
        {
            string[] backupFiles = Directory.GetFiles(backupFolder)
                                    .Where(f => f.EndsWith(".yml"))
                                    .ToArray();

            foreach (string file in backupFiles)
            {
                ulong diff = GetDiff(newestBackupN, ParseFileNumber(GetFileName(file)));

                if (diff >= (uint)MaxBackups)
                {
                    File.Delete(file);
                }

            }
        }

        private void SaveBackup(string yamlText, string splitFileName)
        {
            string backupFolder = $"{_configFolder}/backups/{splitFileName}/";
            Directory.CreateDirectory(backupFolder);

            ulong? lastBackupN = GetLatestBackupNumber(backupFolder);
            string lastBackup = lastBackupN is null
                                ? ""
                                : File.ReadAllText(
                                    $"{backupFolder}/{splitFileName}_{lastBackupN}.yml");

            ulong newBackupN = lastBackupN + 1 ?? 0;
            if (lastBackup != yamlText)
            {
                File.WriteAllText(
                    $"{backupFolder}/{splitFileName}_{newBackupN}.yml",
                    yamlText
                );
            }

            PurgeOldBackups(newBackupN, backupFolder);
        }
    }
}
