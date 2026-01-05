# UI Setup for Infinitecraft Demo (Unity 2D)

This guide walks you through creating a scene that mimics Neal.fun Infinite Craft flow using the scripts included in the repo.

1) Open the scene `Assets/Scenes/InfinitecraftDemo.unity`.

2) If the scene is placeholder, follow the in-scene setup instructions to create Canvas, Grid, and UI elements. Otherwise the scene should be wired.

3) Create a Canvas (GameObject -> UI -> Canvas) and set Render Mode to Screen Space - Overlay.

4) Create a left-side Panel for the discovery grid using a Scroll View. On the `Content` Rect add a GridLayoutGroup.

5) Create a Button prefab for element cells with a TextMeshProUGUI child. Add the `ElementCellUI` component to the root of the prefab and assign the `nameLabel` field.

6) Create selection input fields (TMP InputField) for slot A and slot B, a Combine Button, and a Result panel with TextMeshPro fields for the resulting element's name and description.

7) Create an empty GameObject `GameManager` and attach `ElementCombiner`. Configure starting elements if desired.

8) Create an empty GameObject `UIManager` and attach `ElementCombinerDemoUI`. Assign the `combiner` reference and all input/result fields.

9) Create a `DiscoveryGrid` GameObject and attach `DiscoveryGridUI`. Assign `combiner`, `contentParent` and `elementCellPrefab`.

10) Press Play. Use the grid to select two elements and press Combine to discover new elements.
