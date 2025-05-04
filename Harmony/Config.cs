using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;


public class Config
{
    private static readonly Dictionary<string, string> modConfig = ReadModConfig();

    public static KeyCode autoMeleeKey = GetKeyCode();

    private static KeyCode GetKeyCode()
    {
        if (modConfig is null)
            return KeyCode.Mouse2;

        string keyCodeStr = modConfig["autoMeleeKey"];

        if (Enum.TryParse<KeyCode>(keyCodeStr, out var parsedKey))
            return parsedKey;

        Log.Error($"[AutoMelee] Invalid KeyCode: '{keyCodeStr}'");
        return KeyCode.Mouse2;
    }

    private static Dictionary<string, string> ReadModConfig()
    {
        var configDict = new Dictionary<string, string>();

        var modPath = Assembly.GetExecutingAssembly().Location;
        var modConfig = Path.GetFullPath($"{modPath}/../ModConfig.cfg");

        if (!File.Exists(modConfig))
        {
            Log.Error($"ModConfig.cfg not found: '{modConfig}'");
            return null;
        }

        Log.Out("[AutoMelee] Path: " + modConfig);

        using (var reader = new StreamReader(modConfig))
        {
            foreach (var row in reader.ReadToEnd().Split('\n'))
            {
                if (row.Trim().StartsWith("#"))
                    continue;

                var parts = row.Split('=');

                if (parts.Length < 2)
                    continue;

                var key = parts[0].Trim();
                var value = parts[1].Trim();

                configDict[key] = value;
            }
        }

        return configDict;
    }
}