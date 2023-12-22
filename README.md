# PoryMoves


PoryMoves is a program that allows Pokémon decompilation developers to quickly and easily generate the moves that a Pokémon can learn, based on their learnsets across the different [core series games](https://bulbapedia.bulbagarden.net/wiki/Core_series). As of right now, this tool is designed for use with the Pokémon Generation 3 decompilation projects.


# Features

- Define TM/HM/TR Moves and Tutor Moves that are available for a given fangame.

- Define a game to be used for

  - Level Up Moves
  - TM/HM/TR Moves
  - Egg Moves
  - Tutor Moves

- Export files that be directly input into a decompilation fangame

# [Changelog](CHANGELOG.md)

## Why PoryMoves?

Rioluwott is working on a fangame built from [pokeemerald-expansion](https://github.com/rh-hideout/pokeemerald-expansion/). He wants:

- Pokémon use their RBY learnsets. For example, this means Charmander does not learn Metal Claw via Level Up.
- to use the Technical Machine list from Ultra Sun and Ultra Moon.
- To use the Gen 4 (HGSS, PT, DP) Egg Moves lists.
- No Tutor Moves at all.

This would normally require Rioluwott to compile:

- RB and Y learnsets for each Pokémon.
- USUM TM compatibility for each Pokémon.
- HGSS, PT, and DP Egg Moves for each Pokémon

And then compile and format all the data into the [correct format to be used by pokeemerald](https://github.com/pret/pokeemerald/tree/master/src/data/pokemon). Even with the help of macros, this entire process could take hours.

With PoryMoves, this can be done in 5 minutes!


# Installation

PoryMoves does not require any installation to one’s computer or fangame. Simply download the [latest release](https://github.com/AsparagusEduardo/PoryMoves/releases), unzip the resulting file, and run the `PoryMoves.exe` within.


## Compatibility

PoryMoves is only supported on devices running Windows 10 or Windows 11.


# Anatomy

<img src="https://lh4.googleusercontent.com/Fl3Rtk2TBDQusLdJ5E_TP8AQIPpwkqJ2RmersqbfdM0hQQmj32gHfxiV1g26wSf9tes9sW5Je9FAFMmB3BncNdJOVQz8v-Hj3jyE45YJh0e7MTRz8jcFbOwzaACGPh9K18jOoD0Ps3ymF442QoQZNCs" width=50% height=50%>

A. **Shows Folders: Opens the following folders in the user’s default file explorer:**

   1. **Lists where TM/HM/TR Moves and Tutor Moves are defined**
   2. Exported learnsets from PoryMoves.

B. **General Options**: Impacts all the different kinds of moves. Hover the mouse over the text to check the details of each option.

C. **Game Lists**: List of main series games that developers can use as a basis for their fangame

D. **Move Options**: Specific options for different kinds of moves

E. **Export Buttons**: Exports files for each type of Move.

F. **Export Mode**: Changes how the export is formatted, depending on what modifications the developer has previously made.

G. **Stacking Exports**: Allows developers to include other types of moves from a Pokémon in this specific type of learnset.


# Usage


## Define Input

Open PoryMoves. Click Open Input Folder, and open `TM.txt` and `tutor.txt`.

`TM.txt` takes a list of TMs for your fangame, using the TM constant found in [include/constants/items.h](https://github.com/pret/pokeemerald/blob/588100e4ab4c68f2bfa27913367dfd642faca4ad/include/constants/items.h#L384). Putting an `\*` in front of the line designates that TM as a [near-universal TM](https://bulbapedia.bulbagarden.net/wiki/TM#Near-universal_TMs).

In our example, Rioluwott would have to compile this list using an [external source, like Bulbapedia](https://bulbapedia.bulbagarden.net/wiki/List_of_TMs#Generation_VII), and then convert it to a list of TMs that matches the formatting of pokeemerald. Developers can [make a copy of this spreadsheet](https://docs.google.com/spreadsheets/d/1H-A380z62pdRBav_20cJXCW8KEkqqiB8clBxsEXzc0E/edit?usp=sharing) to be used for this process.

`tutor.txt` takes a list of moves that will be taught by [Move Tutors](https://bulbapedia.bulbagarden.net/wiki/Move_Tutor) over the course of a fangame. This list uses the constants found in [include/constants/moves.h](https://github.com/pret/pokeemerald/blob/588100e4ab4c68f2bfa27913367dfd642faca4ad/include/constants/moves.h#L4).


## General Options

There are two options here that impact how all moves are exported for a developer’s fangame.

**Use updated move defines (RHH)**: In [pokeemerald-expansion](https://github.com/rh-hideout/pokeemerald-expansion/), certain move defines were updated to match their current names, such as `VICE_GRIP` -> `VISE_GRIP` or `FAINT_ATTACK` -> `FEINT_ATTACK`. If you are using expansion, this box should be checked. Unchecking this box will use the default pokeemerald's labels.

**Mew can learn exclusive Tutor Moves**: In official Pokémon games, Mew is able to learn every TM, HM, TR and Tutor Move except those exclusive to a particular Pokémon or group of Pokémon, such as Draco Meteor and Secret Sword. When this setting is checked, Mew will be able to learn those exclusive moves.


### Specific Options


#### Include Pre-Evo Moves
In Generation 8+, moves learned by previous evolutions but not by future evolutions were prepended to the future evolution’s learnset at Level 1. This option adds this functionality regardless of Generation. One such example is Nidoking’s Horn Drill in Generation 3.

#### Use Latest Moveset vs Combine Movesets
The option "Use Latest Moveset" will export the latest moveset available of each Pokémon. For example, if Generation 1-9 are selected, Arctozolt will get the moveset from SwSh, which was the latest game it was available in.
The option "Combine Movesets(Avg)" will combine all movesets selected into a single one, averaging their levels. This averaging doesn't include evolution, pre-evo or level 1 moves, which are added at the beginning of the list. This was always active for versions 1.0.0-1.4.0.
The option "Combine Movesets(Max)" will also combine all movesets selected into a single one, with each move added on their highest level among the selected generations. This also doesn't include evolution, pre-evo or level 1 moves, which are added at the beginning of the list.

#### Align moves to name

<img src="https://lh6.googleusercontent.com/LJcXV2wu6GzagJ5r71p1TVJsFdMAVLJr7C0uXiRXSIwhHRuugIOJK5EEOIbXDv5y-7OfN9nK6IXjzUhMGIMVNPsSh2GZrfCZkmrhXOi9AtY1Zc-k1_fNGOV6HeDXtJqzWPbYx0giUnPtVbdBTGFpViU" width=50% height=50%>

This adjusts the spacing in `egg_moves.h` to be vertically aligned to the species in question or not. This does not impact the fangame in any way and is purely for the developer's quality of life.


## Moves

Developers choose which game learnsets should be used for their fangame. Each game with a unique set of learnsets for this type of move has a separate checkbox.

![](https://lh4.googleusercontent.com/B2TEfUSUG2GBwJVJC69epLpPpOVeqCUMyzlrXVKlLYsKQ7_iU6obSrFnTaQDNhDemV7knj3p64Z61fOIUafcU6Lk0neG9eSZL1Fo1b7YRWyeKaWmQxauwrixeCk4UmdpXORXAa2VrimQVPw3iq0QVII)

In our example, Rioluwott’s configuration is shown above.


## Export

Once the selected games have been chosen for each Move type, click the Export button at the bottom of each row. This will generate a list that can be seen by clicking on "Open output folder".


### Export Modes

For TM/HM/TR moves and Tutor Moves, there are three different modes of exporting.

- **Vanilla Mode**: The developer is using pokeemerald, and has not made any changes to the formatting or method by which `src/data/pokemon/tmhm_learnsets.h` is read. If you are not sure which option to use, use this one.
- **SBird’s TM Refactor**: The developer has merged [Sbird’s TM Refactor](https://github.com/LOuroboros/pokeemerald/tree/sbirds_tmhm_system) into pokeemerald.
- **SBird’s Tutor Refactor**: The developer has merged [Sbird’s Tutor Refactor](https://github.com/AsparagusEduardo/pokeemerald/commit/42eb0f8dc225062ccb9b08980b568152e82a2757) into pokeemerald.
- **RHH 1.0.0**: The developer is using pokeemerald-expansion.


## Stacking Exports

Checking the boxes Include X Moves will include the moves from X into the export for the row that is being configured.

For example, Pikachu can learn Surf via Move Tutor, but not via TM/HM/TR. If "Include Tutor Moves" is checked under TM/HM/TR moves, then Pikachu will be able to learn Surf via either method.


## Incorporate Into ROM

Click on "Open output folder". Copy the files that show in the folder there, and paste them into the fangame’s `src/data/pokemon/` folder. Like all changes, the game will need to be compiled again, but then the learnsets will be updated! If changes are desired, open PoryMoves, make changes, export again, and paste the files again.


# FAQ and Support


## Why didn’t my TMs update?

PoryMoves generates a file that says

> Charmander can learn Psycho Boost via TM

,but does not handle 

> change TM 38 to be Psycho Boost

. This needs to be done manually, following [these instructions from Lunos](https://www.pokecommunity.com/showpost.php?p=10140674&postcount=60).


# Donations

If you got some use out of this feature, please consider donating. We are currently not taking any donations, so please donate to some of our favorite charities.

- [Direct Relief](https://www.charitynavigator.org/ein/951831116)
- [Doctors Without Borders, USA](https://www.charitynavigator.org/ein/133433452)
- [The Climate Reality Project](https://www.charitynavigator.org/ein/870745629)


# Contributors


## [MandL27](https://github.com/MandL27)

- Provided the original export code for PoryMoves


## [TheMitchelson](https://github.com/TheMitchelson)

- Added Regieleki, Regidrago, Glastrier, Spectrier, Calyrex, and their forms, as well as new moves from The Crown Tundra and The Isle of Armor


## [psf](https://github.com/pkmnsnfrn)

- Added Pokémon and moves for BDSP, LA, and SV
- Wrote 1.0 of this documentation
