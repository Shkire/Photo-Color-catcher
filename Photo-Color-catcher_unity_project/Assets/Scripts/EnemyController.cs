using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasicDataTypes;

/// <summary>
/// Component thaht manages all behaviours of an enemy.
/// </summary>
public class EnemyController : MonoBehaviour
{
    /// <summary>
    /// The RGB components combination associated to this enemy.
    /// </summary>
    [SerializeField]
    private RGBContent p_RGBComponent;

    /// <summary>
    /// Gets the RGB components combination associated to this enemy.
    /// </summary>
    /// <value>The RGBComponent associated to this enemy.</value>
    public RGBContent _RGBComponent
    {
        get
        {
            return p_RGBComponent;
        }
    }

    /// <summary>
    /// The time needed to move the enemy from one LevelCell to the adjacent.
    /// </summary>
    [SerializeField]
    private float p_movementTime;

    /// <summary>
    /// The time needed to move the enemy from one LevelCell to the adjacent when following the player.
    /// </summary>
    [SerializeField]
    private float p_followingMovementTime;

    /// <summary>
    /// The maximum time that the enemy can stay waiting.
    /// </summary>
    [SerializeField]
    private float p_maxTime;

    /// <summary>
    /// The minimum time that the enemy can stay waiting.
    /// </summary>
    [SerializeField]
    private float p_minTime;

    /// <summary>
    /// The maximum number of LevelCells up to the enemy can see.
    /// </summary>
    [SerializeField]
    private float p_viewRange;

    /// <summary>
    /// The starting position of the movement.
    /// </summary>
    private Vector3 p_startPos;

    /// <summary>
    /// The final position of the movement.
    /// </summary>
    private Vector3 p_goal;

    /// <summary>
    /// The time past since the last movement.
    /// </summary>
    private float p_timePast;

    /// <summary>
    /// The remaining time for the next movement.
    /// </summary>
    private float p_remainingTime;

    /// <summary>
    /// If the way is locked.
    /// </summary>
    private bool p_locked;

    /// <summary>
    /// If the enemy will move at the next frame.
    /// </summary>
    private bool p_willMove;

    /// <summary>
    /// The GameObject of the Collider that detects barriers.
    /// </summary>
    private GameObject p_barrierDetectionCollider;

    /// <summary>
    /// The list of directions available to the enemy.
    /// </summary>
    private List<Vector3> p_directions;

    /// <summary>
    /// The position where the player was seen last time by this enemy.
    /// </summary>
    private Vector3 p_detectedPlayerPos;

    /// <summary>
    /// If it is searching the player.
    /// </summary>
    private bool p_searching;

    /// <summary>
    /// The animator of this enemy.
    /// </summary>
    private Animator p_animator;

