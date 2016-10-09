﻿using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ModMyFactory.Helpers;

namespace ModMyFactory
{
    [JsonObject(MemberSerialization.OptOut)]
    class Settings
    {
        public static Settings CreateDefault(string fileName)
        {
            var defaultSettings = new Settings(fileName)
            {
                ManagerMode = ManagerMode.PerFactorioVersion,
                FactorioDirectoryOption = DirectoryOption.AppData,
                ModDirectoryOption = DirectoryOption.AppData,
                FactorioDirectory = string.Empty,
                ModDirectory = string.Empty,
                SelectedLanguage = "en",
                MainWindowInfo = WindowInfo.Empty,
                ModGridLength = new GridLength(1, GridUnitType.Star),
                ModpackGridLength = new GridLength(1, GridUnitType.Star),
                SelectedVersion = default(Version),
                VersionManagerWindowInfo = WindowInfo.Empty,
                OnlineModsWindowInfo = WindowInfo.Empty,
            };
            return defaultSettings;
        }

        public static Settings Load(string fileName, bool createDefault = false)
        {
            var file = new FileInfo(fileName);
            if (!file.Exists && createDefault)
            {
                Settings defaultSettings = CreateDefault(fileName);
                defaultSettings.Save();
                return defaultSettings;
            }

            Settings settings = JsonHelper.Deserialize<Settings>(file);
            settings.file = file;
            return settings;
        }

        FileInfo file;

        public ManagerMode ManagerMode;

        public DirectoryOption FactorioDirectoryOption;

        public DirectoryOption ModDirectoryOption;

        public string FactorioDirectory;

        public string ModDirectory;

        public string SelectedLanguage;

        public WindowInfo MainWindowInfo;

        public GridLength ModGridLength, ModpackGridLength;

        [JsonConverter(typeof(VersionConverter))]
        public Version SelectedVersion;

        public WindowInfo VersionManagerWindowInfo;

        public WindowInfo OnlineModsWindowInfo;

        [JsonConstructor]
        private Settings()
        { }

        private Settings(string fileName)
        {
            file = new FileInfo(fileName);
        }

        public void Save()
        {
            JsonHelper.Serialize(this, file);
        }

        public DirectoryInfo GetFactorioDirectory()
        {
            const string directoryName = "Factorio";

            switch (FactorioDirectoryOption)
            {
                case DirectoryOption.AppData:
                    return new DirectoryInfo(Path.Combine(App.Instance.AppDataPath, directoryName));
                case DirectoryOption.ApplicationDirectory:
                    return new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, directoryName));
                case DirectoryOption.Custom:
                    return new DirectoryInfo(FactorioDirectory);
            }

            throw new InvalidOperationException();
        }

        public DirectoryInfo GetModDirectory(Version version = null)
        {
            const string directoryName = "mods";

            switch (ModDirectoryOption)
            {
                case DirectoryOption.AppData:
                    if (version != null)
                        return new DirectoryInfo(Path.Combine(App.Instance.AppDataPath, directoryName, version.ToString(2)));
                    else
                        return new DirectoryInfo(Path.Combine(App.Instance.AppDataPath, directoryName));

                case DirectoryOption.ApplicationDirectory:
                    if (version != null)
                        return new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, directoryName, version.ToString(2)));
                    else
                        return new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, directoryName));

                case DirectoryOption.Custom:
                    if (version != null)
                        return new DirectoryInfo(Path.Combine(ModDirectory, version.ToString(2)));
                    else
                        return new DirectoryInfo(ModDirectory);
            }

            throw new InvalidOperationException();
        }
    }
}
