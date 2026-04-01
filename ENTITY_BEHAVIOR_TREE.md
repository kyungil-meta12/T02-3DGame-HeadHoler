# Entity_00.asset Behavior Tree Analysis

## Overview
This document provides a complete structural analysis of the Unity Behavior Tree defined in `Entity_00.asset`. The tree implements AI behavior for NPC entities in the Head Holer game, with role-based branching and complex coordination patterns.

## Blackboard Variables

### Core Entity References
- **Self** (rid: 8995693993379759051) - GameObject - The entity itself
- **Animator** (rid: 8995693993379759052) - Animator - Entity's animator component

### Patrol System
- **PatrolPoints** (rid: 8995693993379759053) - List<GameObject> - List of patrol waypoints
- **PatrolPoint** (rid: 8995693993379759054) - GameObject - Current patrol target
- **FurthestPoint** (rid: 8995693993379759055) - GameObject - Farthest patrol point from AlertTarget
- **patrolSpeed** (rid: 8995693993379759063) - float: 0.1 - Movement speed during patrol
- **patrolWaitMinTime** (rid: 8995693993379759064) - float: 5.0 - Minimum idle time at patrol point
- **patrolWaitMaxTime** (rid: 8995693993379759065) - float: 10.0 - Maximum idle time at patrol point

### Alert & Combat System
- **AlertTarget** (rid: 8995693993379759056) - GameObject - Target causing alert state (player/threat)
- **chaseSpeed** (rid: 8995693993379759061) - float: 1.0 - Movement speed when chasing target

### Cooperation System
- **NearFriend** (rid: 8995693993379759057) - GameObject - Closest ally found by FindNearFriendAction
- **HelpTarget** (rid: 8995693993379759059) - GameObject - Ally requesting help
- **HelpTargets** (rid: 8995693993379759058) - List<GameObject> - List of allies needing help
- **helpTime** (rid: 8995693993379759066) - float: 10.0 - Duration to help allies
- **callRange** (rid: 8995693993379759062) - float: 10.0 - Radius for CallFriendAction propagation

### Role System
- **Role** (rid: 8995693993379759060) - Role enum: 0 (Enemy_None) - Entity's role determining behavior branch

## Tree Structure

### Root: SwitchComposite (rid: 8995693995185668559, line: 2029)
**Purpose**: Role-based behavior branching
**EnumVariable**: Role (rid: 8995693993379759060)

The root switch node branches execution based on the entity's Role enum value:
- **Branch 0**: Enemy_None (rid: 8995693995185668569, line: 2182)
- **Branch 1**: Citizen_None (rid: 8995693995185668570, line: 2197)
- **Branch 2**: Citizen_Police (rid: 8995693995185668571, line: 2212)
- **Branch 3**: Enemy_Boss (rid: 8995693995185668572, line: 2226)

---

## Enemy_None Branch (Sequence rid: 8995693995185668569, line: 2182)

### Overview
This branch handles the behavior for basic enemy NPCs. The structure follows a 3-stage sequence:
1. Surprise reaction trigger
2. Wait period
3. Main behavior loop

### Stage 1: Surprise Animation
**Node**: SetAnimatorTrigger (rid: 8995693995185668586, line: 2420)
- **Type**: SetAnimatorTriggerAction
- **Trigger**: "Surprised" (rid: 8995693995180163118)
- **TriggerState**: true (rid: 8995693995180163119)
- **Purpose**: Play surprise animation when entity spawns/initializes

### Stage 2: Initial Wait
**Node**: WaitAction (rid: 8995693995185668587, line: 2436)
- **Type**: WaitAction
- **SecondsToWait**: 3.0 (rid: 8995693995180163120)
- **Purpose**: Delay before starting main behavior (allows surprise animation to play)

### Stage 3: Main Behavior Sequence (rid: 8995693995185668588, line: 2448)

