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
        private string _splitsStoragePath = null!;

        public StorageManager(string configFolder)
        {
            _configFolder = configFolder;
        }
        public int MaxBackups { get; set; }

        public AppConfig? LoadConfig(string configPath, string splitsPath, ref Result result)
        {
            if (!File.Exists(configPath))
            {
                result.Error = $"Config file \"{configPath}\" could not be found.";
                result.Success = false;
                return null;
            }

            if (LoadText(configPath) is string configText)
            {
                return TryLoadingConfig(configText, splitsPath, ref result);
            }
            else
            {
                result.Error = "Failed to read config.";
                return null;
            }
        }

        public SpeedrunTimer? LoadTimer(string splitPath, AppConfig config, ref Result result)
        {
            if (LoadText(splitPath) is string splitsYml)
            {
                return TryLoadingTimerFromSplits(config, splitsYml, splitPath, ref result);
            }
            else
            {
                result.Error = "Failed to read splits.";
                result.Success = false;
                return null;
            }
        }

        public void SaveTimerSplits(SpeedrunTimer timer)
        {
            string yamlText = _serializer.Serialize(timer.Splits);
            File.WriteAllText(_splitsStoragePath, yamlText);
            SaveBackup(yamlText, GetFileName(_splitsStoragePath));
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

        private void ShowUninitializedFields(List<string> fields)
        {
            Console.WriteLine("YML file doesn't contain all necessary fields.");
            fields.ForEach(f => Console.WriteLine($"{f} is missing."));
            Console.WriteLine(
                "If unsure about what to do, read docs, open an issue or ask for help."
            );
        }

        private AppConfig? TryLoadingConfig(string configText, string splitsPath, ref Result result)
        {
            try
            {
                AppConfig config = _deserializer.Deserialize<AppConfig>(configText);
                List<string> uninitializedFields = InitializationChecker
                                                    .GetUninitializedPrivateFields(config);

                if (uninitializedFields.Count > 0)
                {
                    ShowUninitializedFields(uninitializedFields);
                    result.Success = false;
                    return null;
                }

                _splitsStoragePath = splitsPath;
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

        private SpeedrunTimer? TryLoadingTimerFromSplits(
            AppConfig config, string splitsYml, string splitFilePath, ref Result result)
        {
            try
            {
                Splits splits = _deserializer.Deserialize<Splits>(splitsYml);
                // TODO: add null checks to all sub-configurations (e.g. hex_colors.background).
                List<string> unitializedFields =
                                        InitializationChecker.GetUninitializedPrivateFields(splits);

                if (unitializedFields.Count > 0)
                {
                    ShowUninitializedFields(unitializedFields);
                    result.Success = false;
                    return null;
                }

                SpeedrunTimer timer = new SpeedrunTimer(config, splits, this);
                SaveBackup(splitsYml, GetFileName(splitFilePath));
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
    }
}
