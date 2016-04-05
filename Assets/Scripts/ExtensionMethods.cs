using UnityEngine;
using System.Collections;

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
}
