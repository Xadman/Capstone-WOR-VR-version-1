Thanks for purchasing SLOTS CREATOR PRO!

Slots Creator Pro is the premiere slot creator on the Asset store with over 550 sold since it was released in 2014!

Email: slotscreatorpro@gmail.com
Discord: https://discord.gg/DUBFDxVZHu (preferred method of communication for questions or just to hang out!)

Docs:
https://docs.google.com/document/d/1gUCZFV8hVNaUsmWiJZASEWzPSBs5Hae8bYPVUw0gNKk/edit
API (WIP):
Open the file found in Docs/html/annotated.html

Special thanks to DoTween and it's creator:
https://www.assetstore.unity3d.com/en/#!/content/27676

Changelog:

V1.15 (x/x/x)

- Added support for "freezing" positions like is seen in some popular slots like Lightning Link.  When specified, the same symbol(s) will continue
to land in the specified spot(s) for consecutive turns until you tell it not anymore.
    - slot.frozenPositions - 2D array for specifying frozen positions (-1 is not frozen) using the symbol index
    - slot.resetFrozenPositions() - resets frozen position array to all -1
    - slot.getResultsArray() - returns a 2D array of the current turns symbol indexes that can be compared against to identify symbols for freezing
- Added support for new Enum SetType "anywhere" which allows matches to be made on winlines without the restriction of being left to right in order
- Added a check that ensures that wilds do not recognize wins when there isn't at least 1 of the actual symbol as a part of it
- Added "Explicit Order" checkbox to Symbol Sets.  This will enable symbol sets to require explicit order according to the symbol order in the set.
    - Quite simply, if you have symbol A, B and C in the symbol set, with this checked, those symbols will need to land in that order to count as a win.  
    - You can still have pays on all 3 symbols, but the first pay will be for just A, second pay A + B, third pay A + B + C.
    - Will work with wilds if checked
    - Will only work with Normal Symbol Set (SCP will automatically make sure of that)
-Added support to specify the start up symbols of your slot
    - set suppliedResult array
    - set slot.useSuppliedResult to true
    - Example can be seen in the BeachDaysCallbacks.cs Awake method
-Some minor bug fixes and code cleanup

V1.14 (8/24/2020)

- Migrated to 2019
- Fixed issue with rotation of slot always being reset to 0
- Fixed issue with reel spinning integrity failing when rotating a slot

V1.13 (11/19/2019)

- Migrated to 2018
- Updated a lot of old code and comments
- Updated contact information etc.

V1.12 (6/19/2016)

- Fixed bug with linked symbol validation
- Fixed bug with spinWithResult and machine's with height buffer > 1/2014
- Upgraded DOTween version to latest
- Fixed UI bugs in Beach Days example
- Fixed some spinning problems due to new version of DOTween
- Updated compatability to latest version of Unity!


V1.11 (6/9/2015)

-Adding "Linked Symbol" (beta) functionality which makes it possible to have consecutive symbols which will always appear in an explicit pre-defined order.
-Added entire scene example game called "Beach Days" which uses new uGUI for all buttons and text, has Mecanim animated symbols and more.
-Setting for callback script has been removed.  You must now manually attach whatever you used for a callback script to the GameObject of your slot.
-Fixed bug with linebox clamping and scatter type wins when the number of scatters was greater than the number of reels.
-Now the entire slot component will be tinted red when there is an issue with your slot.
-Updated math calculations to take in account linked symbols (beta).

V1.10 (3/31/2015)

-Unity5 compatibility fix

V1.09 (3/16/2015)

-Unity5 compatibility update

V1.08 (12/2/2014)

-Switched from HOTween to newer DOTween for all tweening functionality
-Added currentWin to refs.credits which is reset as soon as the next spin is started, unlike lastWin which remains until the next win is awarded.
-Added ability to specify a winbox for each each symbol
-Added ability to clamp display of line boxes on the right-hand side
-Added decLinesPlayed to compliment incLinesPlayed in the in the SlotCredits class
-Added OnDecrementLinesPlayed callback
-Added ability to specify reel and symbol padding as percentages of the symbol width and height
-Added example for spinning with a new 4.6 uGUI button (spawning the slot from a prefab)
-Some bug fixes

