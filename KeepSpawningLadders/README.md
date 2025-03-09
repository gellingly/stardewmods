# Keep Spawning Ladders

Mod that spawns ladders when the game stops spawning them because the floor
randomly generated one on arrival or because one was spawned from breaking
rocks. This mod will avoid spawning ladders if there is already one nearby, but
otherwise will attempt to maintain at similar ladder spawn rate as the base
game.

Likely to be incompatible with multiplayer.

## Config Options
This mod does not have integration with Generic Mod Config Menu, but does
provide the following config option for debugging purposes:
* `PlayDebugSound` - Play dog bark sound and log to console when this mod creates a ladder. Useful for checking if the mod is working or is spawning too many ladders.

Thanks to the Stardew Valley discord for their help with answering questions and code review.

## Notes for Me
For testing:
```
world_downminelevel
```