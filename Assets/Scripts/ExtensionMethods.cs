using UnityEngine;
using System.Collections;

public static class ExtensionMethods {

    public static void ResetForces(this Rigidbody rb) {

        rb.isKinematic = true;
        rb.isKinematic = false;

    }

    public static void ResetForces(this Rigidbody2D rb)
    {

        rb.isKinematic = true;
        rb.isKinematic = false;

    }

    public static void ResetAndAddForce(this Rigidbody rb, Vector3 force)
    {

        rb.ResetForces();
        rb.AddForce(force);

    }

    public static void ResetAndAddForce(this Rigidbody2D rb, Vector3 force)
    {

        rb.ResetForces();
        rb.AddForce(force);

    }

}
