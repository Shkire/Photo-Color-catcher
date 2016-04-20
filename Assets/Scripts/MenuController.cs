using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {


	[SerializeField]
	private GameObject start;

	[SerializeField]
	private GameObject mainMenu;

	[SerializeField]
	private Text play;
	[SerializeField]
	private Text gallery;
	[SerializeField]
	private Text options;
	[SerializeField]
	private Text exit;

	private GameObject[] enemies;

	int count = 1;


	// Use this for initialization
	void Start () {
	
		/*enemies [0] = play;
		enemies [1] = gallery;
		enemies [2] = options;
		enemies [3] = exit;
		*/

	}
	
	// Update is called once per frame
	void Update () {

		if (count == 1) {
			play.fontStyle = FontStyle.BoldAndItalic;
			gallery.fontStyle = FontStyle.Normal;
			options.fontStyle = FontStyle.Normal;
			exit.fontStyle = FontStyle.Normal;


			if(Input.GetKeyDown (KeyCode.Space)){
				//Jump to Play Scene
				Application.LoadLevel("demoScene");
			}

			if(Input.GetKeyDown (KeyCode.DownArrow))
				count++;
			

		}else if (count == 2) {
			play.fontStyle = FontStyle.Normal;
			gallery.fontStyle = FontStyle.BoldAndItalic;
			options.fontStyle = FontStyle.Normal;
			exit.fontStyle = FontStyle.Normal;


			if(Input.GetKeyDown (KeyCode.Space)){
				//Jump to Gallery Scene
			}

			if(Input.GetKeyDown (KeyCode.DownArrow))
				count++;
			if (Input.GetKeyDown (KeyCode.UpArrow))
				count--;

		}else if (count == 3) {
			play.fontStyle = FontStyle.Normal;
			gallery.fontStyle = FontStyle.Normal;
			options.fontStyle = FontStyle.BoldAndItalic;
			exit.fontStyle = FontStyle.Normal;


			if(Input.GetKeyDown (KeyCode.Space)){
				//Jump to Options Scene
			}

			if(Input.GetKeyDown (KeyCode.DownArrow))
				count++;

			if (Input.GetKeyDown (KeyCode.UpArrow))
				count--;

		}else if (count == 4) {
			play.fontStyle = FontStyle.Normal;
			gallery.fontStyle = FontStyle.Normal;
			options.fontStyle = FontStyle.Normal;
			exit.fontStyle = FontStyle.BoldAndItalic;


			if(Input.GetKeyDown (KeyCode.Space))
				Application.Quit();


			if (Input.GetKeyDown (KeyCode.UpArrow))
				count--;

		}






		if (start.activeSelf && Input.GetKeyDown (KeyCode.Return)) {
			start.SetActive (false);
			mainMenu.SetActive (true);

		} 

		if (mainMenu.activeSelf && Input.GetKeyDown (KeyCode.Escape)) {
			mainMenu.SetActive (false);
			start.SetActive (true);

		}




	}
		
}

