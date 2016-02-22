using UnityEngine;
using System.Collections;

public class PjAttackController : MonoBehaviour {

	[SerializeField]
	private GameObject frame;

	private bool active;

	private Texture2D savingTexture;
	// Update is called once per frame
	void FixedUpdate () {

		if (active) {
			frame.GetComponent<Collider2D> ().enabled = false;
			active = false;
		}
		

		if (Input.GetKeyDown (KeyCode.Space)) {
			frame.GetComponent<SpriteRenderer> ().enabled = true;

		}


		if (Input.GetKeyUp (KeyCode.Space)) {
			frame.GetComponent<SpriteRenderer> ().enabled = false;
			frame.GetComponent<Collider2D> ().enabled = true;
			Vector2 min = Camera.main.WorldToScreenPoint(frame.GetComponent<Collider2D> ().bounds.min);
			Vector2 max = Camera.main.WorldToScreenPoint(frame.GetComponent<Collider2D> ().bounds.max);
			savingTexture = new Texture2D (Mathf.FloorToInt(max.x-min.x),Mathf.FloorToInt(max.y-min.y));
			savingTexture.ReadPixels(new Rect(min, new Vector2 (Mathf.FloorToInt(max.x-min.x),Mathf.FloorToInt(max.y-min.y))),0,0);
			byte[] savingBytes;
			savingBytes = savingTexture.EncodeToPNG();
			string auxString = string.Empty;
			int i = 0;
			while (System.IO.File.Exists (Application.persistentDataPath + "/" + System.DateTime.Now.ToString ("yyyyMMdd_HHmmss") + auxString + ".png"))
			{
				i++;
				auxString = "(" + i + ")";
			}
			System.IO.File.WriteAllBytes(Application.persistentDataPath+"/"+System.DateTime.Now.ToString("yyyyMMdd_HHmmss")+auxString+".png", savingBytes );
			active = true;
		}



	}


}
