using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelSelectionManager : Singleton<LevelSelectionManager> {

	[SerializeField]
	private int actualImage;

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
		PersistenceManager.MainLoad ();
		PersistenceManager.LevelDataLoad (actualImage);
		InitLevelSelector ();

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
		positions = new Dictionary<Vector2, GameObject> ();
		ProcessedImage tempImg =PersistenceManager.GetImage (actualImage);
		ProcessedImage auxImg;
		Image img;
		Sprite spr;
		GameObject currentGo;
		GameObject childOnGUI;
		for (int i = 0; i < (int)Mathf.Sqrt (tempImg.GetChildrenCount ()); i++)
			for (int j = 0; j < (int)Mathf.Sqrt (tempImg.GetChildrenCount ()); j++) 
			{
				currentGo = new GameObject ("LevelPos"+i+"-"+j);
				currentGo.AddComponent<RectTransform> ();
				currentGo.transform.SetParent (parentTransform);
				((RectTransform)currentGo.transform).anchorMin = new Vector2 (i*(1/Mathf.Sqrt (tempImg.GetChildrenCount ())),1-((j+1)/Mathf.Sqrt (tempImg.GetChildrenCount ())));
				((RectTransform)currentGo.transform).anchorMax = new Vector2 ((i+1)*(1/Mathf.Sqrt (tempImg.GetChildrenCount ())),1-(j/Mathf.Sqrt (tempImg.GetChildrenCount ())));
				((RectTransform)currentGo.transform).localScale = Vector3.one;
				((RectTransform)currentGo.transform).offsetMax = -Vector2.one;
				((RectTransform)currentGo.transform).offsetMin = Vector2.one;
				currentGo.AddComponent<Image> ();
				currentGo.AddComponent<GUISelectableElement> ();
				currentGo.AddComponent<GUILoadLevel> ();
				auxImg=PersistenceManager.GetImage(tempImg.GetChildId (i,((int)Mathf.Sqrt (tempImg.GetChildrenCount ())-(j+1))));
				spr = new Sprite ();
				spr = Sprite.Create (auxImg.ToTexture2D (), new Rect (0, 0, 25,25), new Vector2 (0, 0));
				img = currentGo.GetComponent<Image> ();
				img.sprite = spr;
				childOnGUI = (GameObject)Instantiate (playerOnGUI);
				childOnGUI.transform.SetParent (currentGo.transform);
				childOnGUI.transform.localScale = Vector3.one;
				currentGo.SendMessage ("NonFocused");
				positions.Add (new Vector2 (i, j), currentGo);
			}
		/*
				
				
		ProcessedImage auxImg;
		auxImg=PersistenceManager.GetImage(tempImg.GetChildId (0, 0));
		Image img = Pos3_0.GetComponent<Image> ();
		Sprite spr = new Sprite ();
		spr = Sprite.Create (auxImg.ToTexture2D (), new Rect (0, 0, 25,25), new Vector2 (0, 0));
		img.sprite = spr;
		auxImg=PersistenceManager.GetImage(tempImg.GetChildId (1, 0));
		img = Pos3_1.GetComponent<Image> ();
		spr = new Sprite ();
		spr = Sprite.Create (auxImg.ToTexture2D (), new Rect (0, 0, 25,25), new Vector2 (0, 0));
		img.sprite = spr;
		auxImg=PersistenceManager.GetImage(tempImg.GetChildId (2, 0));
		img = Pos3_2.GetComponent<Image> ();
		spr = new Sprite ();
		spr = Sprite.Create (auxImg.ToTexture2D (), new Rect (0, 0, 25,25), new Vector2 (0, 0));
		img.sprite = spr;
		auxImg=PersistenceManager.GetImage(tempImg.GetChildId (3, 0));
		img = Pos3_3.GetComponent<Image> ();
		spr = new Sprite ();
		spr = Sprite.Create (auxImg.ToTexture2D (), new Rect (0, 0, 25,25), new Vector2 (0, 0));
		img.sprite = spr;
		auxImg=PersistenceManager.GetImage(tempImg.GetChildId (0, 1));
		img = Pos2_0.GetComponent<Image> ();
		 spr = new Sprite ();
		spr = Sprite.Create (auxImg.ToTexture2D (), new Rect (0, 0, 25,25), new Vector2 (0, 0));
		img.sprite = spr;
		auxImg=PersistenceManager.GetImage(tempImg.GetChildId (1, 1));
		img = Pos2_1.GetComponent<Image> ();
		spr = new Sprite ();
		spr = Sprite.Create (auxImg.ToTexture2D (), new Rect (0, 0, 25,25), new Vector2 (0, 0));
		img.sprite = spr;
		auxImg=PersistenceManager.GetImage(tempImg.GetChildId (2, 1));
		img = Pos2_2.GetComponent<Image> ();
		spr = new Sprite ();
		spr = Sprite.Create (auxImg.ToTexture2D (), new Rect (0, 0, 25,25), new Vector2 (0, 0));
		img.sprite = spr;
		auxImg=PersistenceManager.GetImage(tempImg.GetChildId (3, 1));
		img = Pos2_3.GetComponent<Image> ();
		spr = new Sprite ();
		spr = Sprite.Create (auxImg.ToTexture2D (), new Rect (0, 0, 25,25), new Vector2 (0, 0));
		img.sprite = spr;
		auxImg=PersistenceManager.GetImage(tempImg.GetChildId (0, 2));
		img = Pos1_0.GetComponent<Image> ();
		spr = new Sprite ();
		spr = Sprite.Create (auxImg.ToTexture2D (), new Rect (0, 0, 25,25), new Vector2 (0, 0));
		img.sprite = spr;
		auxImg=PersistenceManager.GetImage(tempImg.GetChildId (1, 2));
		img = Pos1_1.GetComponent<Image> ();
		spr = new Sprite ();
		spr = Sprite.Create (auxImg.ToTexture2D (), new Rect (0, 0, 25,25), new Vector2 (0, 0));
		img.sprite = spr;
		auxImg=PersistenceManager.GetImage(tempImg.GetChildId (2, 2));
		img = Pos1_2.GetComponent<Image> ();
		spr = new Sprite ();
		spr = Sprite.Create (auxImg.ToTexture2D (), new Rect (0, 0, 25,25), new Vector2 (0, 0));
		img.sprite = spr;
		auxImg=PersistenceManager.GetImage(tempImg.GetChildId (3, 2));
		img = Pos1_3.GetComponent<Image> ();
		spr = new Sprite ();
		spr = Sprite.Create (auxImg.ToTexture2D (), new Rect (0, 0, 25,25), new Vector2 (0, 0));
		img.sprite = spr;
		auxImg=PersistenceManager.GetImage(tempImg.GetChildId (0, 3));
		img = Pos0_0.GetComponent<Image> ();
		spr = new Sprite ();
		spr = Sprite.Create (auxImg.ToTexture2D (), new Rect (0, 0, 25,25), new Vector2 (0, 0));
		img.sprite = spr;
		auxImg=PersistenceManager.GetImage(tempImg.GetChildId (1, 3));
		img = Pos0_1.GetComponent<Image> ();
		spr = new Sprite ();
		spr = Sprite.Create (auxImg.ToTexture2D (), new Rect (0, 0, 25,25), new Vector2 (0, 0));
		img.sprite = spr;
		auxImg=PersistenceManager.GetImage(tempImg.GetChildId (2, 3));
		img = Pos0_2.GetComponent<Image> ();
		spr = new Sprite ();
		spr = Sprite.Create (auxImg.ToTexture2D (), new Rect (0, 0, 25,25), new Vector2 (0, 0));
		img.sprite = spr;
		auxImg=PersistenceManager.GetImage(tempImg.GetChildId (3, 3));
		img = Pos0_3.GetComponent<Image> ();
		spr = new Sprite ();
		spr = Sprite.Create (auxImg.ToTexture2D (), new Rect (0, 0, 25,25), new Vector2 (0, 0));
		img.sprite = spr;
		Pos0_1.GetChild ().SetActive (false);
		Pos0_2.GetChild ().SetActive (false);
		Pos0_3.GetChild ().SetActive (false);
		Pos1_0.GetChild ().SetActive (false);
		Pos1_1.GetChild ().SetActive (false);
		Pos1_2.GetChild ().SetActive (false);
		Pos1_3.GetChild ().SetActive (false);
		Pos2_0.GetChild ().SetActive (false);
		Pos2_1.GetChild ().SetActive (false);
		Pos2_2.GetChild ().SetActive (false);
		Pos2_3.GetChild ().SetActive (false);
		Pos3_0.GetChild ().SetActive (false);
		Pos3_1.GetChild ().SetActive (false);
		Pos3_2.GetChild ().SetActive (false);
		Pos3_3.GetChild ().SetActive (false);
*/
		actualPos = Vector2.zero;
		positions [actualPos].SendMessage ("Focused");
	}

}
