# Task 3: Complete Patterns Integration

# Project Evolution

- Task 2 focused on establishing core systems using the Singleton pattern (centralized managers) and implementing a basic playable loop.
- Task 3 completes the architecture by adding decoupled eventing and state-driven behavior to both the player and game flow, enabling easier feature expansion and clearer responsibilities.

# Task 2 Foundation

- Singleton Pattern:
  - GameManager — central game lifecycle, scene transitions, persistence between scenes.
  - AudioManager — centralized audio playback (music, effects), volume control, and audio pooling.

- Basic game with centralized management:
  - Core gameplay loop, input mapping, scoring, and basic UI.

## Task 3 Additions

Task 3 extends the codebase with two main patterns: Observer (Event) and State Machine.

## Observer Pattern

- EventManager:
  - A lightweight EventManager (Singleton) dispatches events across the project so systems do not hold direct references to each other.

- Events implemented:
  - onScoreChanged
  - onEnemyDefeated
  - onCollectibleCollected
  - onLevelComplete
  - onPlayerHealthChanged
  - onPowerUpCollected
  - onAchievementUnlocked
  - onTimerTicked
  - onBulletShot

- Observers:
  - UIManager — listens for score, game state, and player state events to update UI.
  - Achievements — listens for specific triggers to unlock achievements and notify the player.
  - AudioManager — listens for audio cue events and global game state changes to play sounds.

## State Machine Pattern

- Player States:
  - Idle 
  - Jumping 
  - Attacking
  - Moving

- Game States:
  - Menu
  - Playing 
  - onLevelComplete 

- State transitions:
  - Player Idle -> Jumping -> Idle
  - Player Idle -> Moving -> Idle
  - Player Moving -> Attacking -> Moving

### Key Integration Points

1. Score System: Singleton → Observer → UI
   - GameManager updates score value and triggers event onScoreChanged.
   - UIManager subscribes to onScoreChanged to update the score text.

2. Player Actions: Input → State → Event → Audio
   - PlayerController drives the State class (Left-Click sets Attacking state).
   - StateMachine fires onBulletFired event.
   - AudioManager plays corresponding SFX based on event, playing the shootSFX sound.

3. Game Flow: GameState → Events → Scene Changes
   - For now, a simple menu scene to game scene is implemented, state changes coming soon.

## Repository Statistics

- Total Commits: 40
- Task 3 Commits: 17
- Lines of Code: ~1000
- Development Time: 13 hours

Tip: run your repo analytics script (git shortlog / cloc) and update these values.

## How to Play

- Controls:
  - Movement: A / D Left, Right; W / S Forward, Backward
  - Jump: Space
  - Attack: Left Mouse Button
  - Pause: Esc

- Objective:
  - Score as high of a score as possible within a two minute window by collecting coins and shooting enemies.

- New Features:
  - Achievement System — unlockables on milestone events.

