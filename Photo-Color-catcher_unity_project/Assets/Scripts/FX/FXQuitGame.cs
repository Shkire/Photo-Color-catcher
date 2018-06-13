using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXQuitGame : FXElement {

    public override IEnumerator StartFX(float i_duration)
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying=false;
        #else
        Application.Quit();
        #endif

        yield break;
    }
}
