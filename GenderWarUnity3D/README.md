# Gender War - Unity 3D Edition

A Dating Disaster Simulator - Now in 3D!

## Overview

This is the Unity 3D conversion of the original HTML/JavaScript "Gender War" dating simulation game. The game is a satirical commentary on modern dating culture, featuring exaggerated archetypes and branching narrative choices.

## Features

- **4 Unique Date Characters**: The Incel, The Femcel, The Performative Ally, The Bop
- **Part 3 Meta-Commentary**: The Themcel/Group integration system
- **Branching Dialogue**: 80+ dialogue nodes with meaningful choices
- **24+ Unique Endings**: Based on your choices and reputation
- **Character Customization**: Build your own Playboy Bunny avatar
- **Reputation System**: Hidden tracking of your behavior across dates
- **Integration Mechanics**: Part 3 evaluates legibility, friction, and reframing acceptance
- **Receipt System**: Track your spending with humorous itemized receipts
- **Phone Feature**: View your date's DisasterMatch profile

## Project Structure

```
GenderWarUnity3D/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/           # Game manager, state, camera, scene setup
│   │   ├── Dialogue/       # Dialogue system and database
│   │   ├── UI/             # All UI controllers
│   │   ├── Characters/     # Character data and 3D display
│   │   └── Systems/        # Reputation and integration systems
│   ├── Data/
│   │   ├── Dialogues/      # JSON dialogue files for each route
│   │   └── Characters/     # Character data ScriptableObjects
│   ├── Scenes/             # Unity scenes
│   ├── Prefabs/            # Reusable prefabs
│   ├── Materials/          # 3D materials
│   ├── Sprites/            # 2D character sprites (from original)
│   ├── Fonts/              # Typography
│   └── Audio/              # Sound effects and music
└── ProjectSettings/
```

## Setup Instructions

1. Open the project in Unity 2021.3 LTS or newer
2. Import TextMeshPro if prompted
3. Open the MainMenu scene
4. Press Play to test

## Importing Original Assets

Copy the following from the original HTML game's `sprites/` folder:
- All character sprite PNG files
- `bg_dialogue.png` (background)
- `title_bg.mp4` (title video)

## How It Works

### Game Flow
1. Title Screen → Character Creator → Date Selection (Part 1)
2. Play through dialogue, make choices
3. Reach ending, view receipt
4. Unlock Part 2 dates
5. Complete Part 2, unlock Part 3 (The Themcel)
6. Part 3 evaluates your reputation and determines final acceptance

### Reputation Tags
- `leftEarly` - You left dates early
- `stayedTooLong` - You stayed through uncomfortable moments
- `boundarySetter` - You set firm boundaries
- `humorDeflect` - You used humor as defense
- `engagedInSpreadsheet` - You engaged with frameworks/data
- `highSpend` - You spent $40+ on dates
- `chaosAgent` - You caused chaos

### Integration System (Part 3)
- **Legibility**: How predictable/categorizable you are
- **Friction**: How costly you are to process
- **Reframing Acceptance**: Your willingness to be defined by others

## Technical Notes

- Uses ScriptableObjects for dialogue databases
- JSON files can be edited and reimported
- Supports both 2D sprite overlay and full 3D characters
- UI built with Unity's Canvas system + TextMeshPro

## Credits

Original HTML Game: Gender War - A Dating Disaster Simulator
Unity 3D Conversion: Automated via Claude Code

## License

This is a satirical work for entertainment purposes.
