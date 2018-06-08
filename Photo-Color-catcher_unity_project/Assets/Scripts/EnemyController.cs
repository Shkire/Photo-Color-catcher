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
    private float p_followingMovementTime;

    [SerializeField]
    private float p_maxTime;

    [SerializeField]
    private float p_minTime;

    [SerializeField]
    private GameObject p_defaultFloorTile;

    [SerializeField]
    private float p_viewRange;

    private Vector3 p_startPos;

    private Vector3 p_goal;

    private float p_timePast;

    private float p_remainingTime;

    private bool p_locked;

    private bool p_willMove;

    private GameObject p_barrierDetectionCollider;

    private List<Vector3> p_directions;
      
    private Vector3 p_direction;

    private Vector3 p_detectedPlayerPos;

    private bool p_searching;

    private Animator p_animator;

    void Start()
    {
        p_goal = transform.position;
        p_barrierDetectionCollider = gameObject.GetChild("BarrierDetectionCollider");
        RemoteCollider2DEventController collider = p_barrierDetectionCollider.GetComponent<RemoteCollider2DEventController>();
        collider._onTriggerEnter2D += WayLocked;
        collider._onTriggerExit2D += WayUnlocked;
        p_remainingTime = Random.Range(p_minTime, p_maxTime);
        p_animator = GetComponent<Animator>();

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
        //Moving
        if (p_goal != transform.position)
        {
            p_timePast += Time.fixedDeltaTime;
            if (p_searching)
            {
                transform.position = Vector3.Lerp(p_startPos, p_goal, p_timePast / (p_followingMovementTime)); 

                if (transform.position == p_goal)
                {
                    p_animator.SetTrigger("Idle");

                    if (transform.position != p_detectedPlayerPos)
                        p_remainingTime = p_minTime;
                    else
                    {
                        p_remainingTime = Random.Range(p_minTime, p_maxTime);

                        p_searching = false;

                        p_directions = new List<Vector3>()
                        {
                            new Vector3(0, 0, 0),
                            new Vector3(0, 0, 90),
                            new Vector3(0, 0, 180),
                            new Vector3(0, 0, 270)
                        };
                    }
                }
            }
            else
            {
                transform.position = Vector3.Lerp(p_startPos, p_goal, p_timePast / p_movementTime); 

                if (transform.position == p_goal)
                {
                    p_animator.SetTrigger("Idle");
                    p_remainingTime = Random.Range(p_minTime, p_maxTime);
                }

                p_directions = new List<Vector3>()
                {
                    new Vector3(0, 0, 0),
                    new Vector3(0, 0, 90),
                    new Vector3(0, 0, 180),
                    new Vector3(0, 0, 270)
                };
            }
        }
        else
        {
            DetectPlayer();
            //Rotated
            if (p_willMove)
            {
                p_willMove = false;

                if (p_locked)
                {
                    p_remainingTime = p_minTime; 
                }
                //Check if posible moving
                else if (LevelController.Instance.EmptySpace(transform.position + transform.rotation * new Vector3(0, p_defaultFloorTile.GetSize().y, 0)))
                {
                    Move(transform.position + transform.rotation * new Vector3(0, p_defaultFloorTile.GetSize().y, 0));
                }
                //if not (occupied)
                else
                {
                    p_searching = false;
                    p_directions.Add(transform.rotation.eulerAngles);
                    p_remainingTime = Random.Range(p_minTime, p_maxTime);
                }

            }
            //New action
            else if (p_remainingTime <= 0)
            {
                p_direction = p_directions[Random.Range(0, p_directions.Count)];

                if (p_searching || p_direction == transform.rotation.eulerAngles)
                {
                    //if locked
                    if (p_locked)
                    {
                        p_remainingTime = p_minTime; 
                        p_directions.Remove(p_direction);
                    }
                    //Check if posible moving
                    else if (LevelController.Instance.EmptySpace(transform.position + transform.rotation * new Vector3(0, p_defaultFloorTile.GetSize().y, 0)))
                    {
                        Move(transform.position + transform.rotation * new Vector3(0, p_defaultFloorTile.GetSize().y, 0));
                    }
                    //if not (occupied)
                    else
                    {
                        p_searching = false;
                        p_remainingTime = Random.Range(p_minTime, p_maxTime);
                    }
                }
                else
                {
                    p_barrierDetectionCollider.SetActive(false);
                    transform.rotation = Quaternion.Euler(p_direction);
                    p_willMove = true;
                    p_barrierDetectionCollider.SetActive(true);
                    p_directions.Remove(p_direction);
                }
            }
            //less time
            else
            {
                p_remainingTime -= Time.fixedDeltaTime;
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

    private void Move(Vector3 i_pos)
    {
        p_startPos = transform.position;
        p_goal = i_pos;
        p_timePast = 0;
        LevelController.Instance.NextPosition(gameObject, p_goal); 
        if (p_searching)
            p_animator.SetTrigger("Following");
        else
            p_animator.SetTrigger("Moving");
    }

    private void DetectPlayer()
    {
        RaycastHit2D hit;

        hit = Physics2D.Raycast(transform.position + transform.rotation * new Vector2(0, gameObject.GetSize().y / 2f), Quaternion.Euler(transform.rotation.eulerAngles) * Vector2.up, p_viewRange * p_defaultFloorTile.GetSize().x, LayerMask.GetMask("Barrier", "Enemy", "Player"));

        Debug.DrawRay(transform.position, Quaternion.Euler(transform.rotation.eulerAngles) * Vector2.up, Color.white, 2);

        if (hit != null)
            Debug.Log(hit.collider.gameObject);

        if (hit != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (transform.rotation.eulerAngles == new Vector3(0, 0, 0) || transform.rotation.eulerAngles == new Vector3(0, 0, 180))
            {
                p_detectedPlayerPos = new Vector3(transform.position.x, transform.position.y + Mathf.Round((hit.transform.position.y - transform.position.y) / p_defaultFloorTile.GetSize().y) * p_defaultFloorTile.GetSize().y, transform.position.z);
            }
            else
            {
                p_detectedPlayerPos = new Vector3(transform.position.x + Mathf.Round((hit.transform.position.x - transform.position.x) / p_defaultFloorTile.GetSize().x) * p_defaultFloorTile.GetSize().x, transform.position.y, transform.position.z);
            }

            p_searching = true;

            /*

            if (transform.rotation.eulerAngles == new Vector3(0, 0, 0))
            {
                Move(transform.position + new Vector3(0, p_defaultFloorTile.GetSize().y, 0));
            }
            else if (transform.rotation.eulerAngles == new Vector3(0, 0, 180))
            {
                Move(transform.position - new Vector3(0, p_defaultFloorTile.GetSize().y, 0));
            }
            else if (transform.rotation.eulerAngles == new Vector3(0, 0, 270))
            {
                Move(transform.position + new Vector3(p_defaultFloorTile.GetSize().x, 0, 0));
            }
            else if (transform.rotation.eulerAngles == new Vector3(0, 0, 90))
            {
                Move(transform.position - new Vector3(p_defaultFloorTile.GetSize().x, 0, 0));
            }
            */

            if (p_remainingTime > p_minTime)
                p_remainingTime = p_minTime;
        }
    }
}
