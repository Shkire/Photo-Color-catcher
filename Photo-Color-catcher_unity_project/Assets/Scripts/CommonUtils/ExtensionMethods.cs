using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CIEColor;

/// <summary>
/// Extension methods.
/// </summary>
public static class ExtensionMethods
{
    /// <summary>
    /// Resizes the texture using bilinear interpolation.
    /// </summary>
    /// <returns>The resized texture.</returns>
    /// <param name="i_newXSize">New X size.</param>
    /// <param name="i_newYSize">New Y size.</param>
    public static Texture2D ResizeBilinear(this Texture2D i_texture, int i_newXSize, int i_newYSize)
    {
        //Creates result texture and get pixels.
        Texture2D result = new Texture2D(i_newXSize, i_newYSize, i_texture.format, true);
        UnityEngine.Color[] rpixels = result.GetPixels();

        //Calculates image size increment.
        float incX = (1.0f / (float)i_newXSize);
        float incY = (1.0f / (float)i_newYSize);

        //For each pixel of the result texture.
        for (int px = 0; px < rpixels.Length; px++)
        {

            //Gets pixel color value.
            rpixels[px] = i_texture.GetPixelBilinear(incX * ((float)px % i_newXSize), incY * ((float)Mathf.Floor(px / i_newXSize)));
        }

        //Sets result texture pixels.
        result.SetPixels(rpixels); 
        result.Apply(); 

        return result;
    }

    /// <summary>
    /// Gets a GameObject that is child of this GameObject.
    /// </summary>
    /// <returns>The child GameObject.</returns>
    public static GameObject GetChild(this GameObject i_gameObject)
    {
        //For each Transform component in children GameObjects.
        foreach (Transform aux in i_gameObject.GetComponentsInChildren<Transform>(true))

            //If the GameObject is not the parent GameObject.
            if (aux.gameObject.GetInstanceID() != i_gameObject.GetInstanceID())
                return aux.gameObject;
        
        return null;
    }

    /// <summary>
    /// Gets a GameObject that is child of this GameObject.
    /// </summary>
    /// <returns>The child GameObject.</returns>
    /// <param name="i_name">The name of the GameObject to get.</param>
    public static GameObject GetChild(this GameObject i_gameObject, string i_name)
    {
        //For each Transform component in children GameObjects.
        foreach (Transform aux in i_gameObject.GetComponentsInChildren<Transform>(true))

            //If the GameObject is not the parent GameObject and the name of the GameObject is i_name.
            if (aux.gameObject.GetInstanceID() != i_gameObject.GetInstanceID() && aux.gameObject.name.Equals(i_name))
                return aux.gameObject;
        
        return null;
    }

    /// <summary>
    /// Gets all the GameObjects that are children of this GameObject.
    /// </summary>
    /// <returns>The children GameObjects.</returns>
    public static GameObject[] GetChildren(this GameObject i_gameObject)
    {
        //The list of children GameObjects.
        List<GameObject> childrenList = new List<GameObject>();

        //For each Transform component in children GameObjects.
        foreach (Transform aux in i_gameObject.GetComponentsInChildren<Transform>(true))

            //If the GameObject is not the parent GameObject.
            if (aux.gameObject.GetInstanceID() != i_gameObject.GetInstanceID())

                //Adds the GameObject to the list.
                childrenList.Add(aux.gameObject);
        
        return childrenList.ToArray();
    }

    //NEEDS TO BE IMPROVED
    /// <summary>
    /// Gets the size of this GameObject.
    /// </summary>
    /// <returns>The size of the GameObject.</returns>
    public static Vector3 GetSize(this GameObject i_gameObject)
    {
        //Gets the Renderer of the GameObject.
        Renderer rend = i_gameObject.GetComponent<Renderer>();

        //If there is a Renderer.
        if (rend != null)
        {

            //Returns the size of the Renderer.
            return rend.bounds.extents * 2;
        }

        //Gets the Collider.
        Collider coll = i_gameObject.GetComponent<Collider>();

        //If there is a Collider.
        if (coll != null)
        {

            //Returns the size of the Collider.
            return coll.bounds.extents * 2;
        }

        //Gets a Renderer from the children GameObjects.
        rend = i_gameObject.GetComponentInChildren<Renderer>();


        //If there is a Renderer.
        if (rend != null)
        {

            //Returns the size of the Renderer.
            return rend.bounds.extents * 2;
        }

        //Gets a Collider from the children GameObjects.
        coll = i_gameObject.GetComponentInChildren<Collider>();

        //If there is a Collider.
        if (coll != null)
        {

            //Returns the size of the Collider.
            return coll.bounds.extents * 2;
        }

        return Vector3.zero;
    }

