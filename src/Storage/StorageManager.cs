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
using System.Diagnostics;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace JumpDiveClock.Storage
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
        private string _splitsStoragePath;

        public StorageManager(string configFolder, string splitsPath)
        {
            _configFolder = configFolder;
            _splitsStoragePath = splitsPath;
        }

        public int MaxBackups { get; set; }

        public UpgradableYml? CreateObjectFromYml<T>(string path, ref Result result)
        {
            Debug.Assert(typeof(UpgradableYml).IsAssignableFrom(typeof(T)));

            if (!File.Exists(path))
            {
                result.Error = $"Config file \"{path}\" could not be found.";
                result.Success = false;
                return null;
            }

            if (LoadText(path) is string text)
            {
                Console.WriteLine($"Trying to serialize \"{path}\"...");
                UpgradableYml? uYml = TrySerializeYml<T>(text, path, ref result);

                if (uYml is not null)
                {
                    if (uYml is Splits)
                    {
                        SaveBackup(text, GetFileName(path));
                    }
                }

                Console.WriteLine($"Serialized \"{path}\".");
                return uYml;
            }
            else
            {
                result.Error = $"Failed to read \"{path}\".";
                result.Success = false;
                return null;
            }

        }

        public void SaveToYmlFile<T>(string path, T obj)
        {
            string yml = _serializer.Serialize(obj);
            File.WriteAllText(path, yml);
        }

        public void SaveTimerSplits(Splits splits)
        {
            string yamlText = _serializer.Serialize(splits);
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

        private ulong ParseFileNumber(string fileName) => UInt64.Parse(fileName.Split("_").Last());

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
                    $"{backupFolder}/{splitFileName}_{newBackupN}.yml", yamlText
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

        private UpgradableYml? TrySerializeYml<T>(string yamlText, string path, ref Result result)
        {
            try
            {
                /* 
                    We need to pass the exact type to Deserialize(), hence the use of
                    generics. But we also need to call UpgradeYml.SetDefaultValue()
                    and there's no way to directly cast the generic directly to
                    UpgradableYml, so we cast to Object first
                */
                // You may not like it, but that's what peak programming looks like.
                UpgradableYml objFromYml = (UpgradableYml)(Object)_deserializer.Deserialize<T>(yamlText)!;

                objFromYml.UpgradeConfig();

                // TODO: check if sub-fields are null.
                List<string> unitializedFields = InitializationChecker
                                                    .GetUninitializedPrivateFields(objFromYml);

                if (unitializedFields.Count > 0)
                {
                    ShowUninitializedFields(unitializedFields);
                    result.Success = false;
                    return null;
                }

                result.Success = true;
                return objFromYml;
            }
            catch (YamlException ex)
            {
                Console.WriteLine(ex.InnerException);
                result.Error = "Failed to deserialize yaml file.\n" + ex.Message;
                Console.WriteLine(ex.StackTrace);
                result.Success = false;
                return null;
            }
        }
    }
}
