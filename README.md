![screenshot](/images/layout_keyboard.png?raw=true)
![screenshot](/images/layout_media.png?raw=true)
![screenshot](/images/layout_blender.png?raw=true)

# OpenKeyboard

**Purpose**:
This keyboard app is meant for Windows tablet users. The default keyboard starting with windows 8 is pretty big which takes up quite a bit of screen space. So originally this was to create a small and useful keyboard that expanded into being able to create any sort of keyboard that can run short cuts. For example, there is a Blender3D layout that has many useful quick keys setup to perform many functions within the application. This is great if you are modeling using a Surface Pro and want to be able to quickly use the touch screen instead of the keyboard to activiate a few functions.


**Feature list**:

 * Custom Keyboard Layouts by add/editing XML files
 * Resizable, keys will resize along with the window.
 * Opacity can be set so its out of the way
 * Custom popup menus on any button to save space on less used items.
 * Uses Font-Awsome to create icons for buttons.
 * Portable, so just unzip and run.

___

### Create New Layout
The easiest way is to probably take an existing layout and start tweaking it. The way the layout works is by Rows, then you add all the keys you want for each row. You are able to give weight to keys so some will appear longer then others. You can also assign a custom menu on a button if you want to avoid making your layout to big.

Another great feature is to set the border around the application. This allows you to set where its easier to touch drag the keyboard around. Depending on the layout you might want the border to be thicker on the side instead of at the top.

Here a simple example of a layout xml file.

```xml
<?xml version="1.0" encoding="utf-8"?>
<layout width="150" height="200" vpos="center" hpos="right" margin="6,20,6,6">
  <row>
    <key code="PGUP" text="\uf102" fsize="20"/>
    <key code="ENTER" text="Enter" weight="2"/>
    <key code="PGDOWN" text="\uf103" fsize="20"/>
  </row>

  <row>
    <menu text="\uf078" fsize="16">
      <key code="LCTRL c" text="Copy"/>
      <key code="LCTRL x" text="Cut"/>
      <key code="LCTRL v" text="Paste"/>
      <key code="LCTRL a" text="SelAll"/>
    </menu>
  </row>
</layout>
```

**Things that may be of help:**
* [FontAwesome CheetSheet](http://fortawesome.github.io/Font-Awesome/cheatsheet/)

