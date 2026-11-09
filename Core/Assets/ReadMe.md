# Asset Manager
**by Maximilian Schecklmann**

### I: Assets
Asset identifactors are assigned via the file name excluding their extension.
They have to be placed in a directory nested in assigned asset directories you set in the script loading the assets.

### II: Modding
This setup neatlessly allows for modding as it reads all files in the given directories and prefers files read last.

### III: Usage
Usually you first clear the assets via ```ClearAssets()``` and then load anew via the ```LoadAssets(params string[])``` method.