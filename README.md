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
- A list of all possible inputs from connected controller.
# HOW TO INSTALL:
1. Install Unity 6.3 LTS (6000.3.9f1)
2. Download the ZIP file from GitHub.
3. Extract the contents to preferred file location.
4. In the Unity Hub, add project from disk and select the extracted folder. This also includes the dependencies for the project.
5. Wait for Unity to import the project and open the project.
6. Navigate to the toolbar at the top of the Unity Editor Window and hover over Tools then DIPT then you can open our 3 tools: ControllerDectection, InputVisualizer, LogPlugin

# HOW TO TEST:
1. Open the project in Unity after downloading
2. Navigate to the toolbar at the top of the Unity Editor Window and hover over Window then General then click Test Runner to open the window
3. The tests can be revealed with the dropdown and then can be ran with the buttons at the bottom of the window.
