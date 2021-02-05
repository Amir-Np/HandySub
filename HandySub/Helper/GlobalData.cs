﻿using System.IO;
using Newtonsoft.Json;

namespace HandySub
{
    internal class GlobalData
    {
        public static void Init()
        {
            if (File.Exists(AppConfig.SavePath))
            {
                try
                {
                    var json = File.ReadAllText(AppConfig.SavePath);
                    Config = (string.IsNullOrEmpty(json)
                        ? new AppConfig()
                        : JsonConvert.DeserializeObject<AppConfig>(json)) ?? new AppConfig();
                }
                catch
                {
                    Config = new AppConfig();
                }
            }
            else
            {
                Config = new AppConfig();
            }
        }

        public static void Save()
        {
            var json = JsonConvert.SerializeObject(Config, Formatting.Indented);
            File.WriteAllText(AppConfig.SavePath, json);
        }

        public static AppConfig Config { get; set; }
    }
}