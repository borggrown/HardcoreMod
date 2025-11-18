# Hardcore Mode for Aviassembly

A BepInEx mod that adds Expert and Hardcore difficulty modes to Aviassembly, providing extra challenge for experienced players.

## Features

This mod introduces two new difficulty levels beyond the standard Hard mode:

### Expert Mode
- **No Partial Deliveries**: You must deliver the exact amount requested - no more accepting contracts with only partial cargo
- **No Mid-Flight Resets**: Cannot reset to last airport or reload while your plane is in motion

### Hardcore Mode
All Expert Mode features, plus:
- **Permadeath**: When your plane explodes, your save file is no longer playable in with this mod
- **Single Save File**: The "Save As" option is disabled - you cannot create backup save files

## Installation

### Prerequisites
**BepInEx 5.x** must be installed in your Aviassembly game directory
   - Download from [BepInEx releases](https://github.com/BepInEx/BepInEx/releases)
   - Extract to your Aviassembly installation folder
   - Run the game once to generate BepInEx folders

### Installing the Mod
0. **Aviassembly must be on the _experimental_ branch**
1. Download the latest release of HardcoreMode
2. Navigate to your Aviassembly installation folder, and find the BepInEx directory:
   ```
   Steam: steamapps/common/Aviassembly/BepInEx/
   ```
4. Copy the `plugins` folder into the `BepInEx` directory
5. Launch the game - the mod will load automatically

The mod files should be located at:
```
Aviassembly/BepInEx/plugins/HardcoreMode/HardcoreMode.dll
Aviassembly/BepInEx/plugins/HardcoreMode/assets/uibundle
```


## Usage

1. Launch Aviassembly with the mod installed
2. Clicking "New Game" will show the new difficulty options: Expert and Hardcore
  - Hardcore must be played on a new save file.
  - If you want to apply the restricted rules to an existing save file, you can lower the minimum difficulty to "Normal" in the config file.

## Configuration

After running the game with the mod installed for the first time, a configuration file will be automatically created at:
```
Aviassembly/BepInEx/config/com.borggrown.plugins.hardcoremode.cfg
```

### Configuration Options

Open the config file in any text editor to customize the following settings:

```ini
[GameRules]

## The minimum difficulty required to stop accepting partial deliveries in the game.
# Setting type: MyDifficulty
# Default value: Expert
# Acceptable values: Relaxed, Normal, Expert, Hardcore
OnlyWholeDeliveriesMinDifficulty = Expert

## The minimum difficulty required to prevent resets while mid-air.
# Setting type: MyDifficulty
# Default value: Expert
# Acceptable values: Relaxed, Normal, Expert, Hardcore
PreventResetsInMotionMinDifficulty = Expert

[Hardcore]

## Whether or not to show the red banner while playing hardcore saves
# Setting type: Boolean
# Default value: true
DisplayBanner = true

[MainMenu.Misc]

## Enabling this stops the main menu camera from moving
# Setting type: Boolean
# Default value: false
Remove Camera sway = false
```

## Warnings

- **Hardcore Mode is permanent**: Once you die in Hardcore mode, the only way to continue the save is to disable the mod.



## Troubleshooting

### Mod not loading
- Verify BepInEx is correctly installed
- Check `BepInEx/LogOutput.log` for errors
- Ensure the mod DLL is in `BepInEx/plugins/HardcoreMode/`

## License

This mod is provided as-is for use with Aviassembly.

## Credits

Created by borggrown