#### 3.1 Navigate to Alert Target
**Node**: NavigateToTargetAction (rid: 8995693995185668606, line: 2691)
- **Agent**: Self (rid: 8995693993379759051)
- **Target**: AlertTarget (rid: 8995693993379759056)
- **Speed**: chaseSpeed (rid: 8995693993379759063)
- **DistanceThreshold**: (rid: 8995693995180163149)
- **AnimatorSpeedParam**: (rid: 8995693995180163150)
- **SlowDownDistance**: (rid: 8995693995180163151)
- **TargetPositionMode**: (rid: 8995693995180163152)
- **Purpose**: Move towards the alert target (player)

#### 3.2 Set Alert Animation State
**Node**: SetAnimatorBoolAction (rid: 8995693995185668607, line: 2715)
- **Parameter**: (rid: 8995693995180163153)
- **Animator**: Animator (rid: 8995693993379759052)
- **Value**: (rid: 8995693995180163154)
- **Purpose**: Update animator to reflect alert state

#### 3.3 Parallel Combat Actions (ParallelAll rid: 8995693995185668608, line: 2731)
**Purpose**: Execute looking and calling behaviors simultaneously

##### 3.3.1 Look At Target
**Node**: LookAtAction (rid: 8995693995185668623, line: 2955)
- **Transform**: Self transform (rid: 8995693995185668638)
- **Target**: AlertTarget transform (rid: 8995693995185668639)
- **Continuous**: true (rid: 8995693995180163191)
- **LimitToYAxis**: (rid: 8995693995180163192)
- **Purpose**: Keep facing the alert target

##### 3.3.2 Alert & Scan Loop (Sequence rid: 8995693995185668624, line: 2973)

###### 3.3.2.1 Call Nearby Allies
**Node**: CallFriendAction (rid: 8995693995185668640, line: 3236)
- **Self**: AlertTarget (rid: 8995693993379759056)
- **Radius**: callRange (rid: 8995693993379759062) - 10.0
- **Target**: (rid: 8995693995180163228)
- **Purpose**: Alert nearby allies within callRange radius to set their AlertTarget

###### 3.3.2.2 Wait Before Scan
**Node**: WaitRangeAction (rid: 8995693995185668641, line: 3252)
- **Min**: (rid: 8995693995180163229)
- **Max**: (rid: 8995693995180163230)
- **Purpose**: Random wait duration before scanning

---

## Citizen_None Branch (Sequence rid: 8995693995185668570, line: 2197)

### Overview
Behavior for civilian NPCs without special roles. Similar structure to Enemy_None but with different parameters.

### Stage 1: Surprise Animation
**Node**: SetAnimatorTrigger (rid: 8995693995185668589, line: 2463)
- **Trigger**: "Surprised" (rid: 8995693995180163124)
- **TriggerState**: true (rid: 8995693995180163125)

### Stage 2: Initial Wait
**Node**: WaitAction (rid: 8995693995185668590, line: 2479)
- **SecondsToWait**: (rid: 8995693995180163126)

### Stage 3: Main Behavior Sequence (rid: 8995693995185668591, line: 2491)

#### 3.1 Navigate to Alert Target
**Node**: NavigateToTargetAction (rid: 8995693995185668609, line: 2745)
- **Agent**: Self (rid: 8995693993379759051)
- **Target**: AlertTarget (rid: 8995693993379759056)
- **Speed**: patrolSpeed (rid: 8995693993379759061)
- **DistanceThreshold**: (rid: 8995693995180163157)
- **AnimatorSpeedParam**: (rid: 8995693995180163158)
- **SlowDownDistance**: (rid: 8995693995180163159)
- **TargetPositionMode**: (rid: 8995693995180163160)

#### 3.2 Call For Help
**Node**: CallFriendAction (rid: 8995693995185668610, line: 2769)
- **Self**: AlertTarget (rid: 8995693993379759056)
- **Radius**: callRange (rid: 8995693993379759062)
- **Target**: (rid: 8995693995180163161)

#### 3.3 Set Alert State
**Node**: SetAnimatorBoolAction (rid: 8995693995185668611, line: 2785)
- **Parameter**: (rid: 8995693995180163162)
- **Animator**: Animator (rid: 8995693993379759052)
- **Value**: (rid: 8995693995180163163)

