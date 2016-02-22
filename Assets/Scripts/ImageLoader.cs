using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ImageLoader : MonoBehaviour {

	[SerializeField]
	private List<Texture2D> loadedImages;

	void Awake(){
		loadedImages = new List<Texture2D> ();
		string[] files = System.IO.Directory.GetFiles (Application.persistentDataPath+"/");
		foreach (string loadingFile in files) 
		{
			byte[] loadingBytes;
			loadingBytes = System.IO.File.ReadAllBytes (loadingFile);
			Texture2D tempText = new Texture2D (108, 109);
			tempText.LoadImage (loadingBytes);
			loadedImages.Add (tempText);
		}

	}
}
