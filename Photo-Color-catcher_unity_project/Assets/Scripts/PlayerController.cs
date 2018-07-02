using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component that manages the player actions.
/// </summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// The time needed to move the player from one LevelCell to the adjacent.
    /// </summary>
    [SerializeField]
    private float p_movementTime;

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
    private bool p_locked;

    /// <summary>
    /// If the player will move at the next FixedUpdate.
    /// </summary>
    private bool p_willMove;

    /// <summary>
    /// The GameObject of the Collider that detects barriers.
    /// </summary>
    private GameObject p_barrierDetectionCollider;

    /// <summary>
    /// The GameObject of the Collider that is instantiated when the player attacks.
    /// </summary>
    private GameObject p_photoAtackCollider;

    /// <summary>
    /// If there is a color stored.
    /// </summary>
    [HideInInspector]
    public bool _colorStored;

    /// The minimum time passed between player attacks.
    [SerializeField]
    private float p_timeBetweenAttacks;

    /// <summary>
    /// The minimum time passed between attacks that stores a color cell (or releasing a stored color).
    /// </summary>
    [SerializeField]
    private float p_storeColorCooldown;

    /// <summary>
    /// The remaining time for the next attack.
    /// </summary>
    private float p_attackRemainingTime;

    /// <summary>
    /// The remaining time for the next attack that stores a color cell (or that releases a stored color).
    /// </summary>
    private float p_storeRemainingCooldown;

    /// <summary>
    /// If there was an input order of movement gotten at last Update.
    /// </summary>
    private bool p_caughtMovement;

    /// <summary>
    /// If there was an input order of rotate the player gotten at last Update.
    /// </summary>
    private bool p_caughtRotation;

    /// <summary>
    /// The direction gotten through input at last Update. 
    /// </summary>
    private Quaternion p_caughtDirection;

    /// <summary>
    /// If there was an input order of making the player attack gotten at last Update.
    /// </summary>
    private bool p_caughtAttack;

    /// <summary>
    /// If there was an input order of making the player attack (with an attack that stores or releases a color cell) gotten at last Update.
    /// </summary>
    private bool p_caughtStore;

    /// <summary>
    /// If there was an input order of pausing the game gotten at last Update.
    /// </summary>
    private bool p_caughtPause;

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

        //Gets the GameObject of the Collider that is instantiated when the player attacks.
        p_photoAtackCollider = gameObject.GetChild("PhotoAttackCollider");
        p_photoAtackCollider.SetActive(false);
    }

    //Inputs and Physics
    void Update()
    {
        p_caughtMovement = true;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            p_caughtDirection = Quaternion.Euler(0, 0, 0);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            p_caughtDirection = Quaternion.Euler(0, 0, 180);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            p_caughtDirection = Quaternion.Euler(0, 0, 270);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            p_caughtDirection = Quaternion.Euler(0, 0, 90);
        }
        else
            p_caughtMovement = false;

        p_caughtRotation = ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) && p_caughtMovement)) ? true : false;

        p_caughtAttack = (Input.GetKey(KeyCode.Space)) ? true : false;

        p_caughtStore = (Input.GetKey(KeyCode.C)) ? true : false;

        p_caughtPause = (Input.GetKey(KeyCode.P)) ? true : false;
    }

    //Graphics and movements
    void FixedUpdate()
    {
        if (p_storeRemainingCooldown > 0 && !p_caughtPause)
            p_storeRemainingCooldown -= Time.fixedDeltaTime;

        if (p_attackRemainingTime > 0 && !p_caughtPause)
            p_attackRemainingTime -= Time.fixedDeltaTime;

        //If the game has to be paused.
        if (p_caughtPause)
            LevelController.Instance.Pause();
        //If the player is moving.
        else if (p_goal != transform.position)
        {
            //Moves the player.
            p_timePast += Time.fixedDeltaTime;
            transform.position = Vector3.Lerp(p_startPos, p_goal, p_timePast / p_movementTime);
        }
        //If the player was rotated at the last FixedUpdate and must move on this FixedUpdate.
        else if (p_willMove)
        {
            p_willMove = false;

            //If the way is not locked.
            if (!p_locked)
            {
                //Starts moving the player.
                p_startPos = transform.position;
                p_goal = transform.position + p_caughtDirection * new Vector3(0, LevelLoadingManager.Instance._floorTile.GetSize().y, 0);
                p_timePast = 0;
                LevelController.Instance.NextPosition(gameObject, p_goal);
            }
        }
        //If the player must attack.
        else if (p_attackRemainingTime <= 0 && p_caughtAttack)
        {
            //Instantiates the attack Collider GameObject.
            GameObject aux = (GameObject)Instantiate(p_photoAtackCollider);
            aux.transform.position = p_photoAtackCollider.transform.position;
            aux.transform.localScale = new Vector3(0.8f, 0.8f, 1);
            aux.SetActive(true);
            p_attackRemainingTime = p_timeBetweenAttacks;
        }
        //If the player must attack (storing or releasing a color).
        else if (p_attackRemainingTime <= 0 && p_storeRemainingCooldown <= 0 && p_caughtStore)
        {
            //If there isn't a color stored.
            if (!_colorStored)
            {
                //Instantiates the attack Collider GameObject.
                GameObject aux = (GameObject)Instantiate(p_photoAtackCollider);
                aux.GetComponent<PhotoAttackController>()._storeColor = true;
                aux.transform.position = p_photoAtackCollider.transform.position;
                aux.transform.localScale = new Vector3(0.8f, 0.8f, 1);
                aux.SetActive(true);
            }
            //If there is a color stored.
            else
            {
                //Releases it.
                LevelController.Instance.ReleaseColor();
            }
            p_storeRemainingCooldown = p_storeColorCooldown;
            p_attackRemainingTime = p_timeBetweenAttacks;
        }
        //If the player must rotate.
        else if (p_caughtRotation)
        {
            p_barrierDetectionCollider.SetActive(false);
            transform.rotation = p_caughtDirection;
            p_barrierDetectionCollider.SetActive(true);
        }
        //If the player must move.
        else if (p_caughtMovement)
        {
            //If the player is looking to the movement direction.
            if (transform.rotation == p_caughtDirection)
            {
                //If the way is not locked.
                if (!p_locked)
                {
                    //Starts moving the player.
                    p_startPos = transform.position;
                    p_goal = transform.position + p_caughtDirection * new Vector3(0, LevelLoadingManager.Instance._floorTile.GetSize().y, 0);
                    p_timePast = 0;
                    LevelController.Instance.NextPosition(gameObject, p_goal);
                }
            }
            else
            {
                //Rotates the player and it will move at the next FixedUpdate.
                p_barrierDetectionCollider.SetActive(false);
                transform.rotation = p_caughtDirection;
                p_willMove = true;
                p_barrierDetectionCollider.SetActive(true);
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