#### 3.4 Parallel Investigation Actions (ParallelAll rid: 8995693995185668612, line: 2801)

##### 3.4.1 Look At Target
**Node**: LookAtAction (rid: 8995693995185668625, line: 2987)
- **Transform**: Self transform (rid: 8995693995185668642)
- **Target**: AlertTarget transform (rid: 8995693995185668643)
- **Continuous**: true (rid: 8995693995180163197)
- **LimitToYAxis**: (rid: 8995693995180163198)

##### 3.4.2 Investigation Sequence (rid: 8995693995185668626, line: 3005)

###### 3.4.2.1 Wait
**Node**: WaitAction (rid: 8995693995185668644, line: 3286)
- **SecondsToWait**: (rid: 8995693995180163231)

###### 3.4.2.2 Scan Area
**Node**: ScanTargetAction (rid: 8995693995185668645, line: 3298)
- **Target**: AlertTarget (rid: 8995693993379759056)
- **Self**: Self (rid: 8995693993379759051)
- **Purpose**: Scan for threats in the area

###### 3.4.2.3 Clear Alert Target
**Node**: SetVariableValueAction (rid: 8995693995185668646, line: 3312)
- **Variable**: AlertTarget (rid: 8995693993379759056)
- **Value**: null (rid: 8995693995180163232)
- **Purpose**: Clear alert state after investigation

---

## Citizen_Police Branch (Sequence rid: 8995693995185668571, line: 2212)

### Overview
Behavior for police officer NPCs. Shorter reaction time before patrol behavior.

### Stage 1: Surprise Animation
**Node**: SetAnimatorTrigger (rid: 8995693995185668592, line: 2507)
- **Trigger**: (rid: 8995693995180163131)
- **TriggerState**: (rid: 8995693995180163132)

### Stage 2: Main Police Behavior (Sequence rid: 8995693995185668593, line: 2523)

#### 2.1 Wait
**Node**: WaitAction (rid: 8995693995185668613, line: 2815)
- **SecondsToWait**: (rid: 8995693995180163615)

#### 2.2 Investigation Sequence (rid: 8995693995185668614, line: 2827)

##### 2.2.1 Navigate to Alert Target
**Node**: NavigateToTargetAction (rid: 8995693995185668627, line: 3020)
- **Agent**: Self (rid: 8995693993379759051)
- **Target**: AlertTarget (rid: 8995693993379759056)
- **Speed**: patrolSpeed (rid: 8995693993379759061)
- **DistanceThreshold**: (rid: 8995693995185668487)
- **AnimatorSpeedParam**: (rid: 8995693995185668488)
- **SlowDownDistance**: (rid: 8995693995185668489)
- **TargetPositionMode**: (rid: 8995693995185668490)

##### 2.2.2 Alert Response Sequence (rid: 8995693995185668628, line: 3044)

###### 2.2.2.1 Call For Backup
**Node**: CallFriendAction (rid: 8995693995185668647, line: 3326)
- **Self**: AlertTarget (rid: 8995693993379759056)
- **Radius**: callRange (rid: 8995693993379759062)
- **Target**: (rid: 8995693995185668495)

###### 2.2.2.2 Combat Response Sequence (rid: 8995693995185668648, line: 3342)

**2.2.2.2.1 Set Alert Animation**
**Node**: SetAnimatorBoolAction (rid: 8995693995185668651, line: 3376)
- **Parameter**: (rid: 8995693995185668498)
- **Animator**: Animator (rid: 8995693993379759052)
- **Value**: (rid: 8995693995185668499)

**2.2.2.2.2 Parallel Combat Actions (ParallelAll rid: 8995693995185668652, line: 3392)**

*2.2.2.2.2.1 Look At Target*
**Node**: LookAtAction (rid: 8995693995185668653, line: 3406)
- **Transform**: Self transform (rid: 8995693995185668655)
- **Target**: AlertTarget transform (rid: 8995693995185668656)
- **Continuous**: (rid: 8995693995185668504)
- **LimitToYAxis**: (rid: 8995693995185668505)

*2.2.2.2.2.2 Scan & Clear Loop (Sequence rid: 8995693995185668654, line: 3424)*

