# Infinitecraft (Unity 2D)

This repository contains a Neal.fun–inspired Infinite Craft game built in Unity 2D.

Quick start
1. Clone the repo and open it in Unity 2021.3 LTS (recommended) or newer.
2. Switch to the feature branch `feature/complete-infinitecraft`.
3. Open the scene `Assets/Scenes/InfinitecraftDemo.unity`.
4. If prompted, import TextMeshPro essentials.
5. Press Play to run the demo. The UI is wired to the ElementCombiner systems.

Files of interest
- Assets/Scripts/ElementCombiner.cs — core game logic and persistence
- Assets/Scripts/ElementCombinerDemoUI.cs — simple UI glue and selection flow
- Assets/Scripts/DiscoveryGridUI.cs, ElementCellUI.cs — discovery grid cells
- Assets/Scripts/SaveExportUtility.cs — export/import helpers
- Assets/Scenes/InfinitecraftDemo.unity — demo scene (placeholder/wired)

License
This project is provided under the MIT License. See LICENSE for details.
