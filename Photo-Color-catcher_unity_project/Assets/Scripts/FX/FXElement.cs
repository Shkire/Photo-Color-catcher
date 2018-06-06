using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXElement : MonoBehaviour {

    public GameObject _target;

    public IEnumerator LaunchFXElement()
    {
        yield return new WaitForSeconds(GetComponent<FXTime>()._start);

        yield return StartCoroutine(StartFX(GetComponent<FXTime>()._duration));
    }

    public virtual IEnumerator StartFX(float i_duration)
    {
        yield return null;
    }
}
