using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float p_time;

    [SerializeField]
    private GameObject p_defaultFloorTile;

    private Vector3 p_startPos;

    private Vector3 p_goal;

    private float p_timePast;

    private bool p_locked;

    private bool p_willMove;

    private GameObject p_barrierDetectionCollider;

    private GameObject p_photoAtackCollider;

    void Start()
    {
        p_goal = transform.position;
        p_barrierDetectionCollider = gameObject.GetChild("BarrierDetectionCollider");
        RemoteCollider2DEventController collider = p_barrierDetectionCollider.GetComponent<RemoteCollider2DEventController>();
        collider._onTriggerEnter2D += WayLocked;
        collider._onTriggerExit2D += WayUnlocked;
        p_photoAtackCollider = gameObject.GetChild("PhotoAttackCollider");
        p_photoAtackCollider.SetActive(false);
    }


    void FixedUpdate()
    {
        if (p_goal != transform.position)
        {
            p_timePast += Time.fixedDeltaTime;
            transform.position = Vector3.Lerp(p_startPos, p_goal, p_timePast / p_time);
        }
        else if (p_willMove)
        {
            p_willMove = false;

            if (!p_locked)
            {
                if (transform.rotation.eulerAngles == new Vector3(0, 0, 0))
                {
                    p_startPos = transform.position;
                    p_goal = transform.position + new Vector3(0, p_defaultFloorTile.GetSize().y, 0);
                    p_timePast = 0;
                }
                else if (transform.rotation.eulerAngles == new Vector3(0, 0, 180))
                {
                    p_startPos = transform.position;
                    p_goal = transform.position - new Vector3(0, p_defaultFloorTile.GetSize().y, 0);
                    p_timePast = 0;
                }
                else if (transform.rotation.eulerAngles == new Vector3(0, 0, 270))
                {
                    p_startPos = transform.position;
                    p_goal = transform.position + new Vector3(p_defaultFloorTile.GetSize().x, 0, 0);
                    p_timePast = 0;
                }
                else if (transform.rotation.eulerAngles == new Vector3(0, 0, 90))
                {
                    p_startPos = transform.position;
                    p_goal = transform.position - new Vector3(p_defaultFloorTile.GetSize().x, 0, 0);
                    p_timePast = 0;
                }

                LevelController.Instance.NextPosition(gameObject, p_goal);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameObject aux = (GameObject)Instantiate(p_photoAtackCollider);
                aux.transform.position = p_photoAtackCollider.transform.position;
                aux.transform.localScale = new Vector3(0.8f, 0.8f, 1);
                aux.SetActive(true);
            }
            else if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    p_barrierDetectionCollider.SetActive(false);
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    p_barrierDetectionCollider.SetActive(true);
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    p_barrierDetectionCollider.SetActive(false);
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                    p_barrierDetectionCollider.SetActive(true);
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    p_barrierDetectionCollider.SetActive(false);
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
                    p_barrierDetectionCollider.SetActive(true);
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    p_barrierDetectionCollider.SetActive(false);
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                    p_barrierDetectionCollider.SetActive(true);
                }
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                if (transform.rotation.eulerAngles == new Vector3(0, 0, 0))
                {
                    if (!p_locked)
                    {
                        p_startPos = transform.position;
                        p_goal = transform.position + new Vector3(0, p_defaultFloorTile.GetSize().y, 0);
                        p_timePast = 0;
                        LevelController.Instance.NextPosition(gameObject, p_goal);
                    }
                }
                else
                {
                    p_barrierDetectionCollider.SetActive(false);
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    p_willMove = true;
                    p_barrierDetectionCollider.SetActive(true);
                }
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                if (transform.rotation.eulerAngles == new Vector3(0, 0, 180))
                {
                    if (!p_locked)
                    {
                        p_startPos = transform.position;
                        p_goal = transform.position - new Vector3(0, p_defaultFloorTile.GetSize().y, 0);
                        p_timePast = 0;
                        LevelController.Instance.NextPosition(gameObject, p_goal);
                    }
                }
                else
                {
                    p_barrierDetectionCollider.SetActive(false);
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                    p_willMove = true;
                    p_barrierDetectionCollider.SetActive(true);
                }
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                if (transform.rotation.eulerAngles == new Vector3(0, 0, 270))
                {
                    if (!p_locked)
                    {
                        p_startPos = transform.position;
                        p_goal = transform.position + new Vector3(p_defaultFloorTile.GetSize().x, 0, 0);
                        p_timePast = 0;
                        LevelController.Instance.NextPosition(gameObject, p_goal);
                    }
                }
                else
                {
                    p_barrierDetectionCollider.SetActive(false);
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
                    p_willMove = true;
                    p_barrierDetectionCollider.SetActive(true);
                }
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                if (transform.rotation.eulerAngles == new Vector3(0, 0, 90))
                {
                    if (!p_locked)
                    {
                        p_startPos = transform.position;
                        p_goal = transform.position - new Vector3(p_defaultFloorTile.GetSize().x, 0, 0);
                        p_timePast = 0;
                        LevelController.Instance.NextPosition(gameObject, p_goal);
                    }
                }
                else
                {
                    p_barrierDetectionCollider.SetActive(false);
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                    p_willMove = true;
                    p_barrierDetectionCollider.SetActive(true);
                }
            }
        }
    }

    public void WayLocked()
    {
        p_locked = true;
    }

    public void WayUnlocked()
    {
        p_locked = false;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        LevelController.Instance.PlayerHit();
    }

    public void ResetPlayer()
    {
        p_goal = transform.position;
        p_willMove = false;
    }
}
