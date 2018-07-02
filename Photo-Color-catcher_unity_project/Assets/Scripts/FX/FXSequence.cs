using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component used to organize and launch a sequence of FXElements.
/// </summary>
public class FXSequence : MonoBehaviour
{

    /// <summary>
    /// If the sequence is launched automatically (without being explicity launched).
    /// </summary>
    public bool _autoLaunch;

    /// <summary>
    /// The collection of FXElements to launch.
    /// </summary>
    private FXElement[] p_fxElements;

    void Start()
    {
        //Gets all the FXElements of the sequence.
        p_fxElements = GetComponentsInChildren<FXElement>();

        if (_autoLaunch)
            Launch();
    }

    /// <summary>
    /// Launchs the sequence of FX.
    /// </summary>
    public void Launch()
    {
        
        //For each FXElement of the sequence.
        foreach (FXElement fxElement in p_fxElements)
        {

            //Launches the coroutine that launches the FX associated with the FXElement.
            StartCoroutine(LaunchFXElement(fxElement));
        }
    }

    /// <summary>
    /// Launchs the FXElement.
    /// </summary>
    /// <param name="i_fxElement">The FXElement to launch.</param>
    public IEnumerator LaunchFXElement(FXElement i_fxElement)
    {
        //Gets the FXTime associated with the FXElement.
        FXTime time = i_fxElement.GetComponent<FXTime>();

        //Gets the time when the sequence was launched.
        float initialTime = Time.realtimeSinceStartup;

        //While FXElement start time hasn't been reached.
        while (Time.realtimeSinceStartup < initialTime + time._start)
            yield return null;

        //Starts the FX associated with the FXElement.
        yield return StartCoroutine(i_fxElement.StartFX(time._duration));
    }
}
