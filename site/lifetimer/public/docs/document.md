# LifeTimer Documentation

## Overview

This is the documentation page for LifeTimer.

LifeTimer is a Windows utility that runs a highly configurable clock on your desktop, where it can blend flawlessly with your
windows theme and desktop wallpaper. LifeTimer also lets you track the time to (or from) significant events in your life.

Do you want to know how many days you have been alive? Want to know how long until your wedding? Do you need to know the the minute when Halley's Comet will return? LifeTimer has your covered!

## Key Features

#### Highly Configurable Desktop Clock

- Display a highly configurable clock on your desktop
- Configure fonts, colors, borders and transparency
- Blends perfectly with your desktop theme.
- Supports autostart

#### Multiple Timer Tracking

- Tracks the time to (and from) significant life events
- Extensive timer configuration options
- Timers can rotate in sequence

#### Fully resizeable

- Timer display can be resized and positioned anywhere on your desktop

## Getting Started

### Installation

1. Download LifeTimer from the Windows Store
2. Launch the application
3. Position and resize the Timer Window
4. Configure the appearance
5. Configure your timers
6. Configure your startup settings
7. Save your settings

## Concepts

### User Interface

The LifeTimer user interface consists of two windows:

1. **Timer Window**: Displays the clock display. Can be positioned and resized.
2. **Settings Window**: Used to configure the application

![LifeTimer Windows](/images/docs/doc_interface_windows.png)

### Operating Modes

LifeTimer has two operating modes. These modes control how the Timer Window is displayed.

1. **Interactive Mode**

In Interactive Mode, the Timer Window acts like a regular window. Use this mode to position and resize the timer on your desktop.

![LifeTimer Interactive Mode](/images/docs/doc_interactive_mode.png)

2. **Background Mode**

In Background Mode, the Timer Window becomes transparent, moves behind all other windows and ignores user input. The Timer Window acts like desktop wallpaper.

![LifeTimer Background Mode](/images/docs/doc_background_mode.png)

### System Tray Icon

The LifeTimer System Tray Icon can be used to switch operating modes, open the Settings Window or exit the application.
This enables the application to be controlled when the it is running in Background Mode and the Settings Window is closed.

![LifeTimer System Tray Icon](/images/docs/doc_system_tray_icon.png)

## LifeTimer Settings

The LifeTimer Settings Window is used to configure the behaviour of the application. The Settings Window has a side
navigation menu that can be used to switch between different pages. The following pages are available:

1. **Command Page**: Switch between modes. Save settings. Exit.
2. **Timers Page**: Configure the timers to display. Configure optional timer rotation.
3. **Appearance Page**: Configure the fonts, colors, borders and transparency of the timer display.
4. **Settings Page**: Configure startup behaviour.
5. **Help Page**: Links to this website and upgrade options.

Each settings page is described in more detail in the following sections

### Command Page

The Command Page provides the Main Menu for the application. This page has the following buttons

1. **Mode Toggle**: Switch the Timer Window between Interactive and Background modes.
2. **Save Settings**: Save the current application settings
3. **Quit Application**: Shut down and exit the LifeTimer application.

Please note that configuration changes are not automatically saved. All configuration changes must be manually saved using the 'Save Settings' for them to be remembered after the application restarts. This includes any changes on the Timer, Appearance or Settings pages.

![LifeTimer Command Page](/images/docs/doc_command_page.png)

### Timers Page

The Timers Page is used to configure the time displays that are shown in the timer window, and to configure the automatic cycling between these timers.

- To add a new timer, click the Add Timer button and fill in the Timer details.

- To edit a timer, click the Edit button next to the timer entry.

- To display a timer in the Timer Window, click the Activate button.

- To delete a timer, click the Delete button.

- To cycle the browser window between a list of timers, activate the Timer Rotation toggle. Use the Rotation Delay field to change how long each Timer in the list is displayed for.

![LifeTimer Timers Page](/images/docs/doc_timers_page.png)

### Appearance Page

The Appearance Page is used to configure how the Timer Window looks on your desktop. You can use this page to change the fonts, colors and border settings for the display.

![LifeTimer Appearance Page](/images/docs/doc_appearance_page.png)

### Settings Page

The Settings Page is used to configure the startup behaviour of the application. It has the following options:

1. **System Start**

This toggle switch configures whether the application will start automatically.
If it is set to Off, then the application will not start automatically, and will need to be manually started.
If it is set to On, then the application will start automatically with Windows after you sign in.

2. **Interactive Start**

This toggle switch configures which mode the application will start in.
If it is set to Off, then the timer window will start in Background Mode.
If it is set to On, then the timer will start in Interactive Mode

3. **Settings on Start**

This toggle switch configures whether the Settings Window will be automatically opened when the application starts.
If it is set to Off, then the Settings Window will not be automatically displayed when the application starts.
If it is set to On, then the Settings Window will be automatically displayed when the application starts.

![LifeTimer Settings Page](/images/docs/doc_settings_page.png)

### Help Page

The Help Page provides a convenient way to link to this web site.

Click the lifetimer site link to open a regular browser window to this site.

Choose an option and click the upgrade button to upgrade the application to the Professional version.

![LifeTimer Help Page](/images/docs/doc_help_page.png)

## Troubleshooting

### Known Issues

**Windows Desktop Only:**

- LifeTimer is a Windows Desktop application and is not available for other operating systems

**Desktop Icons are Covered:**

- This is by design. Applications that attach to the Windows Desktop and run behind the desktop icons require elevated privileges and can be a security risk. LifeTimer emulates a desktop window but avoids these security concerns. Unfortunately, this means that the LifeTimer browser window will obscure icons on the desktop even when it is running in Background Mode.

**Limited Windows 10 Support:**

- LifeTimer is designed for Windows 11 and may have issues with older versions of Windows 10
- Windows 10 users are encouraged to thoroughly evaluate the free version before purchasing an upgrade.

### Getting Help

For additional support:

- Visit the [Help Page](/help)

---

_Last updated: November 2025_
