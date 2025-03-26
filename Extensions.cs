using System;
using System.IO;
using System.Runtime.InteropServices;
using Il2CppInterop.Runtime;
using ProjectM;
using ProjectM.Shared;
using Stunlock.Core;
using Unity.Entities;

namespace ChangeName;
// HOW-TO: You will always see an Extensions or ECSExtensions class in V Rising mods. 
// These are the methods that let us access, read, and write data to Unity's Entity Component System objects (Entities).
// You'll see some variations of this file both with more and less methods in it based on the developer's needs / preferences. 
//#pragma warning disable CS8500
public static class Extensions
{
    /// <summary>
    /// Writes component data to an entity using raw memory operations.
    /// </summary>
    /// <typeparam name="T">The type of component data to write.</typeparam>
    /// <param name="entity">The target entity to write the component data to</param>
    /// <param name="componentData">The component data to write</param>
    /// <remarks>
    /// This method uses unsafe code to directly manipulate memory for efficient component data writing.
    /// The component data is marshaled to a byte array before being written to the entity.
    /// </remarks>
    /// <example>
    /// entity.Write(new MyComponentData { value = 42 });
    /// </example>
    public unsafe static void Write<T>(this Entity entity, T componentData) where T : struct
    {
        // Get the ComponentType for T
        var ct = new ComponentType(Il2CppType.Of<T>());

        // Marshal the component data to a byte array
        byte[] byteArray = StructureToByteArray(componentData);

        // Get the size of T
        int size = Marshal.SizeOf<T>();

        // Create a pointer to the byte array
        fixed (byte* p = byteArray)
        {
            // Set the component data
            Core.EntityManager.SetComponentDataRaw(entity, ct.TypeIndex, p, size);
        }
    }

    /// <summary>
    /// Reads component data from an entity using raw memory operations.
    /// </summary>
    /// <typeparam name="T">The type of component data to read. Must be a struct.</typeparam>
    /// <param name="entity">The target entity to read the component data from</param>
    /// <returns>The component data of type T read from the entity</returns>
    /// <remarks>
    /// This method uses unsafe code to directly access memory for efficient component data reading.
    /// The raw component data is marshaled from memory into the specified struct type.
    /// </remarks>
    /// <example>
    /// var data = entity.Read<MyComponentData>();
    /// </example>
    public static byte[] StructureToByteArray<T>(T structure) where T : struct
    {
        int size = Marshal.SizeOf(structure);
        byte[] byteArray = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);

        Marshal.StructureToPtr(structure, ptr, true);
        Marshal.Copy(ptr, byteArray, 0, size);
        Marshal.FreeHGlobal(ptr);

