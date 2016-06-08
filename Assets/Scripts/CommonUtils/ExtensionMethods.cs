using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Extension methods.
/// </summary>
public static class ExtensionMethods
{
	/// <summary>
	/// Resets the forces of the Rigidbody.
	/// </summary>
    public static void ResetForces(this Rigidbody rb)
	{
        rb.isKinematic = true;
        rb.isKinematic = false;
    }

	/// <summary>
	/// Resets the forces of the Rigidbody2D.
	/// </summary>
    public static void ResetForces(this Rigidbody2D rb)
    {
        rb.isKinematic = true;
        rb.isKinematic = false;
    }

	/// <summary>
	/// Resets the forces and add a new force to the Rigidbody.
	/// </summary>
	/// <param name="force">Force added.</param>
    public static void ResetAndAddForce(this Rigidbody rb, Vector3 force)
    {
        rb.ResetForces();
        rb.AddForce(force);
    }

	/// <summary>
	/// Resets the forces and add a new force to the Rigidbody2D.
	/// </summary>
	/// <param name="force">Force added.</param>
    public static void ResetAndAddForce(this Rigidbody2D rb, Vector2 force)
    {
        rb.ResetForces();
        rb.AddForce(force);
    }

	public static Texture2D ResizeBilinear(this Texture2D text, int newXSize, int newYSize)
	{
		//Create result texture and get pixels
		Texture2D result = new Texture2D (newXSize, newYSize, text.format, true);
		UnityEngine.Color[] rpixels = result.GetPixels ();
		//Calculate inc
		float incX=(1.0f/(float)newXSize);
		float incY=(1.0f/(float)newYSize);
		for (int px = 0; px < rpixels.Length; px++) 
		{
			rpixels [px] = text.GetPixelBilinear (incX * ((float)px % newXSize), incY * ((float)Mathf.Floor (px / newYSize)));
		}
		result.SetPixels(rpixels); 
		result.Apply(); 
		return result;
	}

	public static GameObject GetChild(this GameObject go)
	{
		foreach (Transform aux in go.GetComponentsInChildren<Transform>())
			if (aux.gameObject.GetInstanceID () != go.GetInstanceID ())
				return aux.gameObject;
		return null;
	}

	public static GameObject[] GetChildren(this GameObject go)
	{
		List<GameObject> auxList = new List<GameObject> ();
		foreach (Transform aux in go.GetComponentsInChildren<Transform>())
			if (aux.gameObject.GetInstanceID () != go.GetInstanceID ())
				auxList.Add (aux.gameObject);
		return auxList.ToArray();
	}

	public static GameObject GetChild(this GameObject go, string name)
	{
		foreach (Transform aux in go.GetComponentsInChildren<Transform>())
			if (aux.gameObject.GetInstanceID () != go.GetInstanceID () && aux.gameObject.name.Equals (name))
				return aux.gameObject;
		return null;
	}

	public static Vector3 GetSize(this GameObject go)
	{
		Renderer rend = go.GetComponent<Renderer> ();
		if (rend != null) 
		{
			return rend.bounds.extents * 2;
		}
		Collider coll = go.GetComponent<Collider> ();
		if (coll != null) 
		{
			return coll.bounds.extents * 2;
		}
		return Vector3.zero;
	}

	public static void SetSize(this GameObject go, Vector3 i_size)
	{
		if (go.GetSize () != Vector3.zero) 
		{
			go.transform.localScale = new Vector3 (go.transform.localScale.x * (i_size.x / go.GetSize ().x), go.transform.localScale.y * (i_size.y / go.GetSize ().y), go.transform.localScale.z * (i_size.z / go.GetSize ().z));
		}
	}
}