    /// <summary>
    /// Sets the size of this GameObject.
    /// </summary>
    /// <param name="i_size">The new size of the GameObject.</param>
    public static void SetSize(this GameObject i_gameObject, Vector3 i_size)
    {
        //If the size of the GameObject is not 0.
        if (i_gameObject.GetSize() != Vector3.zero)
        {

            //Resizes the GameObject.
            i_gameObject.transform.localScale = new Vector3(i_gameObject.transform.localScale.x * (i_size.x / i_gameObject.GetSize().x), i_gameObject.transform.localScale.y * (i_size.y / i_gameObject.GetSize().y), i_gameObject.transform.localScale.z * (i_size.z / i_gameObject.GetSize().z));
        }
    }

    /// <summary>
    /// Gets the grayscale version of this Texture2D.
    /// </summary>
    /// <returns>The grayscale texture.<returns>
    public static Texture2D ToGrayscale(this Texture2D i_texture)
    {
        //Creates the result texture.
        Texture2D resTexture = new Texture2D(i_texture.width, i_texture.height);

        //Get the pixel colors of the original texture.
        UnityEngine.Color[] pixels = i_texture.GetPixels();

        //For each pixel.
        for (int i = 0; i < pixels.Length; i++)
        {

            //Gets the grayscale value of the pixel.
            float gray = pixels[i].grayscale;

            //Sets the grayscale value as the pixel color.
            pixels[i].r = gray;
            pixels[i].g = gray;
            pixels[i].b = gray;
        }

        //Set the pixels to the result texture.
        resTexture.SetPixels(pixels);
        resTexture.Apply();

        return resTexture;
    }

    /// <summary>
    /// Transforms this UnityEngine.Color into CIEColor.CIE_XYZColor.
    /// </summary>
    /// <returns>The CIE XYZ value of the Color.</returns>
    public static CIE_XYZColor ToCIE_XYZ(this Color color)
    {
        //Ecuations: http://www.brucelindbloom.com/

        CIE_XYZColor cieXyz = new CIE_XYZColor();

        //Inverse companding
        float auxR = color.r <= 0.04045f ? color.r / 12.92f : Mathf.Pow((color.r + 0.055f) / 1.055f, 2.4f);
        float auxG = color.g <= 0.04045f ? color.g / 12.92f : Mathf.Pow((color.g + 0.055f) / 1.055f, 2.4f);
        float auxB = color.b <= 0.04045f ? color.b / 12.92f : Mathf.Pow((color.b + 0.055f) / 1.055f, 2.4f);

        //Linear RGB to XYZ
        float[][] transformationMatrix =
            {
                new float[] { 0.4124564f, 0.3575761f, 0.1804375f },
                new float[] { 0.2126729f, 0.7151522f, 0.0721750f },
                new float[] { 0.0193339f, 0.1191920f, 0.9503041f }
            };

        float[][] rgbMatrix =
            {
                new float[] { auxR },
                new float[] { auxG },
                new float[] { auxB }
            };

        float[][] cieXyzMatrix = MathMatrix.MatrixMultiplication(transformationMatrix, rgbMatrix);

        cieXyz._x = cieXyzMatrix[0][0];
        cieXyz._y = cieXyzMatrix[1][0];
        cieXyz._z = cieXyzMatrix[2][0];

        return cieXyz;
    }

    /// <summary>
    /// Transforms this UnityEngine.Color into CIEColor.CIELabColor.
    /// </summary>
    /// <returns>The CIE Lab value of the Color.</returns>
    public static CIELabColor ToCIELab(this Color color)
    {
        //Ecuations: http://www.brucelindbloom.com/

        CIE_XYZColor cieXyz = color.ToCIE_XYZ();

        CIE_XYZColor referenceWhite = Color.white.ToCIE_XYZ();

        //xr = X / Xr
        float auxX = cieXyz._x / referenceWhite._x;

        //yr = Y / Yr
        float auxY = cieXyz._y / referenceWhite._y;

        //zr = Z / Zr
        float auxZ = cieXyz._z / referenceWhite._z;

        //if xr > 0.008856
        //  fx = xr ^ 1/3
        //else
        //  fx = (903.3 * xr + 16) / 116
        auxX = (auxX > 0.008856f) ? Mathf.Pow(auxX, 1 / 3f) : ((903.3f * auxX + 16) / 116f);

        //if yr > 0.008856
        //  fy = yr ^ 1/3
        //else
        //  fy = (903.3 * yr + 16) / 116
        auxY = (auxY > 0.008856f) ? Mathf.Pow(auxY, 1 / 3f) : ((903.3f * auxY + 16) / 116f);

        //if zr > 0.008856
        //  fz = zr ^ 1/3
        //else
        //  fz = (903.3 * zr + 16) / 116
        auxZ = (auxZ > 0.008856f) ? Mathf.Pow(auxZ, 1 / 3f) : ((903.3f * auxZ + 16) / 116f);

        CIELabColor cieLab = new CIELabColor();

        //L = 116 * fy -16
        cieLab._l = 116 * auxY - 16;

        //a = 500 * (fx - fy)
        cieLab._a = 500 * (auxX - auxY);

        //b = 200 * (fy - fz)
        cieLab._b = 200 * (auxY - auxZ);

        return cieLab;
    }
}