**2.2.2.2.2.2.1 Wait Before Scan**
**Node**: WaitAction (rid: 8995693995185668657, line: 3459)
- **SecondsToWait**: (rid: 8995693995185668509)

**2.2.2.2.2.2.2 Scan Area**
**Node**: ScanTargetAction (rid: 8995693995185668658, line: 3471)
- **Target**: AlertTarget (rid: 8995693993379759056)
- **Self**: Self (rid: 8995693993379759051)

**2.2.2.2.2.2.3 Clear Alert**
**Node**: SetVariableValueAction (rid: 8995693995185668659, line: 3485)
- **Variable**: AlertTarget (rid: 8995693993379759056)
- **Value**: null (rid: 8995693995185668510)

---

## Enemy_Boss Branch (Sequence rid: 8995693995185668572, line: 2226)

### Overview
Behavior for boss enemy NPCs. More aggressive with escape mechanics.

### Stage 1: Surprise Animation
**Node**: SetAnimatorTrigger (rid: 8995693995185668594, line: 2537)
- **Trigger**: (rid: 8995693995180163137)
- **TriggerState**: (rid: 8995693995180163138)

### Stage 2: Initial Wait
**Node**: WaitAction (rid: 8995693995185668595, line: 2553)
- **SecondsToWait**: (rid: 8995693995180163139)

### Stage 3: Boss Combat Behavior (ParallelAll rid: 8995693995185668596, line: 2565)
**Purpose**: Execute multiple behaviors simultaneously for complex boss AI

#### 3.1 Alert Allies Sequence (rid: 8995693995185668615, line: 2841)

##### 3.1.1 Call For Reinforcements
**Node**: CallFriendAction (rid: 8995693995185668629, line: 3058)
- **Self**: AlertTarget (rid: 8995693993379759056)
- **Radius**: callRange (rid: 8995693993379759062)
- **Target**: (rid: 8995693995180163209)

##### 3.1.2 Wait
**Node**: WaitRangeAction (rid: 8995693995185668630, line: 3074)
- **Min**: (rid: 8995693995180163210)
- **Max**: (rid: 8995693995180163211)

#### 3.2 Escape & Patrol Loop (Sequence rid: 8995693995185668616, line: 2855)

##### 3.2.1 Calculate Escape Point
**Node**: EscapeAction (rid: 8995693995185668631, line: 3088)
- **FurthestPoint**: FurthestPoint (rid: 8995693993379759055)
- **PatrolPoints**: PatrolPoints (rid: 8995693993379759053)
- **AlertTarget**: AlertTarget (rid: 8995693993379759056)
- **Self**: Self (rid: 8995693993379759051)
- **Purpose**: Find farthest patrol point from AlertTarget for tactical retreat

##### 3.2.2 Navigate to Escape Point
**Node**: NavigateToTargetAction (rid: 8995693995185668632, line: 3106)
- **Agent**: Self (rid: 8995693993379759051)
- **Target**: FurthestPoint (rid: 8995693993379759055)
- **Speed**: patrolSpeed (rid: 8995693993379759061)
- **DistanceThreshold**: (rid: 8995693995180163212)
- **AnimatorSpeedParam**: (rid: 8995693995180163213)
- **SlowDownDistance**: (rid: 8995693995180163214)
- **TargetPositionMode**: (rid: 8995693995180163215)

##### 3.2.3 Set Escape Animation
**Node**: SetAnimatorBoolAction (rid: 8995693995185668633, line: 3130)
- **Parameter**: (rid: 8995693995180163216)
- **Animator**: Animator (rid: 8995693993379759052)
- **Value**: (rid: 8995693995180163217)

##### 3.2.4 Look Away From Threat
**Node**: LookAtAction (rid: 8995693995185668634, line: 3146)
- **Transform**: Self transform (rid: 8995693995185668649)
- **Target**: AlertTarget transform (rid: 8995693995185668650)
- **Continuous**: (rid: 8995693995180163220)
- **LimitToYAxis**: (rid: 8995693995180163221)
- **Purpose**: Face escape direction while retreating

---

