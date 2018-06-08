using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FXChangeUIElementAlpha : FXElement{

    public float _value;

    public override IEnumerator StartFX(float i_duration)
    {
        float initialTime = Time.realtimeSinceStartup;

        List<Graphic> uiGraphics;

        if (_target != null)
        {
            uiGraphics = new List<Graphic>(_target.GetComponentsInChildren<Graphic>());

            List<float> originalAlpha = new List<float>();

            foreach (Graphic graphic in uiGraphics)
            {
                originalAlpha.Add(graphic.color.a);
            }

            bool finished = false;
            Color auxColor;

            while(!finished)
            {
                for (int i = 0; i < uiGraphics.Count; i++)
                {
                    auxColor = uiGraphics[i].color;
                    auxColor.a = Mathf.Lerp(originalAlpha[i], _value, (Time.realtimeSinceStartup - initialTime) / i_duration);
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
