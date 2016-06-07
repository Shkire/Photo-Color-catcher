using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : Singleton<LevelLoader> {

	public int state = -1;
	private AsyncOperation asyncLoad;
	private int p_id;

	void Update()
	{
		if (asyncLoad != null) {
			if (!asyncLoad.isDone)
				Debug.Log ("Progreso:" + asyncLoad.progress);
			else if (state == 1) {
				Debug.Log ("Asigno el numero de imagen");
				LevelSelectionManager.Instance.actualImage = p_id;
				Debug.Log ("Inicializo el selector");
				LevelSelectionManager.Instance.InitLevelSelector ();
				Debug.Log ("FIN");
				state = -1;
				this.enabled = false;

			} else if (state == 2) {
				GameObject background = new GameObject ("BG");
				background.AddComponent<RectTransform> ();
				background.transform.SetParent (FindObjectOfType<Canvas> ().transform);
				background.AddComponent<Image> ();
				ProcessedImage auxImg = PersistenceManager.GetImage (p_id);
				Sprite spr = new Sprite ();
				spr = Sprite.Create (auxImg.ToTexture2D (), new Rect (0, 0, 25, 25), new Vector2 (0, 0));
				Image img = background.GetComponent<Image> ();
				img.sprite = spr;
				state = -1;
				this.enabled = false;

			}
		}
	}

	public void LoadLevelSelector(int id)
	{
		PersistenceManager.LoadLevelPack (id);
		DontDestroyOnLoad (this.gameObject);
		asyncLoad = SceneManager.LoadSceneAsync ("LevelSelection");
		state = 1;
		p_id = id;
		this.enabled = true;
		asyncLoad.allowSceneActivation = true;
	}

	public void LoadAndConfig(int id)
	{
		PersistenceManager.LoadLevelParent (id);
		DontDestroyOnLoad (this.gameObject);
		asyncLoad = SceneManager.LoadSceneAsync ("LevelScene");
		state = 2;
		p_id = id;
		this.enabled = true;
		asyncLoad.allowSceneActivation = true;
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
		ProcessedImage auxImg = PersistenceManager.GetImage(id);
		Sprite spr = new Sprite ();
		spr = Sprite.Create (auxImg.ToTexture2D (), new Rect (0, 0, 25,25), new Vector2 (0, 0));
		Image img = background.GetComponent<Image> ();
		img.sprite = spr;	
		yield return null;
	}

}
