# Infinitecraft (Unity 2D) - Example

This repository contains starter scaffolding for an Infinitecraft-style 2D project in Unity.

Included in this commit:
- Assets/Scripts/JokeManager.cs (legacy: supports Unity UI Text and TextMeshPro)
- Assets/Scripts/JokeManagerTMP.cs (TextMeshPro-only convenience script)
- Assets/Scenes/JokeDemo.unity (placeholder and setup instructions)

Quick start
1. Open this project in Unity (2020.1+ recommended).
2. Import TextMeshPro essentials if prompted.
3. Open the scene: Assets/Scenes/JokeDemo.unity (or create it following the placeholder instructions).
4. In the scene: select the JokeManager GameObject and wire the Button and TMP fields in the inspector.
5. Play the scene and click the "Fetch" button to retrieve a joke from the Official Joke API.

Notes
- The provided scene file is a placeholder with step-by-step setup instructions. If you'd like, I can create a fully serialized .unity scene file wired to the script, but Unity will generally regenerate script GUID links when opened in the editor.
- The JokeManager scripts use UnityWebRequest in a coroutine and display simple status messages.

License
- Add a LICENSE file to choose your preferred license (MIT recommended for starters).
