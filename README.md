Official Valheim mod release page: https://www.nexusmods.com/valheim/mods/8

SkToolbox - Valheim
AUTHOR: Skrip (https://steamcommunity.com/id/Skrip037/)

SkToolbox is a fully clientside utility meant for improving the gameplay experience, by enabling the player to modify their character or the environment to their liking. The mod enables features that could allow a player to drastically change their character or others.

It is recommended that you use discretion when using these commands in Multiplayer, as this will affect the gameplay experience of others.

:: REQUIREMENTS ::
BepInEx for Valheim
Unstripped UnityEngine, Core, IMGUI, InputLegacy, Physics, and UI modules.

:: FEATURES ::
• This mod works by extending the functionality of the console and chat commands. Most of the commands shown below are usable in both the console and via chat. Most commands work both via console or chat.
• The SkToolbox now allows customization of the look of the console. Changes have been made by default. See the configuration file for details.
• Please see NexusMods page for most up to date feature list.

:: HOW TO INSTALL ::
  Method 1
	• Simply open the zip file and extract all contents to the game's root directory (You'll see valheim.exe and UnityPlayer.dll in this folder).
After extraction, in the game root folder, you should see the Valheim.exe file, and the BepInEx folder in the same folder. (https://i.imgur.com/QKx9yxU.png)
	• Run the game and enjoy.
	
  Method 2	
	• Simply use your mod manager to download and manage the files.
	• Run the game and enjoy.

• I just need to update! What do I do?
	• Extract the .dll file to the plugins folder, run the game.

:: HOW TO USE ::
• Complete one of the two installation methods, then simply run the game and open the console.
	• Type help or /?
	• (/? also works as a chat command)

:: CHANGELOG ::
• Version 1.8.0
   ![NEW] Version Checker has been implemented. Toolbox will now alert when an update has been released.
   	[NEW] New configuration option added - OpenConsoleWithSlash
	[NEW] New configuration options added - ConsoleFont, ConsoleFontSize, ConsoleOutputTextColor, ConsoleSelectionColor, ConsoleCaretColor. These allow the customization of the look of the console. Console now set to new look settings by default.
	[NEW] New configuration options added - Alternate on-screen controls are now fully configurable!
• Version 1.7.0
   ![NEW] AutoRun commands and command chaining now work with any command, including the standard commands and most other custom mods!
	[NEW] New configuration options added - AllowPublicResponse, AllowExecuteOnClear, OpenChatWithSlash, AllowChatCommandInput
	[NEW] /clear - Clears the current output shown in the console
	[NEW] /q - Quickly exit the game from the console
	[CHG] Many behind the scene fixes and optimizations
	[CHG] Terrain modification radius' limits changed again based on new equations
   ![FIX] /seed is back and working!
	[FIX] The caret is now set properly on command scroll in console
	[FIX] /tp is now much more likely to properly calculate the height of the ground and place the player accordingly. Sometimes there is still a 'blocked' issue, but this now happens less often. The workaround is to fly up into the air before teleporting, so nothing can block your destination.
	[FIX] Chat now opens properly when an output from the Toolbox is sent to chat.
• Version 1.6.3
	[NEW] /tu added to world on-screen menu
	[NEW] /resetmap - Reset the map exploration
	[FIX] Terrain modification commands have been significantly optimized, with credit to BlueAmulet for this code. Thanks BlueAmulet!
• Version 1.6.2
	[FIX] /nores - You can now properly remove structures if they were built in a restricted area
• Version 1.6.1
	[CHG] On-Screen menu will no longer open if the console is already open
	[FIX] /farinteract with no range in auto run caused auto run to run twice
• Version 1.6.0
	[NEW] /nores - Toggle no restrictions to where you can build (except ward zones)
	[NEW] /tu [Radius=5] - Undo terrain modifications around you. Radius 40 max.
	[NEW] On-Screen World commands added!
	[NEW] /wind and /resetwind added!
	[CHG] Changes some command descriptions
	[FIX] Terrain commands had a radius actually stuck at 20. Changed this to 40
	[FIX] On-screen toggle text fixed
	[FIX] Fixed a console functionality issue where if you hit ctrl+A then backspace, it would execute the command.
• Version 1.5
    [NEW] Auto-run commands on server-join
    [NEW] Command chaining! For example: "/give Wood 5; /give PickaxeWood" would run both commands!
    [NEW] Command chaining works with auto-run!
	[FIX] Terrain Commands are now working really well, should no longer cause stuttering or game breaking issues!
    [CHG] Modified the position of the /detect window so it no longer covers the inventory
• Version 1.4
	[NEW] /nosup - Toggle building support requirements 
   ![NEW] Terrain Editor - /tr /td /tl - Raise, dig, and level terrain in a radius
	[NEW] Alternate controls now available for on-screen menus. /alt (then use Home, insert, page up, page down, and delete)
	[FIX] Down arrow now properly cycles the console commands as expected
	[FIX] Console no longer strangely executes commands too many times sometimes
• Version 1.3
  !![NEW] Modified the console so you can now press the up arrow to get as many previous commands as you've entered.
  !![NEW] On-Screen Controls!
	[NEW] Teleport to Mouse
	[NEW] /set difficulty [Player Count] - Set the difficulty (default is number of connected players)
	[NEW] /set exploreradius [Radius] - Set the explore radius (default = 100f)
	[NEW] /set jumpforce [Force] - Set jump force (default 10). Careful if you fall too far!
	[NEW] /set pickup [Range] - Auto pickup radius can now be set (default 2)
	[NEW] /set speed [Speed Type] [Speed] - Speed Types: crouch (def: 2), run (def: 120), swim (def: 2)
	[NEW] /clearinventory - Clears the player inventory
	[FIX] Infinite stamina no longer causes lag, and is actually infinite now.
	[CHG] /listitems no longer requires caps
	[FIX] Backend changes. Performance increasecis.
	[CHG] /? is now organized by pages. Ex. /? 1
	[REM] /seed removed until it is properly working
	[REM] /findtrader removed until it is properly working
	[REM] /itp removed until it is properly working
	[REM] /ttp removed until it is properly working
	[CHH] Changes to give command. It now can be used with different parameters: /give [Item] [Qty=1], OR /give [Item] [Qty=1] [Player] [Level=1]
• Version 1.2
	[UPD] Updated BepInEx Version
	[NEW] /listskills will list all possible skills
• Version 1.1
	[CHG] /detect now has a range variable. /detect 50 (/detect performs a default of 20)
	[NEW] /imacheater now enables standard in-game cheats on any server (for you only)
	[FIX] /heal [Player=local] now properly heals other plyers
	[NEW] /set skill [Skill] [Level]
	[NEW] /seed - Display the world seed
	[FIX] /coords now show correctly
	[FIX] /tp now works correctly
	[CHG] /? now has pages. /? 1 or /? 2
	[NEW] /stopevent has been added
	[REM] /imaxstacks has been removed until properly working

:: KNOWN ISSUES ::

:: CREDITS ::
♦ Mod authored by: Skrip
♦ ♦ ♦ https://steamcommunity.com/id/Skrip037/

♦ Thank you to the Harmony team for their patcher.
♦ ♦ https://harmony.pardeike.net 

♦ Thank you to the Doorstop team for their injector.
♦ ♦ https://github.com/NeighTools/UnityDoorstop

♦ Thank you to the BepInEx team for their injector.
♦ ♦ https://github.com/BepInEx/BepInEx

♦ Thank you to BlueAmulet for the optimized terrain modification code.
