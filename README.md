# ChangeName VRising Mod
<img src="https://github.com/user-attachments/assets/081eee78-2c61-4b8f-8d67-fba37bb6928b" width=400px height="400px">


## Description
Server Admins Rejoice! No more support tickets for name changes!

ChangeName is a mod for the game *V Rising* that allows players to change their character name without assistance from an admin.
Admins can setup a currency and price for the name change, and players can use the `.changename` or `.cn` command to change their name if the required amount of currency is in their inventory.

Note this mod does not do any type of filtering and does not prevent players for changing their name to something inappropriate.


For other great mods and support join the modding community at on discord by going to https://wiki.vrisingmods.com/

---

## Patch Notes (v1.0.0)
- **New `.changename (newName)` / `.cn (newName)` command**: Allows players to change their character name.


---

## Features
- **Player Command**: Players can change their character name.
- **Admin Configuration**: Set the price and currency for name changes.
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
The name of the currency, The currency Prefab ID, and Cost (Setting 0 makes it free) 

The default values are:
| Config Name | Default Value |
| --- | --- |
| Currency | Silver Coins |
| RequiredCurrencyGUID | -949672483 |
| CurrencyCost | 500 |

## Dependencies
- VampireCommandFramework
- Bloodstone

## Developer
Kris Coulson (Shadez).

## Shoutouts
inility (Darrean) - for inspiration and outlining the currency inventory system
Odjit - for the base work of character renaming (kinderd commands)
Helskog - for helping me run down Map player icons!

## License
This mod is free to use or modify.

## Disclaimer:
ChangeName is a third-party mod, not affiliated with the official V Rising development team. Use at your own risk.

