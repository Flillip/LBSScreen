using IniParser;
using IniParser.Model;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace LBSScreen
{
    internal static class Settings
    {
        private static readonly string _configPath = "./config.ini";
        private static readonly string _configHeader = "settings";
        private static readonly FileIniDataParser _parser = new FileIniDataParser();
        private static IniData _data;

        private static Dictionary<string, string> uiDefaultSettings = new()
        {
            { "delayBetweenPictures", "10" },
            { "lerpTimeBetweenPictures", "2" },
            { "screenWidth", "1280" },
            { "screenHeight", "720" },
            { "token", "enter_token_here" },
            { "guildId", "enter_guild_id_here" },
            { "roleId", "enter_role_id_here" },
            { "downloadPath", "./images" }
        };

        private static void VerifyConfigFile()
        {
            if (!File.Exists(_configPath))
                File.Create(_configPath).Close();
        }

        public static void WriteDefaults(string specificName = "")
        {
            IniData newData = new();

            if (string.IsNullOrEmpty(specificName) == false)
            {
                VerifyConfigFile();
                _data ??= _parser.ReadFile(_configPath);

                _data[_configHeader][specificName] = uiDefaultSettings[specificName];
                _parser.WriteFile(_configPath, _data);
                return;
            }

            foreach (KeyValuePair<string, string> kvp in uiDefaultSettings)
                newData[_configHeader][kvp.Key] = kvp.Value;

            _parser.WriteFile(_configPath, newData);
        }

        public static Color GetColor(string name)
        {
            name = char.ToLower(name[0]) + name[1..];
            float[] rgb = GetData<float[]>(name);
            int r = (int)rgb[0];
            int g = (int)rgb[1];
            int b = (int)rgb[2];
            return new Color(r, g, b);
        }

        public static Vector2 GetVector2(string name)
        {
            name = char.ToLower(name[0]) + name[1..];
            float[] vec = GetData<float[]>(name);
            return new(vec[0], vec[1]);
        }
        public static T GetData<T>(string name)
        {
            try
            {
                VerifyConfigFile();
                _data ??= _parser.ReadFile(_configPath);

                string value = _data[_configHeader][name];

                if (value == null)
                    throw new NullReferenceException(nameof(value));

                if (typeof(T) == typeof(float[]))
                {
                    float[] split = value.Split(",").Select((num) => Convert.ToSingle(num)).ToArray();
                    return (T)(object)split;
                }

                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            }

            catch (Exception e)
            {
                Logger.Error(e);
                Logger.Log($"Rewriting the key {name} in settings inside config since it errored");
                WriteDefaults(name);
                return GetData<T>(name);
            }
        }
    }
}
