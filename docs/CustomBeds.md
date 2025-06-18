# Custom Beds
## User Guide

<div align=center>
   
<img src="images/CustomBedsUser1.jpg">

</div>

### Automatic Bed-Pack Installation (Recommended)

1. Open the Settings Menu
2. Choose the Mods Option at the Bottom
3. Expand the CustomBed Config section.
4. Click the Automatic Bed Install button to install the bed pack with one click.
5. When prompted, select your Bed-Pack ZIP files in the Windows file picker and click Open
6. A message box will appear showing the installation results
7. Restart the Game
8. That's it — enjoy your new beds!


### Manual Installation

1. In the Mods tab of the Settings menu, click the "Open CustomBed Folder" button
2. Extract your ZIP files into the folder (subfolders are supported)
3. Restart the game
4. That's it — enjoy your new beds!


## Bed Developer Guide

To develop a bed pack for this mod, you’ll need to edit a texture (.png) file and a JSON configuration file.

### Requirements
- A text editor, such as the pre-installed Notepad on Windows (I personally recommend [Visual Studio Code](https://code.visualstudio.com/) from Microsoft)
- An image editor, like [GIMP](https://www.gimp.org/), [Photoshop](https://www.adobe.com/de/products/photoshop.html), or a similar program
- A basic understanding of how to use the image and text editor
- Original Texture from the Game:

<div align=center>
   
<img src="images/CustomBedsTexture.png">

</div>

### Texture Editing
To guide you in editing the texture, refer to the image below.
Each section of the image corresponds to a different part of the bed model in the game (pillow, matress and blanket).
The Different Bed types have different Components.

| Bed Type | Double Bed 1 (Bed1) | Double Bed 2 (Bed2) | Single Bed (NarrowBed) |
|----------|---------------------|---------------------|------------------------|
| Pillow 1 |         Yes         |         Yes         |           Yes          |
| Pillow 2 |         Yes         |         Yes         |           No           |
| Matress  |         Yes         |         Yes         |           Yes          |
| Blanket  |          No         |         Yes         |           Yes          |

**Note:** The second pillow is always a mirrored version of the first pillow.

<div align=center>
   
<img src="images/TextureGuideCustomBeds.png">

</div>