## Other Behavior Nodes (Referenced from Root)

### Dead Monitoring System
**Node**: ParallelAllComposite (rid: 8995693995185668558, line: 2015)
**Parent**: Root selector (rid: 8995693995185668550)
**Purpose**: Monitor death state alongside main behavior

#### Dead Check Loop
**Children**:
- **LookAtAction** (rid: 8995693995185668567, line: 2150) - Look at target while monitoring
- **Death Sequence** (rid: 8995693995185668568, line: 2168)
  - **WaitAction** (rid: 8995693995185668584, line: 2394) - Wait using helpTime (rid: 8995693993379759066) = 10.0 seconds
  - **Help Resolution Sequence** (rid: 8995693995185668585, line: 2406)
    - **HelpFriendAction** (rid: 8995693995185668604, line: 2665) - Execute help behavior
    - **SetVariableValueAction** (rid: 8995693995185668605, line: 2677) - Clear HelpTarget (rid: 8995693993379759059)

### Request Help System
**Node**: FindNearFriendAction (rid: 8995693995185668621, line: 2927)
**Purpose**: Populate NearFriend variable with closest ally
- **Self**: Self (rid: 8995693993379759051)
- **NearFriend**: NearFriend (rid: 8995693995180163186)

**Node**: RequestHelpAction (rid: 8995693995185668563, line: 2081)
**Purpose**: Request help from nearby friend
- **HelpTargets**: HelpTargets list (rid: 8995693995180163072)
- **NearFriend**: NearFriend (rid: 8995693993379759057)
- **Self**: Self (rid: 8995693993379759051)

### Alert System Dead Detection
**Node**: TimeOutModifier (rid: 8995693995185668577, line: 2302)
**Purpose**: Time-limited death check
- **Duration**: (rid: 8995693995180163107)
- **Child**: DeadAction (rid: 8995693995185668598, line: 2593)
  - **Self**: Self (rid: 8995693993379759051)

### Patrol System (Normal State)
**Node**: PatrolAction (rid: 8995693995185668635, line: 3164)
**Parent**: TimeOutModifier (rid: 8995693995185668618, line: 2885)
- **Agent**: Self (rid: 8995693993379759051)
- **Waypoints**: PatrolPoints (rid: 8995693993379759053)
- **Speed**: chaseSpeed (rid: 8995693993379759063)
- **WaypointWaitTime**: (rid: 8995693995180163222)
- **DistanceThreshold**: (rid: 8995693995180163223)
- **AnimatorSpeedParam**: (rid: 8995693995180163224)
- **PreserveLatestPatrolPoint**: (rid: 8995693995180163225)

**Parent Sequence** (rid: 8995693995185668597, line: 2579):
- **WaitRangeAction** (rid: 8995693995185668617, line: 2871)
  - **Min**: patrolWaitMinTime (rid: 8995693993379759064) = 5.0
  - **Max**: patrolWaitMaxTime (rid: 8995693993379759065) = 10.0
- **TimeOutModifier** with PatrolAction

### Friend Communication System
**Node**: IsFriendKnowMeAction (rid: 8995693995185668636, line: 3188)
**Purpose**: Check if NearFriend is aware of Self
- **NearFriend**: NearFriend (rid: 8995693993379759057)
- **Self**: Self (rid: 8995693993379759051)

**Parent Sequence** (rid: 8995693995185668619, line: 2899):
- **IsFriendKnowMeAction**
- **WaitRangeAction** (rid: 8995693995185668637, line: 3202)
  - **Min**: (rid: 8995693995180163226)
  - **Max**: (rid: 8995693995180163227)

**Parent Selector** (rid: 8995693995185668601, line: 2625):
- On success: Execute friend recognition sequence
- On failure: **SetVariableValueAction** (rid: 8995693995185668620, line: 2913) - Clear NearFriend (rid: 8995693993379759057)

### Repeated Alert Checks
**Node**: RepeaterModifier (rid: 8995693995185668579, line: 2334)
**Parent**: ParallelAll for Dead monitoring (rid: 8995693995185668564, line: 2097)
- **Child**: Selector (rid: 8995693995185668601) - Friend communication system
- **AllowMultipleRepeatsPerTick**: false

