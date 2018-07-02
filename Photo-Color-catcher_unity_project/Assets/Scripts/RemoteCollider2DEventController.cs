using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Component used to assign Actions to Collider2D physics events.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class RemoteCollider2DEventController : MonoBehaviour
{
    public event Action _onCollsionEnter2D;

    public event Action _onCollsionExit2D;

    public event Action _onCollsionStay2D;

    public event Action _onTriggerEnter2D;

    public event Action _onTriggerExit2D;

    public event Action _onTriggerStay2D;

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (_onCollsionEnter2D != null)
            _onCollsionEnter2D();
    }

    void OnCollisionExit2D(Collision2D coll)
    {
        if (_onCollsionExit2D != null)
            _onCollsionExit2D();
    }

    void OnCollisionStay2D(Collision2D coll)
    {
        if (_onCollsionStay2D != null)
            _onCollsionStay2D();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_onTriggerEnter2D != null)
            _onTriggerEnter2D();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (_onTriggerExit2D != null)
            _onTriggerExit2D();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (_onTriggerStay2D != null)
            _onTriggerStay2D();
    }
}
