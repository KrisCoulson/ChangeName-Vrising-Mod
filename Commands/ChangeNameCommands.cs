using ChangeName.Services;
using ChangeName.Structs;
using ProjectM.Network;
using Stunlock.Core;
using System.Text.RegularExpressions;
using Unity.Collections;
using VampireCommandFramework;



namespace ChangeName.Commands;



internal static class ChangeNameCommands {
    [Command(name: "changename", shortHand: "cn", description: "Change your characters name.", adminOnly: false)]
    public static void Rename(ChatCommandContext ctx, NewName newName) {
        var playerCharacter = ctx.Event.SenderCharacterEntity;
        var playerUser = ctx.Event.SenderUserEntity;
        var currencyName = Settings.CurrencyName.Value;
        var currencyCost = Settings.CurrencyCost.Value;
        var requiredCurrencyGUID = new PrefabGUID(Settings.RequiredCurrencyGUID.Value);

        if(!ctx.User.IsAdmin) {
            if(!InventoryTransactionManager.HasSufficientItemCount(playerCharacter, requiredCurrencyGUID, currencyCost)) {
                ctx.Reply($" You need {currencyCost} {currencyName} to change your name.".Color("red"));
                return;
            }

            if(!InventoryTransactionManager.TryRemoveItems(playerCharacter, requiredCurrencyGUID, currencyCost)) {
                ctx.Reply("Failed to remove the required items from your inventory.".Color("red"));
                return;
            }
        }

        var user = Core.EntityManager.GetComponentData<User>(playerUser);

        PlayerService.ChangeName(playerUser, playerCharacter, newName.Name);

        ctx.Reply($"You've changed your name from {user.CharacterName.ToString().Color("#FF0000")} to {newName.Name.ToString().Color("#00FF00").Bold().Medium()}".Color("#eb902b"));

        return;
    }
    public record struct NewName(FixedString64Bytes Name);

    public class NewNameConverter : CommandArgumentConverter<NewName> {
        public override NewName Parse(ICommandContext ctx, string input) {
            if(!IsAlphaNumeric(input)) {
                throw ctx.Error("Name must be alphanumeric.");
            }
            var newName = new NewName(input);
            if(newName.Name.utf8LengthInBytes > 20) {
                throw ctx.Error("Name too long.");
            }

            var userEntities = PlayerService.GetEntitiesByComponentType<User>();
            var lowerName = input.ToLowerInvariant();
            foreach(var userEntity in userEntities) {
                var user = userEntity.Read<User>();
                if(user.CharacterName.ToString().ToLowerInvariant().Equals(lowerName)) {
                    throw ctx.Error("Name already in use.");
                }
            }
            userEntities.Dispose();

            return newName;
        }
        public static bool IsAlphaNumeric(string input) {
            return Regex.IsMatch(input, @"^[a-zA-Z0-9\[\]]+$");
        }
    }
}
