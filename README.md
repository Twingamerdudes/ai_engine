# ai_engine
## Intro
ai_engine is a toolkit for making ai shows similar to ai_spongebob. It is the engine that powers [ai_rick](https://www.youtube.com/@ai_rick_morty/)

## Installation
To install, create a 3D unity project. Install tmpro, cinemachine, and [OpenAI Unity](https://github.com/srcnalt/OpenAI-Unity).

Then drag all of the files from the repo, into your assets folder. That's it.

**YOU NEED A API KEY, FOLLOW OPENAI-UNITY'S INSTRUCTIONS ON SETTING UP A API KEY JSON**

## How it works
### Manager
ai_engine is mainly powered by one script. The **manager**. The manager handles ai generation, along with subtitle managment. To use it, make a empty gameobject, attach the manager.

Then fill out the parameters in the inspector.

### Character
The character script handles character movement and looking. Attach this to all of your characters. Your character should also have a animator with a walk animation. But it isn't required.

### AISplash
This handles AI Splash screens. Just attach it to your moments later screen, and that's it. **You do not need to attach it!!**

### Location Manager
Handles the location of where the characters will be. Don't attach this to anything. It's functions are staticly trigged.

### Location
Location is a scriptable object that allows you to define locations (The beach, a house, space, etc.). It's pretty simple.
