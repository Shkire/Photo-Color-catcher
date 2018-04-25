using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

/// <summary>
/// Image loader.
/// </summary>
public class ImageLoader : MonoBehaviour
{
	/// <summary>
	/// The loaded images.
	/// </summary>
	[SerializeField]
	private List<Texture2D> loadedImages;

	[SerializeField]
	private GameObject modelButton;

	[SerializeField]
	private InputMenuController menuController;

	void Awake()
	{
		loadedImages = new List<Texture2D> ();
		string[] files = System.IO.Directory.GetFiles (Application.persistentDataPath+"/");
		int counter = 0;
		foreach (string loadingFile in files) 
		{
			if (!Path.GetExtension (loadingFile).ToUpper ().Equals (".PNG") && !Path.GetExtension (loadingFile).ToUpper ().Equals (".JPG") && !Path.GetExtension (loadingFile).ToUpper ().Equals (".GIF"))
				continue;
			byte[] loadingBytes;
			loadingBytes = System.IO.File.ReadAllBytes (loadingFile);
			Texture2D tempText = new Texture2D (108, 109);
			tempText.LoadImage (loadingBytes);
			GameObject imageButton = (GameObject)Instantiate (modelButton);
			imageButton.SetActive (true);
			imageButton.GetChild ("FocusedButton Background").GetComponent<Image> ().sprite = Sprite.Create(tempText,new Rect(0,0,tempText.width,tempText.height),Vector2.zero);
			imageButton.GetChild ("UnfocusedButton Background").GetComponent<Image> ().sprite = Sprite.Create(tempText.ToGray(),new Rect(0,0,tempText.width,tempText.height),Vector2.zero);
			imageButton.transform.SetParent (modelButton.transform.parent,false);
			(imageButton.transform as RectTransform).anchorMin = new Vector2((0.5f+(counter%4)*1.5f)/6.5f,1-((1.5f+(counter/4)*1.5f)/7f));
			(imageButton.transform as RectTransform).anchorMax = new Vector2((1.5f+(counter%4)*1.5f)/6.5f,1-((0.5f+(counter/4)*1.5f)/7f));
			string[] splitPath = loadingFile.Split ('/');
			//imageButton.GetComponent<GUIProcessImage> ().path = splitPath [splitPath.Length - 1];
			if (counter == 0) 
			{
				if (menuController.uiElements == null)
					menuController.uiElements = new List<GameObject>();
				menuController.startingElem = imageButton;
			}
			menuController.uiElements.Add (imageButton);
			counter++;
			loadedImages.Add (tempText);
		}
	}
}
