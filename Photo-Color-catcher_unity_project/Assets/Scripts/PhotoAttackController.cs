using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component used to detect attack collisions with enemies.
/// </summary>
public class PhotoAttackController : MonoBehaviour
{
    /// <summary>
    /// The time that the attack longs.
    /// </summary>
    [SerializeField]
    private float p_time;

    /// <summary>
    /// If the attacks stores the color of the enemy killed ot not.
    /// </summary>
    public bool _storeColor;

    /// <summary>
    /// The cell where the attack was launched.
    /// </summary>
    private GameObject p_cell;

    /// <summary>
    /// The enemy that has been hit by the attack.
    /// </summary>
    private GameObject p_enemy;

    /// <summary>
    /// The Coroutine that destroys this attack GameObject.
    /// </summary>
    private Coroutine p_destroy;

    void Start()
    {
        p_destroy = StartCoroutine(DestroyCoroutine());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //If the GameObject detected is in the Enemy layer.
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (p_enemy == null)
            {
                //Gets the enemy hit by the attack.
                p_enemy = other.gameObject.GetComponentInParent<EnemyController>().gameObject;
            }
        }
        //If the attack doesn't stores the color and the layer of the hit GameObject is Ground.
        else if (!_storeColor && other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (p_cell == null)
            {
                //Gets the cell where the attack was launched.
                p_cell = other.gameObject;
            }
        }

        //If the attack doesn't stores the color and there is an enemy hit by the attack, or if there is an enemy hit by the attack and the cell where the attack was launched was caught.
        if ((_storeColor && p_enemy != null) || (p_cell != null && p_enemy != null))
        {
            //Stops the destroy Coroutine.
            if (p_destroy != null)
                StopCoroutine(p_destroy);

            //Activates the effect of the attack.
            AttackEffect();

            //Destroy the GameObject.
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Coroutine that destroys the GameObject when the time of the attack has passed.
    /// </summary>
    private IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(p_time);

        Destroy(gameObject);
    }

    /// <summary>
    /// Destroys the enemy and color the cell where the enemy was killed or stores the color of the enemy.
    /// </summary>
    private void AttackEffect()
    {
        //If the color has to be stored.
        if (_storeColor)
        {
            //Stores the color of the enemy.
            LevelController.Instance.StoreColor(p_enemy.GetComponent<EnemyController>()._RGBComponent);
        }
        else
            //Adds the enemy color to the cell.
            p_cell.GetComponent<CellColorGoal>().AddRGBComponent(p_enemy.GetComponent<EnemyController>()._RGBComponent);

        //Notifies the LevelController that the enemy was killed.
        LevelController.Instance.EnemyKilled(p_enemy);

        //Destroys the enemy.
        Destroy(p_enemy);
    }

}
