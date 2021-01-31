# ASE
Converts .aseprite and .ase files from Aseprite into Sprite Textures for Unity3D

## Usage:
```
using ASE;

string path = "file:///c://Sprite-0001.aseprite";
StartCoroutine(OpenAseprite.Load(string path));
```

### Output
Creates an Aseprite Object that constains all the Frames with in it.
Sprites are also automatically created when a CelChunk is found.
Each layer and each frame is rendered individually nothing is combined.
All information from inside an aseprite file is parsed and available.

#### What it doesn't do
Doesn't apply any blending. Pixel data is 1:1
CelChunk sprite will be the Header size and not the CelChunk size. Image unaffected.
Does not create animations from frame data.
Output Cel Layers are not combined.

##### Support
This is being created for Sprite Creator found @ https://facebook.com/SpriteCreator3 a free app for sprite avatar creation.

###### Other peoples projects
Currently working on support for Gather @ https://gather.town/ and the sprites they are making.
