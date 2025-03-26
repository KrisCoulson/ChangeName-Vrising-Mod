using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using ChangeName.Structs;
using HarmonyLib;
using VampireCommandFramework;

namespace ChangeName;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("gg.deca.VampireCommandFramework")]
[Bloodstone.API.Reloadable]
public class Plugin : BasePlugin {
    Harmony _harmony;
    public static Plugin Instance { get; private set; }
    public static Harmony Harmony => Instance._harmony;
    public static ManualLogSource LogInstance => Instance.Log;
    public static Settings Settings { get; private set; }


    public override void Load() {
        Instance = this;
        Settings = new Settings();
        Settings.InitConfig();

        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loaded!");

        _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        _harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());


        CommandRegistry.RegisterAll();
    }

    public override bool Unload() {
        CommandRegistry.UnregisterAssembly();
        _harmony?.UnpatchSelf();
        return true;
    }
}
