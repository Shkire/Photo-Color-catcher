using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowAppVersion : MonoBehaviour {

    public string header;

	void Start () 
    {
        GetComponent<Text>().text = header + Application.version;
    }
}
