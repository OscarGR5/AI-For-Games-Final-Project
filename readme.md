# AI For Games Final Project

## Introduction

This project centres around developing AI for a 2D fighting game or survival platformer, where the player character must evade a relentless boss, survive waves of spawned enemies, and navigate a procedurally generated map filled with platforms, obstacles, and bombs. The goal is to survive as long as possible, escaping the boss and fighting the enemies that attempt to attack the player.

In the game, the boss is a large, intelligent AI entity with the role of pursuing the player. The boss is designed to follow the player dynamically, expressing "thoughts" or emotional states through text displays or flashing its colour that respond to the player’s actions, such as attacking, picking up bombs, or healing when killing an enemy. This AI feature not only heightens the sense of challenge but adds personality to the boss.

The enemy minions are smaller AI-controlled units that spawn continuously. Their primary behaviour is to detect, chase, and attack the player. This enemy AI adds pressure to the gameplay, requiring the player to constantly adapt and defend while evading the boss. Unlike the boss, these enemies rely on pathfinding techniques to chase the player efficiently through the map's obstacles.

So, the AI techniques used in our game are:
1. **Pathfinding**: Enables the minions to navigate the procedurally generated terrain, chasing the player without getting obstructed by the map’s structure.
2. **Boss personification**: The boss's behaviour and decision-making are governed by a state-based system. The boss dynamically reacts to game events and changes its movement, colour, and speech based on context.
3. **Procedural map generation**: The map consists of dynamically generated tiles, creating varied and unpredictable terrains.
4. **Mini enemy reaction system**: Enemies use simple state-based mechanics to determine when to attack the player or flash upon taking damage.

## Analysis

### 1. Pathfinding for Enemies (Minions)
**Implementation**:
- The Pathfinder method identifies the optimal path from the enemy to the player by using grid-based traversal.
- The ReconstructPath function builds a path by backtracking from the target tile to the start tile.
- Movement is updated dynamically via the EnemyMovement coroutine.
- A stopping mechanism checks proximity to the player and stops movement once the enemy is close enough.

**Rationale**:
- Using a grid-based pathfinding method is appropriate due to its reliability in uniformly structured environments like grid-based maps.

**Effectiveness**:
- The use of BFS ensures computational efficiency and accurate path generation even in complex, procedurally generated maps.

**Limitations**:
- Jump mechanics might limit flexibility when enemies traverse uneven terrains with multiple height levels.
- The lack of advanced algorithms like A* may result in less optimal paths in larger or more complex maps.

### 2. Boss Personification
**Implementation**:
- Speed Adjustments: The boss dynamically calculates its movement speed through coroutines.
- Reactions to Bombs: The boss reacts to the player's bomb count with custom animations, colour changes, and thought bubble messages.
- Reactions to Health: A function tracks the player's health and adjusts the boss's actions.
- Thought Bubble System: The UpdateThoughtBubble function uses a queue to manage and sequentially display messages.
- Colour Feedback: The FlashColor coroutine handles the boss's colour transitions.

**Rationale**:
- The use of predefined states and visual feedback was inspired by research on enhancing player engagement through "readable" AI behaviour.

**Effectiveness**:
- The boss feels more "alive" and responsive due to its thought bubble and dynamic visual feedback.

**Limitations**:
- The reactions rely heavily on predefined states. Incorporating machine learning or adaptive algorithms could improve variability and replayability.

### 3. Procedural Map Generation
**Implementation**:
- The map is constructed using tiles which are spawned dynamically as the player progresses.
- Different shapes and configurations of tiles are generated programmatically.
- Tile placement considers the player’s movement direction and position.

**Rationale**:
- Procedural generation was chosen for its ability to create variety in gameplay.

**Effectiveness**:
- The dynamic generation ensures that no two playthroughs feel exactly the same.

**Limitations**:
- Without detailed control over generation algorithms, there could be issues like inaccessible areas or overly challenging layouts.

### 4. Mini Enemy Reaction System
**Implementation**:
- Enemies attack the player upon entering a specific range and flash upon receiving damage.
- Proximity detection is used to decide when to attack the player.
- A flashing effect is used to visually communicate damage.

**Rationale**:
- A proximity-based detection approach was chosen due to its low computational cost and quick response times.

**Effectiveness**:
- The simplicity of the system allows for quick and responsive reactions.

**Limitations**:
- The system does not incorporate complex strategies like coordinated attacks or defensive manoeuvres.

## Reflection

### Main Results and AI Behaviour
The implemented system of AI works well, fitting the desired functionality and behaviours. The enemy AIs developed into a tile-based pathfinding system, and the boss AI showed great success in addressing player events. Procedural map generation introduced variation, and the mini enemies behaved logically in chasing the player and showing visual reactions for damage.

### Challenges and Issues
- Pathfinding was a challenge, especially with obstacles and jumping logic.
- Procedural map generation sometimes produced impassable structures or overly difficult sections.
- Balancing the boss AI's speed with the player’s movements was problematic.
- The boss AI's preprogrammed reactions did not allow it to adapt to emergent gameplay.

### Limitations
- The pathfinding system struggled with densely packed or complex map layouts.
- Procedural map generation lacked validation to ensure navigability or balance.
- The boss AI was bound to a limited set of pre-defined responses.

### Possible Improvements and Future Directions
- Applying an advanced pathfinding algorithm like A*.
- Procedural map generation could support a validation phase.
- Boss AI could be improved with reinforcement learning.
- Mini-boss enemies could use communal tactics of ambushes or team attacks.
- Advanced AI frameworks, such as utility-based systems, could raise the level of adaptability and decision-making.

## Conclusion
The report described the implementation, reasoning, and evaluation of our project AI and gameplay systems. While these were successful in making gameplay engaging, there are areas of improvement. The project showed good teamwork and application of the principles of game AI, setting a good foundation for refinement.
