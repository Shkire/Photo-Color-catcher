using UnityEngine;
using System.Collections;

public class ChangeBGTest : MonoBehaviour {

	[SerializeField]
	GameObject objeto;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.Q)) {

			if (objeto.activeSelf) {
				objeto.SetActive (false);

			} else {

				objeto.SetActive(true);
			}

		}

	
	}
}
