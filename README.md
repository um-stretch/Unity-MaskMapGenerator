# Unity Mask Map Generator
An in-editor mask map generator for the Unity engine. 

## Features
* OS-agnostic multithreaded texture generation
* Uncapped texture resolution (note: large textures consume more memory)
* Fully in-engine
* Per-channel fallback sliders when not using textures
* Automatically ensures input textures are readable (note: some built-in engine textures cannot be used)

## Limitations
* Input textures should be square
* Input textures should have the same resolution

## Planned
The following features are planned for development.
* Texture resampling to target resolution
* Chunked processing of large files
* Expanded handling of non-readable textures and unusable pixels

<br>

## How To Use
Install via Unity's package manager using the git url of this repo. Unity provides instructions here: https://docs.unity3d.com/6000.0/Documentation/Manual/upm-ui-giturl.html
<br><br>

#### Once installed, navigate to Tools/Mask Map Generator:
<img width="684" height="65" alt="image" src="https://github.com/user-attachments/assets/c270c58d-ba69-43d6-86f4-ce0e084f5c20" />

<br><br>

#### Add your textures via the provided fields, or set fallback values manually:
<img width="382" height="576" alt="image" src="https://github.com/user-attachments/assets/fc3c976b-96a5-4620-9b13-cfbbedc932af" />

* "Name" is the desired file name of the generated texture.
* The "?" button opens a webpage to this repo.
* The "..." button opens a folder panel, allowing you to select your desired save location. Save location must be within Assets folder.
* Fallback sliders can be used in place of input textures, where their value represents the opacity of the given channel (grayscale).

#### Click Generate Mask Map to create a mask map texture at the chosen save location.

## Config
Config file can be found at `Packages/Mask Map Generator/Edior/Config.cs`
```cs
public static readonly int defaultResolution = 1024;        // Default resolution when no input textures are used.
public static readonly bool useMultithreading = true;       // Whether multithreading should be used when generating textures.
```
NOTE: multithreading typically reduces generation time by ~40%.
