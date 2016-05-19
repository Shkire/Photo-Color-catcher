using UnityEngine;
using System.Collections;

public class LoadScreeController : MonoBehaviour {
	[SerializeField]
	private UnityEngine.UI.Text cargando;

	[SerializeField]
	private UnityEngine.UI.Image camera;

	[SerializeField]
	private UnityEngine.UI.Image fondo;


	[SerializeField]
	private GameObject pj;

	[SerializeField]
	private GameObject enemySpawner1;

	[SerializeField]
	private GameObject enemySpawner2;

	[SerializeField]
	private GameObject tp1;

	[SerializeField]
	private GameObject tp2;

	[SerializeField]
	private GameObject audio;

	private Coroutine corrutina;
	private Coroutine corrutina2;


	private int i;
	void OnEnable () {
		corrutina = StartCoroutine (LoadingText());
		corrutina2 = StartCoroutine (StartGame());

	}

	void OnDisable() {
		StopCoroutine (corrutina);
		StopCoroutine (corrutina);

	}



	IEnumerator LoadingText() {
		while (true) {
			cargando.text = "CARGANDO .";
			yield return new WaitForSeconds (1f);
			cargando.text = "CARGANDO . .";
			yield return new WaitForSeconds (1f);
			cargando.text = "CARGANDO . . .";
			yield return new WaitForSeconds (1f);
		}
	}

	IEnumerator StartGame() {
		yield return new WaitForSeconds (5f);
			//lanzar el juego
		pj.SetActive(true);
		enemySpawner1.SetActive(true);
		enemySpawner2.SetActive(true);
		tp1.SetActive(true);
		tp2.SetActive(true);
		audio.SetActive(true);
		cargando.enabled = false;
		camera.enabled = false;
		fondo.enabled = false;
		Destroy (this);


	}
}
