using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXSequence : MonoBehaviour {

    public bool _autoLaunch;

    private FXElement[] p_fxElements;

    void Start()
    {
        p_fxElements = GetComponentsInChildren<FXElement>();

        if (_autoLaunch)
            Launch();
    }

    public void Launch()
    {
        foreach (FXElement fxElement in p_fxElements)
        {
            StartCoroutine(LaunchFXElement(fxElement.GetComponent<FXTime>(), fxElement));
        }
    }

    public IEnumerator LaunchFXElement(FXTime i_time, FXElement i_fxElement)
    {
        float initialTime = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup < initialTime + i_time._start)
            yield return null;

        yield return StartCoroutine(i_fxElement.StartFX(i_time._duration));
    }
}
