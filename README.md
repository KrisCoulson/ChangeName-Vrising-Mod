"""
# ChangeName Mod - README


## Description
ChangeName is a mod for the game *V Rising* that allows players to change their character name without assistance from an admin.
Admins can setup a price for the name change, and players can use the `.changename` or `.cn` command to change their name. As long as they have the required amount of currency in their inventory.


For other great mods and support join the modding community at on discord by going to https://wiki.vrisingmods.com/

---

## Patch Notes (v1.0.0)
- **New `.changename (newName)` / `.cn (newName)` command**: Allows players to change their character name.


---

## Features
- **Player Command**: Players can update thier character name changes.
- **Admin Configuration**: Set the price for name changes.
- **Map Icon Updates**: Player names are updated on the map when they are in a clan.
- **Reloadable**: Simply place the mod in the Bloodstone plugins folder run `!reload`. No need to restart the server when updating settings.

---

## Commands
Prefix all commands with a period (`.`).

### `.changename (newName)` or `.cn (newName)`
**Description**: Change your character name.
- `Admins` → No cost to change name.
- `Players` → Admins can set a cost for name changes.



### Installation Instructions

Download the Mod Files:

Obtain the latest ChangeName.dll release from our official repository or release page.

#### Extract Files:

Place ChangeName.dll in your server’s BepInEx plugins directory
ChangeName is also reloadable. Instead of placing the mod in the plugins folder, you can place it in the Bloodstone plugins folder and run `!reload` from the chat box. All of the mods settings will be reloaded without shutting down the server.

#### Launch the Server:

Start your V Rising dedicated server. ChangeName will initialize automatically, creating any needed config files.

### Configuration:
##### The name of the currency to use.
###### Currency = Silver Coins

#####  The GUID of the required currency
#####  The GUID of the required currency
###### RequiredCurrencyGUID = -949672483

#####  The cost in currency for the upgrade. Setting to 0 makes it free
###### CurrencyCost = 500

## Dependencies
- VampireCommandFramework
- Bloodstone

## Developer
Kris Coulson (Shadez).

## License
This mod is free to use or modify.

## Disclaimer:
ChangeName is a third-party mod, not affiliated with the official V Rising development team. Use at your own risk.

