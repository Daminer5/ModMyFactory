﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace ModMyFactory
{
    /// <summary>
    /// Represents a mod-list.json file.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    sealed class ModTemplateList
    {
        /// <summary>
        /// Data object for serialization.
        /// </summary>
        [JsonObject(MemberSerialization.OptOut)]
        sealed class ModTemplate
        {
            [JsonProperty(PropertyName = "name")]
            public readonly string Name;

            [JsonProperty(PropertyName = "enabled")]
            [JsonConverter(typeof(BooleanToStringJsonConverter))]
            public bool Enabled;

            [JsonConstructor]
            public ModTemplate(string name, bool enabled)
            {
                Name = name;
                Enabled = enabled;
            }
        }

        /// <summary>
        /// Loads a specified mod-list.json file.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>Returns a ModTemplateList representing the specified mod.list.json file.</returns>
        public static ModTemplateList Load(string path)
        {
            var file = new FileInfo(path);
            if (file.Exists)
            {
                ModTemplateList templateList = JsonHelper.Deserialize<ModTemplateList>(file);
                templateList.file = file;
                return templateList;
            }
            else
            {
                var templateList = new ModTemplateList(file);
                templateList.Save();
                return templateList;
            }
        }

        const bool DefaultActiveState = true;
        FileInfo file;

        [JsonProperty(PropertyName = "mods")]
        List<ModTemplate> Mods;

        public Version Version { get; set; }

        [JsonConstructor]
        private ModTemplateList()
        { }

        private ModTemplateList(FileInfo file)
        {
            this.file = file;

            Mods = new List<ModTemplate>();
            Mods.Add(new ModTemplate("base", true));
        }

        private bool Contains(string name)
        {
            return Mods.Exists(mod => mod.Name == name);
        }

        /// <summary>
        /// Gets the active state of a mod.
        /// </summary>
        /// <param name="name">The mods name.</param>
        /// <returns>Returns if the specified mod is active.</returns>
        public bool GetActive(string name)
        {
            if (Contains(name))
            {
                return Mods.First(mod => mod.Name == name).Enabled;
            }
            else
            {
                Mods.Add(new ModTemplate(name, DefaultActiveState));
                Save();
                return DefaultActiveState;
            }
        }

        /// <summary>
        /// Sets the active state of a mod.
        /// </summary>
        /// <param name="name">the mods name.</param>
        /// <param name="value">The new active state of the mod.</param>
        public void SetActive(string name, bool value)
        {
            if (Contains(name))
            {
                Mods.First(mod => mod.Name == name).Enabled = value;
            }
            else
            {
                Mods.Add(new ModTemplate(name, value));
                Save();
            }
        }

        /// <summary>
        /// Saves this ModTemplateList to its file.
        /// </summary>
        public void Save()
        {
            JsonHelper.Serialize(this, file);
        }
    }
}
