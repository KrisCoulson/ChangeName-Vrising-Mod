using BepInEx;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ChangeName.Structs;

public readonly struct Settings {
    public static ConfigEntry<bool> ToggleMod { get; private set; }
    public static ConfigEntry<string> CurrencyName { get; private set; }
    public static ConfigEntry<int> RequiredCurrencyGUID { get; private set; }
    public static ConfigEntry<int> CurrencyCost { get; private set; }

    private static readonly List<string> OrderedSections = new() {
        "Config",
        "Currency"
    };

    public static void InitConfig() {

        ToggleMod = InitConfigEntry(OrderedSections[0], "Toggle", true, "If true the mod will be usable; otherwise it will be disabled.");
        CurrencyName = InitConfigEntry(OrderedSections[1], "Currency", "Silver Coins", "The name of the currency to use.");
        RequiredCurrencyGUID = InitConfigEntry(OrderedSections[1], "RequiredCurrencyGUID", -949672483, "The GUID of the required currency.");
        CurrencyCost = InitConfigEntry(OrderedSections[1], "CurrencyCost", 500, "The cost in currency for the upgrade.");


        ReorderConfigSections();
    }

    static ConfigEntry<T> InitConfigEntry<T>(string section, string key, T defaultValue, string description) {
        // Bind the configuration entry and get its value
        var entry = Plugin.Instance.Config.Bind(section, key, defaultValue, description);

        // Check if the key exists in the configuration file and retrieve its current value
        var newFile = Path.Combine(Paths.ConfigPath, $"{MyPluginInfo.PLUGIN_GUID}.cfg");

        if(File.Exists(newFile)) {
            var config = new ConfigFile(newFile, true);
            if(config.TryGetEntry(section, key, out ConfigEntry<T> existingEntry)) {
                // If the entry exists, update the value to the existing value
                entry.Value = existingEntry.Value;
            }
        }
        return entry;
    }

    private static void ReorderConfigSections() {
        var configPath = Path.Combine(Paths.ConfigPath, $"{MyPluginInfo.PLUGIN_GUID}.cfg");
        if(!File.Exists(configPath))
            return;

        var lines = File.ReadAllLines(configPath).ToList();
        var sectionsContent = new Dictionary<string, List<string>>();
        string currentSection = "";

        // Parse existing file
        foreach(var line in lines) {
            if(line.StartsWith("[")) {
                currentSection = line.Trim('[', ']');
                sectionsContent[currentSection] = new List<string> { line };
            }
            else if(!string.IsNullOrWhiteSpace(currentSection)) {
                sectionsContent[currentSection].Add(line);
            }
        }

        // Rewrite file in ordered sections
        using var writer = new StreamWriter(configPath, false);
        foreach(var section in OrderedSections) {
            if(sectionsContent.ContainsKey(section)) {
                foreach(var line in sectionsContent[section]) {
                    writer.WriteLine(line);
                }
                writer.WriteLine(); // Add spacing between sections
            }
        }
    }
}