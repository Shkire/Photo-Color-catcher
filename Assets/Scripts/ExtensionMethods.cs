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
}
