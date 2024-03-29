# Simple Audio Manager

## Installing

### Using git

- Open the Package Manager window
- Click on the "plus" sign
- Select "Add package from git URL"
- Paste this repo's SSH link (<git@github.com>:PixelRouge/AudioManagerForUnity.git)

### Editing manifest file

Add the following line to your `manifest.json` found in the "Packages" folder

`"com.pixelrouge.audiomanagerforunity": "git@github.com:PixelRouge/AudioManagerForUnity.git"`

## How to use

Add the script `AudioManager.cs` to an object created at the start of the game. This script is a Singleton and will prevent its object from being destroyed.

Example use:

`AudioManager.Instance.PlaySfx(_swordAttack01);`

### Background music

`public void PlayBgm(AudioClip clip, bool crossFade = true, bool forceRestart = false)`

`public void ToggleBgm()`

`public void StopBgm(bool fadeOut = true, float fadeOutDuration = 3f)`

### Sound effects

`public void PlaySfx(AudioClip clip, bool loop = false)`

`public void PlaySfx(AudioClip clip, Vector2 position, bool loop = false)`

`public void PlaySfx(AudioClip clip, Vector3 position, bool loop = false)`

`public void StopAllSfx()`

### Voice

`public void PlayVoice(AudioClip clip)`

`public void PlayVoice(AudioClip clip, Vector2 position)`

`public void PlayVoice(AudioClip clip, Vector3 position)`

`public void StopAllVoice()`

## Why can't I open the demo scene?

This is a limitation known by the Unity team. Until they look at it, you will have to drag the scene file into somewhere in your "Assets" folder.

### Extra

- A script called `AudioSettingsMenu.cs` can be used to speed up the implementation of a audio settings menu.
- There is a demo scene to play around with the manager.

## Credits (Demo)

Graphics: [kenney.nl](https://kenney.nl)

Font: [kenney.nl](https://kenney.nl)

Audio effects and voiceovers: [kenney.nl](https://kenney.nl)

Music: [freepd.com](https://freepd.com) (by Kevin MacLeod, Anonymous and dogsounds.)
