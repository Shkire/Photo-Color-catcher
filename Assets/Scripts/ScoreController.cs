using UnityEngine;
using System.Collections;

public class ScoreController : MonoBehaviour {
	[SerializeField]
	private UnityEngine.UI.Text scoreText;

	int score;

	void Start(){

		score = 0;
	}
		

	public void Score(){

		score++;
		scoreText.text= score.ToString ();
	}
}
