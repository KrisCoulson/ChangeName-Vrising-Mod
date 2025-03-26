using ChangeName.Services;
using System;
using System.Linq;
using Unity.Entities;

namespace ChangeName;

// HOW-TO: Essentially, this is where you can initialize all of your mod's services.
// This class is also initialized after the game has loaded, ensuring anything you need to retrieve is available.
// Credits: The "Core" pattern as far as I know is from deca's CommunityCommands mod (outdated; see: KindredCommands)
internal static class Core {
    // You will almost always want to have references to the Server world and EntityManager for any mod.
    public static World Server { get; } = GetServerWorld() ?? throw new Exception("There is no Server world (yet)...");
    public static EntityManager EntityManager => Server.EntityManager;

    // In this example, our mod requires a PlayerService, so we define it here and initialize it in the Initialize method.
    public static PlayerService PlayerService { get; internal set; }

    public static bool hasInitialized = false;

    // Here is where you would initialize your different services. We will call this method from one of the game's initialization functions with a Harmony Patch.
    public static void Initialize() {
        if(hasInitialized)
            return;

        PlayerService = new PlayerService();
        hasInitialized = true;
    }

    static World GetServerWorld() {
        return World.s_AllWorlds.ToArray().FirstOrDefault(world => world.Name == "Server");
    }
}