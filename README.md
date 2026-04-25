# Starfall Crossing Prototype

This workspace contains a Unity-ready, fully original 2D side-scrolling platformer prototype built in C#.

## Features

- Original player character: `Sky Explorer`
- Smooth left/right movement with acceleration and deceleration
- Variable-height jumping
- Patrol enemies with stomp defeat logic
- Score pickups using crystal collectibles
- Health-based fail state
- Goal portal at the end of the level
- Smooth horizontal camera follow with look-ahead
- Start / Restart UI and simple win / lose screen
- Modular scripts for easy expansion

## Data-driven structure

- `CharacterDefinition`
  - Character identity, tint/sprite, collider size, move stats, jump tuning, health
- `EnemyBehaviorDefinition`
  - Patrol settings, contact knockback, scoring values
- `LevelDefinition`
  - Spawn point, goal position, fall limit, platforms, collectibles, enemies

## How to run in Unity

1. Create a new Unity `2D Core` project.
2. Copy this workspace's `Assets` folder into the Unity project.
3. Wait for Unity to finish compiling scripts.
4. Run `Tools > Side Scroller Prototype > Create Sample Prototype`.
5. Open `Assets/Scenes/PrototypeScene.unity`.
6. Press `Play`.

## Browser version

You can also run the browser prototype directly without Unity.

1. Open [index.html](C:/Users/us/Documents/Codex/2026-04-22-create-a-simple-2d-side-scrolling/index.html) in a browser.
2. Press `Start`.

Files for the browser version:

- [index.html](C:/Users/us/Documents/Codex/2026-04-22-create-a-simple-2d-side-scrolling/index.html)
- [styles.css](C:/Users/us/Documents/Codex/2026-04-22-create-a-simple-2d-side-scrolling/styles.css)
- [js/config.js](C:/Users/us/Documents/Codex/2026-04-22-create-a-simple-2d-side-scrolling/js/config.js)
- [js/entities.js](C:/Users/us/Documents/Codex/2026-04-22-create-a-simple-2d-side-scrolling/js/entities.js)
- [js/game.js](C:/Users/us/Documents/Codex/2026-04-22-create-a-simple-2d-side-scrolling/js/game.js)
- [js/main.js](C:/Users/us/Documents/Codex/2026-04-22-create-a-simple-2d-side-scrolling/js/main.js)

## Controls

- Move: `A/D` or `Left/Right Arrow`
- Jump: `Space`, `W`, or `Up Arrow`
- Hold jump for a higher jump
- Release early for a shorter jump

## Expansion notes

- Swap the hero by assigning a different `CharacterDefinition` to `RuntimeGameBootstrap.playerCharacter`.
- Add more enemies by creating new `EnemyBehaviorDefinition` assets.
- Build new stages by creating new `LevelDefinition` assets.
- Replace placeholder art by assigning custom sprites to the data assets.

## Notes

- All placeholder visuals are generated from simple shapes or colors and do not use outside IP.
- I could not launch Unity in this environment, so the code and setup menu were prepared for import, but I did not run a live playtest here.
