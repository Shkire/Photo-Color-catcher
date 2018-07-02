using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasicDataTypes;

/// <summary>
/// Component used for assigning a RGB combination goal to a LevelCell. It also manages if the goal is completed or not.
/// </summary>
public class CellColorGoal : MonoBehaviour
{
    /// <summary>
    /// The RGB combination goal.
    /// </summary>
    public RGBContent _RGBGoal;

    /// <summary>
    /// The RGB combination now.
    /// </summary>
    private RGBContent p_RGBNow;

    /// <summary>
    /// The white cell prefab, used for coloring the LevelCell with the colors of the enemies that the player have killed in it.
    /// </summary>
    [SerializeField]
    private GameObject p_whiteCell;

    /// <summary>
    /// The time that the colored LevelCell needs to be cleaned.
    /// </summary>
    [SerializeField]
    private float p_restartTime;

    /// <summary>
    /// The GameObject of the colored LevelCell.
    /// </summary>
    private GameObject p_colorNow;

    /// <summary>
    /// The remaining time to the LevelCell cleaning.
    /// </summary>
    private float p_remainigTime;

    /// <summary>
    /// Adds the RGB component to color the LevelCell.
    /// </summary>
    /// <param name="i_RGBContent">The RGBContent of the color to add to the LevelCell.</param>
    public void AddRGBComponent(RGBContent i_RGBContent)
    {        
        //If the LevelCell is completed.
        if (_RGBGoal.Equals(p_RGBNow))
        {
            //Adds the color to the garbage.
            LevelController.Instance.AddGarbage(i_RGBContent);
        }
        else
        {
            //If the LevelCell is not colored.
            if (p_RGBNow == null)
                p_RGBNow = new RGBContent(false, false, false);

            //If the color has the R component and the cell is not colored with the R component.
            if (i_RGBContent._r && !p_RGBNow._r)
            {
                p_RGBNow._r = true;
                p_remainigTime = p_restartTime;
            }
            //If the color has the G component and the cell is not colored with the G component.
            if (i_RGBContent._g && !p_RGBNow._g)
            {
                p_RGBNow._g = true;
                p_remainigTime = p_restartTime;
            }
            //If the color has the B component and the cell is not colored with the B component.
            if (i_RGBContent._b && !p_RGBNow._b)
            {
                p_RGBNow._b = true;
                p_remainigTime = p_restartTime;
            }

            //If the color goal is equal to the color of the LevelCell now.
            if (_RGBGoal.Equals(p_RGBNow))
            {
                //Notifies the LevelController that the LevelCell has been completed.
                LevelController.Instance.CellCompleted(gameObject);

                //Disables the uncolored cell.
                GetComponent<SpriteRenderer>().enabled = false;
            }

            //If the GameObject of the colored cell is null.
            if (p_colorNow == null)
            {
                //Creates a new colored cell GameObject.
                p_colorNow = (GameObject)Instantiate(p_whiteCell, transform.position, transform.rotation);
                p_colorNow.transform.SetParent(transform);
            }

            //Gets the color of the LevelCell now and applies it to the white cell Sprite.
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
        //If the LevelCell is colored.
        if (p_colorNow != null)
        {
            //Calculates remainingTime.
            if (p_remainigTime > 0)
                p_remainigTime -= Time.fixedDeltaTime;

            //If time needed to clean the cell has passed.
            if (p_remainigTime <= 0)
            {
                //Destroys the colored cell.
                Destroy(p_colorNow);

                //If the color of the colored cell is not the goal color.
                if (!p_RGBNow.Equals(_RGBGoal))
                {
                    //Adds the color to the garbage.
                    LevelController.Instance.AddGarbage(p_RGBNow);
                    p_RGBNow = new RGBContent(false, false, false);
                }
            }
            else
            {
                //Linearly interpolates the alpha value of the colored cell (between 0 and restartTime).
                Color aux = p_colorNow.GetComponent<SpriteRenderer>().color;
                aux.a = Mathf.Lerp(0, 1, p_remainigTime / p_restartTime);
                p_colorNow.GetComponent<SpriteRenderer>().color = aux;
            }
        }
    }
}
