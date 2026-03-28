# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Language Policy

- **Documentation & MD files**: Write in English
- **Communication with the user**: Always respond in Korean (한국어)

## Project Overview

**Head Holer** - Unity 6000.3.10f1 3D tactical stealth-shooter. The player must eliminate NPCs (civilians) in an office environment while avoiding detection by AI guards. Built with C# using NavMesh AI, physics destruction (DinoFracture), and the new Unity Input System.

## Development Environment

- **Engine**: Unity 6000.3.10f1
- **IDE**: Visual Studio / JetBrains Rider (`.sln` at root)
- **Platform**: Windows

Unity does not have a CLI build command for typical dev workflows — open the project in the Unity Editor directly. To run tests, use Unity's Test Runner window (Window > General > Test Runner).

## Code Conventions (`project convention.txt`)

- **Classes, structs, methods**: PascalCase
- **Fields, local variables**: camelCase
- **Singleton classes**: `Sg_` prefix (e.g., `Sg_ScoreManager`), instance variable always named `Inst`
- **Static utility classes**: `St_` prefix (e.g., `St_Clamp`)
- **Custom module types**: trailing `_` suffix (e.g., `Matrix_`) to avoid Unity/system reserved name conflicts
- **Braces**: Microsoft style (opening brace on new line)
- **Conditionals**: always use braces even for single-line bodies
- **Hierarchy folders**: `__FolderName__` format (e.g., `__Singleton Modules__`), spaces not underscores

## Folder Structure (`Assets/_PROJECT/`)

```
0_Scenes/       - All Unity scenes
1_Scripts/      - C# source code (see below)
2_Prefabs/      - Prefab assets
3_SingletonModules/  - Sg_* singleton scripts
4_StaticModules/     - St_* static utility scripts
5_CustomModules/     - Custom module definitions
6_ExtAssets/    - Free external assets (committed to repo)
7_IgnoreAssets/ - Paid assets (.gitignore'd, never commit)
```

New folders must continue the numbering sequence (e.g., `8_NewFolder`). Do not rename or move the `0_`–`7_` directories.

## Script Architecture (`Assets/_PROJECT/1_Scripts/`)

### Character System
- `Character.cs` — Core NPC state machine. Two independent state axes:
  - `FirstState`: lower body/locomotion (`Idle`, `Walk`, `Run`, `See`, `Discover`, `Dead`)
  - `SecondState`: upper body actions (`None`, `Careful`, `Scream`, `Hurt`)
  - Uses `NavMeshAgent` for pathfinding, patrol points, view cone detection, and sound detection
  - Behavior managed via coroutines
- `CharacterView.cs` — Sensor component; detects dead bodies (`Evidence` tag) and triggers NPC reactions
- `CharacterCall.cs` — Sound propagation when NPC calls for help (expanding sphere collider)

### Obstacle System
- `Obstacle.cs` — Base class for destructible objects using DinoFracture physics
- Subclasses: `Jukebox`, `PaperBox`, `Switchboard` — each with specialized interaction logic
- Hitting an obstacle triggers sound, which alerts nearby NPCs via the Character system

### Singleton Modules (`3_SingletonModules/`)
- `Sg_ScoreManager` — Centralized score tracking
- `Sg_MouseMan` — Mouse/cursor control, sensitivity, recoil
- `Sg_CameraController` — Camera management
- `Sg_GunIndex` — Weapon state
- `Sg_ObjectPool` — Object pooling

### Static Utilities (`4_StaticModules/`)
- `St_Clamp` — Vector3/Vector2 clamping (Block/Return modes, Min/Max directions)
- `St_Range` — Range utilities

## Key Design Patterns

1. **State Machine**: `Character.cs` uses two orthogonal state enums + coroutines (not Unity Animator states)
2. **Singleton**: `Sg_*` classes expose a `public static Sg_ClassName Inst` field
3. **Inheritance**: `Obstacle` base → specialized subclasses
4. **Object Pooling**: Use `Sg_ObjectPool.Inst` for frequently spawned objects

## Asset Rules

- Free external assets → `6_ExtAssets/` (committed)
- Paid/licensed assets → `7_IgnoreAssets/` (gitignored, never commit)