        return byteArray;
    }

    /// <summary>
    /// Reads component data from an entity using raw memory operations.
    /// </summary>
    /// <typeparam name="T">The type of component data to read. Must be a struct.</typeparam>
    /// <param name="entity">The target entity to read the component data from</param>
    /// <returns>The component data of type T read from the entity</returns>
    /// <remarks>
    /// This method uses unsafe code to directly access memory for efficient component data reading.
    /// The raw component data is marshaled from memory into the specified struct type.
    /// </remarks>
    /// <example>
    /// var data = entity.Read<MyComponentData>();
    /// </example>
    public unsafe static T Read<T>(this Entity entity) where T : struct
    {
        // Get the ComponentType for T
        var ct = new ComponentType(Il2CppType.Of<T>());

        // Get a pointer to the raw component data
        void* rawPointer = Core.EntityManager.GetComponentDataRawRO(entity, ct.TypeIndex);

        // Marshal the raw data to a T struct
        T componentData = Marshal.PtrToStructure<T>(new IntPtr(rawPointer));

        return componentData;
    }

    /// <summary>
    /// Gets a DynamicBuffer of components from an entity.
    /// </summary>
    /// <typeparam name="T">The type of buffer elements to read. Must be a struct.</typeparam>
    /// <param name="entity">The target entity to read the buffer from</param>
    /// <returns>A DynamicBuffer containing elements of type T</returns>
    /// <remarks>
    /// DynamicBuffers are useful for storing variable-length arrays of components on an entity.
    /// </remarks>
    /// <example>
    /// var buffer = entity.ReadBuffer<BufferElement>();
    /// foreach(var element in buffer) { /* Process element */ }
    /// </example>
    public static DynamicBuffer<T> ReadBuffer<T>(this Entity entity) where T : struct
    {
        return Core.Server.EntityManager.GetBuffer<T>(entity);
    }

    /// <summary>
    /// Checks if an entity has a specific component type.
    /// </summary>
    /// <typeparam name="T">The type of component to check for</typeparam>
    /// <param name="entity">The entity to check</param>
    /// <returns>True if the entity has the component, false otherwise</returns>
    /// <example>
    /// if (entity.Has<HealthComponent>())
    /// {
    ///     // Process entity with health component
    /// }
    /// </example>
    public static bool Has<T>(this Entity entity)
    {
        var ct = new ComponentType(Il2CppType.Of<T>());
        return Core.EntityManager.HasComponent(entity, ct);
    }

    /// <summary>
    /// Converts a PrefabGUID to its corresponding name string from the game's prefab collection.
    /// </summary>
    /// <param name="prefabGuid">The PrefabGUID to look up</param>
    /// <returns>A string containing the prefab name and GUID, or "GUID Not Found" if the prefab doesn't exist</returns>
    /// <remarks>
    /// Uses the PrefabCollectionSystem to retrieve human-readable names for game objects.
    /// The returned string includes both the name and the GUID for complete identification.
    /// </remarks>
    /// <example>
    /// string itemName = someGuid.LookupName(); // Returns "Sword 12345" or "GUID Not Found"
    /// </example>
    public static string LookupName(this PrefabGUID prefabGuid)
    {
        var prefabCollectionSystem = Core.Server.GetExistingSystemManaged<PrefabCollectionSystem>();
        return (prefabCollectionSystem.PrefabGuidToNameDictionary.ContainsKey(prefabGuid)
            ? prefabCollectionSystem.PrefabGuidToNameDictionary[prefabGuid] + " " + prefabGuid : "GUID Not Found").ToString();
    }

    /// <summary>
    /// Adds a component of type T to the specified entity.
    /// </summary>
    /// <typeparam name="T">The type of component to add</typeparam>
    /// <param name="entity">The entity to add the component to</param>
    /// <remarks>
    /// Uses the EntityManager to add a new component instance to the entity.
    /// The component will be initialized with default values.
    /// </remarks>
    /// <example>
    /// entity.Add<HealthComponent>();
    /// </example>
    public static void Add<T>(this Entity entity)
    {
        var ct = new ComponentType(Il2CppType.Of<T>());
        Core.EntityManager.AddComponent(entity, ct);
    }

    /// <summary>
    /// Removes a component of type T from the specified entity.
    /// </summary>
    /// <typeparam name="T">The type of component to remove</typeparam>
    /// <param name="entity">The entity to remove the component from</param>
    /// <remarks>
    /// Uses the EntityManager to remove an existing component from the entity.
    /// Any data stored in the component will be lost upon removal.
    /// </remarks>
    /// <example>
    /// entity.Remove<HealthComponent>();
    /// </example>
    public static void Remove<T>(this Entity entity)
    {
        var ct = new ComponentType(Il2CppType.Of<T>());
        Core.EntityManager.RemoveComponent(entity, ct);
    }

    /// <summary>
    /// Attempts to get a component of type T from the entity.
    /// </summary>
    /// <typeparam name="T">The type of component to retrieve. Must be a struct.</typeparam>
    /// <param name="entity">The entity to get the component from</param>
    /// <param name="componentData">When this method returns, contains the component data if found, or the default value if not found</param>
    /// <returns>True if the component was found and retrieved, false otherwise</returns>
    /// <remarks>
    /// This method provides a safe way to retrieve components without throwing exceptions.
    /// </remarks>
    /// <example>
    /// if (entity.TryGetComponent&lt;HealthComponent&gt;(out var health))
    /// {
    ///     // Work with health component
    /// }
    /// </example>
    public static bool TryGetComponent<T>(this Entity entity, out T componentData) where T : struct
    {
        componentData = default;
        if (entity.Has<T>())
        {
            componentData = entity.Read<T>();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Dumps all component information of an entity to a specified file for debugging purposes.
    /// </summary>
    /// <param name="entity">The entity to dump information from</param>
    /// <param name="filePath">The file path where the dump will be written</param>
    /// <remarks>
    /// The dump includes a list of all components attached to the entity and their detailed information.
    /// The output is appended to the specified file with clear section separators.
    /// </remarks>
    /// <example>
    /// someEntity.Dump("C:/debug/entity_dump.txt");
    /// </example>
    public static void Dump(this Entity entity, string filePath)
    {
        File.AppendAllText(filePath, $"--------------------------------------------------" + Environment.NewLine);
        File.AppendAllText(filePath, $"Dumping components of {entity.ToString()}:" + Environment.NewLine);
        foreach (var componentType in Core.Server.EntityManager.GetComponentTypes(entity))
        { File.AppendAllText(filePath, $"{componentType.ToString()}" + Environment.NewLine); }
        File.AppendAllText(filePath, $"--------------------------------------------------" + Environment.NewLine);
        File.AppendAllText(filePath, DumpEntity(entity));
    }

    private static string DumpEntity(Entity entity, bool fullDump = true)
    {
        var sb = new Il2CppSystem.Text.StringBuilder();
        EntityDebuggingUtility.DumpEntity(Core.Server, entity, fullDump, sb);
        return sb.ToString();
    }

    /// <summary>
    /// Destroys an entity with proper cleanup and logging.
    /// </summary>
    /// <param name="entity">The entity to destroy</param>
    /// <remarks>
    /// This method follows the recommended destruction pattern:
    /// 1. Disables the entity
    /// 2. Creates a destroy event for tracking
    /// 3. Performs the actual entity destruction
    /// </remarks>
    /// <example>
    /// someEntity.DestroyWithReason();
    /// </example>
    public static void DestroyWithReason(this Entity entity)
    {
        Core.EntityManager.AddComponent<Disabled>(entity);
        DestroyUtility.CreateDestroyEvent(Core.EntityManager, entity, DestroyReason.Default, DestroyDebugReason.ByScript);
        DestroyUtility.Destroy(Core.EntityManager, entity);
    }

    /// <summary>
    /// Checks if an entity currently exists in the game world.
    /// </summary>
    /// <param name="entity">The entity to check</param>
    /// <returns>True if the entity exists, false if it has been destroyed or is invalid</returns>
    /// <example>
    /// if (entity.Exists())
    /// {
    ///     // Perform operations on the existing entity
    /// }
    /// </example>
    public static bool Exists(this Entity entity)
    {
        return Core.EntityManager.Exists(entity);
    }

    /// <summary>
    /// Checks if an entity is currently disabled.
    /// </summary>
    /// <param name="entity">The entity to check</param>
    /// <returns>True if the entity has the Disabled component, false otherwise</returns>
    /// <example>
    /// if (entity.Disabled())
    /// {
    ///     // Handle disabled entity logic
    /// }
    /// </example>
    public static bool Disabled(this Entity entity)
    {
        return entity.Has<Disabled>();
    }
}
//#pragma warning restore CS8500