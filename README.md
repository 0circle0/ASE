# ASE
Converts .aseprite and .ase files from Aseprite into Sprite Textures for Unity3D

string path = "file:///c://Sprite-0001.aseprite";
StartCoroutine(OpenAseprite.Load(string path)); 


Creates an Aseprite Object with a header and frames.
Cels have the layer they are on assigned to them.
Cels have the sprite created.

All information from inside an aseprite file is parsed and available.

Doesn't apply any blending. Pixel data is 1:1

This is being created for Sprite Creator found @ https://facebook.com/SpriteCreator3 a free app for sprite avatar creation.
Currently working on support for Gather @ https://gather.town/ and the sprites they are making.
