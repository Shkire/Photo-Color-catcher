using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : Singleton<LevelLoader> {

	public void LoadAndConfig(int id)
	{
		StartCoroutine (LoadLevel (id));
	}

	public IEnumerator LoadLevel(int id)
	{
		DontDestroyOnLoad (this.gameObject);
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync ("LevelScene");
		asyncLoad.allowSceneActivation = true;
		while (!asyncLoad.isDone) 
		{
			Debug.Log ("Progreso:"+asyncLoad.progress);
			yield return null;
		}
		StartCoroutine (ConfigLevel (id));
	}

	public IEnumerator ConfigLevel (int id){
		GameObject background = new GameObject ("BG");
		background.AddComponent<RectTransform> ();
		background.transform.SetParent (FindObjectOfType<Canvas>().transform);
		background.AddComponent<Image> ();
		ProcessedImage auxImg=PersistenceManager.GetImage(id);
		Sprite spr = new Sprite ();
		spr = Sprite.Create (auxImg.ToTexture2D (), new Rect (0, 0, 25,25), new Vector2 (0, 0));
		Image img = background.GetComponent<Image> ();
		img.sprite = spr;	
		yield return null;
	}

}
