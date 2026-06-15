# Arkanoid Clone — Advanced Unity Architecture Sample

A 2D Arkanoid clone built as an **architecture reference**, demonstrating how to structure a Unity game around dependency injection, a hierarchical state machine, and strict assembly isolation. The gameplay is intentionally simple; the value is in the patterns.

---

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                        GameLifetimeScope                        │
│  (VContainer root — all services registered here)               │
│                                                                 │
│  ┌──────────────┐   ┌─────────────────┐   ┌─────────────────┐  │
│  │  AppState    │   │   BallManager   │   │  BrickManager   │  │
│  │  (HSM root)  │   │                 │   │                 │  │
│  │              │   │  SpawnBall()    │   │  OnBrickDest-   │  │
│  │  ┌─────────┐ │   │  SpawnAndLaunch │   │  royed event    │  │
│  │  │MainMenu │ │   │  RemoveAll()    │   │  OnAllBricksD-  │  │
│  │  ├─────────┤ │   └────────┬────────┘   │  estroyed event │  │
│  │  │Prepare  │ │            │            └────────┬────────┘  │
│  │  ├─────────┤ │            │                     │           │
│  │  │InGame ◄─┼─┼────────────┴─────────────────────┘           │
│  │  ├─────────┤ │                                               │
│  │  │ Pause  │ │   ┌─────────────────┐   ┌─────────────────┐   │
│  │  ├─────────┤ │   │  PowerUpManager │   │ CameraShake-    │   │
│  │  │ EndGame │ │   │  (ITickable)    │   │ Controller      │   │
│  │  └─────────┘ │   └─────────────────┘   └─────────────────┘   │
│  └──────────────┘                                               │
└─────────────────────────────────────────────────────────────────┘
```

---

## Key Patterns

### 1 — VContainer Dependency Injection

`GameLifetimeScope` (`LifetimeScope/`) is the single VContainer root. Every system is registered there and resolved via constructor injection:

```csharp
// Plain C# class — constructor injection
public class BallManager
{
    [Inject]
    public BallManager(IBallFactory factory, BallSettings settings, CameraManager camera) { ... }
}

// MonoBehaviour — field injection with [Inject]
public class PrepareGameState : StateMachine
{
    [Inject] private LevelCreator _levelCreator;
    [Inject] private PaddlePlacer _paddlePlacer;
    ...
}
```

Classes that need a per-frame update implement **`ITickable`** instead of using `MonoBehaviour.Update()`, and are registered with `.AsImplementedInterfaces()` so VContainer drives them:

```csharp
builder.Register<PowerUpManager>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
// PowerUpManager : ITickable, IDisposable
```

States are created through **`VContainerStateFactory`** so the resolver wires their dependencies automatically — no manual `new` calls anywhere in game flow.

---

### 2 — Hierarchical State Machine

`Assets/Scripts/HSM/StateMachine.cs` (`Devkit.HSM`) is a hand-rolled hierarchical state machine. `AppState` builds the full transition graph in `OnEnter()`:

```
MainMenuState ──[START_GAME_REQUEST]──► PrepareGameState
PrepareGameState ──[PREPARE_COMPLETE]──► InGameState
InGameState ──[PAUSE_GAME_REQUEST]──► PauseGameState
PauseGameState ──[CONTINUE_GAME_REQUEST]──► InGameState
InGameState ──[GAME_OVER_REQUEST]──► EndGameState
EndGameState ──[RETRY_GAME_REQUEST]──► InGameState
```

Transitions fire from anywhere with `SendTrigger((int)StateTriggers.SOME_TRIGGER)`. States override `OnEnter()` / `OnExit()` — never polling, never checking state flags in `Update()`.

---

### 3 — Assembly Definitions

Every script folder has its own `.asmdef`. This enforces dependency direction at compile time — Unity refuses to build if a reference cycle forms.

**Rule:** reference by assembly name string, not GUID, when possible:
```json
{
  "name": "arkanoid.powerup",
  "references": [ "arkanoid.powerup-type", "arkanoid.brick", "arkanoid.brick-manager", ... ]
}
```

**Handling circular dependencies:** when two assemblies would otherwise reference each other, extract the shared type into a third, no-dependency assembly.  
Example: `Brick` needs `PowerUpType`, and `PowerUp` needs `Brick` — solved by `PowerUpType/arkanoid.powerup-type.asmdef` (no references), which both sides reference:

```
arkanoid.powerup-type   (no deps)
      ▲              ▲
arkanoid.brick    arkanoid.powerup
```

---

## Systems at a Glance

| System | Type | Key responsibility |
|---|---|---|
| `BallManager` | Plain C# | Owns all `Ball` instances; spawn, launch, lifecycle |
| `BrickManager` | Plain C# | Tracks active bricks; fires `OnBrickDestroyed`, `OnAllBricksDestroyed` |
| `PowerUpManager` | Plain C# / ITickable | Spawns pickups on brick death; ticks timed effects; reverts on expiry |
| `CameraShakeController` | Plain C# / IDisposable | Subscribes to events; calls `CameraManager.Shake()` |
| `BrickExplosionController` | Plain C# / IDisposable | Spawns GPU shader effect on brick death via shared `Material` + `MaterialPropertyBlock` |
| `LevelCreator` | MonoBehaviour | Loads level JSON via Addressables; instantiates `Brick` components at runtime |
| `PaddlePlacer` | Plain C# | Creates and repositions the paddle; recalculates movement bounds after size changes |
| `CameraManager` | MonoBehaviour | Orthographic fit to level bounds; runs screen shake offset in `Update()` |

---

## Adding Things

**New power-up**
1. Add a value to `PowerUpType` enum (`PowerUpType/PowerUpType.cs`)
2. Create a class implementing `IPowerUpEffect` in `PowerUp/Effects/`
3. Add a case to `PowerUpManager.CreateEffect()`

**New game state**
1. Create a class extending `StateMachine` in `GameStates/`
2. Add `[Inject]` fields for dependencies
3. Register it as `Lifetime.Transient` in `GameLifetimeScope`
4. Wire a transition in `AppState.BuildHierarchy()`

**New system that needs per-frame update**
1. Implement `ITickable` (and `IDisposable` if it subscribes to events)
2. Register with `.AsImplementedInterfaces()` in `GameLifetimeScope`
3. No `MonoBehaviour` needed

---

## Tech Stack

| | |
|---|---|
| Engine | Unity 2D (URP) |
| DI | [VContainer](https://github.com/hadashiA/VContainer) 1.17.0 |
| Async | [UniTask](https://github.com/Cysharp/UniTask) |
| Asset loading | Unity Addressables 2.7.6 |
| Input | Unity Input System 1.16.0 |
| Shader | Custom HLSL (URP Unlit + built-in fallback) |
