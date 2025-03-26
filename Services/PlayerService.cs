using Il2CppInterop.Runtime;
using Il2CppSystem;
using ProjectM;
using ProjectM.Network;
using Stunlock.Core;
using Unity.Collections;
using Unity.Entities;


namespace ChangeName.Services;


internal static class Query {
    private static Type GetType<T>() => Il2CppType.Of<T>();

    public static unsafe T GetComponentDataAOT<T>(this EntityManager entityManager, Entity entity) where T : unmanaged {
        var type = TypeManager.GetTypeIndex(GetType<T>());
        var result = (T*)entityManager.GetComponentDataRawRW(entity, type);

        return *result;
    }
}

internal class PlayerService {
    // So we need to get A World organizes entities into isolated groups. A world owns both an EntityManager and a set of Systems.
    internal static bool ChangeName(Entity userEntity, Entity characterEntity, FixedString64Bytes newName) {
        var entityManager = Core.EntityManager;
        var userData = entityManager.GetComponentData<User>(userEntity);
        var characterData = entityManager.GetComponentData<PlayerCharacter>(characterEntity);


        var debugEventSystem = Core.Server.GetExistingSystemManaged<DebugEventsSystem>();
        var networkId = entityManager.GetComponentData<NetworkId>(userEntity);
        var renameEvent = new RenameUserDebugEvent {
            NewName = newName,
            Target = networkId
        };

        var fromCharacter = new FromCharacter {
            User = userEntity,
            Character = characterEntity
        };


        debugEventSystem.RenameUser(fromCharacter, renameEvent);
        UpdateIcon(characterEntity);
        return true;
    }

    public static void UpdateIcon(Entity requestingPlayerEntity) {
        PrefabGUID playerMapIcon = new(-892362184);

        // Query for all map icons
        EntityQuery mapIconDataQuery = Core.EntityManager.CreateEntityQuery(new EntityQueryDesc() {
            All = new ComponentType[]
            {
            ComponentType.ReadOnly<MapIconData>()
            },
            Options = EntityQueryOptions.IncludeDisabled
        });

        var mapIconDataObjects = mapIconDataQuery.ToEntityArray(Allocator.Temp);

        foreach(var mapIconObject in mapIconDataObjects) {
            if(Core.EntityManager.HasComponent<PrefabGUID>(mapIconObject) &&
                Core.EntityManager.GetComponentData<PrefabGUID>(mapIconObject).Equals(playerMapIcon)) {
                if(Core.EntityManager.HasComponent<Attach>(mapIconObject)) {
                    Attach attachComponent = Core.EntityManager.GetComponentData<Attach>(mapIconObject);

                    // Ensure we are updating only the requesting player's icon
                    if(attachComponent.Parent == requestingPlayerEntity &&
                        Core.EntityManager.HasComponent<PlayerCharacter>(requestingPlayerEntity) &&
                        Core.EntityManager.HasComponent<PlayerMapIcon>(mapIconObject)) {
                        PlayerCharacter parentPlayerCharacter = Core.EntityManager.GetComponentData<PlayerCharacter>(requestingPlayerEntity);
                        PlayerMapIcon iconObject = Core.EntityManager.GetComponentData<PlayerMapIcon>(mapIconObject);

                        if(!iconObject.UserName.Equals(parentPlayerCharacter.Name)) {
                            // Update the existing PlayerMapIcon with the new username
                            iconObject.UserName = parentPlayerCharacter.Name;

                            // Write back the updated PlayerMapIcon component
                            Core.EntityManager.SetComponentData(mapIconObject, iconObject);

                            Plugin.LogInstance.LogMessage($"Synced username to {parentPlayerCharacter.Name.ToString()}'s map icon.");
                        }
                        break; // Exit early since we only need to update one icon
                    }
                }
            }
        }

        mapIconDataObjects.Dispose();
    }

    public static NativeArray<Entity> GetEntitiesByComponentType<T1>(bool includeAll = false, bool includeDisabled = false, bool includeSpawn = false, bool includePrefab = false, bool includeDestroyed = false) {
        EntityQueryOptions options = EntityQueryOptions.Default;
        if(includeAll)
            options |= EntityQueryOptions.IncludeAll;
        if(includeDisabled)
            options |= EntityQueryOptions.IncludeDisabled;
        if(includeSpawn)
            options |= EntityQueryOptions.IncludeSpawnTag;
        if(includePrefab)
            options |= EntityQueryOptions.IncludePrefab;
        if(includeDestroyed)
            options |= EntityQueryOptions.IncludeDestroyTag;

        EntityQueryDesc queryDesc = new() {
            All = new ComponentType[] { new(Il2CppType.Of<T1>(), ComponentType.AccessMode.ReadWrite) },
            Options = options
        };

        var query = Core.EntityManager.CreateEntityQuery(queryDesc);

        var entities = query.ToEntityArray(Allocator.Temp);
        return entities;
    }
}