# TODO

* Having the ability to generate a lot of the inital data / prefabs would make this a lot faster
* https://docs.unity3d.com/Packages/com.unity.inputsystem@0.2/changelog/CHANGELOG.html

# Project Creation

* Create a new Unity Project
* Create .gitignore
* git init

# Pre-Setup

* Create Assets/csc.rsp
  * -nowarn:0649
* Create Data/Physics/frictionless.physicMaterial
  * Static Friction: 0
  * Dynamic Friction: 0
* Create Data/Physics/frictionless 2D.physicsMaterial2D
  * Friction: 0
* Copy Art/Core/pdxparrot.png
* Copy Art/Core/progress.png
  * Texture Type: Sprite (2D and UI)

# Project Setup

* Audio Settings
* Editor Settings
  * Version Control Mode: Visible Meta Files
  * Asset Serialization Mode: Force Text
  * Default Behavior Mode: 3D for 3D, 2D for 2D
  * Line Endings: OS Native
* Graphics Settings
  * Set the Render Pipeline Asset if desired (https://github.com/Unity-Technologies/ScriptableRenderPipeline)
    * This will require creating the asset first, which itself may be configured as desired
* Input Settings
* Player Settings
  * Set the Company Name (PDX Party Parrot)
  * Set the Product Name
  * Set the Default Icon (Art/Core/pdxparrot.png)
  * Set any desired Splash Images/Logos
  * Color Space: Linear (or Gamma if targeting old mobile/console platforms)
    * Fix up any Grahics API issues that this might cause (generally this means disabling Auto Graphics APIs on certain platforms)
  * Enable Multithreaded Rendering on platforms that support it
  * Enable Static and Dynamic Batching
  * Set the Bundle Identifier
  * Scripting Runtime: .NET 4.x
  * Scripting Backend: IL2CPP
  * API Compatability Level: .NET Standard 2.0
  * C++ Compiler Configuration: Release
  * Active Input Handling: InputSystem (Preview)
  * Minimum Android API: Marshmallow
  * Target ARMv7 / ARM64 / x86 Android
  * Scripting Define Symbols
    * USE_SPINE if using Spine
* Preset Manager
* Quality
* Script Execution Order
  * Set in the Script Execution Order section later
* Tags and Layers
  * Add a PostProcessing layer if it doesn't already exist
  * Add a NoPhysics layer
  * Add a Vfx layer
  * Add a Viewer layer
  * Add a Player layer
  * Add an NPC layer
  * Add a World layer
  * Add a Weather layer
* Physics Settings
  * Set the Default Material to frictionless if desired
  * Only enable the minimum necessary collisions
    * **TODO:** water?
    * Vfx -> Vfx
    * Viewer -> Weather, World
    * Player -> Weather, World, NPC
    * NPC -> Weather, World
    * World -> Weather
* Physics 2D Settings
  * Set the Default Material to frictionless if desired
  * Only enable the minimum necessary collisions
    * **TODO:** water?
    * Vfx -> Vfx
    * Viewer -> Weather, World
    * Player -> Weather, World, NPC
    * NPC -> Weather, World
    * World -> Weather
* TextMesh Pro
  * Import TMP Essentials if not already done
  * Optionally import TMP Examples & Extras if desired
* Time Settings
* VFX Settings

# Packages

* Update default packages
* Remove default packages
  * Ads
  * Analytics Library
  * In App Purchasing
  * Unity Collaborate
* Add release packages
  * Asset Bundle Browser
  * Core RP Library
  * Multiplayer HLAPI
    * **TODO:** This is deprecated and should be replaced
  * Post Processing
  * ProBuilder
  * Shader Graph
* Add preview packages
  * Android Logcat if building for Android
  * Input System (https://github.com/Unity-Technologies/InputSystem)
    * Project Settings -> Input System Package
      * Create settings asset
  * ProGrids
  * HD/Lightweight Render Pipeline (optional - whichever best fits the project)
  * Burst/Entities (if using ECS)
  * Visual Effect Graph
* Add Keijiro Kino
  * Add "jp.keijiro.kino.post-processing": "https://github.com/keijiro/kino.git#upm" to package manifest.json dependencies
* Add desired assets
  * DOTween (not Pro)
    * Make sure to run the setup
    * Make sure to create ASMDEF
* If using Spine, download the latest Spine-Unity package and import it
  * Assets/Spine* must be added to the .gitignore to prevent committing this
  * It may be necessary to create an ASMDEF in Spine/Runtime for this package

# Engine Source

## Core Scripts

* Copy Core Scripts
* Create ASMDEF
  * Scripts/Core/com.pdxpartyparrot.Core.asmdef
    * References: Unity.InputSystem, com.unity.multiplayer-hlapi.Runtime, Unity.Postprocessing.Runtime, Unity.TextMeshPro, Kino.Postprocessing
      * Create and a reference to a spine-unity ASMDEF if necessary
  * Scripts/Core/Editor/com.pdxpartyparrot.Core.Editor
    * Editor platform only
    * References: com.pdxpartyparrot.Core.asmdef, Unity.TextMeshPro
* Clean up TODOs as necessary
* Remove any FormerlySerializedAs attributes

## Game Scripts

* Copy Game Scripts
* Create ASMDEF
  * Scripts/Game/com.pdxpartyparrot.Game.asmdef
    * References: com.pdxpartyparrot.Core.asmdef, Unity.InputSystem, com.unity.multiplayer-hlapi.Runtime, Unity.TextMeshPro
  * Scripts/Game/Editor/com.pdxpartyparrot.Game.Editor
    * Editor platform only
    * References: com.pdxpartyparrot.Game.asmdef
* Clean up TODOs as necessary
* Remove any FormerlySerializedAs attributes

## Editor Scripts

* Copy Editor Scripts
* Create ASMDEF
  * Scripts/Editor/com.pdxpartyparrot.Editor.asmdef
    * Editor platform only
* Clean up TODOs as necessary
* Remove any FormerlySerializedAs attributes

## Initial Project Scripts

* Create Loading Manager
  * Create a new project LoadingManager script that overrides Game LoadingManager
* Create ASMDEFs
  * Scripts/{project}/com.pdxpartyparrot.{project}.asmdef
    * References: com.pdxpartyparrot.Core.asmdef, com.pdxpartyparrot.Game.asmdef, Unity.InputSystem

## Set Script Execution Order

* TextMeshPro
* InputSystem PlayerInput
* pdxpartyparrot.{project}.Loading.LoadingManager
* pdxpartyparrot.Core.Time.TimeManager
* pdxpartyparrot.Game.State.GameStateManager
* Default Time
* pdxpartyparrot.Core.Debug.DebugMenuManager
  * This must be run last

# Engine Asset Setup

* Create Data/Animation/empty.controller Animation Controller
* Create Data/Audio/main.mixer Mixer
  * 3 Master child groups
    * Music
      * Expose the Volume parameter and set it to -5db
        * Rename it to MusicVolume
    * SFX
      * Expose the Volume parameter and set it to 0db
        * Rename it to SFXVolume
    * Ambient
      * Expose the Volume parameter and set it to -10db
        * Rename it to AmbientVolume
  * Expose the Master Volume parameter and set it to 0db
    * Rename it to MasterVolume
  * Add a Lowpass filter to the Master group
  * Rename the default Snapshot to Unpaused
  * Create a new Snapshot and name it to Paused
    * Set the Lowpass filter to 350Hz
* Copy button-click.mp3 and button-hover.mp3 to Data/Audio/UI
* Create Data/Input/ServerSpectator.inputactions
  * Generate C# Class
    * File: Assets/Scripts/Game/Input/ServerSpectatorControls.cs
      * Need to create containing directory first
    * Class Name: ServerSpectatorControls
    * Namespace: pdxpartyparrot.Game.Input
  * Action Maps
    * ServerSpectator
      * Actions
        * move forward button
          * press and release w
        * move backward button
          * press and release s
        * move left button
          * press and release a
        * move right button
          * press and release d
        * move up button
          * press and release space
        * move down button
          * press and release left shift
        * look axis
          * mouse delta
  * Add ENABLE_SERVER_SPECTATOR to the Scripting Define Symbols
* Data/Prefabs/Input/EventSystem.prefab
  * Create using default EventSystem that gets added automatically when adding a UI object
  * Replace Standalone Input Module with InputSystemUIInputModule
  * Add EventSystemHelper script to this
* Create Data/Prefabs/Lighting/GlobalLighting.prefab
  * Add a Direction Light under this
    * Set X Rotation to 45
  * **TODO:** This should get hooked up somewhere, right?

## Manager Prefabs Setup

* Managers go in Data/Prefabs/Managers
* ActorManager
  * Create an empty Prefab and add the ActorManager component to it
* AudioManager
  * Create an empty Prefab and add the AudioManager component to it
  * Create an AudioData in Data/Data and attach it to the manager
    * Attach the main mixer to the data
    * Ensure all of the Parameters look correct
  * Add 4 Audio Sources to the prefab
    * Disable Play on Awake
  * Attach each audio source to an audio source on the AudioManager component
* DebugMenuManager
  * Create an empty Prefab and add the DebugMenuManager component to it
* EffectsManager
  * Create an empty Prefab and add the EffectsManager component to it
* EngineManager
  * Create an empty Prefab and add the PartyParrotManager component to it
  * Create a UIData in Data/Data and attach it to the manager
    * Attach a TMP_Font Asset to the Default font
      * LiberationSans SDF is currently the default TMP font
  * Attach the frictionless physics materials
  * Set the UI layer to UI
* GameStateManager
  * Create an empty Prefab and add the GameStateManager component to it
  * Create an empty Prefab in Data/Prefabs/State and add the MainMenuState component to it
    * Set the Scene Name to main_menu
    * Set the MainMenuState as the Main Menu State Prefab in the GameStateManager
  * Create an empty Prefab in Data/Prefabs/State and add the NetworkConnectState component to it
    * Set the NetworkConnectState as the Network Connect State Prefab in the GameStateManager
  * Create an empty Prefab in Data/Prefabs/State and add the SceneTester component to it
    * Check Make Scene Active
    * Set the SceneTester as the Scene Tester Prefab in the GameStateManager
* InputManager
  * Create an empty Prefab and add the InputManager component to it
  * Attach the EventSystem prefab
* LocalizationManager
  * Create an empty Prefab and add the LocalizationManager component to it
  * Create a LocalizationData in Data/Data and attach it to the manager
* NetworkManager
  * Create an empty Prefab and add the (not Unity) NetworkManager component to it
  * Uncheck Don't Destroy on Load
* ObjectPoolManager
  * Create an empty Prefab and add the ObjectPoolManager component to it
* SaveGameManager
  * Create an empty Prefab and add the SaveGameManager component to it
  * Set the Save File Name
* SceneManager
  * Create an empty Prefab and add the SceneManager component to it
* SpawnManager
  * Create an empty Prefab and add the SpawnManager component to it
  * Create a SpawnData in Data/Data and attach it to the manager
* TimeManager
  * Create an empty Prefab and add the TimeManager component to it
* UIManager
  * Create an empty Prefab and add the UIManager component to it
* ViewerManager
  * Create an empty Prefab and add the ViewerManager component to it

## GameManager

* Create a new GameManager script that overrides the Game GameManager
  * Implement the required interface
* Add a connection to the project GameManager in the project LoadingManager
  * Override CreateManagers() in the loading manager to create the GameManager prefab
* Create a new GameData script that overrides the Game GameData
* Create an empty Prefab and add the GameManager component to it
* Create a GameData in Data/Data and attach it to the manager
  * Set the Viewer Layer to Viewer
  * Set the World Layer to World
  * Configure the Players section as desired

# Splash Scene Setup

* Create and save a new scene (Scenes/splash.unity)
  * The only object in the scene should be a Main Camera
* Setup the camera in the scene
  * Clear Flags: Solid Color
  * Background: Opaque Black
  * Culling Mask: Nothing
  * Projection: Orthographic
  * Uncheck Occlusion Culling
  * Disable HDR
  * Disable MSAA
  * Leave the Audio Listener attached to the camera for audio to work
  * Add the UICameraAspectRatio component to the camera
* Remove the Skybox Material
* Environment Lighting Source: Color
* Disable Realtime Global Illumination
* Disable Baked Global Illumination
* Disable Auto Generate lighting
* Add the scene to the Build Settings and ensure that it is Scene 0
* Add a new GameObject to the scene (SplashScreen) and add the SplashScreen component to it
* Attach the camera to the Camera field of the SplashScreen component
* Add whatever splash screen videos to the list of Splash Screens on the SplashScreen component
* Set the Main Scene Name to match whatever the name of your main scene is
  * The main scene should also have been added (or will need to be added) to the Build Settings along with any other required scenes

# Main Scene Setup

* Create and save a new scene (Scenes/main.unity)
  * The only object in the scene should be a camera
* Setup the camera in the scene
  * Clear Flags: Solid Color
  * Background: Opaque Black
  * Culling Mask: Nothing
  * Projection: Orthographic
  * Uncheck Occlusion Culling
  * Disable HDR
  * Disable MSAA
  * Leave the Audio Listener attached to the camera for audio to work
  * Add the UICameraAspectRatio component to the camera
* Remove the Skybox Material
* Environment Lighting Source: Color
* Disable Realtime Global Illumination
* Disable Baked Global Illumination
* Disable Auto Generate lighting
* Add the scene to the Build Settings

## Loading Screen Setup

* Add a new LoadingScreen object to the scene with the LoadingScreen component
  * Layer: UI
  * Add a new Canvas object below the LoadingScreen and attach it to the LoadingScreen
    * UI Scale Mode: Scale With Screen Size
    * Reference Resolution: 1280x720
    * Match Width Or Height: 0.5
    * Remove the Graphic Raycaster
    * Remove the EventSystem object that gets added (or turn it into a prefab if that hasn't been created yet)
* Add a Panel under the Canvas
  * Disable Raycast Target
  * Color: (255, 0, 255, 255)
* Add a TextMeshPro - Text (Name) under the Panel
  * Text: "Placeholder"
  * Center the text
  * Disable Raycast Target
* Add an Empty GameObject (Progress) under the Panel and add the ProgressBar component to it
  * Pos Y: -125
* Attach the ProgressBar component to the LoadingScreen component
* Add an Image under the Progress Bar (Background)
  * Move the image below the Name text
  * Color: (0, 0, 0, 255)
  * Size: (500, 25)
  * Source Image: Core Progress Image
  * Disable Raycast Target
* And an Image under the Background Image (Foreground)
  * Position: (0, 0, 0)
  * Size: (500, 25)
  * Source Image: Core Progress Image
  * Disable Raycast Target
  * Image Type: Filled
  * Fill Method: Horizontal
  * Fill Origin: Left
  * Fill Amount: 0.25
* Attach the images to the ProgressBar component
* Add a TextMeshPro - Text (Status) under the Progress Bar
  * Pos Y: -75
  * Size: (750, 50)
  * Text: "Loading..."
  * Center the text
  * Disable Raycast Target
* Attach the Text to the LoadingScreen component

## Loader Setup

* Add an empty GameObject (Loader) and add the project LoadingManager component to it
* Attach the Main Camera
* Attach the LoadingScreen to the Loader
* Attach the Manager prefabs to the Loader

# Main Menu Setup

* Create a new MainMenu script that overrides the Game MainMenu
* Create a MainMenu Prefab in Prefabs/Menus and add the Game Menu component to it
  * Layer: UI
  * Add a Canvas under the prefab
    * UI Scale Mode: Scale With Screen Size
    * Reference Resolution: 1280x720
    * Match Width Or Height: 0.5
    * Set the Canvas on the Menu object
  * Add a Panel under the Canvas (Main)
    * Remove the Image
    * Add a Vertical Layout Group
      * Spacing: 10
      * Alignment: Middle Center
      * Child Controls Width / Height
      * No Child Force Expand
    * Add the MainMenu script to the panel
      * Set Owner to the Menu object
      * Set the Main Panel on the Menu object to the Main panel
  * Add a Button (TextMeshPro) under the Main panel (Start)
    * Normal Color: (255, 0, 255, 255)
    * Highlight Color: (0, 255, 0, 255)
    * Add an On Click handler that calls the MainMenu OnStart method
    * Add a Button Helper to the button and setup EffectTriggers to play the hover and click audio
    * Add a Layout Element to the Button
      * Preferred Width: 200
      * Preferred Height: 50
      * Text: "Start"
      * Center the text
      * Disable Raycast Target
    * Set the Main Menu Initial Selection to the Start Button
  * **TODO:** Multiplayer
  * **TODO:** Character Select
  * Duplicate the Play Button (High Scores)
    * Set the On Click handler to the MainMenu OnHighScores method
    * Set the Text to "High Scores"
    * **TODO:** Setup the High Scores panel
  * Duplicate the Play Button (Credits)
    * Set the On Click handler to the MainMenu OnCredits method
    * Set the Text to "Credits"
    * **TODO:** Setup the Credis panel
  * Duplicate the Credits Button (Quit)
    * Set the On Click handler to the MainMenu OnQuitGame method
    * Set the Text to "Quit"
  * Attach the MainMenu prefab to the MainMenuState Menu Prefab

## Main Menu Scene Setup

* Create and save a new scene (Scenes/main_menu.unity)
  * The only object in the scene should be a camera
* Setup the camera in the scene
  * Clear Flags: Solid Color
  * Background: Opaque Black
  * Culling Mask: Everything
  * Projection: Orthographic
  * Uncheck Occlusion Culling
  * Disable HDR
  * Disable MSAA
  * Remove the Audio Listener
* Remove the Skybox Material
* Environment Lighting Source: Color
* Disable Realtime Global Illumination
* Disable Baked Global Illumination
* Disable Auto Generate lighting
* Add the scene to the Build Settings
* Add a new TitleScreen object to the scene
  * Layer: UI
  * Add a new Canvas object below the TitleScreen
    * UI Scale Mode: Scale With Screen Size
    * Reference Resolution: 1280x720
    * Match Width Or Height: 0.5
    * Remove the Graphic Raycaster
    * Remove the EventSystem object that gets added (or turn it into a prefab if that hasn't been created yet)
  * Add a Panel under the Canvas
    * Disable Raycast Target
    * Color: (255, 0, 0, 255)
  * Add a TextMeshPro - Text (Status)
    * Pos Y: 256
    * Text: "Placeholder"
    * Center the text
    * Disable Raycast Target
  * The scene should now load when the main scene is run as long as the name of the scene matches what was set in the MainMenuState prefab

# Initial Game State Setup

## TODO: MainGameState
## TODO: GameOverState

## Game Data

* Create a new GameData script that overrides Game GameData and adds an Asset Menu item for it
* Create a new GameData data object
  * Set the World Layer to World
  * Create and attach a ServerSpectator prefab if desired
    * **TODO:** Configure this
    * **TODO:** Create a viewer prefab for it

## Player Data

* Create a new PlayerData script that overrides Game PlayerData and adds an Asset Menu item for it
* Create a new PlayerData data object
  * Set the Player Layer to Player
  * Set the Viewer Layer to Viewer
  * **TODO:** Create a viewer prefab for it
    * Create a new Viewer script that overrides a Core Viewer (? Core or Game?)
    * Create an empty Prefab and add the Viewer component to it
      * Add a camera under the prefab
        * Clear Mode: Sky
        * Background Color: Default
        * Projection: Depends on viewer needs
        * Remove the Audio Listener
        * Add a Post Process Layer component to the Camera object
        * Add an Aspect Ratio component to the Camera (UI) object
      * Add another camera under the prefab (UI)
        * Layer: UI
        * Clear Mode: None
        * Culling Mask: UI
        * Projection: Orthographic
        * Remove the AudioListener
        * Add an Aspect Ratio component to the Camera (UI) object
      * Add an empty GameObject under the prefab and add a Post Process Volume to it
      * Attach the Cameras and the Post Process Volume to the Viewer component
      * **Create the Post Process Layer (one per-viewer, Viewer{N}_PostProcess)**
* Create a new GameState subclass and attach it to a new empty Prefab
  * This state should probably get the ViewerManager and InputManager state setup
* Attach the new GameState prefab to the GameStateManager prefab
* **TODO:** More GameStates
* **TODO:** Pause / Pause Menu
* **TODO:** Create the PlayerManager script/prefab
  * This must be a prefab due to the abstract base class
* **TODO:** Create the Player script/prefab
* **TODO:** How to controls
* **TODO:** Creating Data
* **TODO:** Credits

# Performance Notes

* Mark all static objects as Static in their prefab editor
