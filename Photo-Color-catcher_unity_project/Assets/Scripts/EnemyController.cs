using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasicDataTypes;

public class EnemyController : MonoBehaviour
{
    public RGBContent _RGBComponent;

    [SerializeField]
    private float p_movementTime;

    [SerializeField]
    private float p_maxTime;

    [SerializeField]
    private float p_minTime;

    [SerializeField]
    private GameObject p_defaultFloorTile;

    private Vector3 p_startPos;

    private Vector3 p_goal;

    private float p_timePast;

    private float p_remainingTime;

    private bool p_locked;

    private bool p_willMove;

    private GameObject p_barrierDetectionCollider;

    private List<Vector3> p_directions;
      
    private Vector3 p_direction;

    void Start()
    {
        p_goal = transform.position;
        p_barrierDetectionCollider = gameObject.GetChild("BarrierDetectionCollider");
        RemoteCollider2DEventController collider = p_barrierDetectionCollider.GetComponent<RemoteCollider2DEventController>();
        collider._onTriggerEnter2D += WayLocked;
        collider._onTriggerExit2D += WayUnlocked;
        p_remainingTime = Random.Range(p_minTime, p_maxTime);

        p_directions = new List<Vector3>()
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 90),
            new Vector3(0, 0, 180),
            new Vector3(0, 0, 270)
        };
    }


    void FixedUpdate()
    {
        if (p_goal != transform.position)
        {
            p_timePast += Time.fixedDeltaTime;
            transform.position = Vector3.Lerp(p_startPos, p_goal, p_timePast / p_movementTime);

            if (transform.position == p_goal)
                p_remainingTime = Random.Range(p_minTime, p_maxTime);

            p_directions = new List<Vector3>()
            {
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 90),
                new Vector3(0, 0, 180),
                new Vector3(0, 0, 270)
            };
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
            else
            {
                p_remainingTime = p_minTime;
            }
        }
        else if (p_remainingTime <= 0)
        {
            p_direction = p_directions[Random.Range(0, p_directions.Count)];

            p_directions.Remove(p_direction);

            if (p_direction == new Vector3(0, 0, 0))
            {
                if (transform.rotation.eulerAngles == p_direction)
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
            else if (p_direction == new Vector3(0, 0, 180))
            {
                if (transform.rotation.eulerAngles == p_direction)
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
            else if (p_direction == new Vector3(0, 0, 270))
            {
                if (transform.rotation.eulerAngles == p_direction)
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
            else if (p_direction == new Vector3(0, 0, 90))
            {
                if (transform.rotation.eulerAngles == p_direction)
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
        else
        {
            p_remainingTime -= Time.fixedDeltaTime;
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
}
