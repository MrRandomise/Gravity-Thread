# 🧵 Gravity Thread

> A 2D physics-based puzzle-platformer built with **Unity 6** and **C#**.  
> Pull a ball through procedurally generated mazes using a gravity-thread mechanic — dodge obstacles, match colors, and scale to survive.

---

## 🎮 Gameplay Overview

The player controls a **physics-driven ball** by holding and dragging to create a "thread" — a pull force that attracts the ball toward the touch/click point. The goal is to navigate from the bottom to the top of a procedurally generated maze while managing:

- **Energy** — pulling drains a limited energy bar; release to regenerate
- **Size** — spikes shrink the ball, bottles grow it; squeeze through tight gaps or smash wide open ones
- **Color** — collect colors from sources, then pass through matching color gates
- **Health** — 3 HP; certain collisions reduce health

---

## ✨ Key Features

| Feature | Details |
|---|---|
| Thread mechanic | Distance-based pull force with energy drain/regen loop |
| Procedural levels | Seeded maze generation with progressive difficulty scaling |
| Multiple obstacle types | Spikes, bottles, color gates, color sources, pulsing walls |
| Ball states | Size system (0.2–1.5), color system, health (1–3 HP) |
| Scoring | Base score + energy-efficiency bonus multiplier |
| Multi-platform input | Mouse and multi-touch (New Input System) |
| Localization | English / Russian (extendable) |
| Audio | Full audio service with ScriptableObject config |
| Debug overlay | In-editor overlay for live stat inspection |

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Engine | Unity 6 (6000.3.11f1 LTS) |
| Language | C# (.NET Standard 2.1) |
| Rendering | Universal Render Pipeline (URP 17.3) |
| Physics | Unity 2D (Box2D) |
| Input | Unity New Input System 1.19.0 |
| Dependency Injection | Zenject 3.0.39 |
| UI | UGUI + TextMesh Pro |
| Audio | Unity Audio System |
| Data | ScriptableObjects |

---

## 🏗️ Architecture & Design Decisions

The project follows **clean architecture** principles adapted for Unity:

```
Assets/Game/
├── Bootstrap/          ← Entry point: scene bootstrapper, DI root
├── Configs/            ← ScriptableObject configs (no magic numbers)
├── Core/               ← GameState, EventBus, lifecycle management
│   ├── Events/         ← Strongly-typed event system (pub/sub)
│   └── Interfaces/     ← ITickable, ILifecycle contracts
├── Gameplay/           ← Ball, Thread, Level, Camera systems
│   ├── Views/          ← Visual representations (BallView, GateView…)
│   └── LevelGen/       ← Procedural maze: generator + builder + data
├── Services/           ← Input, Audio, Score, Achievements, Localization
├── Installers/         ← Zenject DI bindings (scene + settings)
└── UI/                 ← HUD, Pause, LevelComplete, LevelFailed screens
```

### Notable patterns

- **Dependency Injection (Zenject)** — all services injected; no manual singletons or scattered `GetComponent` lookups
- **Event Bus (Pub/Sub)** — decoupled communication between systems; e.g., `BallHitSpike` → `HealthService` → `HUD` without direct references
- **Single `MonoBehaviour` Dispatcher** — `GameLifecycleRunner` calls `ITickable.Tick()` on all registered systems, keeping the codebase predominantly plain C#
- **ScriptableObject Configs** — gameplay tuning (speed, energy, gap width, difficulty) done in the editor without recompilation
- **Seeded Procedural Generation** — `LevelGenerator` produces a winding path from a seed, enabling reproducible levels and difficulty curves

---

## 🚀 Getting Started

### Requirements

- **Unity 6** (6000.3.11f1 or newer LTS)
- Any platform target: Windows, macOS, Android, iOS

### Running the project

1. Clone the repository:
   ```bash
   git clone https://github.com/MrRandomise/Gravity-Thread.git
   ```
2. Open the project in **Unity Hub** → *Add project from disk*
3. Let Unity import all packages (first launch may take a few minutes)
4. Open `Assets/Scenes/SampleScene.unity`
5. Press **▶ Play**

### Controls

| Platform | Action |
|---|---|
| PC | Hold Left Mouse Button → drag to pull the ball |
| Mobile | Hold finger → drag to pull the ball |

### Tuning gameplay (no code needed)

All gameplay parameters live in `Assets/Game/Configs/Scriptable/`:

| Config | Controls |
|---|---|
| `BallConfig` | Scale limits, health, speed |
| `ThreadConfig` | Max pull distance, energy, force curve |
| `LevelGenerationConfig` | Maze width, obstacle density, difficulty scaling |
| `GameConfig` | Global game settings |
| `AudioConfig` | Sound volumes, clips |

---

## 👤 My Role

This is a **solo project** developed from scratch. I was responsible for:

- Game design and core mechanic ideation
- Full architecture design (DI container setup, event system, lifecycle management)
- All gameplay programming (ball physics, thread force, procedural generation, obstacle logic)
- UI/UX implementation (HUD, pause, level end screens)
- Service layer (input, audio, scoring, localization)
- ScriptableObject-based configuration system

---

## 📈 Status & Roadmap

**Current state:** Functional gameplay prototype with complete core loop.

### Done ✅
- [x] Core thread-pull mechanic with energy system
- [x] Procedural maze generation with difficulty scaling
- [x] All obstacle types (spikes, bottles, color gates, pulsing walls)
- [x] Scoring with efficiency bonus
- [x] Multi-platform input (mouse + touch)
- [x] Game state machine (play, pause, level complete, game over)
- [x] Localization (EN/RU)
- [x] Audio service

### Planned 🔜
- [ ] Main menu and level selection screen
- [ ] Visual polish — particle effects, screen shake, juice
- [ ] Save system — persistent high scores and progress
- [ ] More obstacle and power-up types
- [ ] Mobile build and store release

---

## 📄 License

This project is provided for portfolio and demonstration purposes.
