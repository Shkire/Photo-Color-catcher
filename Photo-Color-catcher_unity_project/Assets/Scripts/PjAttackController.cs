using UnityEngine;
using System.Collections;

/// <summary>
/// Pj attack controller.
/// </summary>
public class PjAttackController : MonoBehaviour
{
	/// <summary>
	/// The photo frame GameObject.
	/// </summary>
	[SerializeField]
	private GameObject frame;

	/// <summary>
	/// The LeapMotion front plane value.
	/// </summary>
	[SerializeField]
	int front;

	/// <summary>
	/// The LeapMotion back plane value.
	/// </summary>
	[SerializeField]
	int back;

	/// <summary>
	/// If the frame is active.
	/// </summary>
	private bool active;

	/// <summary>
	/// If LeapMotion is preparing a photo.
	/// </summary>
	private bool range;

	/// <summary>
	/// The texture that is going to be saved.
	/// </summary>
	private Texture2D savingTexture;

	void OnEnable ()
	{
		
	}

	void FixedUpdate ()
	{
		if (active) {
			frame.GetComponent<Collider2D> ().enabled = false;
			active = false;
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			frame.GetComponent<SpriteRenderer> ().enabled = true;
			range = true;
		}
			
		if (Input.GetKeyUp (KeyCode.Space) && range) {
			frame.GetComponent<SpriteRenderer> ().enabled = false;
			frame.GetComponent<Collider2D> ().enabled = true;
			Vector2 min = Camera.main.WorldToScreenPoint (frame.GetComponent<Collider2D> ().bounds.min);
			Vector2 max = Camera.main.WorldToScreenPoint (frame.GetComponent<Collider2D> ().bounds.max);
			savingTexture = new Texture2D (Mathf.FloorToInt (max.x - min.x), Mathf.FloorToInt (max.y - min.y));
			savingTexture.ReadPixels (new Rect (min, new Vector2 (Mathf.FloorToInt (max.x - min.x), Mathf.FloorToInt (max.y - min.y))), 0, 0);
			byte[] savingBytes;
			savingBytes = savingTexture.EncodeToPNG ();
			string auxString = string.Empty;
			int i = 0;
			while (System.IO.File.Exists (Application.persistentDataPath + "/" + System.DateTime.Now.ToString ("yyyyMMdd_HHmmss") + auxString + ".png")) {
				i++;
				auxString = "(" + i + ")";
			}
			System.IO.File.WriteAllBytes (Application.persistentDataPath + "/" + System.DateTime.Now.ToString ("yyyyMMdd_HHmmss") + auxString + ".png", savingBytes);
			active = true;
			range = false;
		}
	}
}