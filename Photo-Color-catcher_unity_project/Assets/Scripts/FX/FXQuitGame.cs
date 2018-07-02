using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// FXElement that allows to quit the game.
/// </summary>
public class FXQuitGame : FXElement
{

    /// <summary>
    /// Starts the FX.
    /// </summary>
    /// <param name="i_duration">Duration of the FX.</param>
    public override IEnumerator StartFX(float i_duration)
    {
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif

        yield break;
    }
}
