# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Language Policy

- **Documentation & MD files**: Write in English
- **Communication with the user**: Always respond in Korean (한국어)

## Code Modification Policy

**CRITICAL: Never generate or modify code without explicit user permission.**

- **Analysis First**: When the user reports an issue, analyze and explain the root cause thoroughly before suggesting any code changes
- **Explain Before Action**: Provide a clear explanation of what needs to be changed and why
- **Wait for Permission**: Always wait for explicit user approval before using Edit, Write, or any code generation tools
- **No Assumptions**: Do not assume the user wants code generated just because a problem is identified
- **Debugging**: When adding debug logs or making changes, explain the changes first and get approval
- **User is in Control**: The user decides when and what code to modify - Claude only executes after permission

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
8_Material/     - Material assets
9_Texture/      - Texture assets
10_Behavior/    - Unity Behavior Tree custom nodes
```

New folders must continue the numbering sequence. Do not rename or move existing numbered directories.

## Script Architecture (`Assets/_PROJECT/1_Scripts/`)

### NPC AI System (Dual Architecture)

The project uses **two separate AI systems** for NPCs:

#### Legacy Character System (Coroutine-based)
- `Character.cs` — Core NPC state machine with two independent state axes:
  - `FirstState`: lower body/locomotion (`Idle`, `Walk`, `Run`, `See`, `Discover`, `Dead`)
  - `SecondState`: upper body actions (`None`, `Careful`, `Scream`, `Hurt`)
  - Uses `NavMeshAgent` for pathfinding, patrol points, view cone detection, and sound detection
  - Behavior managed via coroutines
- `CharacterView.cs` — Sensor component; detects dead bodies (`Evidence` tag) and triggers NPC reactions
- `CharacterCall.cs` — Sound propagation when NPC calls for help (expanding sphere collider)

#### Unity Behavior Tree System (`10_Behavior/`)
- `Entity.cs` — New NPC wrapper integrating Unity.Behavior:
  - Manages `BehaviorGraphAgent` component for behavior tree execution
  - **Team/Role System**: `Team` enum (CitizenSide/EnemySide), `Role` enum (Citizen_None, Citizen_Police, Enemy_None, Enemy_Boss)
  - Exposes blackboard variables (Role, PatrolPoints) to behavior graph
  - Health tracking with damage/headshot differentiation
  - Integrates with `RagdollController` for physics death
- **Custom Behavior Nodes** (extending `Unity.Behavior.Action`/`Condition`):
  - Actions: `PatrolPointerAction`, `CallFriendAction`, `RequestHelpAction`, `FindNearFriendAction`, `EscapeAction`, `HelpFriendAction`, `DeadAction`, `OnStopAction`
  - Conditions: `NullCheckCondition` (blackboard variable null checks)
  - Nodes communicate via blackboard (SetVariableValue/GetVariable)
  - Team-aware cooperation: NPCs only help/call allies on same Team
- `Sg_GameManager.Inst.entities` — Central list of all Entity objects; used by behavior tree for proximity queries (e.g., FindNearFriendAction)

##### Entity_00.asset Behavior Graph (`10_Behavior/Entity_00.asset`)

**Blackboard Variables:**
- `FurthestPoint` (GameObject) — Farthest patrol point
- `PatrolPoint` (GameObject) — Current patrol target
- `HelpTarget` (GameObject) — Target requesting help
- `NearFriend` (GameObject) — Closest ally found by FindNearFriendAction
- `AlertTarget` (GameObject) — Target causing alert state
- `patrolSpeed` (float: 1.0) — Movement speed during patrol
- `patrolWaitMinTime` (float: 5.0) — Minimum idle time at patrol point
- `patrolWaitMaxTime` (float: 10.0) — Maximum idle time at patrol point
- `helpTime` (float: 10.0) — Duration to help allies
- `isHurt` (bool) — Damage state flag
- `isHide` (bool) — Hide state flag

**Tree Structure:**
```
Start
└── Selector (Root)
    └── BranchingCondition (Alert check)
        ├── True → Alert Sequence
        │   ├── CallFriendAction (radius-based alert propagation)
        │   └── Sequence
        │       ├── SetAnimatorBool (isAlert = true)
        │       └── Sequence
        │           ├── FindNearFriendAction (populate NearFriend)
        │           └── Selector
        │               └── BranchingCondition
        │                   ├── True → Sequence (request help)
        │                   └── False → Sequence (NullCheckCondition + fallback)
        │
        └── False → Patrol Sequence
            ├── SetAnimatorBool (isAlert = false)
            └── Selector
                └── BranchingCondition (help request check)
                    ├── True → Sequence
                    │   ├── FindNearFriendAction
                    │   └── Selector
                    │       └── BranchingCondition (NullCheckCondition)
                    │           ├── True → HelpFriendAction
                    │           └── False → EscapeAction
                    │
                    └── False → PatrolPointerAction