V1.07 (8/7/2014)

-Added SlotSpinWin structure, returned to OnAllWinsComputed callback
-Added prefab option for lineboxPrefab, which is a prefab that would be used for non-contributing-to-win symbols in the Misc section of settings.  The SimpleExample has been updated to illustrate it's usage
-Fixed bug with OnIncrementBetPerLine and OnDecrementBetPerLine
-Fixed some minor issue with the SlotCompute component
-Cleaned up the SlotCredits component
-Added option to specify -1 to spin(int[,]) which will resort to regular random symbol selection so you can specify some explicit and some random symbols in a spin.
-Updated Anticipation to scatter to spin the reel faster as well as longer
-Updated counting of wins to use an intermediate queue, so wins actually "count" up.  Updated SlotSimpleGUI to reflect that.

V1.06

-Minor updates

V1.05

-More compatibility issues identified and fixed for Unity 4.5
-Fixed persistence bugs with slot symbol settings being lost upon Unity startup (like clamp and wild) when not using a prefab.
-Added a reelCenter Transform to Misc settings.  The reel basket will be centered on this transfer.  If none is specified, it will use the transform of the Slot gameobject itself. (which is what it currently does)

V1.04 (6/1/2014)

-Added wasAnticipating to SlotScatterHitData which is passed to various Callbacks and indicates whether the scatter symbol hitting was anticipating.
-Added freeSpin bool parameter to both spin() and spinWithResult().  If true, the bet amount will not be deducted for that spin.  The default is false.
-Added decrementBetPerLine function to SlotCredits.cs and also added OnDecrementBetPerLine callback
-Fixed some compatibility issues with Unity 4.5

V1.03

-Added OnCompletedBonusCreditCountOff callback
-Added scatterCount parameter to OnScatterSymbolLanded which will now also return the total scatter count on the symbol's set that hit
-Added spin(int[,]) overload that takes an array of int that represents the symbol indexes for explicitly specifying the result of the spin.  This makes it easy to feed SCP results from another source.
-Added pauseWins and resumeWins to SlotWins class, which pauses and resumes the playing of wins.
-Fixed bug in up/down reorder symbol sets.
-Added totalIn and totalOut to SlotCredits
-Added awardBonus to SlotCredits which you can specify whether that amount is added to the totalIn or not - useful for when your game has other ways besides traditional ways to add credits, such as bonus games.
-Added isFirstLoop boolean to the OnLineWinDisplayed callback, useful if you want to do something only the first time a particular win is displayed (such as play a sound, or animation)
-Minor bug fixes

Note:  The OnLineWinDisplayed callback will need to be altered upon upgrading.  See above notes.

V1.02

-Changed the way you set up bets per line. Rather than just a linear max bet per line, you now specify each bet per line, and you can also control which are enabled and disable.
-Added Callback	OnIncreasedBetPerLine (int bet)
-Added Callback OnIncreasedLinesPlayed (int linesPlayed)
-Added Callback OnBeginDelayBetweenLineWinDisplayed (SlotWinData data)
-Added Callback OnScatterSymbolLanded - fired when scatter symbol is landing on a valid reel position; half way through the reel ease out tween.
-Callback OnBeginSpin now returns the last displayed Win, or null if there wasn't one.
-Added bet per line and lines played to Math compiler as a result of the bet changes made.  Now you can compute a return based on any number of lines and bets per line.
-Fixed a bug with calculating the math on scatter symbols that had a maximum total occurences set.
-Added winbox pooling which will automatically be enabled with Use Pool is true.
-Ability to reorder symbol sets pays (beta)
-Added callback OnReelStop(int)
-Added callback OnBeginCreditWinCountOff
-Added callback OnBeginCreditBonusCountOff
-Added callback OnCompletedCreditCountOff
-Fixed bugs

Note: Although this version is backwards compatible, because of the changes to the way you define betting, you will need to specify some new settings.  Default is just one bet of 1.

V1.01

-Few minor bug fixes

V1.0

Released!