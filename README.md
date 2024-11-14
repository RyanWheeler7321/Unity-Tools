# UnityTools
Set of C# scripts for Unity with a bunch of simple and useful functions.


### MAIN
This is a central singleton for the main manager of all gameobjects and logic. General functions and references can be found from any script using this singleton. Only necessary functionality should be implemented with this to save on development time. Includes extendable methods for a screen fade, level loading, and debug tools.

### OptionsMenu
Options menu script handling local saving of primary game settings, like volume and display options like resolution and quality. Adjusts volume values to match decibel calculations, converts necessary options data between types, and automatically saves the data locally via PlayerPrefs. This specific script is implemented with a custom horizontal selector interface, so a similar implementation must be created and hooked in or a separate UI for the menu should be created.

### Save
Data storage script for saving custom data to a file. Singleton can be referenced from any script. Anything serializable can be stored, even custom classes. Starting data contains triggers, example custom data, with default values.

### Sound
Handles most SFX and music needs. Static calls from any script make it easy to implement. For SFX, soundbank scriptable object can be created to group many sounds and play random ones, or to simply keep related sounds together. Pooling is implemented for SFX to improve performance. Has pitch and volume variation, with options for 3D SFX, UI SFX, reverb, importance ranking, and panning.

Music functionality can play looped tracks from a set list, fade in or out music, adjust volume, and play music with a starting section that leads into a looped section to work with gameplay scenarios.

### Util
Static class containing many useful tools and tricks for a bunch of helpful functions to speed up development, example calls of a few are commented at the bottom. Functions include delayed reactions, progressive actions, random calculations, object referencing, and more.