```

**Behavior Logic:**
1. **Alert State**: If AlertTarget exists → CallFriendAction propagates alert to nearby allies (sets their AlertTarget via blackboard), then finds nearest friend for coordination
2. **Normal State**: Patrols using PatrolPointerAction with wait times (5-10s)
3. **Help Request**: If HelpTarget exists → FindNearFriendAction → NullCheckCondition → either HelpFriendAction or EscapeAction fallback
4. **Team Cooperation**: All friend-finding actions filter by matching `myTeam` enum via Sg_GameManager.Inst.entities list

**CRITICAL DEPENDENCY**: Requires `Sg_GameManager.Inst` to be initialized before Entity.Start() executes, otherwise FindNearFriendAction fails with NullReferenceException

### Physics & Death System
- **RagdollController.cs** — Manages animated NPC → physics ragdoll transition:
  - Separate rigidbody arrays for ragdoll skeleton
  - Head collider tracking for headshot detection
  - Smooth kinematic→dynamic transition with momentum transfer
  - Integrates with `Sg_ScoreManager` on death (giveScoreOnDeath flag)
  - updateWhenOffscreen optimization for performance

### Obstacle System
- `Obstacle.cs` — Base class for destructible objects using DinoFracture physics
- Subclasses: `Jukebox`, `PaperBox`, `Switchboard` — each with specialized interaction logic
- Hitting an obstacle triggers sound, which alerts nearby NPCs via the Character system

### Weapon System
- **Sg_GunIndex.cs** — Singleton weapon selection tracker (persists across scenes via DontDestroyOnLoad)
- **GunController.cs** — Weapon mechanics component:
  - SphereCast raycast system (0.1f radius) for hit detection
  - Fire interval timer-based system (not instant)
  - Layer-based filtering (Entity layer only)
  - Integrated hit feedback via `Sg_HitIndicator`
  - Recoil system via `Sg_MouseMan`

### Player System
- **PlayerController.cs** — Player character controller:
  - **Two-Bone IK Constraint**: Dynamically switches IK targets between weapon types (Unreal-style hand tracking)
  - `RigBuilder` rebuilt per weapon change for IK re-targeting
  - Reload state machine via Animator state info tracking (upper layer)
  - Magazine visibility toggle tied to reload animation events
  - Weapon selection caching (handList, hintList) for performance
  - Uses UnityEngine.InputSystem (direct polling: isPressed, wasPressedThisFrame)

### Singleton Modules (`3_SingletonModules/`)
- **Sg_GameManager** — Global entity tracking (maintains `entities` list; NPCs register in Start/unregister in OnDisable)
- **Sg_ScoreManager** — Score tracking with history (ScoreLogEntry), labels ("Kill"), popup notifications via ScorePopupNotifier, UI binding
- **Sg_MouseMan** — Mouse/cursor control (cursor locking with Tab toggle in editor), sensitivity multipliers, recoil offset accumulation with Lerp decay, persists across scenes
- **Sg_CameraController** — FOV-based zoom system:
  - defaultFov → zoomedFov with quadratic acceleration
  - Adaptive mouse sensitivity scaling based on zoom level
  - Scope canvas toggle (appears at 50% default FOV)
  - Scope magnification adjustment via mouse wheel (offsets FOV by -1 to -9)
  - Separate crosshair canvas toggle
- **Sg_GunIndex** — Weapon selection tracker (see Weapon System)
- **Sg_ObjectPool** — Generic pooling (Get<T>/Return<T>), pool-per-prefab dictionary, lazy initialization, nulls cleanup
- **Sg_HitIndicator** — UI-based hit feedback (image alpha fade), shared by gun and obstacle interactions

### Static Utilities (`4_StaticModules/`)
- `St_Clamp` — Vector3/Vector2 clamping (Block/Return modes, Min/Max directions)
- `St_Range` — Range utilities

### UI Architecture
- **Data Binding Pattern**: `AmmoProgressBarBinder` connects GunController ammo to UI
- **Event-Driven Score**: `KillScoreHandler` for score display
- **Weapon Selection**: `WeaponCard` system (WeaponCardUI, WeaponCardGroupUI, WeaponCardHoverUI, WeaponCardPreviewRotator) for main menu preview
- **Scene Loading**: `UI_SceneLoader` with hardcoded scene names (StageScene_00, 01_StageScene, 02_StageScene, 03_StageScene, CustomizeScene, PlayScene)
- **Game Timer**: `GameTimer.cs` countdown with UnityEvent callbacks

## Key Design Patterns

1. **Dual AI Architecture**: Legacy coroutine state machine (`Character.cs`) + Unity Behavior Tree (`Entity.cs` with BehaviorGraphAgent)
2. **Behavior Tree Communication**: Blackboard variables for inter-NPC coordination (e.g., CallFriendAction sets "AlertTarget" on allies via `otherAgent.SetVariableValue()`)
3. **Team-Based AI**: Team/Role enums for faction-aware cooperation
4. **State Machine**: `Character.cs` uses two orthogonal state enums + coroutines (not Unity Animator states)
5. **Singleton**: `Sg_*` classes expose a `public static Sg_ClassName Inst` field
6. **Inheritance**: `Obstacle` base → specialized subclasses
7. **Object Pooling**: Use `Sg_ObjectPool.Inst` for frequently spawned objects
8. **IK Rigging**: PlayerController rebuilds `RigBuilder` per weapon change for dynamic IK re-targeting

## Asset Rules

- Free external assets → `6_ExtAssets/` (committed)
- Paid/licensed assets → `7_IgnoreAssets/` (gitignored, never commit)