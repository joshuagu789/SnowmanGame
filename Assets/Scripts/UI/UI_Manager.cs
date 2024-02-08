using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for deciding which piece of UI should be displayed to the player. The script mentions canvases which is a group of UI elements for
/// a particular function
/// </summary>
public static class UI_Manager
{
    /// <summary>
    /// Order is in priority of which UI is more important to display (as each element has a number associated with it)
    /// </summary>
    public enum UI
    {
        NONE,
        GAME,
        MAIN_MENU,
        BASE,
        PAUSE,
        CINEMATIC,
    }

    private static UI currentUI;
    private static Dictionary<UI, Canvas> canvases = new Dictionary<UI, Canvas>();

    /// <summary>
    /// Every script that is a child of CanvasUI will attempt to register their canvas with the UI Manager so that it can be used in the game.
    /// Each script should use this method when the game starts
    /// </summary>
    /// <param name="ui"></param>
    /// <param name="canvas"></param>
    /// <returns></returns>
    public static bool TryAddCanvas(UI ui, Canvas canvas) {
        return canvases.TryAdd(ui, canvas);
    }

    public static UI GetCurrentUI() { return currentUI; }

    /// <summary>
    /// Different UI scripts can request for the UI Manager to switch the current UI being shown to the requested one
    /// <br/>
    /// The UI Manager can refuse to switch if it is currently playing a UI of higher importance (such as a cinematic)
    /// </summary>
    /// <param name="UI element to replace current one with"> aasdas </param>
    /// <returns>true if able to switch to requested UI, false is unable to</returns>

    public static bool TrySetUI(UI ui)
    {
        if (ui > currentUI) {
            currentUI = ui;
            return true;
        }
        return false;
    }

    private static void SetUI(UI ui) { }
}

