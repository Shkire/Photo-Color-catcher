using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelSelectionManager : Singleton<LevelSelectionManager> {

	public int actualImage;

	[SerializeField]
	private float uiResponseTime;

	private float leftUiResponseTime;

	[SerializeField]
	private RectTransform parentTransform;

	private Dictionary<Vector2,GameObject> positions;

	private Vector2 actualPos;

	public GameObject playerOnGUI;

	void Awake()
	{
		//PersistenceManager.LevelDataLoad (actualImage);
		//InitLevelSelector ();

	}

	void Update()
	{
		if (leftUiResponseTime <= 0) {
			if (Input.GetKeyDown (KeyCode.UpArrow)) 
			{
				if (positions.ContainsKey (actualPos + Vector2.down))
				{
					positions [actualPos].SendMessage ("NonFocused");
					actualPos += Vector2.down;
					positions [actualPos].SendMessage ("Focused");
					leftUiResponseTime = uiResponseTime;
				}
			}
			if (Input.GetKeyDown (KeyCode.DownArrow)) 
			{
				if (positions.ContainsKey (actualPos + Vector2.up))
				{
					positions [actualPos].SendMessage ("NonFocused");
					actualPos += Vector2.up;
					positions [actualPos].SendMessage ("Focused");
					leftUiResponseTime = uiResponseTime;
				}
			}
			if (Input.GetKeyDown (KeyCode.RightArrow)) 
			{
				if (positions.ContainsKey (actualPos + Vector2.right))
				{
					positions [actualPos].SendMessage ("NonFocused");
					actualPos += Vector2.right;
					positions [actualPos].SendMessage ("Focused");
					leftUiResponseTime = uiResponseTime;
				}
			}
			if (Input.GetKeyDown (KeyCode.LeftArrow)) 
			{
				if (positions.ContainsKey (actualPos + Vector2.left))
				{
					positions [actualPos].SendMessage ("NonFocused");
					actualPos += Vector2.left;
					positions [actualPos].SendMessage ("Focused");
					leftUiResponseTime = uiResponseTime;
				}
			}
			if (Input.GetKeyDown (KeyCode.Return)) 
			{
				positions [actualPos].SendMessage ("Selected");
				leftUiResponseTime = uiResponseTime;
			}
		}
		else 
		{
			leftUiResponseTime -= Time.deltaTime;
		}
	}

	public void InitLevelSelector(){
		PersistenceManager.LevelDataLoad (actualImage);
		positions = new Dictionary<Vector2, GameObject> ();
		List<ChildImgInfo> childrenInfo = PersistenceManager.GetChildrenInfo ();
		ProcessedImage auxImg;
		Image img;
		Sprite spr;
		GameObject currentGo;
		GameObject childOnGUI;
		for (int i = 0; i < (int)Mathf.Sqrt (childrenInfo.Count); i++)
			for (int j = 0; j < (int)Mathf.Sqrt (childrenInfo.Count); j++) 
			{
				currentGo = new GameObject ("LevelPos"+i+"-"+j);
				currentGo.AddComponent<RectTransform> ();
				currentGo.transform.SetParent (parentTransform);
				((RectTransform)currentGo.transform).anchorMin = new Vector2 (i*(1/Mathf.Sqrt (childrenInfo.Count)),1-((j+1)/Mathf.Sqrt (childrenInfo.Count)));
				((RectTransform)currentGo.transform).anchorMax = new Vector2 ((i+1)*(1/Mathf.Sqrt (childrenInfo.Count)),1-(j/Mathf.Sqrt (childrenInfo.Count)));
				((RectTransform)currentGo.transform).localScale = Vector3.one;
				((RectTransform)currentGo.transform).offsetMax = -Vector2.one;
				((RectTransform)currentGo.transform).offsetMin = Vector2.one;
				currentGo.AddComponent<Image> ();
				currentGo.AddComponent<GUISelectableElement> ();
				currentGo.AddComponent<GUILoadLevel> ();
				auxImg = PersistenceManager.GetImage(i,((int)Mathf.Sqrt (childrenInfo.Count)-(j+1)));
				//Debug.Log (i+","+((int)Mathf.Sqrt (tempImg.GetChildrenCount ())-(j+1)));
				currentGo.GetComponent<GUILoadLevel> ().SetId(auxImg.GetId());
				spr = new Sprite ();
				if (auxImg.IsCompleted())
					spr = Sprite.Create (auxImg.ToTexture2D (), new Rect (0, 0, 25,25), new Vector2 (0, 0));
				else
					spr = Sprite.Create (auxImg.ToGrayscaleTexture2D (), new Rect (0, 0, 25,25), new Vector2 (0, 0));
				img = currentGo.GetComponent<Image> ();
				img.sprite = spr;
				childOnGUI = (GameObject)Instantiate (playerOnGUI);
				childOnGUI.transform.SetParent (currentGo.transform);
				childOnGUI.transform.localScale = Vector3.one;
				currentGo.SendMessage ("NonFocused");
				positions.Add (new Vector2 (i, j), currentGo);
			}
		actualPos = Vector2.zero;
		positions [actualPos].SendMessage ("Focused");
	}

}
