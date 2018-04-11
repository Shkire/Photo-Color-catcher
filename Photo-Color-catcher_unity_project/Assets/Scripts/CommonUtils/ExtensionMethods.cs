using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Extension methods.
/// </summary>
public static class ExtensionMethods
{
    public static Texture2D ResizeBilinear(this Texture2D text, int newXSize, int newYSize)
    {
        //Create result texture and get pixels
        Texture2D result = new Texture2D(newXSize, newYSize, text.format, true);
        UnityEngine.Color[] rpixels = result.GetPixels();
        //Calculate inc
        float incX = (1.0f / (float)newXSize);
        float incY = (1.0f / (float)newYSize);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = text.GetPixelBilinear(incX * ((float)px % newXSize), incY * ((float)Mathf.Floor(px / newYSize)));
        }
        result.SetPixels(rpixels); 
        result.Apply(); 
        return result;
    }

    public static GameObject GetChild(this GameObject go)
    {
        foreach (Transform aux in go.GetComponentsInChildren<Transform>())
            if (aux.gameObject.GetInstanceID() != go.GetInstanceID())
                return aux.gameObject;
        return null;
    }

    public static GameObject[] GetChildren(this GameObject go)
    {
        List<GameObject> auxList = new List<GameObject>();
        foreach (Transform aux in go.GetComponentsInChildren<Transform>())
            if (aux.gameObject.GetInstanceID() != go.GetInstanceID())
                auxList.Add(aux.gameObject);
        return auxList.ToArray();
    }

    public static GameObject GetChild(this GameObject go, string name)
    {
        foreach (Transform aux in go.GetComponentsInChildren<Transform>())
            if (aux.gameObject.GetInstanceID() != go.GetInstanceID() && aux.gameObject.name.Equals(name))
                return aux.gameObject;
        return null;
    }

    public static Vector3 GetSize(this GameObject go)
    {
        Renderer rend = go.GetComponent<Renderer>();
        if (rend != null)
        {
            return rend.bounds.extents * 2;
        }
        Collider coll = go.GetComponent<Collider>();
        if (coll != null)
        {
            return coll.bounds.extents * 2;
        }
        return Vector3.zero;
    }

    public static void SetSize(this GameObject go, Vector3 i_size)
    {
        if (go.GetSize() != Vector3.zero)
        {
            go.transform.localScale = new Vector3(go.transform.localScale.x * (i_size.x / go.GetSize().x), go.transform.localScale.y * (i_size.y / go.GetSize().y), go.transform.localScale.z * (i_size.z / go.GetSize().z));
        }
    }

    public static Texture2D ToGray(this Texture2D texture)
    {
        Texture2D resTexture = new Texture2D(texture.width, texture.height);
        UnityEngine.Color[] temp = texture.GetPixels();
        for (int i = 0; i < temp.Length; i++)
        {
            float gray = temp[i].grayscale;
            temp[i].r = gray;
            temp[i].g = gray;
            temp[i].b = gray;
        }
        resTexture.SetPixels(temp);
        resTexture.Apply();
        return resTexture;
    }

    public static CieXyzColor ToCieXyz(this Color color)
    {
        CieXyzColor cieXyz = new CieXyzColor();

        //Inverse companding
        float auxR = color.r <= 0.04045f ? color.r / 12.92f : Mathf.Pow((color.r + 0.055f) / 1.055f, 2.4f);
        float auxG = color.g <= 0.04045f ? color.g / 12.92f : Mathf.Pow((color.g + 0.055f) / 1.055f, 2.4f);
        float auxB = color.b <= 0.04045f ? color.b / 12.92f : Mathf.Pow((color.b + 0.055f) / 1.055f, 2.4f);

        //Linear rgb to XYZ
        float[][] transformationMatrix =
            {
                new float[] { 0.4124564f, 0.3575761f, 0.1804375f },
                new float[] { 0.2126729f, 0.7151522f, 0.0721750f },
                new float[] { 0.0193339f, 0.1191920f, 0.9503041f }
            };

        float[][] rgbMatrix =
            {
			new float[] {auxR},
			new float[] {auxG},
			new float[] {auxB}
		};

		float[][] cieXyzMatrix = MathMatrix.MatrixMultiplication(transformationMatrix,rgbMatrix);

		cieXyz.x = cieXyzMatrix[0][0];
		cieXyz.x = cieXyzMatrix[1][0];
		cieXyz.x = cieXyzMatrix[2][0];

		return cieXyz;
	}

	public static CieLabColor ToCieLab (this Color color)
	{
		CieXyzColor cieXyz = color.ToCieXyz ();

		CieXyzColor referenceWhite = Color.white.ToCieXyz ();

		float auxX = cieXyz.x / referenceWhite.x;
		float auxY = cieXyz.y / referenceWhite.y;
		float auxZ = cieXyz.z / referenceWhite.z;

		auxX = (auxX > 0.008856f) ? Mathf.Pow (auxX, 1 / 3f) : ((903.3f * auxX + 16) / 116f);
		auxY = (auxY > 0.008856f) ? Mathf.Pow (auxY, 1 / 3f) : ((903.3f * auxY + 16) / 116f);
		auxZ = (auxZ > 0.008856f) ? Mathf.Pow (auxZ, 1 / 3f) : ((903.3f * auxZ + 16) / 116f);

		CieLabColor cieLab = new CieLabColor ();

		cieLab.l = 116 * auxY - 16;
		cieLab.a = 500 * (auxX - auxY);
		cieLab.b = 200 * (auxY - auxZ);

		return cieLab;
	}
}
