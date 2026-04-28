# DIPT
Did I Press That (DIPT) is a free Unity plugin that provides pre-built debugging tools for common game controllers, such as the Xbox and Playstation 4
controller.
These debugging tools include:
- A mock controller GUI that includes:
  - Input visualization, showing when buttons are pressed down, the positions of joysticks, etc.
  - Displayed values of these inputs, like the exact position of the joystick.
  - Input emulation, allowing you to use components of the GUI with your mouse (for testing controller specific inputs, like joystick walking speed).
  - Automatic controller detection, opening the appropriate GUI for each of them.
- An input log:
  - Tracks when buttons are pressed and released, alongside how long they were pressed.
  - Gives values of the joysticks at certain intervals.
  - Allows manual saving of the log file (to prevent clogging up your system).
  - KeyboardMapper which allows users to control emulation and visualizer
# HOW TO INSTALL:
1. Install Unity 6.3 LTS (6000.3.9f1)
2. Download the ZIP file from GitHub.
3. Extract it and copy the Plugins folder found in Assets > Plugins into your own Unity project’s Assets folder.
  - This will bring only the plugin into your project for you to use as you please.
5. Wait for Unity to import your project and open the project.
6. Navigate to the toolbar at the top of the Unity Editor Window and hover over Tools then DIPT then you can open our 3 tools: InputVisualizer, LogPlugin, and KeyboardMapper

# HOW TO TEST:
1. Open the project in Unity after downloading
2. Add the tests folder from the ZIP file to the Unity project folder
3. Navigate to the toolbar at the top of the Unity Editor Window and hover over Window then General then click Test Runner to open the window
4. The tests can be revealed with the dropdown and then can be ran with the buttons at the bottom of the window.

# TESTS CONTINUED:
1. If you want a quick play mode tester include the GameModeTestFiles folder and paste in the Unity project assets then dobule click TestScene
2. After following all HOW TO INSTALL steps to open the visualizer window, you can then use a controller or the emulator to see it fully emulate button inputs

# FUTURE INSTALLATION:
- In the future, we plan to streamline the installation process by allowing importing through both the Unity Asset store and the editor’s built-in package manager. 
- To install via package manager, you would navigate to Window > Package Management > Package Manager. From there, click the + v icon in the top left corner, then click Install Package from git URL. It will then give you an input for which you should put a github link similar to the one above into it to install the package (this link does not exist yet and the one above will not work). This will install only the plugin, not including the demo.
- To install via Unity Asset store, you would then be able to find our project on the asset store with its own link, in which you can simply download and import from the interface into our project (this method also does not work yet, as we do not have an Unity Asset Store link yet). This will also install only the plugin, not including the demo.
