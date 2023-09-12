# Changelog

All changes to this project will be documented in this section. The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project tries to adhere to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

# Unreleased

- Nothing!

**Full Changelog**: https://github.com/AsparagusEduardo/PoryMoves/compare/v1.5.0...master

***

# 1.5.0 - 2023/09/12

### Added
- Option to only use the latest moveset available for a species. 

### Fixed
- Fixed USUM's JSON missing its PreEvoMoves data.
- Fixed move order in USUM's JSON.
- Other USUM data corrections.

**Full Changelog**: https://github.com/AsparagusEduardo/PoryMoves/compare/v1.4.0...v1.5.0

***

# 1.3.0 - 2022/09/10

### Added
- Added support for [pokeemerald-expansion](https://github.com/rh-hideout/pokeemerald-expansion) 1.0.0’s new "Teachable moves" format.
- The "Select All" button now switches to "Deselect All" when pressed.
- Mew can now properly learn all TM and Tutor moves (with exception of a few exclusive moves).
  - Added an option for Mew to learn them anyway.
### Changed
- Updated [Newtonsoft.Json](https://www.newtonsoft.com/json) to 13.0.1

**Full Changelog**: https://github.com/AsparagusEduardo/PoryMoves/compare/v1.2.1...v1.3.0

***

# 1.2.1 - 2022/05/13

### Added
- Added data for Galarian forms.
- Added missing Pokémon data for Toxtricity Low Key and Female Indeedee.
- `db/monNames.json` now supports Pokémon forms using the same learnset tables as their base forms (eg. Rotom).
- Added an option to utilize the new move constants by RHH's branches (eg.`VISE_GRIP` vs `VICE_GRIP`).
- Added button shortcuts to quickly open input and output folders.
- Added support for near-universal TMs (eg. Hidden Power). Species that will not benefit from this can be set up using the `ignoresNearUniversalTMs` attribute in `db/MonNames.json`.
  - A special case was made for Attract, as it uses `isGenderless` to check if the mon should be able to use the move. Nincada doesn't learn it at all though.
- Input files now support comments and empty rows.

### Fixed
- Fixed Solar Beam's TM constant, as it wasn't synced after a [pokeemerald](https://github.com/pret/pokeemerald) change.
- Fixed Mime Jr.'s incorrect species constant.

### Changed
- Disabled the [Bulbapedia](https://bulbapedia.bulbagarden.net/wiki/List_of_Pok%C3%A9mon_by_National_Pok%C3%A9dex_number) Parser.
  - Not only it was broken for Gen 8, but It wasn't really needed to be enabled user-wise and only served to stress the Bulbapedia servers needlessly.
- Changed to using [Serebii](https://serebii.net/pokemon/) for Generation 8 data.
- Merged Sword and Shield Expansion Pass data into Sword and Shield data.
- "Use Extended TMs" changed to "Use Refactored TMs".
- "Use Extended Tutors" changed to "Use Refactored Tutors".
- "Use new Style" changed to "Align moves to name".
  - Choosing this option will use [pokeemerald's formatting for egg_moves.h](https://github.com/pret/pokeemerald/blob/master/src/data/pokemon/egg_moves.h) , while [Pokémon Expansion's format](https://github.com/rh-hideout/pokeemerald-expansion/blob/master/src/data/pokemon/egg_moves.h) will be the default.
- Egg moves are now exported in alphabetical order.

**Full Changelog**: https://github.com/AsparagusEduardo/PoryMoves/compare/v1.2.0...v1.2.1

***

# 1.2.0 - 2021/10/30

### Added
- The internal [Bulbapedia](https://bulbapedia.bulbagarden.net/wiki/List_of_Pok%C3%A9mon_by_National_Pok%C3%A9dex_number) html parser is now available publicly. This feature is used to update the learnset data files in `db/gen/`.
  - The process to use this parser will take several minutes to do so, specially on later generations. Every release of PoryMoves will have the learnset data files updated, so this should only be used in case there's updated information in Bulbapedia and it hasn’t been manually updated by the Porymoves maintainer(s).
- Added missing data from The Crown Tundra
  - Pokémon
  - Forms
  - Moves
## Fixed
- Smeargle's level up moveset was being averaged into a single Sketch past level 1.It now exports its moveset like it's in every generation it's in.
- `party_menu_tutor_list.h` now exports to the root folder instead of `output/`.
- `party_menu_tutor_list.h` now includes TUTOR-in its defines.

## Changed
- Isle of Armor and Crown Tundra learnset JSONs have been merged into a single Expansion Pass file.
- Move data is now read from `db/moveNames.json` instead of being hard-coded into the app. This should allow for future proofing in case new moves are added before a new release.
- TM data in learnset files are now ordered by TM number in each generation.

**Full Changelog**: https://github.com/AsparagusEduardo/PoryMoves/compare/v1.1.0...v1.2.0

***

# 1.1.0 - 2020/11/15

## Added
- Added Moves data from
  - Sword and Shield version 1.3.0
- Added Move data for
  - Shaymin Sky Form
  - Hoopa Unbound
  - Elernal Flower Floette

## Fixed
- Fixed the defines for Tea Time and Eternabeam.

## Changed
- Changed the defines, variable names, and order based on the [changes made to pokeemerald-expansion](https://github.com/rh-hideout/pokeemerald-expansion/commit/c91fba03101a88e37485691bb4f5fe3d5939fe7d).

**Full Changelog**: https://github.com/AsparagusEduardo/PoryMoves/compare/v1.0.0...v1.1.0

***

# 1.0.0 - 2020/10/1

- Initial release!
