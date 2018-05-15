using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasicDataTypes;

public class CellColorGoal : MonoBehaviour
{

    public RGBContent _RGBGoal;

    private RGBContent p_RGBNow;

    [SerializeField]
    private GameObject p_whiteCell;

    [SerializeField]
    private float p_restartTime;

    private GameObject p_colorNow;

    private float p_remainigTime;

    public void AddRGBComponent(RGBContent i_RGBContent)
    {
        if (_RGBGoal.Equals(p_RGBNow))
        {
            LevelController.Instance.AddGarbage(i_RGBContent);
        }
        else
        {
            if (p_RGBNow == null)
                p_RGBNow = new RGBContent(false, false, false);

            if (i_RGBContent._r && !p_RGBNow._r)
            {
                p_RGBNow._r = true;
                p_remainigTime = p_restartTime;
            }
            if (i_RGBContent._g && !p_RGBNow._g)
            {
                p_RGBNow._g = true;
                p_remainigTime = p_restartTime;
            }
            if (i_RGBContent._b && !p_RGBNow._b)
            {
                p_RGBNow._b = true;
                p_remainigTime = p_restartTime;
            }

            if (_RGBGoal.Equals(p_RGBNow))
            {
                LevelController.Instance.CellCompleted(gameObject);
                GetComponent<SpriteRenderer>().enabled = false;
            }

            if (p_colorNow == null)
                p_colorNow = (GameObject)Instantiate(p_whiteCell, transform.position, transform.rotation);
            Color aux = Color.black;
            if (p_RGBNow._r)
                aux.r = 1;
            if (p_RGBNow._g)
                aux.g = 1;
            if (p_RGBNow._b)
                aux.b = 1;
            p_colorNow.GetComponent<SpriteRenderer>().color = aux;
        }
    }

    void FixedUpdate()
    {
        if (p_colorNow != null)
        {
            if (p_remainigTime > 0)
                p_remainigTime -= Time.fixedDeltaTime;

            if (p_remainigTime <= 0)
            {
                Destroy(p_colorNow);
                if (!p_RGBNow.Equals(_RGBGoal))
                {
                    LevelController.Instance.AddGarbage(p_RGBNow);
                    p_RGBNow = new RGBContent(false, false, false);
                }
            }
            else
            {

                Color aux = p_colorNow.GetComponent<SpriteRenderer>().color;

                aux.a = Mathf.Lerp(0, 1, p_remainigTime / p_restartTime);

                p_colorNow.GetComponent<SpriteRenderer>().color = aux;
            }
        }
    }
}
