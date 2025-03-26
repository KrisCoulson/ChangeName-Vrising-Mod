using Bloodstone.API;
using ProjectM;
using Stunlock.Core;
using Unity.Entities;
using Unity.Mathematics;


namespace ChangeName.Services {
    public static class InventoryTransactionManager {
        public static readonly EntityManager entityManager = VWorld.Server.EntityManager;
        public static bool HasSufficientItemCount(Entity playerCharacter, PrefabGUID itemGuid, int requiredCount) {

            // Get the player's inventory entity.
            if(!InventoryUtilities.TryGetInventoryEntity(entityManager, playerCharacter, out var inventory)) {
                return false;
            }

            // Ensure the inventory entity has an InventoryBuffer.
            if(!entityManager.HasComponent<InventoryBuffer>(inventory)) {
                return false;
            }

            // Sum up the total count of the specified item.
            var buffer = entityManager.GetBuffer<InventoryBuffer>(inventory);
            int count = 0;
            foreach(var slot in buffer) {
                if(slot.ItemType == itemGuid) {
                    count += slot.Amount;
                    if(count >= requiredCount) {
                        return true;
                    }
                }
            }

            return count >= requiredCount;
        }

        public static bool TryRemoveItems(Entity playerCharacter, PrefabGUID itemGuid, int countToRemove) {
            if(countToRemove <= 0) {
                return true; // It's freeeeeeeee.
            }

            // Get the player's inventory entity.
            if(!InventoryUtilities.TryGetInventoryEntity(entityManager, playerCharacter, out var inventory)) {
                return false;
            }

            // Ensure the inventory entity has an InventoryBuffer.
            if(!entityManager.HasComponent<InventoryBuffer>(inventory)) {
                return false;
            }

            var buffer = entityManager.GetBuffer<InventoryBuffer>(inventory);
            int remainingToRemove = countToRemove;

            // Loop through each slot and remove items as needed.
            for(int i = 0; i < buffer.Length && remainingToRemove > 0; i++) {
                var slot = buffer[i];
                if(slot.ItemType == itemGuid && slot.Amount > 0) {
                    int toRemove = math.min(slot.Amount, remainingToRemove);
                    InventoryUtilitiesServer.TryRemoveItemAtIndex(entityManager, playerCharacter, slot.ItemType, toRemove, i, false);
                    remainingToRemove -= toRemove;
                }
            }

            // Return true only if the required number of items were successfully removed.
            return remainingToRemove <= 0;
        }
    }

}
