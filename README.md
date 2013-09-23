The Problem
===========
Regardless if it is development or a simple search in a given topic, I usually open new and work with several browser
windows. With the age of HD displays I rarely use maximized windows any more, I do prefer however to fill my screen
vertically as much as possible and the ability to navigate easily (i.e. from keyboard) between the running applications.
I encountered however the following problems during my everyday work:
- while most application will save the position of their current window, they will let the OS decide about the position
of newly created windows: as a result these new windows will be placed 10-30 pixels to the left and to the bottom
compared to the position of the original window, and will either hide other windows, or be partially hidden
(e.g. by the task bar), or even partially off-screen.
- vertically maximizing the windows using Win+Shift+Up is less useful when you (too) prefer using Win+Shift+Down
(as the combo will trigger a restore first and will minimize the window only for the second press)
- reposition the new windows with mouse is too time consuming

The above problem is present for a wide range of applications (Windows Explorer, Internet Explorer, Opera, Chrome,
Acrobat Reader, WinWord, Excel, etc.). 


WindowLock
==========
As a solution I've decided to write my own tool that will automatically detect the opening of new windows, and will
move them back to the position I've previously set for the given application.


How does it work
================
WindowLock will query the list of windows from the OS and in case of applications with multiple windows it will search
for the top-left most one (within a given range) marking it as a main window, and then will reposition all of the other
windows to the position of this main window.
Since having multiple monitors or large monitors is more and more common WindowLock supports multiple main windows
within the same application: thus having one browser on the left side of the screen and another window on the right side
of the screen will create two separate groups for repositioning the newly created windows.


Build
=====
Since my main focus is on the browsers I work with, the current version was build for 32 bit:
- the <ins>window hook</ins> method will work only for 32 bit applications
- for managing the windows of 64bit applications (e.g. Windows Explorer on a 64bit OS) you have to use
<ins>window polling</ins>.


The application is composed of several assemblies that were included in the main executable as resources.
However the library containing the hook procedure could not be merged with the application.
(http://msdn.microsoft.com/en-us/library/windows/desktop/ms644960(v=vs.110).aspx)


Settings
========
All settings are **automatically saved** on exit (to C:\Users\<USERNAME>\AppData\Local\OgmiosVoice\WindowLock32.exe_Url_*\\).
- **AutoFix method**:
  - **None**
  - **Window Hook**: is limited to either 32 bit or 64 bit applications only, and it detects only the creation of new windows.
WindowLock x86 (32 bit) will be able to hook into 32 bit applications only, while the WindowLock x64 will be able to
hook into 64 bit applications only, thus no single build will be able to handle all of the applications using the
window hook method.
  - **Window Polling**: regularly (currently every 1 sec) queries the list of windows form the system, updates the list
displayed in WindowsLock, and fixes the position of newly created or moved windows.
(Thanks to some internal caching this solutions uses ~0.1% CPU. (tested on Intel Core i5-2500))
  - **All**: uses both the window hook and the polling methods, providing basically a faster initial fix
- **Size range**: the possible size difference between the main window and the windows belonging to it
- **Position range**: the number of pixels from the pixels from the top-left corner of the main window, where newly created
and moved windows will be detected
- **Filters / Process Name**: a list of regular expressions for filtering out the processes WindowLock should monitor
('.*' = all processes). Comments are started with '#'.


Known issues
============
- Windows with **0x0 dimension** are not filtered out yet
- **Size range** does not work correctly
- Windows of applications started as a **different user** are not always detected correctly
- The **SysTray popup** is hidden only if you click on the icon of the application, or on selecting any of the menu items
- **Memory consumption** is too high (60-70Mb) compared to my initial expectations (this might as well be a C# / WPF issue)


Future Plans
============
- Ability to **select (force) the main window** (for a given process), and move and resize the rest of the windows based on it.
- **Activate/Minimize/Restore windows** from the list displayed in WindowLock
