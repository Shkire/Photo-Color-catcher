using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIElementMap
{
	public GameObject up;
	public GameObject down;
	public GameObject right;
	public GameObject left;
}

public class InputMenuController : MonoBehaviour {

	public GameObject[] uiElements;
	public GameObject startingElem;
	private Dictionary<GameObject,GUIElementMap> uiMap;
	private GameObject actualElem;
	[SerializeField]
	private float uiResponseTime;

	private float leftUiResponseTime;

	// Use this for initialization
	void Start () {

		uiMap = new Dictionary<GameObject, GUIElementMap> ();

		foreach (GameObject go in uiElements) 
		{
			GUIElementMap elem = new GUIElementMap ();
			foreach (GameObject go2 in uiElements) {
				if (go2 != go) {
					float xVal = go2.transform.position.x - go.transform.position.x;
					float yVal = go2.transform.position.y - go.transform.position.y;
					if (Mathf.Abs (xVal) > Mathf.Abs (yVal)) {
						if (xVal > 0) {
							if (elem.right == null || (Vector3.Distance (go.transform.position, go2.transform.position) < (Vector3.Distance (go.transform.position, elem.right.transform.position))))
								elem.right = go2;
						} else {
							if (elem.left == null || (Vector3.Distance (go.transform.position, go2.transform.position) < (Vector3.Distance (go.transform.position, elem.left.transform.position))))
								elem.left = go2;
						}
					} else {
						if (yVal > 0) {
							if (elem.up == null || (Vector3.Distance (go.transform.position, go2.transform.position) < (Vector3.Distance (go.transform.position, elem.up.transform.position))))
								elem.up = go2;
						} else {
							if (elem.down == null || (Vector3.Distance (go.transform.position, go2.transform.position) < (Vector3.Distance (go.transform.position, elem.down.transform.position))))
								elem.down = go2;
						}
					}
				}
			}
			uiMap.Add (go, elem);
		}
		actualElem = startingElem;
		actualElem.SendMessage ("Focused",SendMessageOptions.DontRequireReceiver);
		Debug.Log ("Actual elem "+actualElem);
		foreach (GameObject go in uiMap.Keys) 
		{
			Debug.Log ("Key " + go);
			Debug.Log ("Up " + uiMap [go].up);
			Debug.Log ("Down " + uiMap [go].down);
			Debug.Log ("Right " + uiMap [go].right);
			Debug.Log ("Left " + uiMap [go].left);
		}
	}

	void Update () {

		if (leftUiResponseTime <= 0) {
			if (actualElem != null) {
				if (Input.GetKey (KeyCode.UpArrow)) {
					if (uiMap [actualElem].up != null) {
						actualElem.SendMessage ("NonFocused");
						actualElem = uiMap [actualElem].up;
						actualElem.SendMessage ("Focused");
						leftUiResponseTime = uiResponseTime;
						Debug.Log ("PARRIBA");
					}
				} else if (Input.GetKey (KeyCode.DownArrow)) {
					if (uiMap [actualElem].down != null) {
						actualElem.SendMessage ("NonFocused");
						actualElem = uiMap [actualElem].down;
						actualElem.SendMessage ("Focused");
						leftUiResponseTime = uiResponseTime;
						Debug.Log ("PABAJO");
					}
				} else if (Input.GetKey (KeyCode.RightArrow)) {
					if (uiMap [actualElem].right != null) {
						actualElem.SendMessage ("NonFocused");
						actualElem = uiMap [actualElem].right;
						actualElem.SendMessage ("Focused");
						leftUiResponseTime = uiResponseTime;
					}
				} else if (Input.GetKey (KeyCode.LeftArrow)) {
					if (uiMap [actualElem].left != null) {
						actualElem.SendMessage ("NonFocused");
						actualElem = uiMap [actualElem].left;
						actualElem.SendMessage ("Focused");
						leftUiResponseTime = uiResponseTime;
					}
				} else if (Input.GetKey (KeyCode.Return)) {
					actualElem.SendMessage ("Selected");
					leftUiResponseTime = uiResponseTime;
				}
			}
	
		}
		else 
		{
			leftUiResponseTime -= Time.deltaTime;
		}
	}
}
