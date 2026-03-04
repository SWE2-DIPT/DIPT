# DIPT
Did I Press That (WIP)

# HOW TO USE:
To use this plugin, you must:
    1.) Open this project (the DIPT directory) in Unity.
    2.) Go to Tools > DIPT > TestPlugin from the menu bar (the bar with File, Edit, Assets, etc).
Right now, clicking TestPlugin does not do anything. Feel free to put some test code in there
to verify it works (this may have to be in a Start() function).

# IMPORTANT
For Unity to recognize this file as a plugin, it must be in a folder named "Editor".

Below is an XML comment, it will display in VS & VSCode when hovering over
a field/method. I will use these whenever creating a method that will be called
in another file.

/// <summary>
/// When you put this right before a class, method, or variable, it will be displayed
/// when you hover over it in VS or VSCode. In this case, hovering over example would
/// show this text.
/// </summary>
public class Example
{
...
}