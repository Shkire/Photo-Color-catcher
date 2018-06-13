using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoAttackController : MonoBehaviour
{

    [SerializeField]
    private float p_time;

    public bool _storeColor;

    private GameObject p_cell;

    private GameObject p_enemy;

    private Coroutine p_destroy;

    void Start()
    {
        p_destroy = StartCoroutine(DestroyCoroutine());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (p_enemy == null)
            {
                p_enemy = other.gameObject.GetComponentInParent<EnemyController>().gameObject;
            }
        }
        else if (!_storeColor && other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (p_cell == null)
            {
                p_cell = other.gameObject;
            }
        }

        if ((_storeColor && p_enemy!=null) || (p_cell != null && p_enemy != null))
        {
            if (p_destroy != null)
                StopCoroutine(p_destroy);

            AttackEffect();

            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(p_time);

        Destroy(gameObject);
    }

    private void AttackEffect()
    {
        if (_storeColor)
        {
            LevelController.Instance.StoreColor(p_enemy.GetComponent<EnemyController>()._RGBComponent);
        }
        else
            p_cell.GetComponent<CellColorGoal>().AddRGBComponent(p_enemy.GetComponent<EnemyController>()._RGBComponent);
        LevelController.Instance.EnemyKilled(p_enemy);
        Destroy(p_enemy);
    }

}
