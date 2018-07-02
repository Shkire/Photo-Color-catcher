using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// FXElement that allows to change the alpha value of the UIElements of the target GameObject.
/// </summary>
public class FXChangeUIElementAlpha : FXElement
{

    /// <summary>
    /// The new value of the alpha.
    /// </summary>
    public float _value;

    /// <summary>
    /// Starts the FX.
    /// </summary>
    /// <param name="i_duration">Duration of the FX.</param>
    public override IEnumerator StartFX(float i_duration)
    {
        //Gets the time when the FX started.
        float initialTime = Time.realtimeSinceStartup;

        //Creates the list of UI Graphic elements.
        List<Graphic> uiGraphics;

        //If there is a target.
        if (_target != null)
        {

            //Gets all the Graphic components in children GameObjects of the target (including itself).
            uiGraphics = new List<Graphic>(_target.GetComponentsInChildren<Graphic>());

            //Creates the list of original alpha values.
            List<float> originalAlpha = new List<float>();

            //For each Graphic.
            foreach (Graphic graphic in uiGraphics)
            {

                //Adds the original alpha value to the list.
                originalAlpha.Add(graphic.color.a);
            }

            bool finished = false;

            //While duration is not finished.
            while (!finished)
            {
                //For each Graphic.
                for (int i = 0; i < uiGraphics.Count; i++)
                {

                    //Gets the color of the Graphic.
                    Color auxColor = uiGraphics[i].color;

                    //Interpolates the alpha value using the time past since FX started.
                    auxColor.a = Mathf.Lerp(originalAlpha[i], _value, (Time.realtimeSinceStartup - initialTime) / i_duration);

                    //Sets the color to the Graphic.
                    uiGraphics[i].color = auxColor;
                }
                    
                if (Time.realtimeSinceStartup >= initialTime + i_duration)
                    finished = true;
                else
                    yield return null;
            }
        }
    }
}