    void Start()
    {
        //Sets the goal position as the current position.
        p_goal = transform.position;

        //Gets the BarrierDetectioCollider.
        p_barrierDetectionCollider = gameObject.GetChild("BarrierDetectionCollider");

        //Sets the actions for OnTriggerEnter and OnTriggerExit.
        RemoteCollider2DEventController collider = p_barrierDetectionCollider.GetComponent<RemoteCollider2DEventController>();
        collider._onTriggerEnter2D += WayLocked;
        collider._onTriggerExit2D += WayUnlocked;

        //Sets the remaining time between min and max.
        p_remainingTime = Random.Range(p_minTime, p_maxTime);

        //Gets the enemy animator.
        p_animator = GetComponent<Animator>();

        //Sets the available directions.
        p_directions = new List<Vector3>()
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 90),
            new Vector3(0, 0, 180),
            new Vector3(0, 0, 270)
        };
    }

    void Update()
    {
        //If the enemy has finished its movement.
        if (p_goal == transform.position)
            //Detects if there is a player on enemy sight.
            DetectPlayer();
    }

    void FixedUpdate()
    {
        Vector3 direction;

        //If the enemy is moving.
        if (p_goal != transform.position)
        {
            p_timePast += Time.fixedDeltaTime;
            //If the enemy is searching the player.
            if (p_searching)
            {
                //Moves the enemy.
                transform.position = Vector3.Lerp(p_startPos, p_goal, p_timePast / (p_followingMovementTime)); 

                //If the enemy has reached the final position of the movement.
                if (transform.position == p_goal)
                {
                    //Changes the animator state to Idle.
                    p_animator.SetTrigger("Idle");

                    //If the enemy hasn't reached the position where the player was detected.
                    if (transform.position != p_detectedPlayerPos)
                        //Sets the minimum time as the remaining time to the next movement.
                        p_remainingTime = p_minTime;
                    else
                    {
                        //The enemy is not following the player now.
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
            //If the enemy is not searching the player.
            else
            {
                //Moves the enemy.
                transform.position = Vector3.Lerp(p_startPos, p_goal, p_timePast / p_movementTime); 

                //If the enemy has reached the final position of the movement.
                if (transform.position == p_goal)
                {
                    //Changes the animator state as Idle.
                    p_animator.SetTrigger("Idle");

                    //Sets a new value as the remaining time to the next movement.
                    p_remainingTime = Random.Range(p_minTime, p_maxTime);

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
        //If the enemy is not moving.
        else
        {
            //If the enemy was rotated at the last FixedUpdate and must move on this FixedUpdate.
            if (p_willMove)
            {
                p_willMove = false;

                //If the way is locked by a barrier.
                if (p_locked)
                {
                    p_remainingTime = p_minTime; 
                }
                //If it is possible to move the enemy to the position.
                else if (LevelController.Instance.IsEmptySpace(transform.position + transform.rotation * new Vector3(0, LevelLoadingManager.Instance._floorTile.GetSize().y, 0)))
                {
                    //Starts the enemy movement.
                    Move(transform.position + transform.rotation * new Vector3(0, LevelLoadingManager.Instance._floorTile.GetSize().y, 0));
                }
                //If not (occupied by another enemy).
                else
                {
                    //Stops following the player.
                    p_searching = false;
                    p_directions.Add(transform.rotation.eulerAngles);
                    p_remainingTime = Random.Range(p_minTime, p_maxTime);
                }

            }
            else if (p_remainingTime <= 0)
            {
                //Chooses a new available directioj randomly.
                direction = p_directions[Random.Range(0, p_directions.Count)];

                //If the enemy is searching the player or if the enemy is looking to the chosen direction.
                if (p_searching || direction == transform.rotation.eulerAngles)
                {
                    //If the way is locked by a barrier (if the enemy is searching the player this case can't happen).
                    if (p_locked)
                    {
                        //Sets the minimum time as the remaining time to the next movement.
                        p_remainingTime = p_minTime;

                        //Removes the direccion from the list of available directions.
                        p_directions.Remove(direction);
                    }
                    //If it is possible to move the enemy to the position. 
                    else if (LevelController.Instance.IsEmptySpace(transform.position + transform.rotation * new Vector3(0, LevelLoadingManager.Instance._floorTile.GetSize().y, 0)))
                    {
                        //Starts the enemy movement.
                        Move(transform.position + transform.rotation * new Vector3(0, LevelLoadingManager.Instance._floorTile.GetSize().y, 0));
                    }
                    //If not (occupied by another enemy).
                    else
                    {
                        p_searching = false;
                        p_remainingTime = Random.Range(p_minTime, p_maxTime);
                    }
                }
                //Rotates the enemy and it will move at the next FixedUpdate.
                else
                {
                    p_barrierDetectionCollider.SetActive(false);
                    transform.rotation = Quaternion.Euler(direction);
                    p_willMove = true;
                    p_barrierDetectionCollider.SetActive(true);
                    p_directions.Remove(direction);
                }
            }
            else
            {
                p_remainingTime -= Time.fixedDeltaTime;
            }
        }
    }

    /// <summary>
    /// Notifies that the way is locked by a barrier.
    /// </summary>
    public void WayLocked()
    {
        p_locked = true;
    }

    /// <summary>
    /// Notifies that the way is not locked by a barrier.
    /// </summary>
    public void WayUnlocked()
    {
        p_locked = false;
    }

    /// <summary>
    /// Sets up the playe movement.
    /// </summary>
    /// <param name="i_pos">The final position of the movement.</param>
    private void Move(Vector3 i_pos)
    {
        //Sets the current position as the starting position of the movement.
        p_startPos = transform.position;

        //Sets the final position of the movement.
        p_goal = i_pos;

        //Sets the time past while moving to 0.
        p_timePast = 0;

        //Notifies the enemy next position to the LevelController.
        LevelController.Instance.NextPosition(gameObject, p_goal);

        //If the enemy is searching the player.
        if (p_searching)
            //Changes the animator state to Following.
            p_animator.SetTrigger("Following");
        else
            //Changes the animator state to Moving.
            p_animator.SetTrigger("Moving");
    }

    /// <summary>
    /// Detects if there is any player on enemy sight.
    /// </summary>
    private void DetectPlayer()
    {
        RaycastHit2D hit;

        hit = Physics2D.Raycast(transform.position + transform.rotation * new Vector2(0, gameObject.GetSize().y / 2f), Quaternion.Euler(transform.rotation.eulerAngles) * Vector2.up, p_viewRange * LevelLoadingManager.Instance._floorTile.GetSize().x, LayerMask.GetMask("Barrier", "Enemy", "Player"));

        Debug.DrawRay(transform.position, Quaternion.Euler(transform.rotation.eulerAngles) * Vector2.up, Color.white, 2);

        if (hit != null)
            Debug.Log(hit.collider.gameObject);

        if (hit != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (transform.rotation.eulerAngles == new Vector3(0, 0, 0) || transform.rotation.eulerAngles == new Vector3(0, 0, 180))
            {
                p_detectedPlayerPos = new Vector3(transform.position.x, transform.position.y + Mathf.Round((hit.transform.position.y - transform.position.y) / LevelLoadingManager.Instance._floorTile.GetSize().y) * LevelLoadingManager.Instance._floorTile.GetSize().y, transform.position.z);
            }
            else
            {
                p_detectedPlayerPos = new Vector3(transform.position.x + Mathf.Round((hit.transform.position.x - transform.position.x) / LevelLoadingManager.Instance._floorTile.GetSize().x) * LevelLoadingManager.Instance._floorTile.GetSize().x, transform.position.y, transform.position.z);
            }

            p_searching = true;

            if (p_remainingTime > p_minTime)
                p_remainingTime = p_minTime;
        }
    }
}
