# Unity Mask Map Generator & Separator
This package provides tooling to both generate and separate mask maps in-editor.

## Features
* OS-agnostic multithreaded texture generation
* Uncapped texture resolution (note: larger textures require more memory)
* Fully in-engine
* Per-channel fallback sliders when not using textures
* Automatically ensures input textures are readable (note: some built-in engine textures cannot be used)

## Limitations
* Input textures should be square when using the generator
* Input textures should have the same resolution when using the generator

## Planned
* Texture resampling to target resolution
* Chunked processing of large textures

<br>

## How To Install
Install via Unity's package manager using the git url of this repo. Unity provides instructions here: https://docs.unity3d.com/6000.0/Documentation/Manual/upm-ui-giturl.html
<br>

#### Once installed, navigate to Tools/um-stretch:
<img width="684" height="65" alt="image" src="https://github.com/user-attachments/assets/a01c6ce2-4cae-46ea-b875-6db660e926c0" />

<br><br>

## How To Use

### Mask Map Generator

#### Add your textures via the provided fields, or set fallback values manually:
<img width="382" height="576" alt="image" src="https://github.com/user-attachments/assets/fc3c976b-96a5-4620-9b13-cfbbedc932af" />

* "Name" is the desired file name of the generated texture.
* The "?" button opens a webpage to this repo.
* The "..." button opens a folder panel, allowing you to select your desired save location. Save location must be within the Assets folder.
* Fallback sliders can be used in place of input textures, where their value represents the opacity of the given channel (grayscale).

#### Click Generate Mask Map to create a mask map texture at the chosen save location.

<br>

### Mask Map Separator
#### Add your mask map to the "Input Texture" texture field:
<img width="382" height="576" alt="image" src="https://github.com/user-attachments/assets/5a22291f-bf2c-4a74-894a-abbd16fdc140" />

* "Name" represents both the name of the folder where textures will be extracted to, as well as the base name for the extracted textures themselves.
* The "?" button opens a webpage to this repo.
* The "..." button opens a folder panel, allowing you to select your desired save location. Save location must be within the Assets folder.


## Config
Config file can be found at `Packages/Mask Map Generator/Editor/Config.cs`
```cs
public static readonly int defaultResolution = 1024;             // Default resolution when no input textures are used.
public static readonly string defaultSaveLocation = "Assets/"    // Default path to save items.  
public static readonly bool useMultithreading = true;            // Whether multithreading should be used when generating textures.
```
NOTE: multithreading typically reduces processing time by ~40%.
