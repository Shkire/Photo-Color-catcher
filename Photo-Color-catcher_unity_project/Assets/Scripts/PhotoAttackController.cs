﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoAttackController : MonoBehaviour
{

    [SerializeField]
    private float p_time;

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
                p_enemy = other.gameObject;
            }
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (p_cell == null)
            {
                p_cell = other.gameObject;
            }
        }

        if (p_cell != null && p_enemy != null)
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
        p_cell.GetComponent<CellColorGoal>().AddRGBComponent(p_enemy.GetComponent<EnemyController>()._RGBComponent);
        Destroy(p_enemy);
    }

}