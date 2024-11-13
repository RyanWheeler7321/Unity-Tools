# UnityTools
Set of C# scripts for Unity with a bunch of simple and useful functions.


## MAIN
This is a central singleton for the main manager of all gameobjects and logic. General functions and references can be found from any script using this singleton. Only very specific functionality should be implemented with this to save on development time. Includes extendable methods for a screen fade, level loading, and debug tools.

## Save
Data storage singleton for saving custom data to a file. Anything serializable can go in here, even custom classes. Starting data contains triggers, example custom data, with default values.

## Sound
Singleton to handle all SFX and music needs. For SFX, soundbank scriptable object can be created to group many sounds and play random ones, or to simply keep related sounds together. Randomized pitch and volume are included, with options for 3D SFX, importance ranking, and panning.

Music functionality can play looped tracks from a set list, fade in orout music, or play music with hits. A music hit is a sound with a starting section that leads into a looped section.

## Util
Static class containing many useful tools for any function, example calls of a few are commented at the bottom. Functions include delayed reactions, progressive actions, calculations, randomness, object referencing, and more.
