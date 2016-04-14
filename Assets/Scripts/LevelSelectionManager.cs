using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelSelectionManager : Singleton<LevelSelectionManager> {

	[SerializeField]
	private int actualImage;

	[Header("Image Positions")]
	[SerializeField]
	private GameObject Pos0_0;
	[SerializeField]
	private GameObject Pos0_1;
	[SerializeField]
	private GameObject Pos0_2;
	[SerializeField]
	private GameObject Pos0_3;
	[SerializeField]
	private GameObject Pos1_0;
	[SerializeField]
	private GameObject Pos1_1;
	[SerializeField]
	private GameObject Pos1_2;
	[SerializeField]
	private GameObject Pos1_3;
	[SerializeField]
	private GameObject Pos2_0;
	[SerializeField]
	private GameObject Pos2_1;
	[SerializeField]
	private GameObject Pos2_2;
	[SerializeField]
	private GameObject Pos2_3;
	[SerializeField]
	private GameObject Pos3_0;
	[SerializeField]
	private GameObject Pos3_1;
	[SerializeField]
	private GameObject Pos3_2;
	[SerializeField]
	private GameObject Pos3_3;

	void Awake()
	{
		PersistenceManager.MainLoad ();
		PersistenceManager.LevelDataLoad (actualImage);
		InitLevelSelector ();
	}

	public void InitLevelSelector(){
		ProcessedImage tempImg =PersistenceManager.GetImage (actualImage);
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
	}

}
