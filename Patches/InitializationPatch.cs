using HarmonyLib;
using ProjectM;

namespace ChangeName.Patches;

// HOW-TO: This is a Harmony Patch. This one is used to initialize our Core class once the game has loaded. 
// Every Patch requires a class to patch (SpawnTeamSystem_OnPersistenceLoad) and a method to patch (OnUpdate).
// You can see that the method has a HarmonyPostfix attribute which means it will be called after the original method has finished executing.
// There are also HarmonyPrefix and HarmonyTranspiler attributes which do the same thing, but before or instead of the original method.
// You can learn more about Harmony patches here: https://harmony.pardeike.net/articles/patching.html
[HarmonyPatch(typeof(SpawnTeamSystem_OnPersistenceLoad), nameof(SpawnTeamSystem_OnPersistenceLoad.OnUpdate))]
public static class InitializationPatch
{
    [HarmonyPostfix]
    public static void OneShot_AfterLoad_InitializationPatch()
    {
        Core.Initialize();
        Plugin.Harmony.Unpatch(typeof(SpawnTeamSystem_OnPersistenceLoad).GetMethod("OnUpdate"), typeof(InitializationPatch).GetMethod("OneShot_AfterLoad_InitializationPatch"));
    }
}