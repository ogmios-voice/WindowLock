This DLL contains a class called "HwndObject".
The HwndObject class encapsulates a handful of functionalities around a windows handle to a UI object.
With HwndObject you can:
 - Search for windows and child controls registered in memory.
 - Change the titles of windows
 - Change the text of UI elements (permissions/privacy permitting)
 - Click on UI elements
 - Read the Titles of windows
 - Read the text from UI elements
 - Navigate through object hierarchies
 - Set object locations
 - Set object sizes

Here is an example of writing text to a Notepad window:


// Finds a new "Untitled" note pad window
var npad = HwndObject.GetWindowByTitle("Untitled - Notepad");
// Finds a control on the notepad window of class "Edit"
var textbox = npad.GetChild("Edit", null);
// Displays the current text to you - as if you didn't already know!
MessageBox.Show("Current text: " + textbox.Text);
// Changes the text in the Notepad window.
textbox.Text = "Here is the new text...";



This is just a very simple demonstration.
I am posting the full source code here and leaving this open for your improvements and tinkering.
As always - feel free to take credit for whatever you like and look smart on the shoulders of others. 

However - make sure to check out http://www.pinvoke.net. There is absolutely no better place on the web to go when working with interop functionality.

Cheers!
