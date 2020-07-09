# TaskbarExtensions

This library began as tool for Windows 8 which allowed to display the clock on all taskbars, not only the primary one.
Now it provides a way to add arbitrary elements to the taskbar of Windows 10.

## Features

- Can add elements to all taskbars integrating with the default look and feel
- Taskbar auto-hide feature is supported
- Elements do not interfere with fullscreen applications
- When changing size/position of a taskbar the custom corresponding elements react accordingly
- No skin engine or custom theme needed
- No setup needed, just launch
- Can easily be integrated in existing applications

## Sample applications

### SecondaryTaskBarClock
Somewhat obsolete, the feature to show the clock on all displays has been added by Microsoft.
- Displayed content depends on taskbar height and regional settings, just as with the native clock
- Clicking a secondary taskbar's clock opens the default calendar flyout on the corresponding secondary display
- Long date format tooltip

### CalendarWeekView
Shows the current German CalendarWeek in the taskbar

## Disclaimer

Be aware, that this uses methods not officially supported by Microsoft (rearranging child windows of the taskbar itself etc.) and relies on implementation details of the windows taskbar which may change in the future. Thus, there certainly exist system configurations where these approaches (or at least the current implementation) do not work (e.g. right-to-left systems).

## Screenshots

The system's default clock on the primary display (for reference) <br/>
<a href="https://cloud.githubusercontent.com/assets/3481307/16447704/112f7932-3dee-11e6-8e8c-70b65d75b27a.png" target="_blank">
<img src="https://cloud.githubusercontent.com/assets/3481307/16447704/112f7932-3dee-11e6-8e8c-70b65d75b27a.png" /> </a>

A clock on a secondary taskbar <br/>
<a href="https://cloud.githubusercontent.com/assets/3481307/16447703/112b1978-3dee-11e6-8c0c-1f9cdd54c048.png" target="_blank">
<img src="https://cloud.githubusercontent.com/assets/3481307/16447703/112b1978-3dee-11e6-8c0c-1f9cdd54c048.png" /> </a>

Secondary clock with calendar flyout <br/>
<a href="https://cloud.githubusercontent.com/assets/3481307/16447705/11427794-3dee-11e6-8a83-727006e42d1f.png" target="_blank">
<img src="https://cloud.githubusercontent.com/assets/3481307/16447705/11427794-3dee-11e6-8a83-727006e42d1f.png" /> </a>