---

## Key Behavioral Patterns

### 1. Role-Based Branching
The SwitchComposite root node divides behavior by Role enum:
- **Enemy_None (0)**: Aggressive combat with ally coordination
- **Citizen_None (1)**: Defensive investigation with quick alert clearing
- **Citizen_Police (2)**: Professional investigation with backup calling
- **Enemy_Boss (3)**: Tactical retreat with reinforcement calling

### 2. Parallel Execution
ParallelAll nodes enable simultaneous behaviors:
- Looking at targets while calling for help
- Movement while monitoring death state
- Animation updates with navigation

### 3. Cooperation Patterns
- **CallFriendAction**: Propagates AlertTarget to allies within callRange radius
- **FindNearFriendAction**: Queries Sg_GameManager.Inst.entities for closest ally
- **RequestHelpAction**: Adds Self to friend's HelpTargets list
- **HelpFriendAction**: Responds to HelpTarget requests
- Team filtering ensures only same-Team entities cooperate

### 4. State Management
- **AlertTarget**: Drives transition from patrol to combat
- **HelpTarget**: Enables ally rescue behavior
- **NearFriend**: Caches closest ally for help requests
- **FurthestPoint**: Calculated dynamically by EscapeAction for tactical retreat

### 5. Animation Coordination
- **SetAnimatorTrigger**: One-shot animations (Surprised)
- **SetAnimatorBool**: State-based animations (isAlert)
- AnimatorSpeedParam: Movement speed synchronized with navigation

### 6. Time-Based Behavior
- **WaitAction**: Fixed delays (surprise reaction: 3 seconds)
- **WaitRangeAction**: Random delays (patrol waits: 5-10 seconds)
- **TimeOutModifier**: Time-limited actions (patrol cycles, death checks)
- **Duration**: Prevents infinite loops in patrol and monitoring

### 7. Null Handling
- **SetVariableValueAction**: Clears references (AlertTarget = null) after investigation
- **NullCheckCondition**: Validates blackboard references before actions
- Prevents NullReferenceExceptions in FindNearFriendAction

---

## Critical Dependencies

1. **Sg_GameManager.Inst.entities**: Must be initialized before Entity.Start() executes
   - Used by: FindNearFriendAction, CallFriendAction
   - Failure: NullReferenceException crashes behavior tree

2. **Team Enum**: All cooperation actions filter by matching Team
   - CitizenSide: Citizen_None, Citizen_Police
   - EnemySide: Enemy_None, Enemy_Boss
   - Cross-team help requests are ignored

3. **NavMeshAgent**: Required for all NavigateToTarget actions
   - Speed synchronized with blackboard variables (patrolSpeed, chaseSpeed)
   - DistanceThreshold determines arrival detection

4. **Animator Parameters**: Must match blackboard string literals
   - "Surprised" trigger (Enemy_None, Citizen_None)
   - "isAlert" bool parameter
   - Speed parameter for movement synchronization

---

## Performance Considerations

1. **Repeater Modifiers**: Set AllowMultipleRepeatsPerTick = false to prevent frame spikes
2. **TimeOut Modifiers**: Limit patrol duration to prevent infinite loops (Duration variable)
3. **CallFriendAction**: Radius-based (10.0 units) to prevent global entity queries
4. **FindNearFriendAction**: Iterates Sg_GameManager.Inst.entities - O(n) complexity per frame
5. **ParallelAll**: All children must succeed - failure in one child fails entire parallel node

---

## File Location
`D:\Git\T02-3DGame-HeadHoler\Assets\_PROJECT\10_Behavior\Entity_00.asset`

**Lines**: 2029-3497 (behavior tree node definitions)
**Format**: Unity YAML asset serialization
**Graph ID**: rid: 8179367720027160577

---

## Document Metadata
- **Analysis Date**: 2026-04-01
- **Analyzer**: Claude Code
- **Unity Version**: 6000.3.10f1
- **Behavior Tree Version**: Unity.Behavior package (Unity 6 native)
